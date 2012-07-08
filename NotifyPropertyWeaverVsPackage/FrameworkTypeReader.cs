using System;
using System.Linq;
using System.Xml.Linq;

public static class FrameworkTypeReader
{

    public static string GetFrameworkType(string projectPath)
    {
        var xDocument = XDocument.Load(projectPath);
        if (GetIsSilverlight(xDocument))
        {
            if (string.Equals(GetTargetFrameworkProfile(xDocument), "WindowsPhone", StringComparison.OrdinalIgnoreCase))
            {
                return "Phone";
            }
            return "Silverlight";
        }
        return "DotNetStandard";
    }


    static string GetTargetFrameworkProfile(XDocument xDocument)
    {
        return xDocument.BuildDescendants("TargetFrameworkProfile")
            .Select(c => c.Value)
            .FirstOrDefault();
    }

    static bool GetIsSilverlight(XDocument xDocument)
    {
        var targetFrameworkIdentifier = xDocument.BuildDescendants("TargetFrameworkIdentifier")
            .Select(c => c.Value)
            .FirstOrDefault();
        return (string.Equals(targetFrameworkIdentifier, "Silverlight", StringComparison.OrdinalIgnoreCase));
    }
}