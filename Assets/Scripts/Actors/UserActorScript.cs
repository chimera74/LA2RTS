using UnityEngine;
using System.Collections;
using LA2RTS;
using cakeslice;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using LA2RTS.LA2Entities;
using System;

public class UserActorScript : ActorScript
{

    private RTSClient _client;
    public RTSClient client
    {
        get => _client;

        set
        {
            _client = value;
            live = value.UserChar;
        }
    }

    public PersonalAI ai;

    protected override void Awake()
    {
        base.Awake();
        ai = GetComponent<PersonalAI>();
    }

    protected virtual void Start()
    {
        client.SelfInfoPacketEvent += UpdateInfo;
        client.SelfInfoPacketEvent += UpdatePosition;
        client.QuickUpdatePacketEvent += UpdateStats;
        client.QuickUpdatePacketEvent += UpdatePosition;

        properties = SM.userActorManager.clientProperties[client];
        properties.SelectionChangedEvent += OnSelectionChanged;

        defaultNameplateVisibility = SM.preferencesManager.IsPlayerNameplatesEnabled;
        SM.preferencesManager.onUNChanged.AddListener(OnNameplatesEnabledChange);
        SM.preferencesManager.onUMLChanged.AddListener(OnDestinationLinesChange);
        SM.preferencesManager.onUTLChanged.AddListener(OnTargetLinesChange);

        UpdateInfo(client);
        SetNameplateVisibility(defaultNameplateVisibility);
        SelectAppearance();
        SetHighlight(properties.isSelected);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected void LateUpdate()
    {
        if (SM.preferencesManager.IsUserMovementLinesEnabled)
            DrawDestinationLine();
        if (SM.preferencesManager.IsUserTargetLinesEnabled)
            DrawTargetLine();
    }

    public void UpdatePosition(RTSClient cl)
    {
        base.UpdatePosition(cl.UserChar);
    }


    private void UpdateInfo(RTSClient cl)
    {
        _actionQueue.Enqueue(() =>
        {
            base.UpdateInfo(cl.UserChar);
        });
    }

    protected void UpdateStats(RTSClient cl)
    {
        _actionQueue.Enqueue(() =>
        {
            base.UpdateStats(cl.UserChar);
        });
    }

    protected override void OnLeftMouseClick(PointerEventData pointerEventData)
    {   
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (properties.isSelected)
                SM.selectionManager.RemoveUserSelection(client);
            else
                SM.selectionManager.AddUserSelection(client);
        }
        else
        {
            SM.selectionManager.SelectUser(client);
        }
    }

    protected override void OnRightMouseClick(PointerEventData pointerEventData)
    {
        OnLeftMouseClick(pointerEventData);
    }

    public override string GetStatsJson()
    {
        return JsonUtility.ToJson(client.UserChar);
    }

    public override bool IsSelected()
    {
        return SM.userActorManager.clientProperties[client].isSelected;
    }
}
