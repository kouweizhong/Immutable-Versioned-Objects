﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using IVO.Definition.Containers;
using IVO.Definition.Models;
using IVO.Definition.Repositories;
using IVO.Implementation.FileSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Security.Cryptography;
using IVO.Definition;

namespace TestIVO.FileSystemTest
{
    [TestClass()]
    public class TreeRepositoryTest
    {
        private FileSystem getFileSystem()
        {
            string tmpPath = System.IO.Path.GetTempPath();
            string tmpRoot = System.IO.Path.Combine(tmpPath, "ivo");

            // Delete our temporary 'ivo' folder:
            var tmpdi = new DirectoryInfo(tmpRoot);
            if (tmpdi.Exists)
                tmpdi.Delete(recursive: true);

            FileSystem system = new FileSystem(new DirectoryInfo(tmpRoot));
            return system;
        }

        [TestMethod()]
        public void PersistTreeTest()
        {
            FileSystem system = getFileSystem();
            ITreeRepository trrepo = new TreeRepository(system);

            new CommonTest.TreeRepositoryTestMethods(trrepo).PersistTreeTest().Wait();

            // Clean up:
            if (system.Root.Exists)
                system.Root.Delete(recursive: true);
        }

        [TestMethod()]
        public void GetTreesTest()
        {
            FileSystem system = getFileSystem();
            ITreeRepository trrepo = new TreeRepository(system);

            new CommonTest.TreeRepositoryTestMethods(trrepo).GetTreesTest().Wait();

            // Clean up:
            if (system.Root.Exists)
                system.Root.Delete(recursive: true);
        }
    }
}
