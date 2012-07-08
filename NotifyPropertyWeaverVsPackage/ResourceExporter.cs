using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class ResourceExporter
{

    public virtual bool Export(string key, FileInfo fileInfo)
    {
        var callingAssembly = Assembly.GetCallingAssembly();
        fileInfo.Directory.Create();
        try
        {
            if (fileInfo.Exists)
            {
                fileInfo.IsReadOnly = false;
                fileInfo.Delete();
            }
            using (var resource = callingAssembly.GetManifestResourceStream(string.Format("{0}.Resources.{1}", callingAssembly.GetName().Name, key)))
            using (var stream = fileInfo.OpenWrite())
            {
                resource.CopyTo(stream);
            }
            return true;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (Exception)
        {
            //TODO: log
            return false;
        }
    }
}