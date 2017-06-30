using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using GlobstarFileSearch;
using NUnit.Framework;

namespace GlobstarFileSearchTest
{
    [TestFixture]
    public class PatternFileSetServiceTest
    {
        #region Private fields

        private MockFileSystem _fs;
        private PatternFileSetService _pfss;

        #endregion

        #region SetUp

        [SetUp]
        public void SetUp()
        {
            _fs = new MockFileSystem();
            _pfss = new PatternFileSetService(_fs);
        }

        #endregion

        #region Tests

        [TestCase(@"C:\a\b\c", "/*")]
        [TestCase(@"C:\a\b\c\", "/*")]
        [TestCase(@"C:\a\b\c", "*")]
        [TestCase(@"C:\a\b\c\", "*")]
        [TestCase(@"C:\a\b\.\c\", "*")]
        [TestCase(@"C:\a\b\c", "./*")]
        [TestCase(@"C:\a\b\c\", "./*")]
        public void StarTest(string sourceDir, string pattern)
        {
            _fs.AddFile(@"C:\a\b\c\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\c\test.txt", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\c\pouet.jpg", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\c\RoulageDeTete.c", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\c\d\TropLoin.c", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\PasLeMemeDossier\PasTouche.c", MockFileData.NullObject);

            string relSrcDir;
            string[] relFilePaths;
            _pfss.Execute(sourceDir, pattern, out relSrcDir, out relFilePaths);


            Assert.AreEqual("", relSrcDir);
            CollectionAssert.AreEquivalent(new[] {"test", "test.txt", "pouet.jpg", "RoulageDeTete.c"}, relFilePaths);
        }

        [TestCase(@"C:\a\", "*/c/test")]
        [TestCase(@"C:\a", "*/c/test")]
        [TestCase(@"C:\a\", "/*/c/test")]
        [TestCase(@"C:\a", "/*/c/test")]
        [TestCase(@"C:\.\a", "/*/c/test")]
        [TestCase(@"C:\a\", "./*/c/test")]
        [TestCase(@"C:\a", "./*/c/test")]
        public void StarTest2(string sourceDir, string pattern)
        {
            _fs.AddFile(@"C:\a\b\c\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\d\c\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\j\c\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\c\c\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\c\pouet.jpg", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\c\RoulageDeTete.c", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\PasLeMemeDossier\PasTouche.c", MockFileData.NullObject);

            string relSrcDir;
            string[] relFilePaths;
            _pfss.Execute(sourceDir, pattern, out relSrcDir, out relFilePaths);

            Assert.AreEqual("", relSrcDir);
            CollectionAssert.AreEquivalent(new[] {"b/c/test", "d/c/test", "j/c/test", "c/c/test"}, relFilePaths);
        }

        [TestCase(@"C:", "a/*/c/test")]
        [TestCase(@"C:\", "a/*/c/test")]
        [TestCase(@"C:", "/a/*/c/test")]
        [TestCase(@"C:\", "/a/*/c/test")]
        [TestCase(@"C:\.\", "/a/*/c/test")]
        [TestCase(@"C:", "./a/*/c/test")]
        [TestCase(@"C:\", "./a/*/c/test")]
        public void StarTest3(string sourceDir, string pattern)
        {
            _fs.AddFile(@"C:\a\b\c\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\d\c\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\j\c\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\c\c\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\c\pouet.jpg", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\c\RoulageDeTete.c", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\PasLeMemeDossier\PasTouche.c", MockFileData.NullObject);

            string relSrcDir;
            string[] relFilePaths;
            _pfss.Execute(sourceDir, pattern, out relSrcDir, out relFilePaths);

            Assert.AreEqual("a", relSrcDir);
            CollectionAssert.AreEquivalent(new[] {"b/c/test", "d/c/test", "j/c/test", "c/c/test"}, relFilePaths);
        }

        [TestCase(@"C:", "a/**/test")]
        [TestCase(@"C:\", "a/**/test")]
        [TestCase(@"C:", "/a/**/test")]
        [TestCase(@"C:\", "/a/**/test")]
        [TestCase(@"C:\.\", "/a/**/test")]
        [TestCase(@"C:", "./a/**/test")]
        [TestCase(@"C:\", "./a/**/test")]
        public void GlobstarTest(string sourceDir, string pattern)
        {
            _fs.AddFile(@"C:\a\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\c\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\d\c\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\j\d\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\c\c\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\c\pouet.jpg", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\c\RoulageDeTete.c", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\PasLeMemeDossier\PasTouche.c", MockFileData.NullObject);

            string relSrcDir;
            string[] relFilePaths;
            _pfss.Execute(sourceDir, pattern, out relSrcDir, out relFilePaths);

            Assert.AreEqual("a", relSrcDir);
            CollectionAssert.AreEquivalent(new[] {"test", "b/test", "b/c/test", "d/c/test", "j/d/test", "c/c/test"}, relFilePaths);
        }

        [TestCase(@"C:", ".a/*")]
        [TestCase(@"C:", "/.a/*")]
        [TestCase(@"C:\", "/.a/*")]
        [TestCase(@"C:\", ".a/*")]
        public void StarTestDots(string sourceDir, string pattern)
        {
            _fs.AddFile(@"C:\.a\test", MockFileData.NullObject);

            string relSrcDir;
            string[] relFilePaths;
            _pfss.Execute(sourceDir, pattern, out relSrcDir, out relFilePaths);

            Assert.AreEqual(".a", relSrcDir);
            CollectionAssert.AreEquivalent(new[] {"test"}, relFilePaths);
        }

        [TestCase(@"C:\.a", "*")]
        [TestCase(@"C:\.a", "/*")]
        [TestCase(@"C:\.a\", "/*")]
        [TestCase(@"C:\.a\", "*")]
        public void StarTestDots2(string sourceDir, string pattern)
        {
            _fs.AddFile(@"C:\.a\test", MockFileData.NullObject);

            string relSrcDir;
            string[] relFilePaths;
            _pfss.Execute(sourceDir, pattern, out relSrcDir, out relFilePaths);

            Assert.AreEqual(string.Empty, relSrcDir);
            CollectionAssert.AreEquivalent(new[] {"test"}, relFilePaths);
        }

        [Test]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void DirectoryNotFoundTest()
        {
            _fs.AddFile(@"C:\a\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\test", MockFileData.NullObject);
            _fs.AddFile(@"C:\a\b\c\pouet.jpg", MockFileData.NullObject);

            string relSrcDir;
            string[] relFilePaths;
            _pfss.Execute(@"C:\a", "DoNotExists/*", out relSrcDir, out relFilePaths);
        }


        [TestCase(@"C:\a", "test.js")]
        [TestCase(@"C:\a", "/test.js")]
        [TestCase(@"C:\a\", "/test.js")]
        [TestCase(@"C:\a\", "test.js")]
        public void SpecificFileTest(string sourceDir, string file)
        {
            _fs.AddFile(@"C:\a\test.js", MockFileData.NullObject);

            string relSrcDir;
            string[] relFilePaths;
            _pfss.Execute(sourceDir, file, out relSrcDir, out relFilePaths);

            Assert.AreEqual(string.Empty, relSrcDir);
            CollectionAssert.AreEquivalent(new[] {"test.js"}, relFilePaths);
        }

        [TestCase(@"C:", "a/test")]
        [TestCase(@"C:", "/a/test")]
        [TestCase(@"C:\", "/a/test")]
        [TestCase(@"C:\", "a/test")]
        public void SpecificFileTest2(string sourceDir, string file)
        {
            _fs.AddFile(@"C:\a\test", MockFileData.NullObject);

            string relSrcDir;
            string[] relFilePaths;
            _pfss.Execute(sourceDir, file, out relSrcDir, out relFilePaths);

            Assert.AreEqual("a", relSrcDir);
            CollectionAssert.AreEquivalent(new[] {"test"}, relFilePaths);
        }

        [TestCase(@"C:", ".a/test")]
        [TestCase(@"C:", "/.a/test")]
        [TestCase(@"C:\", "/.a/test")]
        [TestCase(@"C:\", ".a/test")]
        public void SpecificFileTestDot(string sourceDir, string pattern)
        {
            _fs.AddFile(@"C:\.a\test", MockFileData.NullObject);

            string relSrcDir;
            string[] relFilePaths;
            _pfss.Execute(sourceDir, pattern, out relSrcDir, out relFilePaths);

            Assert.AreEqual(".a", relSrcDir);
            CollectionAssert.AreEquivalent(new[] {"test"}, relFilePaths);
        }

        [TestCase(@"C:\.a", "test")]
        [TestCase(@"C:\.a", "/test")]
        [TestCase(@"C:\.a\", "/test")]
        [TestCase(@"C:\.a\", "test")]
        public void SpecificFileTestDot2(string sourceDir, string pattern)
        {
            _fs.AddFile(@"C:\.a\test", MockFileData.NullObject);

            string relSrcDir;
            string[] relFilePaths;
            _pfss.Execute(sourceDir, pattern, out relSrcDir, out relFilePaths);

            Assert.AreEqual(string.Empty, relSrcDir);
            CollectionAssert.AreEquivalent(new[] {"test"}, relFilePaths);
        }

        [TestCase(@"C:\a", "/NotFound")]
        [TestCase(@"C:\a", "NotFound")]
        [TestCase(@"C:\a\", "NotFound")]
        [TestCase(@"C:\a\", "/NotFound")]
        [ExpectedException(typeof(FileNotFoundException))]
        public void SpecificFileNotFoundTest(string sourceDir, string pattern)
        {
            _fs.AddFile(@"C:\a\test", MockFileData.NullObject);

            string relSrcDir;
            string[] relFilePaths;
            _pfss.Execute(sourceDir, pattern, out relSrcDir, out relFilePaths);
        }

        [TestCase("", "dsf")]
        [TestCase("dsd", "")]
        [TestCase(null, "dsf")]
        [TestCase("dsd", null)]
        [TestCase("", "")]
        [TestCase(null, null)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNullTest(string sourceDir, string pattern)
        {
            string relSrcDir;
            string[] relFilePaths;
            _pfss.Execute(sourceDir, pattern, out relSrcDir, out relFilePaths);
        }

        [Test]
        public void IsPattern()
        {
            Assert.IsFalse(_pfss.IsPattern("dfdfsdf"));
            Assert.IsFalse(_pfss.IsPattern(""));
            Assert.IsTrue(_pfss.IsPattern("dfdf*sdf"));
            Assert.IsTrue(_pfss.IsPattern("dfdf*sd*f"));
            Assert.IsTrue(_pfss.IsPattern("*"));
        }

        #endregion
    }
}