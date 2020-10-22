using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MoveToTargetAIState : PersonalAIState
{
    public const float MOVE_TO_DISTANCE = 100;
    DateTime lastSend = DateTime.Now;

    public ActorScript target;

    public MoveToTargetAIState(ActorScript target)
    {
        this.target = target;
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

        if (WorldUtils.CalculateDistance(personalAI.client.UserChar, target.live) > MOVE_TO_DISTANCE)
        {
            client.SendMoveToCommand(target.live.X, target.live.Y, target.live.Z);
            lastSend = DateTime.Now;
        }
        
    }
}