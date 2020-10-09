using UnityEngine;
using System.Collections;
using System;
using LA2RTS.LA2Entities;
using LA2RTS;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public class SelectionManager : MonoBehaviour
{
    public event Action SelectionChangedEvent;

    [HideInInspector]
    public LinkedList<ActorScript> selectedActors;

    private L2RTSServerManager SM;

    protected void Awake()
    {
        selectedActors = new LinkedList<ActorScript>();
    }

    void Start()
    {
        SM = FindObjectOfType<L2RTSServerManager>();
    }

    void Update()
    {

    }

    public void ClearSelection()
    {
        if (selectedActors.Count == 0)
            return;

        foreach (ActorScript actor in selectedActors)
        {
            ActorProperties props = null;
            if (actor is UserActorScript userActor)
                props = SM.userActorManager.clientProperties[userActor.client];
            else if (actor is PlayerActorScript playerActor)
                props = SM.playerActorManager.playerProperties[playerActor.player];
            else if (actor is NPCActorScript npcActor)
                props = SM.npcActorManager.npcProperties[npcActor.npc];

            if (props == null || !props.isSelected) continue;
            props.isSelected = false;
        }

        selectedActors.Clear();
        SelectionChangedEvent?.Invoke();
    }

    private void ClearAllButUserSelection()
    {
        if (selectedActors.Count == 0)
            return;

        LinkedList<ActorScript> newList = new LinkedList<ActorScript>();
        foreach (ActorScript actor in selectedActors)
        {
            ActorProperties props = null;
            if (actor is PlayerActorScript playerActor)
                props = SM.playerActorManager.playerProperties[playerActor.player];
            else if (actor is NPCActorScript npcActor)
                props = SM.npcActorManager.npcProperties[npcActor.npc];
            else
                newList.AddLast(actor);

            if (props == null || !props.isSelected) continue;
            props.isSelected = false;
        }

        selectedActors = newList;

    }

    private void ClearAllButPlayerSelection()
    {
        if (selectedActors.Count == 0)
            return;

        LinkedList<ActorScript> newList = new LinkedList<ActorScript>();
        foreach (ActorScript actor in selectedActors)
        {
            ActorProperties props = null;
            if (actor is UserActorScript userActor)
                props = SM.userActorManager.clientProperties[userActor.client];
            else if (actor is NPCActorScript npcActor)
                props = SM.npcActorManager.npcProperties[npcActor.npc];
            else
                newList.AddLast(actor);

            if (props == null || !props.isSelected) continue;
            selectedActors.Remove(actor);
            props.isSelected = false;
        }

        selectedActors = newList;
    }

    private void ClearAllButNpcSelection()
    {
        if (selectedActors.Count == 0)
            return;

        LinkedList<ActorScript> newList = new LinkedList<ActorScript>();
        foreach (ActorScript actor in selectedActors)
        {
            ActorProperties props = null;
            if (actor is UserActorScript userActor)
                props = SM.userActorManager.clientProperties[userActor.client];
            else if (actor is PlayerActorScript playerActor)
                props = SM.playerActorManager.playerProperties[playerActor.player];
            else
                newList.AddLast(actor);

            if (props == null || !props.isSelected) continue;
            selectedActors.Remove(actor);
            props.isSelected = false;
        }

        selectedActors = newList;
    }

    public void SelectUser(RTSClient cl)
    {
        foreach (var kv in SM.userActorManager.clientProperties)
        {
            if (kv.Key == cl)
            {
                if (!kv.Value.isSelected)
                {
                    kv.Value.isSelected = true;
                    if (kv.Value.actor != null)
                        selectedActors.AddLast(kv.Value.actor.GetComponent<ActorScript>());
                }
            }
            else if (kv.Value.isSelected)
            {
                kv.Value.isSelected = false;
                if (kv.Value.actor != null)
                    selectedActors.Remove(kv.Value.actor.GetComponent<ActorScript>());
            }

        }

        ClearAllButUserSelection();

        SelectionChangedEvent?.Invoke();
    }

    public void AddUserSelection(RTSClient cl)
    {
        foreach (var kv in SM.userActorManager.clientProperties)
        {
            if (kv.Key == cl)
            {
                if (!kv.Value.isSelected)
                {
                    kv.Value.isSelected = true;
                    if (kv.Value.actor != null)
                        selectedActors.AddLast(kv.Value.actor.GetComponent<ActorScript>());
                }
                break;
            }
        }

        ClearAllButUserSelection();

        SelectionChangedEvent?.Invoke();
    }

    public void RemoveUserSelection(RTSClient cl)
    {
        foreach (var kv in SM.userActorManager.clientProperties)
        {
            if (kv.Value == null)
                continue;

            if (kv.Key == cl)
            {
                if (kv.Value.isSelected)
                {
                    kv.Value.isSelected = false;
                    if (kv.Value.actor != null)
                        selectedActors.Remove(kv.Value.actor.GetComponent<ActorScript>());
                }
                break;
            }
        }

        SelectionChangedEvent?.Invoke();
    }

    public void SelectMultipleUsers(IEnumerable<RTSClient> clientsToSelect)
    {
        foreach (var kv in SM.userActorManager.clientProperties)
        {

            if (clientsToSelect.Contains(kv.Key))
            {
                if (!kv.Value.isSelected)
                {
                    kv.Value.isSelected = true;
                    if (kv.Value.actor != null)
                        selectedActors.AddLast(kv.Value.actor.GetComponent<ActorScript>());
                }
            }
            else if (kv.Value.isSelected)
            {
                kv.Value.isSelected = false;
                if (kv.Value.actor != null)
                    selectedActors.Remove(kv.Value.actor.GetComponent<ActorScript>());
            }
                    
        }

        ClearAllButUserSelection();

        SelectionChangedEvent?.Invoke();
    }

    public void AddUserSelection(IEnumerable<RTSClient> clientsToSelect)
    {
        foreach (var kv in SM.userActorManager.clientProperties)
        {

            if (clientsToSelect.Contains(kv.Key))
            {
                if (!kv.Value.isSelected)
                {
                    kv.Value.isSelected = true;
                    if (kv.Value.actor != null)
                        selectedActors.AddLast(kv.Value.actor.GetComponent<ActorScript>());
                }
            }
        }

        ClearAllButUserSelection();

        SelectionChangedEvent?.Invoke();
    }

    public void InverseUserSelection(IEnumerable<RTSClient> clientsToSelect)
    {
        foreach (var kv in SM.userActorManager.clientProperties)
        {

            if (clientsToSelect.Contains(kv.Key))
            {
                if (!kv.Value.isSelected)
                {
                    kv.Value.isSelected = true;
                    if (kv.Value.actor != null)
                        selectedActors.AddLast(kv.Value.actor.GetComponent<ActorScript>());
                }
                else
                {
                    kv.Value.isSelected = false;
                    if (kv.Value.actor != null)
                        selectedActors.Remove(kv.Value.actor.GetComponent<ActorScript>());
                }
            }
        }

        ClearAllButUserSelection();

        SelectionChangedEvent?.Invoke();
    }


    public void SelectNPC(LA2NPC npc)
    {
        foreach (var kv in SM.npcActorManager.npcProperties)
        {
            if (kv.Key == npc)
                if (kv.Value.isSelected)
                    return;
                else
                {
                    kv.Value.isSelected = true;
                    selectedActors.AddLast(kv.Value.actor.GetComponent<ActorScript>());
                }
            else if (kv.Value.isSelected)
            {
                kv.Value.isSelected = false;
                selectedActors.Remove(kv.Value.actor.GetComponent<ActorScript>());
            }
        }

        ClearAllButNpcSelection();

        SelectionChangedEvent?.Invoke();
    }

    public void SelectPlayer(LA2Char pl)
    {
        foreach (var kv in SM.playerActorManager.playerProperties)
        {
            if (kv.Key == pl)
                if (kv.Value.isSelected)
                    return;
                else
                {
                    kv.Value.isSelected = true;
                    selectedActors.AddLast(kv.Value.actor.GetComponent<ActorScript>());
                }
            else if (kv.Value.isSelected)
            {
                kv.Value.isSelected = false;
                selectedActors.Remove(kv.Value.actor.GetComponent<ActorScript>());
            }
        }

        ClearAllButPlayerSelection();

        SelectionChangedEvent?.Invoke();
    }
}
