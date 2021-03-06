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
    private SelectionRect selectionRect;

    protected void Awake()
    {
        SM = FindObjectOfType<L2RTSServerManager>();
        selectionRect = FindObjectOfType<SelectionRect>();
    }

    protected void Start()
    {
        targetingMode = TargetingMode.None;
    }

    protected void Update()
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

    public void OnGroundMouseClick(PointerEventData peData)
    {
        if (peData.button == PointerEventData.InputButton.Left)
            OnGroundLeftClick(peData);
        if (peData.button == PointerEventData.InputButton.Right)
            OnGroundRightClick(peData);
    }

    private void OnGroundLeftClick(PointerEventData peData)
    {
        switch (targetingMode)
        {
            case TargetingMode.AttackMove:
                SM.selectionManager.DoForEachSelectedUserActor((ua) =>
                {
                    Vector3Int spot = Vector3Int.RoundToInt(WorldUtils.UnityToL2Coords(peData.pointerCurrentRaycast.worldPosition));
                    ua.ai.AttackMove(spot);
                });
                break;
            default:
                if (!selectionRect.isVisible)
                    SM.selectionManager.ClearSelection();
                break;
        }

        ExitTargeting();
    }

    private void OnGroundRightClick(PointerEventData peData)
    {
        if (targetingMode != TargetingMode.None)
        {
            ExitTargeting();
            return;
        }
        
        SM.selectionManager.DoForEachSelectedUserActor((ua) =>
        {
            Vector3Int spot = Vector3Int.RoundToInt(WorldUtils.UnityToL2Coords(peData.pointerCurrentRaycast.worldPosition));
            ua.ai.MoveToSpot(spot);
        });
    }

    public void StartAttackTargeting()
    {
        targetingMode = TargetingMode.AttackMove;
        Cursor.SetCursor(attackCursorTexture, hotSpot, cursorMode);
    }

    public void ExitTargeting()
    {
        targetingMode = TargetingMode.None;
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    public void ProcessHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && targetingMode != TargetingMode.None)
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
