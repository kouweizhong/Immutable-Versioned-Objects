﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IVO.Definition.Models;
using IVO.Definition.Repositories;
using System.IO;
using System.Diagnostics;

namespace IVO.Implementation.FileSystem
{
    public sealed class RefRepository : IRefRepository
    {
        private FileSystem system;

        public RefRepository(FileSystem system)
        {
            this.system = system;
        }

        #region Private details

        private void persistRef(Ref rf)
        {
            FileInfo fi = system.getRefPathByRefName(rf.Name);

            // Create directory if it doesn't exist:
            if (!fi.Directory.Exists)
            {
                Debug.WriteLine(String.Format("New DIR '{0}'", fi.Directory.FullName));
                fi.Directory.Create();
            }

            // Write the contents to the file:
            using (var fs = new FileStream(fi.FullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                Debug.WriteLine(String.Format("New REF '{0}'", fi.FullName));
                rf.WriteTo(fs);
            }
        }

        private async Task<Ref> getRefByName(RefName refName)
        {
            FileInfo fiTracker = system.getRefPathByRefName(refName);
            if (!fiTracker.Exists) return (Ref)null;

            byte[] buf;
            int nr = 0;
            using (var fs = new FileStream(fiTracker.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 16384, true))
            {
                // TODO: implement an async buffered Stream:
                buf = new byte[16384];
                nr = await fs.ReadAsync(buf, 0, 16384);
                if (nr >= 16384)
                {
                    // My, what a large tag you have!
                    throw new NotSupportedException();
                }
            }

            // Parse the CommitID:
            using (var ms = new MemoryStream(buf, 0, nr, false))
            using (var sr = new StreamReader(ms, Encoding.UTF8))
            {
                string line = sr.ReadLine();
                if (line == null) return (Ref)null;

                return new Ref.Builder(refName, new CommitID(line));
            }
        }

        private void deleteRef(RefName name)
        {
            FileInfo fi = system.getRefPathByRefName(name);
            if (fi.Exists) fi.Delete();
        }

        #endregion

        public async Task<Ref> PersistRef(Ref rf)
        {
            await TaskEx.Run(() => persistRef(rf));
            return rf;
        }

        public Task<Ref> GetRefByName(RefName name)
        {
            return getRefByName(name);
        }

        public async Task<Ref> DeleteRefByName(RefName name)
        {
            var rf = await getRefByName(name);
            await TaskEx.Run(() => deleteRef(name));
            return rf;
        }
    }
}
