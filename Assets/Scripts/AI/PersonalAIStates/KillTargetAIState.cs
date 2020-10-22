using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LA2RTS;

public class KillTargetAIState : PersonalAIState
{

    public ActorScript target;

    public KillTargetAIState(ActorScript target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        // target
        client.SendTargetCommand(target.live.OID);
        client.SendEnableBotCommand(true);
    }

    public override void OnUpdate()
    {
        if (target.live.Dead)
        {
            personalAI.SwitchState(personalAI.GetDefaultState());
        }
    }
}
