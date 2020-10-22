using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LA2RTS;
using UnityEngine;

public class PersonalAI : MonoBehaviour
{
    public PersonalAIState state;
    public AIStance stance;

    [HideInInspector]
    public RTSClient client;
    [HideInInspector]
    public UserActorScript actor;

    private bool stateIsSwitching;
    private PersonalAIState newState;

    protected void Awake()
    {
        actor = GetComponent<UserActorScript>();
    }

    protected void Start()
    {
        client = actor.client;
        SwitchState(GetDefaultState());
    }

    protected void Update()
    {
        ProcessStateSwitch();
        state.OnUpdate();
    }

    private void ProcessStateSwitch()
    {
        if (!stateIsSwitching) return;

        stateIsSwitching = false;
        state?.OnExit();
        state = newState;
        state.OnEnter();
    }

    public PersonalAIState GetDefaultState()
    {
        switch (stance)
        {
            case AIStance.Passive:
            default:
                return new IdleState();
        }
    }

    #region StateSwitches

    public void SwitchState(PersonalAIState toState)
    {
        InjectDependencies(ref toState);
        newState = toState;
        stateIsSwitching = true;
    }

    private void InjectDependencies(ref PersonalAIState aiState)
    {
        aiState.client = client;
        aiState.personalAI = this;
    }

    public void MoveToSpot(Vector3Int spot)
    {
        SwitchState(new MoveToSpotAIState(spot));
    }

    public void MoveToTarget(ActorScript target)
    {
        SwitchState(new MoveToTargetAIState(target));
    }

    public void KillTarget(ActorScript target)
    {
        SwitchState(new KillTargetAIState(target));
    }

    public void Idle()
    {

    }

    #endregion
}

public enum AIState
{
    Idle,           // do nothing
    Attack,         // kill target
    AggressiveIdle, // stand and attack everything coming close
    AttackMove,     // move to the spot attacking everything in the path
    Hold,           // stand on the spot don't move, attack everything
    Patrol,         // follow path and attack
    MoveToTarget,   
    MoveToSpot,
}

public enum AIStance
{
    Passive,        // Will not attack the enemies
    Aggressive,     // Will attack enemies in range
    Defensive,      // Will attack only those who are attacking him
}
