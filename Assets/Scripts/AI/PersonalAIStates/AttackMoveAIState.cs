using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LA2RTS.LA2Entities;
using UnityEngine;

public class AttackMoveAIState : PersonalAIState
{

    DateTime lastSend;
    DateTime lastCheck;
    readonly TimeSpan checkInterval = TimeSpan.FromMilliseconds(25);

    public Vector3Int targetSpot;

    public AttackMoveAIState(Vector3Int targetSpot)
    {
        this.targetSpot = targetSpot;
    }

    public override void OnEnter()
    {
        client.SendEnableBotCommand(false);
        if (targetSpot == Vector3Int.zero)
        {
            targetSpot.x = personalAI.client.UserChar.X;
            targetSpot.y = personalAI.client.UserChar.Y;
            targetSpot.z = personalAI.client.UserChar.Z;
            Debug.Log("Move to spot target is not set!");
        }

        if (CheckForTargetsAndSwitch())
            return;

        client.SendMoveToCommand(targetSpot.x, targetSpot.y, targetSpot.z);
        lastSend = DateTime.Now;
        lastCheck = DateTime.Now - TimeSpan.FromSeconds(1);
    }

    public override void OnUpdate()
    {

        // check for attackers or look for target
        if (DateTime.Now - lastCheck > checkInterval)
        {
            lastCheck = DateTime.Now;
            if (CheckForTargetsAndSwitch())
                return;
        }
        
        // check for destination
        if (WorldUtils.CalculateDistance(personalAI.client.UserChar, targetSpot) < PersonalAI.MOVE_TO_SPOT_DISTANCE)
        {
            personalAI.SwitchToPreviousInStack();
        }
        else if (DateTime.Now - lastSend >= TimeSpan.FromMilliseconds(200))
        {
            client.SendMoveToCommand(targetSpot.x, targetSpot.y, targetSpot.z);
            lastSend = DateTime.Now;
        }
    }

    private bool CheckForTargetsAndSwitch()
    {
        lastCheck = DateTime.Now;
        var newTarget = personalAI.LookForTarget(PersonalAI.SEARCH_TARGET_DISTANCE);
        if (newTarget != null)
        {
            // found target - switching to kill target state
            personalAI.AddToStackAndSwitchTo(new KillTargetAIState(newTarget));
            return true;
        }

        return false;
    }
}
