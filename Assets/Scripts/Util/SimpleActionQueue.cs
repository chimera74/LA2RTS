using System.Collections.Generic;
using System;

public class SimpleActionQueue
{
    private System.Object _lock = new System.Object();
    private Queue<Action> _actionQueue;

    public int Count {
        get { return _actionQueue.Count; }
    }

    public SimpleActionQueue()
    {
        _actionQueue = new Queue<Action>();
    }

    public void Enqueue(Action a)
    {
        lock (_lock)
        {
            _actionQueue.Enqueue(a);
        }
    }

    public void InvokeFirst()
    {
        if (_actionQueue.Count == 0)
            return;

        Action dequeuedAction = null;
        lock (_lock)
        {
            dequeuedAction = _actionQueue.Dequeue();
        }

        if (dequeuedAction != null)
            dequeuedAction.Invoke();
    }

    public void InvokeAll()
    {
        while (_actionQueue.Count > 0)
        {
            Action dequeuedAction = null;
            lock (_lock)
            {
                dequeuedAction = _actionQueue.Dequeue();
            }

            if (dequeuedAction != null)
                dequeuedAction.Invoke();
        }
    }
}