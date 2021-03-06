﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asynq;
using IVO.Definition.Containers;
using IVO.Definition.Models;
using IVO.Definition.Repositories;
using IVO.Implementation.SQL;
using IVO.Implementation.SQL.Persists;
using IVO.Implementation.SQL.Queries;

namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var pr = new Program();

            //pr.TestIDGeneration();
            //pr.TestTreeIDEquivalencies();
            //pr.TestAsynqQuery();
            //pr.TestPersistBlob();
            //pr.TestQueryBlobs();

#if false
            // Create a 3-level deep commit hierarchy:
            pr.Create3DeepCommit().Wait();
            // Get the commit tree recursively:
            pr.TestGetCommitTree().Wait();

            pr.TestLargeBlobPersistence().Wait();
            pr.TestQueryByPath().Wait();
#endif
            // Demonstrate the concept of content-addressable hashing:
            pr.DemonstrateHashing().Wait();

#if false
            Console.WriteLine("Press a key.");
            Console.ReadLine();
#else
            Console.WriteLine("Done.");
#endif
        }

        DataContext getDataContext()
        {
            return new DataContext(@"Data Source=.\SQLEXPRESS;Initial Catalog=IVO;Integrated Security=SSPI");
        }

        void TestIDGeneration()
        {
            // Create a Blob:
            Blob bl = new Blob.Builder(
                pContents: Encoding.UTF8.GetBytes("Sample README content.")
            );
            Console.WriteLine(bl.ID.ToString());

            // Create a Tree:
            Tree tr = new Tree.Builder(
                new List<TreeTreeReference>(0),
                new List<TreeBlobReference> {
                    new TreeBlobReference.Builder("README", bl.ID)
                }
            );
            Console.WriteLine(tr.ID.ToString());

            // Create a Commit:
            Commit cm = new Commit.Builder(
                pParents: new List<CommitID>(0),
                pTreeID: tr.ID,
                pCommitter: "James Dunne <james.jdunne@gmail.com>",
                pDateCommitted: DateTimeOffset.Now,
                pMessage: "A commit message here."
            );
            Console.WriteLine(cm.ID.ToString());
        }

        void TestTreeIDEquivalencies()
        {
            byte[] tidBuf = new byte[TreeID.ByteArrayLength];
            byte[] bidBuf = new byte[BlobID.ByteArrayLength];

            // Obviously never generate random fake SHA-1s. This is purely for testing.
            var rnd = new Random(1011);
            rnd.NextBytes(tidBuf);
            rnd.NextBytes(bidBuf);

            // Create several variants of the same Tree:
            Tree tr0 = new Tree.Builder(
                new List<TreeTreeReference> {
                    new TreeTreeReference.Builder("src", new TreeID(tidBuf)),
                    new TreeTreeReference.Builder("Content", new TreeID(tidBuf)),
                    new TreeTreeReference.Builder("Images", new TreeID(tidBuf)),
                },
                new List<TreeBlobReference> {
                    new TreeBlobReference.Builder("main", new BlobID(bidBuf)),
                    new TreeBlobReference.Builder("README", new BlobID(bidBuf)),
                }
            );
            Console.WriteLine(tr0.ID.ToString());

            Tree tr1 = new Tree.Builder(
                new List<TreeTreeReference> {
                    new TreeTreeReference.Builder("Content", new TreeID(tidBuf)),
                    new TreeTreeReference.Builder("src", new TreeID(tidBuf)),
                    new TreeTreeReference.Builder("Images", new TreeID(tidBuf)),
                },
                new List<TreeBlobReference> {
                    new TreeBlobReference.Builder("README", new BlobID(bidBuf)),
                    new TreeBlobReference.Builder("main", new BlobID(bidBuf)),
                }
            );
            Console.WriteLine(tr1.ID.ToString());

            Tree tr2 = new Tree.Builder(
                new List<TreeTreeReference> {
                    new TreeTreeReference.Builder("Content", new TreeID(tidBuf)),
                    new TreeTreeReference.Builder("Images", new TreeID(tidBuf)),
                    new TreeTreeReference.Builder("src", new TreeID(tidBuf)),
                },
                new List<TreeBlobReference> {
                    new TreeBlobReference.Builder("README", new BlobID(bidBuf)),
                    new TreeBlobReference.Builder("main", new BlobID(bidBuf)),
                }
            );
            Console.WriteLine(tr2.ID.ToString());

            // Create a different tree:
            Tree tr3 = new Tree.Builder(
                new List<TreeTreeReference> {
                    new TreeTreeReference.Builder("src", new TreeID(tidBuf)),
                    new TreeTreeReference.Builder("Images", new TreeID(tidBuf)),
                    new TreeTreeReference.Builder("Content", new TreeID(tidBuf)),
                },
                new List<TreeBlobReference> {
                    // Capitalization of name makes a difference:
                    new TreeBlobReference.Builder("Readme", new BlobID(bidBuf)),
                    new TreeBlobReference.Builder("main", new BlobID(bidBuf)),
                }
            );
            Console.WriteLine(tr3.ID.ToString());
        }

        void TestAsynqQuery()
        {
            var db = getDataContext();

            var q = new QueryCommit(new CommitID(new byte[20]));

            const int max = 500;

            Task<Commit>[] results = new Task<Commit>[max];
            for (int i = 0; i < max; ++i)
            {
                // Asynchronously execute the query:
                results[i] = db.ExecuteListQueryAsync(q);
            }

            for (int i = 0; i < max; ++i)
            {
                results[i].Wait();
            }
        }

        void TestPersistBlob()
        {
            // Create a Blob:
            Blob bl = new Blob.Builder(Encoding.UTF8.GetBytes("Sample README content."));
            Console.WriteLine(bl.ID.ToString());

            var db = getDataContext();

            // Check if the Blob exists by this ID:
            var getBlob = db.ExecuteSingleQueryAsync(new QueryBlob(bl.ID));
            getBlob.Wait();

            if (getBlob.Result == null)
            {
                // It does not, persist it:
                Console.WriteLine("PERSIST {0}", bl.ID);

                var persistBlob = db.ExecuteNonQueryAsync(new PersistBlob(bl));
                persistBlob.Wait();

                Console.WriteLine("{0} rows affected", persistBlob.Result);
            }
            else
            {
                Console.WriteLine("Blob retrieved {0}", getBlob.Result.ID.ToString());

                // Destroy the blob:
                Console.WriteLine("DELETE {0}", getBlob.Result.ID);

                var destroyBlob = db.ExecuteNonQueryAsync(new DestroyBlob(getBlob.Result.ID));
                destroyBlob.Wait();

                Console.WriteLine("{0} rows affected", destroyBlob.Result);
            }
        }

        void TestQueryBlobs()
        {
            // Create some Blobs:
            Blob bl0 = new Blob.Builder(Encoding.UTF8.GetBytes("Sample README content."));
            Console.WriteLine(bl0.ID.ToString());

            Blob bl1 = new Blob.Builder(Encoding.UTF8.GetBytes("Sample content."));
            Console.WriteLine(bl1.ID.ToString());

            var blobs = new ImmutableContainer<BlobID, Blob>(bl => bl.ID, bl0, bl1);
            var db = getDataContext();

            // Check which blobs exist already:
            var qBlobs = db.ExecuteListQueryAsync(new QueryBlobsExist(bl0.ID, bl1.ID), expectedCapacity: blobs.Count);
            qBlobs.Wait();

            // Find the blobs to persist:
            var blobIDsToPersist = blobs.Keys.Except(qBlobs.Result).ToArray();

            // Persist each blob asynchronously:
            Task<Blob>[] persists = new Task<Blob>[blobIDsToPersist.Length];
            for (int i = 0; i < blobIDsToPersist.Length; ++i)
            {
                BlobID id = blobIDsToPersist[i];

                Console.WriteLine("PERSIST {0}", id.ToString());
                persists[i] = db.ExecuteNonQueryAsync(new PersistBlob(blobs[id]));
            }

            Console.WriteLine("Waiting for persists...");
            Task.WaitAll(persists);
            Console.WriteLine("Complete.");
        }

        async Task<TreeID> TestPersistTree()
        {
            // Create a Blob:
            Blob bl = new Blob.Builder(Encoding.UTF8.GetBytes("Sample README content."));
            ImmutableContainer<BlobID, Blob> blobs = new ImmutableContainer<BlobID, Blob>(b => b.ID, bl);

            // Create a Tree:
            Tree trPersists = new Tree.Builder(
                new List<TreeTreeReference>(0),
                new List<TreeBlobReference> {
                    new TreeBlobReference.Builder("HelloWorld.cs", bl.ID),
                    new TreeBlobReference.Builder("PersistBlob.cs", bl.ID),
                    new TreeBlobReference.Builder("PersistTree.cs", bl.ID),
                }
            );

            Tree trSrc = new Tree.Builder(
                new List<TreeTreeReference> {
                    new TreeTreeReference.Builder("Persists", trPersists.ID),
                },
                new List<TreeBlobReference> {
                    new TreeBlobReference.Builder("blah", bl.ID),
                }
            );

            Tree trData = new Tree.Builder(
                new List<TreeTreeReference>(0),
                new List<TreeBlobReference> {
                    new TreeBlobReference.Builder("myTest.xml", bl.ID),
                    new TreeBlobReference.Builder("myTest2.xml", bl.ID),
                    new TreeBlobReference.Builder("myTest3.xml", bl.ID),
                }
            );

            Tree trRoot = new Tree.Builder(
                new List<TreeTreeReference> {
                    new TreeTreeReference.Builder("src", trSrc.ID),
                    new TreeTreeReference.Builder("data", trData.ID),
                },
                new List<TreeBlobReference> {
                    new TreeBlobReference.Builder("README", bl.ID),
                    new TreeBlobReference.Builder("main.xml", bl.ID),
                    new TreeBlobReference.Builder("test.xml", bl.ID),
                }
            );

            ImmutableContainer<TreeID, Tree> trees = new ImmutableContainer<TreeID, Tree>(tr => tr.ID, trRoot, trSrc, trData, trPersists);

            var db = getDataContext();

            IBlobRepository blrepo = new BlobRepository(db);
            ITreeRepository trrepo = new TreeRepository(db);

            // Persist the tree and its blobs:
            await blrepo.PersistBlobs(blobs);
            var ptr = await trrepo.PersistTree(trRoot.ID, trees);

            // Make sure we got back what's expected of the API:
            Debug.Assert(trRoot.ID == ptr.ID);

            Console.WriteLine("Root TreeID = {0}", trRoot.ID);

            return trRoot.ID;
        }

        private static Stack<T> newStack<T>(T initial, int initialCapacity = 10)
        {
            var stk = new Stack<T>(initialCapacity);
            stk.Push(initial);
            return stk;
        }

        async Task TestRetrieveTreeRecursively(TreeID rootid)
        {
            var db = getDataContext();

            Console.WriteLine("Retrieving TreeID {0} recursively...", rootid);

            ITreeRepository repo = new TreeRepository(db);
            var tree = await repo.GetTreeRecursively(rootid);

            // Recursively display trees:
            RecursivePrint(tree.Item1, tree.Item2);
        }

        async Task<CommitID> TestCreateCommit(TreeID treeid, int num)
        {
            var db = getDataContext();

            ICommitRepository cmrepo = new CommitRepository(db);
            IRefRepository rfrepo = new RefRepository(db);

            Tuple<Ref, Commit> parent = await cmrepo.GetCommitByRef("HEAD");

            Commit cm = new Commit.Builder(
                pParents:       parent == null ? new List<CommitID>(0) : new List<CommitID>(1) { parent.Item2.ID },
                pTreeID:        treeid,
                pCommitter:     "James Dunne <james.jdunne@gmail.com>",
                pDateCommitted: DateTimeOffset.Parse("2011-08-29 00:00:00 -0500"),
                pMessage:       "Commit #" + num.ToString() + "."
            );

            Console.WriteLine("CommitID {0}", cm.ID);

            // Persist the commit:
            await cmrepo.PersistCommit(cm);
            // Once the commit is persisted, update HEAD ref:
            await rfrepo.PersistRef(new Ref.Builder("HEAD", cm.ID));

            return cm.ID;
        }

        async Task Create3DeepCommit()
        {
            // Create a 3-depth commit tree up from HEAD:
            for (int i = 0; i < 3; ++i)
            {
                TreeID rootid = await TestPersistTree();
                TestRetrieveTreeRecursively(rootid).Wait();

                CommitID cmid = await TestCreateCommit(rootid, i);
                TestCreateTag(cmid).Wait();
            }
        }

        async Task TestCreateTag(CommitID cmid)
        {
            var db = getDataContext();

            ICommitRepository cmrepo = new CommitRepository(db);
            ITagRepository tgrepo = new TagRepository(db);

            Console.WriteLine("Get commit by tag 'v1.0'");
            var cm = await cmrepo.GetCommitByTagName("v1.0");
            if (cm != null)
            {
                Console.WriteLine("v1.0 was {0}", cm.Item2.ID);
#if false
                Console.WriteLine("Deleting Tag by ID {0}", cm.Item1.ID);
                await tgrepo.DeleteTag(cm.Item1.ID).Wait();
#else
                Console.WriteLine("Deleting Tag by name 'v1.0'");
                await tgrepo.DeleteTagByName("v1.0");
#endif
            }

            Tag tg = new Tag.Builder("v1.0", cmid, "James Dunne <james.jdunne@gmail.com>", DateTimeOffset.Now, "Tagged for version 1.0");

            await tgrepo.PersistTag(tg);
            var tgByName = await cmrepo.GetCommitByTagName("v1.0");
            Debug.Assert(tgByName.Item2.ID == cmid);

            Console.WriteLine("Completed.");
        }

        static void RecursivePrint(TreeID treeID, ImmutableContainer<TreeID, Tree> trees, int depth = 1)
        {
            var tr = trees[treeID];
            if (depth == 1)
            {
                Console.WriteLine("tree {1}: {0}/", new string('_', (depth - 1) * 2), tr.ID.ToString(firstLength: 7));
            }

            // Sort refs by name:
            var namedRefs = Tree.ComputeChildList(tr.Trees, tr.Blobs);

            foreach (var kv in namedRefs)
            {
                var nref = kv.Value;
                switch (nref.Which)
                {
                    case Either<TreeTreeReference, TreeBlobReference>.Selected.Left:
                        Console.WriteLine("tree {1}: {0}{2}/", new string('_', depth * 2), nref.Left.TreeID.ToString(firstLength: 7), nref.Left.Name);
                        RecursivePrint(nref.Left.TreeID, trees, depth + 1);
                        break;
                    case Either<TreeTreeReference, TreeBlobReference>.Selected.Right:
                        Console.WriteLine("blob {1}: {0}{2}", new string('_', depth * 2), nref.Right.BlobID.ToString(firstLength: 7), nref.Right.Name);
                        break;
                }
            }
        }

        async Task TestGetCommitTree()
        {
            var db = getDataContext();

            IRefRepository rfrepo = new RefRepository(db);
            ICommitRepository cmrepo = new CommitRepository(db);
            
            var rf = await rfrepo.GetRef("HEAD");
            Debug.Assert(rf != null);

            var cmTree = await cmrepo.GetCommitTree(rf.CommitID, depth: 10);

            // TODO: gather up another 10-deep commit tree from the last partial commit?
            RecursivePrint(cmTree.Item1, cmTree.Item2);
        }

        static void RecursivePrint(CommitID cmID, ImmutableContainer<CommitID, ICommit> commits, int depth = 1)
        {
            ICommit cm = commits[cmID];

            if (cm.IsComplete)
            {
                Console.WriteLine("{0}c {1}:  ({2})", new string(' ', (depth - 1) * 2), cm.ID.ToString(firstLength: 7), String.Join(",", cm.Parents.Select(id => id.ToString(firstLength: 7))));
                foreach (CommitID parentID in cm.Parents)
                {
                    RecursivePrint(parentID, commits, depth + 1);
                }
            }
            else
            {
                Console.WriteLine("{0}p  {1}:  ?", new string(' ', (depth - 1) * 2), cm.ID.ToString(firstLength: 7));
            }
        }

        async Task TestLargeBlobPersistence()
        {
            var db = getDataContext();
            IBlobRepository blrepo = new BlobRepository(db);

            Random rnd = new Random(0x5555AAC0);

            const int count = 4000;

            Console.WriteLine("Constructing {0} blobs...", count);
            Blob[] bls = new Blob[count];
            for (int i = 0; i < count; ++i)
            {
                byte[] c = new byte[8040];
                rnd.NextBytes(c);

                bls[i] = new Blob.Builder(c);
            }
            ImmutableContainer<BlobID, Blob> blobs = new ImmutableContainer<BlobID, Blob>(bl => bl.ID, bls);

            Console.WriteLine("Persisting {0} blobs...", count);
            Stopwatch sw = Stopwatch.StartNew();
            ImmutableContainer<BlobID, Blob> pBlobs = await blrepo.PersistBlobs(blobs);
            Console.WriteLine("Waiting...");
            sw.Stop();
            Console.WriteLine("Completed in {0} ms, {1} blobs/sec, {2} bytes/sec",
                sw.ElapsedMilliseconds,
                (double)count / (double)sw.ElapsedMilliseconds * 1000d,
                (double)(count * 8040) / (double)sw.ElapsedMilliseconds * 1000d);
        }

        async Task TestQueryByPath()
        {
            var db = getDataContext();

            ICommitRepository cmrepo = new CommitRepository(db);
            ITreeRepository trrepo = new TreeRepository(db);

            var rf = await cmrepo.GetCommitByRef("HEAD");
            if (rf == null) return;

            try
            {
                var tree = await trrepo.GetTreeRecursivelyFromPath(rf.Item2.TreeID, ((AbsoluteTreePath)"/src/Persists/").Canonicalize());
                if (tree != null)
                {
                    RecursivePrint(tree.Item1, tree.Item2);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        async Task DemonstrateHashing()
        {
            var db = getDataContext();

            IBlobRepository blrepo = new BlobRepository(db);
            ITreeRepository trrepo = new TreeRepository(db);
            ICommitRepository cmrepo = new CommitRepository(db);
            IRefRepository rfrepo = new RefRepository(db);

            // Create a sample set of blobs:
            Blob readmeBlob;
            ImmutableContainer<BlobID, Blob> blobs = new ImmutableContainer<BlobID, Blob>(
                bl => bl.ID,
                readmeBlob = new Blob.Builder(pContents: Encoding.UTF8.GetBytes(@"Hello world"))
            );
            Console.Out.WriteAsync(String.Format("Blob {0} = \"{1}\"" + Environment.NewLine, readmeBlob.ID.ToString(firstLength: 7), Encoding.UTF8.GetString(readmeBlob.Contents)));
            // Persist them:
            var persistingBlobs = blrepo.PersistBlobs(blobs);

            // Create an initial tree:
            Tree trRoot;
            ImmutableContainer<TreeID, Tree> trees = new ImmutableContainer<TreeID, Tree>(
                tr => tr.ID,
                new Tree[] {
                    trRoot = new Tree.Builder(
                        pTrees: new List<TreeTreeReference>(0),
                        pBlobs: new List<TreeBlobReference> {
                            new TreeBlobReference.Builder(pName: "README", pBlobID: readmeBlob.ID)
                        }
                    )
                }
            );
            // Dump the tree:
            RecursivePrint(trRoot.ID, trees);

            // Now wait for the blob persistence to complete:
            await persistingBlobs;
            // Persist our tree now:
            var persistTrees = trrepo.PersistTree(trRoot.ID, trees);
            await persistTrees;

            // Let's make a commit out of all that:
            Commit cm1 = new Commit.Builder(
                pParents:       new List<CommitID>(0),
                pTreeID:        trRoot.ID,
                pCommitter:     @"James Dunne <james.jdunne@gmail.com>",
                pDateCommitted: DateTimeOffset.Now,
                pMessage:       "Initial commit."
            );
            // Persist that commit:
            Console.Out.WriteAsync(String.Format("Persisting commit {0}..." + Environment.NewLine, cm1.ID.ToString(firstLength: 7)));
            await cmrepo.PersistCommit(cm1);

            // Let's create a ref to point to it:
            await rfrepo.DestroyRefByName("demo/HEAD");
            await rfrepo.PersistRef(new Ref.Builder(pName: "demo/HEAD", pCommitID: cm1.ID));

            await Console.Out.WriteAsync(String.Format("Pointed demo/HEAD to commit {0}" + Environment.NewLine, cm1.ID.ToString(firstLength: 7)));

            // Now let's create a new blob with some revised contents: (adding a period at the end)
            Blob readmeBlob2;
            blobs = new ImmutableContainer<BlobID, Blob>(
                bl => bl.ID,
                readmeBlob2 = new Blob.Builder(Encoding.UTF8.GetBytes(@"Hello world."))
            );
            var persistBlobs2 = blrepo.PersistBlobs(blobs);

            // Make a new tree out of the old tree:
            Tree.Builder trRoot2b = new Tree.Builder(trRoot);
            // Point README to the new BlobID:
            trRoot2b.Blobs[0] = new TreeBlobReference.Builder(trRoot2b.Blobs[0]) { BlobID = readmeBlob2.ID };

            // Freeze our Tree.Builder to a Tree:
            Tree trRoot2;
            trees = new ImmutableContainer<TreeID, Tree>(tr => tr.ID, trRoot2 = trRoot2b);

            // Wait for the blobs to persist:
            await persistBlobs2;
            // Now persist the new tree:
            await trrepo.PersistTree(trRoot2.ID, trees);

            // Load a streamed blob:
            IStreamedBlob strbl = await blrepo.GetStreamedBlob(readmeBlob.ID);
            await strbl.ReadStream(blsr =>
            {
                Console.WriteLine("blob is {0} length", blsr.Length);
                
                byte[] dum = new byte[8040];
                int count = 8040;

                try
                {
                    while ((count = blsr.Read(dum, 0, 8040)) > 0)
                    {
                        for (int i = 0; i < (count / 40) + ((count % 40) > 0 ? 1 : 0); ++i)
                        {
                            Console.WriteLine(dum.ToHexString(i * 40, Math.Min(count - (i * 40), 40)));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
                }
            });
        }
    }
}