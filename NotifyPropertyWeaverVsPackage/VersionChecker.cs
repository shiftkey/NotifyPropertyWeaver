using System.IO;
using System.Reflection;

public static class VersionChecker
{
    public static bool IsVersionNewer(FileInfo targetFile)
    {
        var existingVersion = AssemblyName.GetAssemblyName(targetFile.FullName).Version;
        return existingVersion < CurrentVersion.Version;
    }
}