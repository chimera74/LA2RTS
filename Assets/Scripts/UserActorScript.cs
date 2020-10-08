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
        get
        {
            return _client;
        }

        set
        {
            _client = value;
            _live = value.UserChar;
        }
    }
    

    // Use this for initialization
    public override void Start()
    {
        base.Start();
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
        SelectAppearence();
        SetHighlight(properties.isSelected);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public void LateUpdate()
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

    public void OnPointerClick(BaseEventData data)
    {
        PointerEventData pointerEventData = data as PointerEventData;
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftMouseClick(pointerEventData);
        }
        else if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            OnLeftMouseClick(pointerEventData);
            //OnRightMouseClick(pointerEventData);
        }
    }

    private void OnLeftMouseClick(PointerEventData pointerEventData)
    {   
        if (Input.GetKey(KeyCode.LeftControl) == true)
        {
            if (properties.isSelected)
                SM.selectionManager.RemoveUserSelection(client);
            else
                SM.selectionManager.AddUserSelection(client);
        }
        else
        {
            SM.selectionManager.SelectSingleUser(client);
        }
    }

    protected override void OnSelectionChanged(bool isSelected)
    {
        base.OnSelectionChanged(isSelected);
        if (isSelected)
        {
            _actionQueue.Enqueue(() => UpdateActorInfoPanel(client.UserChar));
        }
    }
}
