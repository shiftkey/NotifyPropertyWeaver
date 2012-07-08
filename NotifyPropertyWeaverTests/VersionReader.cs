using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

public class VersionReader
{
    public decimal FrameworkVersionAsNumber { get; set; }
    public string FrameworkVersionAsString { get; set; }
    public string TargetFrameworkProfile { get; set; }
    public bool IsSilverlight { get; set; }

    public VersionReader(string projectPath)
    {
        var xDocument = XDocument.Load(projectPath);
        GetTargetFrameworkIdentifier(xDocument);
        GetFrameworkVersion(xDocument);
        GetTargetFrameworkProfile(xDocument);
    }

    void GetFrameworkVersion(XDocument xDocument)
    {
        FrameworkVersionAsString = xDocument.BuildDescendants("TargetFrameworkVersion")
            .Select(c => c.Value)
            .First();
        FrameworkVersionAsNumber = decimal.Parse(FrameworkVersionAsString.Remove(0, 1), CultureInfo.InvariantCulture);
    }


    void GetTargetFrameworkProfile(XDocument xDocument)
    {
        TargetFrameworkProfile = xDocument.BuildDescendants("TargetFrameworkProfile")
            .Select(c => c.Value)
            .FirstOrDefault();
    }

    void GetTargetFrameworkIdentifier(XDocument xDocument)
    {
        var targetFrameworkIdentifier = xDocument.BuildDescendants("TargetFrameworkIdentifier")
            .Select(c => c.Value)
            .FirstOrDefault();
        if (string.Equals(targetFrameworkIdentifier, "Silverlight", StringComparison.OrdinalIgnoreCase))
        {
            IsSilverlight = true;
        }

    }

}