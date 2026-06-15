using System;
using System.IO;

namespace Isomerization.UI.Misc;

public static class ResourcePaths
{
    public static string Resolve(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return string.Empty;
        }

        if (Path.IsPathRooted(relativePath))
        {
            return relativePath;
        }

        return Path.Combine(
            AppContext.BaseDirectory,
            relativePath.Replace('/', Path.DirectorySeparatorChar));
    }

    public static bool Exists(string? relativePath)
    {
        var path = Resolve(relativePath);
        return !string.IsNullOrEmpty(path) && File.Exists(path);
    }
}
