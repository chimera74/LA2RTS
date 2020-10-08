using UnityEngine;
using System.Collections;
using LA2RTS;
using cakeslice;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using LA2RTS.LA2Entities;
using System;
using UnityEngine.UI;

public class PlayerActorScript : ActorScript
{
    [Header("Materials")]
    public Material traderMaterial;
    public Material pkMaterial;
    public Material pvpMaterial;

    private LA2Char _player;

    public LA2Char player
    {
        get
        {
            return _player;
        }

        set
        {
            _player = value;
            _live = value;
        }
    }


    // Use this for initialization
    public override void Start()
    {
        base.Start();
        player.UpdateEvent += UpdateInfo;
        player.UpdateEvent += UpdatePosition;
        player.QuickUpdateEvent += UpdatePosition;
        player.QuickUpdateEvent += UpdateStats;
        
        properties = SM.playerActorManager.playerProperties[player];
        properties.SelectionChangedEvent += OnSelectionChanged;

        defaultNameplateVisibility = SM.preferencesManager.IsPlayerNameplatesEnabled;
        SM.preferencesManager.onPNChanged.AddListener(OnNameplatesEnabledChange);
        SM.preferencesManager.onPMLChanged.AddListener(OnDestinationLinesChange);
        SM.preferencesManager.onPTLChanged.AddListener(OnTargetLinesChange);

        UpdateInfo(player);
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
        if (SM.preferencesManager.IsPlayerMovementLinesEnabled)
            DrawDestinationLine();
        if (SM.preferencesManager.IsPlayerTargetLinesEnabled)
            DrawTargetLine();
    }

    private void UpdateInfo(LA2Char obj)
    {
        _actionQueue.Enqueue(() =>
        {
            base.UpdateInfo(obj);
        });
    }

    protected void UpdateStats(LA2Char obj)
    {
        _actionQueue.Enqueue(() =>
        {
            base.UpdateStats(obj);
        });
    }

    override public void SelectAppearence()
    {
        if (player.Dead)
            SetDeadAppearence();        
        else if (player.PK)
        {
            SetPKAppearence();
            SetOutlineColor(OutlineColor.Enemy);
        }
        else if (player.PvP)
        {
            SetPVPAppearence();
            SetOutlineColor(OutlineColor.Enemy);
        }
        else if (player.StoreType != 0)
        {
            SetTraderAppearence();
        }
        else
        {
            SetDefaultAppearence();
            SetOutlineColor(OutlineColor.Neutral);
        }
    }

    private void SetTraderAppearence()
    {
        defaultNameplateVisibility = false;
        SetNameplateVisibility(false);
        GetComponent<Renderer>().material = traderMaterial;
        transform.localScale = new Vector3(0.6f, 0.4f, 0.6f);
    }

    private void SetPKAppearence()
    {
        defaultNameplateVisibility = true;
        SetNameplateVisibility(true);
        GetComponent<Renderer>().material = pkMaterial;
    }

    private void SetPVPAppearence()
    {        
        GetComponent<Renderer>().material = pvpMaterial;
    }

    protected override void OnSelectionChanged(bool isSelected)
    {
        base.OnSelectionChanged(isSelected);
        if (isSelected)
        {
            _actionQueue.Enqueue(() => UpdateActorInfoPanel(player));
        }
    }
}
