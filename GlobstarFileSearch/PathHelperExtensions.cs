namespace GlobstarFileSearch
{
    internal static class PathHelperExtensions
    {
        public static string Sanitise(this string path)
        {
            return path.Replace(@"\", "/");
        }

        public static string ToSanitisedDir(this string path)
        {
            var sanitisedPath = path.Sanitise();
            if (!sanitisedPath.EndsWith("/"))
            {
                sanitisedPath = sanitisedPath + "/";
            }
            return sanitisedPath;
        }
    }
}
