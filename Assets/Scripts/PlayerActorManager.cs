using UnityEngine;
using System.Collections;
using LA2RTS;
using System.Collections.Generic;
using LA2RTS.LA2Entities;
using System.Linq;
using System;

public class PlayerActorManager : MonoBehaviour
{
    private L2RTSServerManager SM;
    private SimpleActionQueue _actionQueue;

    public Dictionary<LA2Char, ActorProperties> playerProperties;

    [Header("Prefabs")]
    public GameObject PlayerActorPrefab;

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
        playerProperties = new Dictionary<LA2Char, ActorProperties>();
    }

    private void RegisterToEvents()
    {
        SM.Server.NewPlayerEvent += AddPlayer;
    }

    private void AddPlayer(LA2Char pl)
    {
        playerProperties.Add(pl, new ActorProperties());
        {
            _actionQueue.Enqueue(() =>
            {
                var newActor = Instantiate(PlayerActorPrefab, WorldUtils.L2ToUnityCoords(pl.X, pl.Y, 0), WorldUtils.ActorDefaultRotation());
                var pas = newActor.GetComponent<PlayerActorScript>();
                pas.player = pl;
                pl.ExpiredEvent += RemovePlayer;
                var pp = playerProperties[pl];
                pp.actor = newActor;
            });
        }
    }

    private void RemovePlayer(LA2Char pl)
    {
        _actionQueue.Enqueue(() =>
        {
            var pp = playerProperties[pl];
            Destroy(pp.actor);
            playerProperties.Remove(pl);
        });

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
}
