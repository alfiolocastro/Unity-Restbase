using System;
using System.Threading.Tasks;
using UnityEngine;

public static class TaskExtensions
{
    public static Task ContinueWithOnMainThread(this Task task, Action<Task> continuationAction)
    {
        var tcs = new TaskCompletionSource<object>();

        task.ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                Debug.LogError(t.Exception);
                tcs.SetException(t.Exception);
            }
            else if (t.IsCanceled)
            {
                tcs.SetCanceled();
            }
            else
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    try
                    {
                        continuationAction(t);
                        tcs.SetResult(null);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                });
            }
        });

        return tcs.Task;
    }

    public static Task<T> ContinueWithOnMainThread<T>(this Task<T> task, Action<Task<T>> continuationAction)
    {
        var tcs = new TaskCompletionSource<T>();

        task.ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                Debug.LogError(t.Exception);
                tcs.SetException(t.Exception);
            }
            else if (t.IsCanceled)
            {
                tcs.SetCanceled();
            }
            else
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    try
                    {
                        continuationAction(t);
                        tcs.SetResult(t.Result);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                });
            }
        });

        return tcs.Task;
    }
}
