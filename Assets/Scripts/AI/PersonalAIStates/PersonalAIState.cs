using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LA2RTS;


public abstract class PersonalAIState
{
    public RTSClient client { get; set; }
    public PersonalAI personalAI { get; set; }

    public virtual void OnEnter() {}
    public virtual void OnExit() {}
    public abstract void OnUpdate();
}
