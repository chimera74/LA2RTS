using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MoveToTargetAIState : PersonalAIState
{
    
    DateTime lastSend = DateTime.Now;

    public ActorScript target;
    public bool isFollowing;

    public MoveToTargetAIState(ActorScript target, bool follow)
    {
        this.target = target;
        isFollowing = follow;
    }

    public override void OnEnter()
    {
        client.SendEnableBotCommand(false);
        client.SendMoveToCommand(target.live.X, target.live.Y, target.live.Z);
        lastSend = DateTime.Now;
    }

    public override void OnUpdate()
    {
        if (DateTime.Now - lastSend < TimeSpan.FromMilliseconds(1000))
            return;

        if (WorldUtils.CalculateDistance(personalAI.client.UserChar, target.live) > PersonalAI.MOVE_TO_TARGET_DISTANCE)
        {
            client.SendMoveToCommand(target.live.X, target.live.Y, target.live.Z);
            lastSend = DateTime.Now;
        }
        
    }
}