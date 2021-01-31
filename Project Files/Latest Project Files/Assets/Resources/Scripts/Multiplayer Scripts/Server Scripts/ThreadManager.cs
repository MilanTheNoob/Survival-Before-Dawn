using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    private static readonly List<Action> executeOnMainThread = new List<Action>();
    private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
    private static bool actionToExecuteOnMainThread = false;

    // Called per frame
    private void Update()
    {
        // Call a func you knob!
        UpdateMain();
    }

    // Sets an action to be executed on the main thread
    public static void ExecuteOnMainThread(Action _action)
    {
        if (_action == null)
            return;

        // Some code I don't understamd
        lock (executeOnMainThread)
        {
            executeOnMainThread.Add(_action);
            actionToExecuteOnMainThread = true;
        }
    }

    // Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread
    public static void UpdateMain()
    {
        // More code I don't understand
        if (actionToExecuteOnMainThread)
        {
            executeCopiedOnMainThread.Clear();
            lock (executeOnMainThread)
            {
                executeCopiedOnMainThread.AddRange(executeOnMainThread);
                executeOnMainThread.Clear();
                actionToExecuteOnMainThread = false;
            }

            for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
            {
                executeCopiedOnMainThread[i]();
            }
        }
    }
}
