using UnityEngine;
using UnityEngine.EventSystems;
using LA2RTS.LA2Entities;

public class NPCActorScript : ActorScript
{

    [Header("Materials")]
    public Material monsterMaterial;
    public Material raidMaterial;

    private LA2NPC _npc;
    public LA2NPC npc
    {
        get => _npc;

        set
        {
            _npc = value;
            _live = value;
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        npc.UpdateEvent += UpdateInfo;
        npc.UpdateEvent += UpdatePosition;
        npc.QuickUpdateEvent += UpdatePosition;
        npc.QuickUpdateEvent += UpdateStats;
        
        properties = SM.npcActorManager.npcProperties[npc];
        properties.SelectionChangedEvent += OnSelectionChanged;

        defaultNameplateVisibility = SM.preferencesManager.IsNPCNameplatesEnabled;
        SM.preferencesManager.onNNChanged.AddListener(OnNameplatesEnabledChange);
        SM.preferencesManager.onNMLChanged.AddListener(OnDestinationLinesChange);
        SM.preferencesManager.onNTLChanged.AddListener(OnTargetLinesChange);


        UpdateInfo(npc);
        SetNameplateVisibility(defaultNameplateVisibility);
        SelectAppearance();
        SetHighlight(properties.isSelected);
    }

    protected void LateUpdate()
    {
        if (SM.preferencesManager.IsNPCMovementLinesEnabled)
            DrawDestinationLine();
        if (SM.preferencesManager.IsNPCTargetLinesEnabled)
            DrawTargetLine();
    }

    protected override void OnLeftMouseClick(PointerEventData pointerEventData)
    {
        if (SM.inputController.targetingMode == InputController.TargetingMode.SimpleAttack)
        {
            AttackThisCommand();
            SM.inputController.ExitTargeting();
            return;
        }
    }

    protected override void OnRightMouseClick(PointerEventData pointerEventData)
    {
        if (SM.inputController.targetingMode == InputController.TargetingMode.None)
        {
            if (pointerEventData.clickCount > 1)
            {
                ShowContextMenu(pointerEventData.pressPosition);
            }
            else
            {
                AttackThisCommand();
            }
        }
        else
        {
            SM.inputController.ExitTargeting();
        }
    }

    protected void ShowContextMenu(Vector2 pos)
    {
        //instantiate as child of canvas (root)
        var canvas = GameObject.Find("GUI");
        var cm = Instantiate(contextMenu, canvas.transform);
        cm.transform.position = pos;
        cm.GetComponent<NPCContextMenu>().npc = npc;
    }

    private void UpdateInfo(LA2NPC obj)
    {
        _actionQueue.Enqueue(() =>
        {
            nameText.text = obj.Name;
            SelectAppearance();
        });
    }

    private void UpdateStats(LA2NPC obj)
    {
        _actionQueue.Enqueue(() =>
        {
            base.UpdateStats(obj);
        });
    }

    override public void SelectAppearance()
    {
        if (npc.Dead)
            SetDeadAppearance();
        else if (SM.knowledgeDBManager.IsRaidBoss(npc.ID))
        {
            SetRaidBossAppearence();
            SetOutlineColor(OutlineColor.Enemy);
        }
        else if (npc.Attackable)
        {
            SetMonsterAppearence();
            SetOutlineColor(OutlineColor.Enemy);
        }
        else
        {
            SetDefaultAppearance();
            SetOutlineColor(OutlineColor.Neutral);
        }
    }

    private void SetMonsterAppearence()
    {
        SetNameplateVisibility(defaultNameplateVisibility);
        GetComponent<Renderer>().material = monsterMaterial;
    }

    private void SetRaidBossAppearence()
    {
        transform.localScale = new Vector3(0.8f, 0.9f, 0.8f);
        defaultNameplateVisibility = true;
        SetNameplateVisibility(true);
        GetComponent<Renderer>().material = raidMaterial;
    }

    public override string GetStatsJson()
    {
        return JsonUtility.ToJson(npc);
    }
}
