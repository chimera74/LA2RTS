using UnityEngine;
using System.Collections;

public class ActionsPanelScript : ButtonPanel
{

    public void AttackMoveAction()
    {

    }

    public void AttackAction()
    {

    }

    public void MoveAction()
    {

    }

    public void StopAction()
    {
        foreach (var cl in userActorManager.clientProperties.Keys)
        {
            var cp = userActorManager.clientProperties[cl];
            if (cp != null && cp.isSelected)
            {
                Vector3 d = WorldUtils.UnityToL2Coords(cp.actor.transform.position);
                cl.SendMoveToCommand((int) d.x, (int) d.y);
            }
                
        }
    }

}
