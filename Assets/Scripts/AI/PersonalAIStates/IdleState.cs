using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LA2RTS;


public class IdleState : PersonalAIState
{

    public override void OnEnter()
    {
        // disable bot
        client.SendEnableBotCommand(false);
        client.SendStopCommand();
    }

    public override void OnUpdate()
    {
        
    }
}
