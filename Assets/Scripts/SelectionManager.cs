using UnityEngine;
using System.Collections;
using System;
using LA2RTS.LA2Entities;
using LA2RTS;
using System.Collections.Generic;
using System.Linq;

public class SelectionManager : MonoBehaviour
{
    public event Action SelectionClearedEvent;

    public int totalSelected;
    public int usersSelected;
    public int playersSelected;
    public int npcSelected;

    private L2RTSServerManager SM;

    void Start()
    {
        SM = FindObjectOfType<L2RTSServerManager>();
        totalSelected = 0;
        usersSelected = 0;
        playersSelected = 0;
        npcSelected = 0;
    }

    void Update()
    {

    }

    public void ClearSelection()
    {
        if (totalSelected == 0)
            return;

        ClearUserSelection();
        ClearPlayerSelection();
        ClearNpcSelection();
        
        totalSelected = 0;

        if (SelectionClearedEvent != null)
            SelectionClearedEvent();
    }

    private void ClearUserSelection()
    {
        if (usersSelected == 0)
            return;

        SM.userActorManager.clientProperties.ForEach((kv) =>
        {
            if (kv.Value != null && kv.Value.isSelected)
                kv.Value.isSelected = false;
        });
        usersSelected = 0;
    }

    private void ClearPlayerSelection()
    {
        if (playersSelected == 0)
            return;

        SM.playerActorManager.playerProperties.ForEach((kv) =>
        {
            if (kv.Value != null && kv.Value.isSelected)
                kv.Value.isSelected = false;
        });
        playersSelected = 0;
    }

    private void ClearNpcSelection()
    {
        if (npcSelected == 0)
            return;

        SM.npcActorManager.npcProperties.ForEach((kv) =>
        {
            if (kv.Value != null && kv.Value.isSelected)
                kv.Value.isSelected = false;
        });
        npcSelected = 0;
    }

    public void SelectSingleUser(RTSClient cl)
    {
        foreach (var kv in SM.userActorManager.clientProperties)
        {
            if (kv.Value == null)
                continue;

            if (kv.Key == cl)
            {
                if (!kv.Value.isSelected)
                    kv.Value.isSelected = true;
            }
            else
                if (!kv.Value.isSelected)
                   kv.Value.isSelected = false;
        }

        ClearPlayerSelection();
        ClearNpcSelection();

        usersSelected = 1;
        totalSelected = 1;
    }

    public void AddUserSelection(RTSClient cl)
    {
        foreach (var kv in SM.userActorManager.clientProperties)
        {
            if (kv.Value == null)
                continue;

            if (kv.Key == cl)
            {
                if (!kv.Value.isSelected)
                {
                    kv.Value.isSelected = true;
                    usersSelected++;
                }
                break;
            }
        }

        ClearPlayerSelection();
        ClearNpcSelection();
        
        totalSelected = usersSelected;
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
                    usersSelected--;
                }
                break;
            }
        }

        playersSelected = 0;
        npcSelected = 0;
        totalSelected = usersSelected;
    }

    public void SelectMultipleUsers(IEnumerable<RTSClient> clientsToSelect)
    {
        int selectedCount = 0;
        foreach (var kv in SM.userActorManager.clientProperties)
        {
            if (kv.Value == null)
                continue;

            if (clientsToSelect.Contains(kv.Key))
            {
                if (!kv.Value.isSelected)
                    kv.Value.isSelected = true;
                selectedCount++;
            }
            else
                if (!kv.Value.isSelected)
                    kv.Value.isSelected = false;
        }

        ClearPlayerSelection();
        ClearNpcSelection();

        usersSelected = selectedCount;
        totalSelected = selectedCount;
    }



    public void SelectNPC(LA2NPC npc)
    {
        foreach (var kv in SM.npcActorManager.npcProperties)
        {
            if (kv.Value == null)
                continue;

            if (kv.Key == npc)
                if (kv.Value.isSelected)
                    return;
                else
                    kv.Value.isSelected = true;
            else
                if (!kv.Value.isSelected)
                kv.Value.isSelected = false;
        }

        ClearPlayerSelection();
        ClearUserSelection();

        usersSelected = 0;
        playersSelected = 0;
        npcSelected = 1;
        totalSelected = 1;
    }

    public void SelectPlayer(LA2Char pl)
    {
        foreach (var kv in SM.playerActorManager.playerProperties)
        {
            if (kv.Value == null)
                continue;

            if (kv.Key == pl)
                if (kv.Value.isSelected)
                    return;
                else
                    kv.Value.isSelected = true;
            else
                if (!kv.Value.isSelected)
                kv.Value.isSelected = false;
        }

        ClearUserSelection();
        ClearNpcSelection();

        usersSelected = 0;
        playersSelected = 1;
        npcSelected = 0;
        totalSelected = 1;
    }
}
