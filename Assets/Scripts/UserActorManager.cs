using UnityEngine;
using System.Collections;
using LA2RTS;
using System.Collections.Generic;
using LA2RTS.LA2Entities;
using System.Linq;
using System;

public class UserActorManager : MonoBehaviour
{

    private L2RTSServerManager SM;
    private SimpleActionQueue _actionQueue;

    public Dictionary<RTSClient, ActorProperties> clientProperties;

    [Header("Prefabs")]
    public GameObject UserActorPrefab;

    [Header("Requred references")]
    public GameObject MainCamera;

    private void OnEnable()
    {
        SM = GameObject.Find("L2RTSServer").GetComponent<L2RTSServerManager>();
        SM.ServerStartedEvent += () =>
        {
            RegisterToEvents();
        };
    }

    void Awake()
    {
        _actionQueue = new SimpleActionQueue();
        clientProperties = new Dictionary<RTSClient, ActorProperties>();
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _actionQueue.InvokeAll();
    }

    private void RegisterToEvents()
    {
        SM.Server.ClientConnectedEvent += AddClientToList;
        SM.Server.ClientDisconnectedEvent += RemoveClientFromList;
    }

    private void AddClientToList(RTSClient cl)
    {
        clientProperties.Add(cl, new ActorProperties());
        cl.StatusPacketEvent += ManagePresence;
    }

    private void CreateActor(RTSClient cl)
    {
        var newActor = Instantiate(UserActorPrefab, WorldUtils.L2ToUnityCoords(cl.UserChar.X, cl.UserChar.Y, 0), WorldUtils.ActorDefaultRotation());
        var uas = newActor.GetComponent<UserActorScript>();
        uas.client = cl;
        var cp = clientProperties[cl];
        cp.actor = newActor;
    }

    private void DestroyActor(RTSClient cl)
    {
        var cp = clientProperties[cl];
        Destroy(cp.actor);
        cp.actor = null;
    }

    private void MoveCameraOnFirstActor(RTSClient cl)
    {   
        _actionQueue.Enqueue(() =>
        {
            var targetPos = WorldUtils.L2ToUnityCoords(cl.UserChar.X, cl.UserChar.Y, 0);
            MainCamera.GetComponent<CameraMovement>().LookAt(targetPos);
        });
    }

    private void ManagePresence(RTSClient cl)
    {
        if (cl.UserChar.Status == LA2UserChar.ClientStatus.InGame)
        {
            if (clientProperties[cl].actor == null)
            {
                if (!clientProperties.Values.Any(p => p.actor != null))
                {
                    cl.FirstPositionPacketEvent += MoveCameraOnFirstActor;
                }

                _actionQueue.Enqueue(() =>
                {
                    CreateActor(cl);
                });
            }
        }
        else
        {
            if (clientProperties[cl].actor != null)
            {
                cl.FirstPositionPacketEvent -= MoveCameraOnFirstActor;
                _actionQueue.Enqueue(() =>
                {
                    DestroyActor(cl);
                });
            }
        }
    }

    private void RemoveClientFromList(RTSClient cl)
    {
        _actionQueue.Enqueue(() =>
        {
            Destroy(clientProperties[cl].actor);
            clientProperties.Remove(cl);
        });
    }

    public void DoForEachSelected(Action<RTSClient> action)
    {
        foreach (RTSClient cl in clientProperties.Keys)
        {
            var cp = clientProperties[cl];
            if (cp != null && cp.isSelected)
            {
                action.Invoke(cl);
            }
        }
    }

}
