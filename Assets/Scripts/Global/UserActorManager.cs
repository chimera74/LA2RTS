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

    [Header("Required references")]
    public GameObject MainCamera;

    private void OnEnable()
    {
        SM = GameObject.Find("L2RTSServer").GetComponent<L2RTSServerManager>();
        SM.ServerStartedEvent += RegisterToEvents;
    }

    protected void Awake()
    {
        _actionQueue = new SimpleActionQueue();
        clientProperties = new Dictionary<RTSClient, ActorProperties>();
    }

    // Update is called once per frame
    protected void Update()
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
        cp.gameObject = newActor;
        cp.actorScript = uas;
    }

    private void DestroyActor(RTSClient cl)
    {
        var cp = clientProperties[cl];
        Destroy(cp.gameObject);
        cp.gameObject = null;
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
            if (clientProperties[cl].gameObject == null)
            {
                if (clientProperties.Values.All(p => p.gameObject == null))
                {
                    cl.FirstPositionPacketEvent += MoveCameraOnFirstActor;
                }

                _actionQueue.Enqueue(() =>
                {
                    CreateActor(cl);
                    SM.selectionManager.UpdateSelectedUsers();
                });
            }
        }
        else
        {
            if (clientProperties[cl].gameObject != null)
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
            Destroy(clientProperties[cl].gameObject);
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
