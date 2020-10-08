using LA2RTS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using LA2RTS.LA2Entities;

public class InputController : MonoBehaviour
{
    public enum TargetingMode
    {
        None,
        SimpleAttack,
        UseAttackSkill,
        UseSupportSkill,
        Move,
        AttackMove
    }

    [Header("Cursors")]
    public Texture2D attackCursorTexture;
    public Vector2 hotSpot = Vector2.zero;

    [HideInInspector]
    public TargetingMode targetingMode;

    private CursorMode cursorMode = CursorMode.Auto;
    private L2RTSServerManager SM;

    // Use this for initialization
    void Start()
    {
        SM = FindObjectOfType<L2RTSServerManager>();
        targetingMode = TargetingMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessMouseInputs();
        ProcessHotkeys();
    }

    public bool IsTargetingModeActive()
    {
        return targetingMode != TargetingMode.None;
    }

    private void ProcessMouseInputs()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ExitTargeting();
        }
    }

    public void OnGroundMouseClick(BaseEventData data)
    {
        PointerEventData peData = data as PointerEventData;
        if (peData.button == PointerEventData.InputButton.Right)
            OnGroundRightClick(peData);
    }

    private void OnGroundRightClick(PointerEventData peData)
    {
        if (targetingMode != TargetingMode.None)
        {
            ExitTargeting();
            return;
        }

        foreach (RTSClient cl in SM.userActorManager.clientProperties.Keys)
        {
            if (SM.userActorManager.clientProperties[cl].isSelected && SM.userActorManager.clientProperties[cl].actor != null)
            {
                var coords = WorldUtils.UnityToL2Coords(peData.pointerCurrentRaycast.worldPosition);
                cl.SendMoveToCommand((int)coords.x, (int)coords.y);
            }
        }
    }

    public void StartAttackTargeting()
    {
        targetingMode = TargetingMode.SimpleAttack;
        Cursor.SetCursor(attackCursorTexture, hotSpot, cursorMode);
    }

    public void ExitTargeting()
    {
        targetingMode = TargetingMode.None;
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    public void ProcessHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitTargeting();
            return;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            StartAttackTargeting();
            return;
        }
    }
}
