using UnityEngine;
using System.Collections;

public class ButtonPanel : MonoBehaviour
{
    protected SimpleActionQueue _actionQueue;
    protected UserActorManager userActorManager;

    public virtual void Awake()
    {
        _actionQueue = new SimpleActionQueue();
    }
    
    public virtual void Start()
    {
        userActorManager = FindObjectOfType<UserActorManager>();
    }

    public virtual void Update()
    {
        _actionQueue.InvokeAll();
    }

}
