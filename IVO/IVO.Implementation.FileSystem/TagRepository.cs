﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IVO.Definition.Models;
using IVO.Definition.Repositories;
using IVO.Definition.Errors;

namespace IVO.Implementation.FileSystem
{
    public sealed class TagRepository : ITagRepository
    {
        private FileSystem system;

        public TagRepository(FileSystem system)
        {
            this.system = system;
        }

        #region Private details

        private async Task<Errorable<Tag>> persistTag(Tag tg)
        {
            // Write the commit contents to the file:
            FileInfo tmpFile = system.getTemporaryFile();
            using (var fs = new FileStream(tmpFile.FullName, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await fs.WriteRawAsync(tg.WriteTo(new StringBuilder()).ToString());
            }

            lock (FileSystem.SystemLock)
            {
                FileInfo fi = system.getPathByID(tg.ID);

                // NOTE: if the record already exists we can either error out or overwrite the existing file with contents known to be good in the case the existing file got corrupt.
                // Let's stick with the self-repair scenario since erroring out doesn't help anyone.
                if (fi.Exists)
                {
                    Debug.WriteLine(String.Format("Self-repair scenario: overwriting old TagID {0} with new contents", tg.ID));
                    fi.Delete();
                }

                // Create directory if it doesn't exist:
                if (!fi.Directory.Exists)
                {
                    Debug.WriteLine(String.Format("New DIR '{0}'", fi.Directory.FullName));
                    fi.Directory.Create();
                }

                Debug.WriteLine(String.Format("New TAG '{0}'", fi.FullName));
                File.Move(tmpFile.FullName, fi.FullName);
            }

            // Now keep track of the tag by its name:
            tmpFile = system.getTemporaryFile();
            using (var fs = new FileStream(tmpFile.FullName, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await fs.WriteRawAsync(tg.ID.ToString());
            }

            lock (FileSystem.SystemLock)
            {
                FileInfo fiTracker = system.getTagPathByTagName(tg.Name);
                // Does this tag name exist already?
                if (fiTracker.Exists)
                {
                    tmpFile.Delete();
                    return new TagNameAlreadyExistsError(tg.Name);
                }

                // Create directory if it doesn't exist:
                if (!fiTracker.Directory.Exists)
                {
                    Debug.WriteLine(String.Format("New DIR '{0}'", fiTracker.Directory.FullName));
                    fiTracker.Directory.Create();
                }

                Debug.WriteLine(String.Format("New TAG '{0}'", fiTracker.FullName));
                File.Move(tmpFile.FullName, fiTracker.FullName);
            }

            return tg;
        }

        private async Task<Errorable<TagID>> getTagIDByName(TagName tagName)
        {
            FileInfo fiTracker = system.getTagPathByTagName(tagName);

            Debug.Assert(fiTracker != null);
            if (!fiTracker.Exists) return new TagNameDoesNotExistError(tagName);

            byte[] buf;
            int nr = 0;
            using (var fs = new FileStream(fiTracker.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 16384, true))
            {
                // TODO: implement an async buffered Stream:
                buf = new byte[16384];
                nr = await fs.ReadAsync(buf, 0, 16384).ConfigureAwait(continueOnCapturedContext: false);
                if (nr >= 16384)
                {
                    // My, what a large tag you have!
                    throw new NotSupportedException();
                }
            }

            // Parse the TagID:
            using (var ms = new MemoryStream(buf, 0, nr, false))
            using (var sr = new StreamReader(ms, Encoding.UTF8))
            {
                string line = sr.ReadLine();
                if (line == null) return new TagNameDoesNotExistError(tagName);

                return TagID.TryParse(line);
            }
        }

        private async Task<Errorable<Tag>> getTag(TagID id)
        {
            FileInfo fi = system.getPathByID(id);
            if (!fi.Exists) return new TagIDRecordDoesNotExistError(id);

            byte[] buf;
            int nr = 0;
            using (var fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 16384, true))
            {
                // TODO: implement an async buffered Stream:
                buf = new byte[16384];
                nr = await fs.ReadAsync(buf, 0, 16384).ConfigureAwait(continueOnCapturedContext: false);
                if (nr >= 16384)
                {
                    // My, what a large tag you have!
                    throw new NotSupportedException();
                }
            }

            Tag.Builder tb = new Tag.Builder();

            // Parse the Tag:
            using (var ms = new MemoryStream(buf, 0, nr, false))
            using (var sr = new StreamReader(ms, Encoding.UTF8))
            {
                string line = sr.ReadLine();

                // Set CommitID:
                if (line == null || !line.StartsWith("commit ")) return new TagParseExpectedCommitError();
                var ecid = CommitID.TryParse(line.Substring("commit ".Length));
                if (ecid.HasErrors) return ecid.Errors;
                tb.CommitID = ecid.Value;

                // Set Name:
                line = sr.ReadLine();
                if (line == null || !line.StartsWith("name ")) return new TagParseExpectedNameError();
                tb.Name = (TagName)line.Substring("name ".Length);

                // Set Tagger:
                line = sr.ReadLine();
                if (line == null || !line.StartsWith("tagger ")) return new TagParseExpectedTaggerError();
                tb.Tagger = line.Substring("tagger ".Length);

                // Set DateTagged:
                line = sr.ReadLine();
                if (line == null || !line.StartsWith("date ")) return new TagParseExpectedDateError();

                // NOTE: date parsing will result in an inexact DateTimeOffset from what was created with, but it
                // is close enough because the SHA-1 hash is calculated using the DateTimeOffset.ToString(), so
                // only the ToString() representations of the DateTimeOffsets need to match.
                DateTimeOffset tmpDate;
                if (!DateTimeOffset.TryParse(line.Substring("date ".Length), out tmpDate))
                    return new TagParseBadDateFormatError();

                tb.DateTagged = tmpDate;

                // Skip empty line:
                line = sr.ReadLine();
                if (line == null || line.Length != 0) return new TagParseExpectedBlankLineError();

                // Set Message:
                tb.Message = sr.ReadToEnd();
            }

            // Create the immutable Tag from the Builder:
            Tag tg = tb;
            // Validate the computed TagID:
            if (tg.ID != id) return new ComputedTagIDMismatchError(tg.ID, id);

            return tg;
        }

        private void deleteTag(Tag tg)
        {
            FileInfo fi = system.getPathByID(tg.ID);
            lock (FileSystem.SystemLock)
            {
                if (fi.Exists) fi.Delete();
            }
            FileInfo fiTracker = system.getTagPathByTagName(tg.Name);
            lock (FileSystem.SystemLock)
            {
                if (fiTracker.Exists) fiTracker.Delete();
            }
        }

        #endregion

        public async Task<Errorable<Tag>> PersistTag(Tag tg)
        {
            await persistTag(tg).ConfigureAwait(continueOnCapturedContext: false);
            return tg;
        }

        public async Task<Errorable<TagID>> DeleteTag(TagID id)
        {
            var etg = await getTag(id).ConfigureAwait(continueOnCapturedContext: false);
            if (etg.HasErrors) return etg.Errors;

            Tag tg = etg.Value;

            deleteTag(tg);
            return tg.ID;
        }

        public async Task<Errorable<TagID>> DeleteTagByName(TagName tagName)
        {
            var eid = await getTagIDByName(tagName).ConfigureAwait(continueOnCapturedContext: false);
            if (eid.HasErrors) return eid.Errors;

            var etg = await getTag(eid.Value).ConfigureAwait(continueOnCapturedContext: false);
            if (etg.HasErrors) return etg.Errors;

            Tag tg = etg.Value;

            deleteTag(tg);

            return eid.Value;
        }

        public Task<Errorable<Tag>> GetTag(TagID id)
        {
            return getTag(id);
        }

        public async Task<Errorable<Tag>> GetTagByName(TagName tagName)
        {
            var eid = await getTagIDByName(tagName).ConfigureAwait(continueOnCapturedContext: false);
            if (eid.HasErrors) return eid.Errors;

            var etg = await getTag(eid.Value).ConfigureAwait(continueOnCapturedContext: false);
            if (etg.HasErrors) return etg.Errors;

            Tag tg = etg.Value;

            // Check that the retrieved TagName matches what we asked for:
            if (tg.Name != tagName) return new TagNameDoesNotMatchExpectedError(tg.Name, tagName);

            return etg;
        }

        private IEnumerable<TagName> getAllTagNames()
        {
            // Create a new stack of an anonymous type:
            var s = new { di = system.getTagsDirectory(), parts = new string[0] }.StackOf();
            while (s.Count > 0)
            {
                var curr = s.Pop();

                // Yield all files as TagNames in this directory:
                FileInfo[] files = curr.di.GetFiles();
                for (int i = 0; i < files.Length; ++i)
                    yield return (TagName)curr.parts.AppendAsArray(files[i].Name);

                // Push all the subdirectories to the stack:
                DirectoryInfo[] dirs = curr.di.GetDirectories();
                for (int i = 0; i < dirs.Length; ++i)
                    s.Push(new { di = dirs[i], parts = curr.parts.AppendAsArray(dirs[i].Name) });
            }
        }

        private async Task<List<Tag>> searchTags(TagQuery query)
        {
            // First, filter tags by name so that we don't have read them all:
            IEnumerable<TagName> filteredTagNames = getAllTagNames();
            if (query.Name != null)
                filteredTagNames = filteredTagNames.Where(tn => tn.ToString().StartsWith(query.Name));

            DateTimeOffset rightNow = DateTimeOffset.Now;

            List<Tag> tags = new List<Tag>();
            foreach (TagName tagName in filteredTagNames)
            {
                var eid = await getTagIDByName(tagName).ConfigureAwait(continueOnCapturedContext: false);
                if (eid.HasErrors) continue;

                var etg = await getTag(eid.Value).ConfigureAwait(continueOnCapturedContext: false);
                if (etg.HasErrors) continue;

                Tag tg = etg.Value;

                // Filter by tagger name:
                if ((query.Tagger != null) &&
                    (!tg.Tagger.StartsWith(query.Tagger)))
                    continue;

                // Filter by date range:
                if (!((!query.DateFrom.HasValue || (tg.DateTagged >= query.DateFrom.Value)) &&
                       (!query.DateTo.HasValue || (tg.DateTagged <= query.DateTo.Value))))
                    continue;

                tags.Add(tg);
            }

            return tags;
        }

        public async Task<FullQueryResponse<TagQuery, Tag>> SearchTags(TagQuery query)
        {
            List<Tag> tags = await searchTags(query);

            // Return our read-only collection:
            return new FullQueryResponse<TagQuery, Tag>(query, new ReadOnlyCollection<Tag>(tags));
        }

        private static readonly Dictionary<TagOrderBy, Func<Tag, object>> orderByFuncs = new Dictionary<TagOrderBy, Func<Tag, object>>
        {
            { TagOrderBy.DateTagged, tg => tg.DateTagged },
            { TagOrderBy.Name, tg => tg.Name },
            { TagOrderBy.Tagger, tg => tg.Tagger }
        };

        private IEnumerable<Tag> orderResults(List<Tag> tags, ReadOnlyCollection<OrderByApplication<TagOrderBy>> orderBy)
        {
            if (orderBy.Count == 0)
                return tags;

            IOrderedEnumerable<Tag> ordered;

            int i = 0;

            if (orderBy[i].Direction == OrderByDirection.Ascending)
                ordered = tags.OrderBy(orderByFuncs[orderBy[i].OrderBy]);
            else
                ordered = tags.OrderByDescending(orderByFuncs[orderBy[i].OrderBy]);

            for (i = 1; i < orderBy.Count; ++i)
                if (orderBy[i].Direction == OrderByDirection.Ascending)
                    ordered = ordered.ThenBy(orderByFuncs[orderBy[i].OrderBy]);
                else
                    ordered = ordered.ThenByDescending(orderByFuncs[orderBy[i].OrderBy]);

            return ordered;
        }

        public async Task<OrderedFullQueryResponse<TagQuery, Tag, TagOrderBy>> SearchTags(TagQuery query, ReadOnlyCollection<OrderByApplication<TagOrderBy>> orderBy)
        {
            // Filter the results:
            List<Tag> tags = await searchTags(query);
            // Order the results:
            IEnumerable<Tag> ordered = orderResults(tags, orderBy);
            tags = ordered.ToList(tags.Count);

            return new OrderedFullQueryResponse<TagQuery, Tag, TagOrderBy>(query, new ReadOnlyCollection<Tag>(tags), orderBy);
        }

        public async Task<PagedQueryResponse<TagQuery, Tag, TagOrderBy>> SearchTags(TagQuery query, ReadOnlyCollection<OrderByApplication<TagOrderBy>> orderBy, PagingRequest paging)
        {
            // Filter the results:
            List<Tag> tags = await searchTags(query);
            // Order the results:
            IEnumerable<Tag> ordered = orderResults(tags, orderBy);
            tags = ordered.ToList(tags.Count);
            // Page the results:
            List<Tag> page = tags.Skip(paging.PageIndex * paging.PageSize).Take(paging.PageSize).ToList(paging.PageSize);

            return new PagedQueryResponse<TagQuery, Tag, TagOrderBy>(query, new ReadOnlyCollection<Tag>(page), orderBy, paging, tags.Count);
        }

        public Task<Errorable<TagID>> ResolvePartialID(TagID.Partial id)
        {
            FileInfo[] fis = system.getPathsByPartialID(id);
            if (fis.Length == 1) return Task.FromResult(TagID.TryParse(id.ToString().Substring(0, 2) + fis[0].Name));
            if (fis.Length == 0) return Task.FromResult((Errorable<TagID>)new TagIDPartialNoResolutionError(id));
            return Task.FromResult((Errorable<TagID>)new TagIDPartialAmbiguousResolutionError(id, fis.SelectAsArray(f => TagID.TryParse(id.ToString().Substring(0, 2) + f.Name).Value)));
        }

        public Task<Errorable<TagID>[]> ResolvePartialIDs(params TagID.Partial[] ids)
        {
            return Task.WhenAll(ids.SelectAsArray(id => ResolvePartialID(id)));
        }
    }
}
