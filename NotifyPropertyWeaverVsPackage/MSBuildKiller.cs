
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class MSBuildKiller
{
    public void Kill()
    {
        try
        {
            var id = Process.GetCurrentProcess().Id;
            foreach (var process in Process.GetProcessesByName("MSBuild"))
            {
                if (process.GetParentProcess().Id == id)
                {
                    process.Kill();
                }
            }
        }
            // ReSharper disable EmptyGeneralCatchClause
        catch (Exception)
        {
        }
        // ReSharper restore EmptyGeneralCatchClause
    }
}