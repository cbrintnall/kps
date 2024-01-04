using System;
using System.Collections.Generic;
using UnityEngine;

public class AsyncOperationAggregate
{
    public event Action Done;

    AsyncOperation[] operations;
    int completionCount;

    public AsyncOperationAggregate(params AsyncOperation[] ops)
    {
        operations = ops;
        foreach (var op in ops)
        {
            op.completed += _ =>
            {
                if (completionCount < operations.Length)
                {
                    completionCount++;
                    if (completionCount == operations.Length)
                    {
                        Done?.Invoke();
                    }
                }
            };
        }
    }
}
