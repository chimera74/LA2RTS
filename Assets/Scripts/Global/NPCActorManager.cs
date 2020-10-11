using UnityEngine;
using System.Collections;
using LA2RTS;
using System.Collections.Generic;
using LA2RTS.LA2Entities;
using System.Linq;
using System;

public class NPCActorManager : MonoBehaviour
{
    // ReSharper disable once InconsistentNaming
    private L2RTSServerManager SM;
    private SimpleActionQueue _actionQueue;

    public Dictionary<LA2NPC, ActorProperties> npcProperties;

    [Header("Prefabs")]
    public GameObject NPCActorPrefab;

    [Header("Requred references")]
    public GameObject MainCamera;

    protected void OnEnable()
    {
        SM = GameObject.Find("L2RTSServer").GetComponent<L2RTSServerManager>();
        SM.ServerStartedEvent += RegisterToEvents;
    }

    protected void Awake()
    {
        _actionQueue = new SimpleActionQueue();
        npcProperties = new Dictionary<LA2NPC, ActorProperties>();
    }

    private void RegisterToEvents()
    {
        SM.Server.NewNpcEvent += AddNPC;
    }

    private void AddNPC(LA2NPC npc)
    {
        npcProperties.Add(npc, new ActorProperties());
        {
            _actionQueue.Enqueue(() =>
            {
                var newActor = Instantiate(NPCActorPrefab, WorldUtils.L2ToUnityCoords(npc.X, npc.Y, 0), WorldUtils.ActorDefaultRotation());
                var nas = newActor.GetComponent<NPCActorScript>();
                nas.npc = npc;
                npc.ExpiredEvent += RemoveNPC;
                var np = npcProperties[npc];
                np.actor = newActor;
            });
        }
    }

    private void RemoveNPC(LA2NPC npc)
    {
        _actionQueue.Enqueue(() =>
        {
            var np = npcProperties[npc];
            Destroy(np.actor);
            npcProperties.Remove(npc);
        });

    }

    protected void Start()
    {

    }

    // Update is called once per frame
    protected void Update()
    {
        _actionQueue.InvokeAll();
    }

}
