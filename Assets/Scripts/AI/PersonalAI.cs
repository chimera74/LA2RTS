using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
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
    [HideInInspector]
    public L2RTSServerManager SM;

    public Stack<PersonalAIState> stateStack;

    private bool stateIsSwitching;
    private PersonalAIState newState;

    public const int MOVE_TO_SPOT_DISTANCE = 10;
    public const int MOVE_TO_TARGET_DISTANCE = 100;
    public const int SEARCH_TARGET_DISTANCE = 600;

    public event Action<PersonalAIState> AIStateChanged;

    protected void Awake()
    {
        actor = GetComponent<UserActorScript>();
        SM = FindObjectOfType<L2RTSServerManager>();
        stateStack = new Stack<PersonalAIState>();
    }

    protected void Start()
    {
        client = actor.client;
        AIStateChanged += SM.userActorManager.clientProperties[client].panel.OnAIStateChanged;
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
        AIStateChanged?.Invoke(state);
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

    private void SwitchState(PersonalAIState toState)
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
        ClearStack();
        AddToStackAndSwitchTo(new MoveToSpotAIState(spot));
    }

    public void MoveToTarget(ActorScript target, bool follow)
    {
        ClearStack();
        AddToStackAndSwitchTo(new MoveToTargetAIState(target, follow));
    }

    public void KillTarget(ActorScript target)
    {
        ClearStack();
        AddToStackAndSwitchTo(new KillTargetAIState(target));
    }

    public void AttackMove(Vector3Int spot)
    {
        ClearStack();
        AddToStackAndSwitchTo(new AttackMoveAIState(spot));
    }

    public void Idle()
    {

    }

    private void ClearStack()
    {
        stateStack.Clear();
    }

    private void AddToStack(PersonalAIState state)
    {
        stateStack.Push(state);
    }

    private void RemoveCurrentFromStack()
    {
        stateStack.Pop();
    }

    public void SwitchToDefaultState()
    {
        ClearStack();
        AddToStack(GetDefaultState());
        SwitchState(stateStack.Peek());
    }

    public void AddToStackAndSwitchTo(PersonalAIState state)
    {
        AddToStack(state);
        SwitchState(stateStack.Peek());
    }

    public void SwitchToPreviousInStack()
    {
        if (stateStack.Count > 0)
            stateStack.Pop();
        if (stateStack.Count > 0)
            SwitchState(stateStack.Peek());
        else
        {
            SwitchToDefaultState();
        }
    }

    #endregion

    /// <summary>
    /// Looks for a valid target to attack in range.
    /// </summary>
    /// <returns>Actor that is in rage of attack.</returns>
    public ActorScript LookForTarget(int range)
    {
        var query = from npc in SM.npcActorManager.npcProperties
            let dist = WorldUtils.CalculateDistance(actor.live, npc.Key)
            where dist <= SEARCH_TARGET_DISTANCE && !npc.Key.Dead && npc.Key.Attackable
            orderby dist
            select new
            {
                npc.Value.actorScript,
                dist
            };
        var closest = query.FirstOrDefault();
        return closest?.actorScript;
    }

    /// <summary>
    /// Looks for an attacker.
    /// </summary>
    /// <returns>Actor that is attacking us.</returns>
    public ActorScript LookForAttacker()
    {
        return null;
    }
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
