using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MoveToSpotAIState : PersonalAIState
{
    DateTime lastSend = DateTime.Now;

    public const float MOVE_TO_SPOT_DISTANCE = 10;
    public Vector3Int targetSpot;

    public MoveToSpotAIState(Vector3Int spot)
    {
        targetSpot = spot;
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
        client.SendMoveToCommand(targetSpot.x, targetSpot.y, targetSpot.z);
        lastSend = DateTime.Now;
    }

    public override void OnUpdate()
    {
        if (WorldUtils.CalculateDistance(personalAI.client.UserChar, targetSpot) < MOVE_TO_SPOT_DISTANCE)
        {
            personalAI.SwitchState(personalAI.GetDefaultState());
        }
        else if (DateTime.Now - lastSend >= TimeSpan.FromMilliseconds(1000))
        {
            client.SendMoveToCommand(targetSpot.x, targetSpot.y, targetSpot.z);
            lastSend = DateTime.Now;
        }

    }
}
