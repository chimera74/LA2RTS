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

    protected void Start()
    {
        SM = FindObjectOfType<L2RTSServerManager>();
    }

    public void ClearSelection()
    {
        ClearUserSelection();
        ClearNpcSelection();
        ClearPlayerSelection();
        SelectionChangedEvent?.Invoke();
    }

    private void ClearUserSelection()
    {
        SM.userActorManager.clientProperties.ForEach((kv) =>
        {
            if (kv.Value != null && kv.Value.isSelected)
                kv.Value.isSelected = false;
        });
    }

    private void ClearPlayerSelection()
    {
        SM.playerActorManager.playerProperties.ForEach((kv) =>
        {
            if (kv.Value != null && kv.Value.isSelected)
                kv.Value.isSelected = false;
        });
    }

    private void ClearNpcSelection()
    {
        SM.npcActorManager.npcProperties.ForEach((kv) =>
        {
            if (kv.Value != null && kv.Value.isSelected)
                kv.Value.isSelected = false;
        });
    }

    /// <summary>
    /// Check properties for all UserActors and update selection list according to them.
    /// </summary>
    public void UpdateSelectedUsers()
    {
        bool isUserSelected = false;
        foreach (var kv in SM.userActorManager.clientProperties)
        {
            if (kv.Value.actor == null)
                continue;
            var uas = kv.Value.actor.GetComponent<UserActorScript>();
            if (kv.Value.isSelected)
            {
                if (!selectedActors.Contains(uas))
                    selectedActors.AddLast(uas);
            }
            else
            {
                if (selectedActors.Contains(uas))
                {
                    selectedActors.AddLast(uas);
                    isUserSelected = true;
                }
            }
        }

        if (isUserSelected)
        {
            ClearPlayerSelection();
            ClearNpcSelection();
            SelectionChangedEvent?.Invoke();
        }
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
                }
            }
            else if (kv.Value.isSelected)
            {
                kv.Value.isSelected = false;
            }

        }

        ClearPlayerSelection();
        ClearNpcSelection();

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
                }
                break;
            }
        }

        ClearPlayerSelection();
        ClearNpcSelection();

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
                }
            }
            else 
                if (kv.Value.isSelected)
                {
                    kv.Value.isSelected = false;
                }
                    
        }

        ClearPlayerSelection();
        ClearNpcSelection();

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
                }
            }
        }

        ClearPlayerSelection();
        ClearNpcSelection();

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
                }
                else
                {
                    kv.Value.isSelected = false;
                }
            }
        }

        ClearPlayerSelection();
        ClearNpcSelection();

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
                }
            else if (kv.Value.isSelected)
            {
                kv.Value.isSelected = false;
            }
        }

        ClearUserSelection();
        ClearPlayerSelection();

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
                }
            else if (kv.Value.isSelected)
            {
                kv.Value.isSelected = false;
            }
        }

        ClearUserSelection();
        ClearNpcSelection();

        SelectionChangedEvent?.Invoke();
    }
}
