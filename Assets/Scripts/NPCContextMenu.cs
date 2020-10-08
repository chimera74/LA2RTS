using UnityEngine;
using System.Collections;
using LA2RTS.LA2Entities;
using System.Collections.Generic;

public class NPCContextMenu : ContextMenu
{

    public LA2NPC npc;

    

    public override void Start()
    {
        base.Start();
    }

    public void Target()
    {
        foreach (var cl in userActorManager.clientProperties.Keys)
        {
            var cp = userActorManager.clientProperties[cl];
            if (cp != null && cp.isSelected)
                cl.SendTargetCommand(npc.OID);
        }
    }

    public void RefreshInfo()
    {
        LinkedList<LA2NPC> npcs = new LinkedList<LA2NPC>();
        npcs.AddLast(npc);
        foreach (var cl in userActorManager.clientProperties.Keys)
        {
            var cp = userActorManager.clientProperties[cl];
            if (cp != null && cp.isSelected)
                cl.SendFullNPCInfoRequestByOIDs(npcs);
        }
    }
}
