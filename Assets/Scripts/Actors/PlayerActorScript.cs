using UnityEngine;
using UnityEngine.EventSystems;
using LA2RTS.LA2Entities;

public class PlayerActorScript : ActorScript
{
    [Header("Materials")]
    public Material traderMaterial;
    public Material pkMaterial;
    public Material pvpMaterial;

    private LA2Char _player;

    public LA2Char player
    {
        get => _player;

        set
        {
            _player = value;
            _live = value;
        }
    }


    // Use this for initialization
    protected override void Start()
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
        SelectAppearance();
        SetHighlight(properties.isSelected);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected void LateUpdate()
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

    public override void SelectAppearance()
    {
        if (player.Dead)
            SetDeadAppearance();        
        else if (player.PK)
        {
            SetPKAppearance();
            SetOutlineColor(OutlineColor.Enemy);
        }
        else if (player.PvP)
        {
            SetPVPAppearance();
            SetOutlineColor(OutlineColor.Enemy);
        }
        else if (player.StoreType != 0)
        {
            SetTraderAppearance();
        }
        else
        {
            SetDefaultAppearance();
            SetOutlineColor(OutlineColor.Neutral);
        }
    }

    protected override void OnRightMouseClick(PointerEventData pointerEventData)
    {
        
    }

    protected override void OnLeftMouseClick(PointerEventData pointerEventData)
    {
        if (SM.inputController.targetingMode == InputController.TargetingMode.None)
        {
            // Select this
            SM.selectionManager.SelectPlayer(player);
        }
    }

    private void SetTraderAppearance()
    {
        defaultNameplateVisibility = false;
        SetNameplateVisibility(false);
        GetComponent<Renderer>().material = traderMaterial;
        transform.localScale = new Vector3(0.6f, 0.4f, 0.6f);
    }

    private void SetPKAppearance()
    {
        defaultNameplateVisibility = true;
        SetNameplateVisibility(true);
        GetComponent<Renderer>().material = pkMaterial;
    }

    private void SetPVPAppearance()
    {        
        GetComponent<Renderer>().material = pvpMaterial;
    }

    public override string GetStatsJson()
    {
        return JsonUtility.ToJson(player);
    }

    public override bool IsSelected()
    {
        return SM.playerActorManager.playerProperties[player].isSelected;
    }
}
