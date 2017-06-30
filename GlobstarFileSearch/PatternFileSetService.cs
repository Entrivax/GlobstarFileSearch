using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text.RegularExpressions;

namespace GlobstarFileSearch
{
    public class PatternFileSetService : IPatternFileSetService
    {
        private readonly IFileSystem _fileSystem;

        public PatternFileSetService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public bool IsPattern(string expr)
        {
            return !string.IsNullOrEmpty(expr) && expr.Contains("*");
        }

        public void Execute(string sourceDir, string sourcePattern, out string relSrcDir, out string[] relFilePaths)
        {
            if (string.IsNullOrEmpty(sourceDir))
            {
                throw new ArgumentNullException(string.Format("Argument {0} can't be null", "sourceDir"));
            }

            if (string.IsNullOrEmpty(sourcePattern))
            {
                throw new ArgumentNullException(string.Format("Argument {0} can't be null", "sourcePattern"));
            }

            var originalSourcePattern = sourcePattern;
            sourcePattern = sourcePattern.Sanitise().Replace("./", "");

            if (!sourcePattern.StartsWith("/"))
            {
                sourcePattern = "/" + sourcePattern;
            }

            if (sourcePattern.Contains("/../"))
            {
                throw new ArgumentException(string.Format("Pattern {0} is invalid", originalSourcePattern));
            }

            var sourceDirSanitised = sourceDir.ToSanitisedDir().Replace("/./", "/");

            relSrcDir = "";
            var indexOfFirstStar = sourcePattern.IndexOf("*", StringComparison.InvariantCulture);
            if (indexOfFirstStar < 0)
            {
                var filePath = sourceDirSanitised + sourcePattern.Substring(1);
                var lastSlash = filePath.LastIndexOf("/", StringComparison.InvariantCulture);
                if (lastSlash != sourceDirSanitised.Length - 1)
                {
                    relSrcDir = filePath.Substring(sourceDirSanitised.Length, lastSlash - sourceDirSanitised.Length);
                }
                if (_fileSystem.File.Exists(filePath))
                {
                    relFilePaths = new[] {filePath.Substring(lastSlash + 1)};
                    return;
                }
                throw new System.IO.FileNotFoundException(string.Format("File {0} not found", filePath));
            }
            var indexOfFirstSlashBeforeStar = sourcePattern.LastIndexOf("/", indexOfFirstStar, StringComparison.InvariantCulture);

            if (indexOfFirstSlashBeforeStar > 0)
            {
                relSrcDir = sourcePattern.Substring(1, indexOfFirstSlashBeforeStar - 1);
            }

            var searchDir = _fileSystem.Path.Combine(sourceDirSanitised, relSrcDir).ToSanitisedDir().Replace("/./", "/");

            var filesToTest = _fileSystem.Directory.EnumerateFiles(searchDir, "*", System.IO.SearchOption.AllDirectories);

            var regex = BuildRegex(sourceDirSanitised + sourcePattern.Substring(1));

            var filePaths = new List<string>();

            foreach (var fileToTest in filesToTest)
            {
                var fileSanitised = fileToTest.Sanitise();
                if (regex.IsMatch(fileSanitised))
                {
                    filePaths.Add(fileToTest.Sanitise().Substring(searchDir.Length));
                }
            }
            relFilePaths = filePaths.ToArray();
        }

        private Regex BuildRegex(string patternRaw)
        {
            const string STAR_REPLACEMENT = "[^/]*?";
            const string GLOBSTAR_REPLACEMENT = ".*/+";

            var globastarElementsEscaped = new List<string>();

            foreach (var globstarElement in patternRaw.Split(new[] {"/**/"}, StringSplitOptions.None))
            {
                var starElementsEscaped = new List<string>();

                foreach (var starElement in globstarElement.Split('*'))
                {
                    starElementsEscaped.Add(Regex.Escape(starElement));
                }

                globastarElementsEscaped.Add(string.Join(STAR_REPLACEMENT, starElementsEscaped));
            }

            return new Regex(string.Join(GLOBSTAR_REPLACEMENT, globastarElementsEscaped) + "$");
        }
    }
}