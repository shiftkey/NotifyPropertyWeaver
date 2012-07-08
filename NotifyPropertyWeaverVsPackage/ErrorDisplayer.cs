using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class ErrorDisplayer
{
    ErrorListProvider errorProvider;


    public ErrorDisplayer()
    {
    }

    [ImportingConstructor]
    public ErrorDisplayer(ErrorListProvider errorProvider)
    {
        this.errorProvider = errorProvider;
    }

    public virtual void ShowError(string error)
    {
        var errorTask = new ErrorTask
                            {
                                Category = TaskCategory.Misc,
                                Text = error,
                                CanDelete = true,
                            };
        errorProvider.Tasks.Add(errorTask);
    }

    public virtual void ShowInfo(string info)
    {
        var task = new Task
                       {
                           Category = TaskCategory.Misc,
                           Text = info,
                           CanDelete = true,
                       };
        errorProvider.Tasks.Add(task);
    }

}