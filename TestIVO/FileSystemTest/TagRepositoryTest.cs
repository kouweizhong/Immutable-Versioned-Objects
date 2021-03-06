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
    public class TagRepositoryTest : FileSystemTestBase<CommonTest.TagRepositoryTestMethods>
    {
        protected override CommonTest.TagRepositoryTestMethods getTestMethods(FileSystem system)
        {
            IStreamedBlobRepository blrepo = new StreamedBlobRepository(system);
            ITreeRepository trrepo = new TreeRepository(system);
            ICommitRepository cmrepo = new CommitRepository(system);
            ITagRepository tgrepo = new TagRepository(system);
            IRefRepository rfrepo = new RefRepository(system);

            return new CommonTest.TagRepositoryTestMethods(cmrepo, blrepo, trrepo, tgrepo, rfrepo);
        }

        [TestMethod()]
        public void PersistTagTest()
        {
            runTestMethod(tm => tm.PersistTagTest());
        }

        [TestMethod()]
        public void DeleteTagTest()
        {
            runTestMethod(tm => tm.DeleteTagTest());
        }

        [TestMethod()]
        public void DeleteTagByNameTest()
        {
            runTestMethod(tm => tm.DeleteTagByNameTest());
        }

        [TestMethod()]
        public void GetTagTest()
        {
            runTestMethod(tm => tm.GetTagTest());
        }

        [TestMethod()]
        public void GetTagByNameTest()
        {
            runTestMethod(tm => tm.GetTagByNameTest());
        }
    }
}
