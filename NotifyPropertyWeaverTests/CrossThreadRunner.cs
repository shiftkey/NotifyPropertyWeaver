using System;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;

public class CrossThreadRunner
{
    Exception lastException;

    void RunInMTA(ThreadStart userDelegate)
    {
        Run(userDelegate, ApartmentState.MTA);
    }

    public void RunInMTA(Action function)
    {
        RunInMTA(new ThreadStart(() => function()));
    }

    void RunInSTA(ThreadStart userDelegate)
    {
        Run(userDelegate, ApartmentState.STA);
    }

    public void RunInSTA(Action function)
    {
        RunInSTA(new ThreadStart(() => function()));
    }

    void Run(ThreadStart userDelegate, ApartmentState apartmentState)
    {
        lastException = null;

        var thread = new Thread(() => MultiThreadedWorker(userDelegate));
        thread.SetApartmentState(apartmentState);

        thread.Start();
        thread.Join();

        if (ExceptionWasThrown())
        {
            ThrowExceptionPreservingStack(lastException);
        }
    }

    void MultiThreadedWorker(ThreadStart userDelegate)
    {
        try
        {
            userDelegate.Invoke();
        }
        catch (Exception e)
        {
            lastException = e;
        }
    }

    bool ExceptionWasThrown()
    {
        return lastException != null;
    }

    [ReflectionPermission(SecurityAction.Demand)]
    static void ThrowExceptionPreservingStack(Exception exception)
    {
        var remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
        remoteStackTraceString.SetValue(exception, exception.StackTrace + Environment.NewLine);
        throw exception;
    }
}