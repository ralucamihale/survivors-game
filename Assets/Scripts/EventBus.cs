using System;
using System.Collections.Generic;

public class EventBus
{
    public static EventBus Instance { get; } = new EventBus();
    private EventBus() { }

    private readonly Dictionary<Type, List<object>> _subscribers = new();

    public void Subscribe<T>(Action<T> listener) where T : class
    {
        var eventType = typeof(T);
        if (!_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType] = new List<object>();
        }
        _subscribers[eventType].Add(listener);
    }

    public void Unsubscribe<T>(Action<T> listener) where T : class
    {
        var eventType = typeof(T);
        if (_subscribers.TryGetValue(eventType, out var subscribers))
        {
            subscribers.Remove(listener);
        }
    }

    public void Broadcast<T>(T eventData) where T : class
    {
        var eventType = typeof(T);
        if (_subscribers.TryGetValue(eventType, out var subscribers))
        {
            foreach (var subscriber in subscribers.ToArray())
            {
                (subscriber as Action<T>)?.Invoke(eventData);
            }
        }
    }
}