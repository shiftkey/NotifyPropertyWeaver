using System;
using System.ComponentModel.Composition;
using System.Text;
using Microsoft.Build.Framework;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class Logger
{
    public bool ErrorHasBeenRaised { get; set; }
    public string SenderName { get; set; }
    public IBuildEngine BuildEngine { get; set; }
    public MessageImportance MessageImportance { get; set; }

    StringBuilder stringBuilder;

    public virtual void Initialise(string messageImportance)
    {
        stringBuilder = new StringBuilder();
        MessageImportance messageImportanceEnum;
        if (!Enum.TryParse(messageImportance, out messageImportanceEnum))
        {
            throw new WeavingException(string.Format("Invalid MessageImportance in config. Should be 'Low', 'Normal' or 'High' but was '{0}'.", messageImportance));
        }
        MessageImportance = messageImportanceEnum;
    }

    public virtual void LogWarning(string message)
    {
        BuildEngine.LogWarningEvent(new BuildWarningEventArgs("", "", "", 0, 0, 0, 0, string.Format("{0}: {1}", SenderName, message), "", SenderName));
    }

    public virtual void LogMessage(string message)
    {
        stringBuilder.AppendLine(message);
    }

    public virtual void Flush()
    {
        var message = stringBuilder.ToString();
        message = message.Substring(0, message.Length - 2);
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs(message, "", SenderName, MessageImportance));
    }

    public virtual void LogError(string message)
    {
        ErrorHasBeenRaised = true;
        BuildEngine.LogErrorEvent(new BuildErrorEventArgs("", "", "", 0, 0, 0, 0, string.Format("{0}: {1}", SenderName, message), "", SenderName));
    }

}