using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void EventDispatch<T>(T ev)
    where T : BaseEvent;

public class BaseEvent
{
    public string ID = Guid.NewGuid().ToString();

    public void Emit()
    {
        SingletonLoader.Get<EventManager>().Publish(this);
    }
}

[Singleton]
public class EventManager : MonoBehaviour
{
    private Dictionary<Type, List<Delegate>> subscribers = new();

    public void Subscribe<T>(Action<T> callback)
        where T : BaseEvent
    {
        if (!subscribers.ContainsKey(typeof(T)))
        {
            subscribers[typeof(T)] = new();
        }

        subscribers[typeof(T)].Add(callback);
    }

    public void Publish<T>(T data)
        where T : BaseEvent
    {
        if (!subscribers.ContainsKey(typeof(T)))
            return;

        List<Delegate> removals = new();
        foreach (var subscriber in subscribers[typeof(T)])
        {
            if (subscriber is Action<T> subscribed)
            {
                try
                {
                    subscribed(data);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Subscribed threw exception {e.GetType()}, removing.");
                    removals.Add(subscriber);
                }
            }
        }

        foreach (var removal in removals)
        {
            subscribers[typeof(T)].Remove(removal);
        }
    }
}
