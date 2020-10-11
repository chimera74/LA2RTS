using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LA2RTS.LA2Entities;
using UnityEngine.EventSystems;

public abstract class ActorScript : MonoBehaviour, IPointerClickHandler
{

    [Header("Prefabs")]
    public GameObject destinationLinePrefab;
    public GameObject targetLinePrefab;
    public GameObject contextMenu;

    [Header("Materials")]
    public Material defaultMaterial;
    public Material deadMaterial;

    [Header("Subcomponents")]
    public Canvas namePlate;
    public Text nameText;

    [Header("Other")]
    public ActorInfoPanelScript actorInfoPanel;

    public ActorProperties properties;

    protected LA2Live _live;
    protected GameObject destinationLine;
    protected GameObject targetLine;

    protected bool hasActualPosition;
    protected bool wasDead;
    protected bool defaultNameplateVisibility;

    protected SimpleActionQueue _actionQueue;
    protected L2RTSServerManager SM;

    public void Awake()
    {
        _actionQueue = new SimpleActionQueue();
        hasActualPosition = false;
    }

    // Use this for initialization
    protected virtual void Start()
    {
        SM = FindObjectOfType<L2RTSServerManager>();
        actorInfoPanel = FindObjectOfType<ActorInfoPanelScript>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        _actionQueue.InvokeAll();
    }

    protected void DrawTargetLine()
    {
        if (_live.TargetOID > 0)
        {
            LA2Live target = SM.Server.FindSpawnByOID(_live.TargetOID);
            if (target == null)
                RemoveTargetLine();

            if (target is LA2UserChar ucTarget)
            {
                var props = SM.userActorManager.clientProperties[ucTarget.client];
                SetTargetLine(props.actor.transform.position);
                return;
            }

            if (target is LA2Char charTarget)
            {
                var props = SM.playerActorManager.playerProperties[charTarget];
                SetTargetLine(props.actor.transform.position);
                return;
            }

            if (target is LA2NPC npcTarget)
            {
                var props = SM.npcActorManager.npcProperties[npcTarget];
                SetTargetLine(props.actor.transform.position);
                return;
            }

            RemoveTargetLine();

        }
        else
        {
            RemoveTargetLine();
        }
    }

    protected void SetTargetLine(Vector3 target)
    {
        const float HOVER_HEIGHT = 0.1f;

        if (targetLine == null)
            targetLine = Instantiate(targetLinePrefab, gameObject.transform);

        var lr = targetLine.GetComponent<LineRenderer>();
        lr.SetPosition(0, transform.position + new Vector3(0, HOVER_HEIGHT, 0));
        lr.SetPosition(1, target + new Vector3(0, HOVER_HEIGHT, 0));
    }

    protected void RemoveTargetLine()
    {
        if (targetLine != null)
            Destroy(targetLine);
    }

    protected void DrawDestinationLine()
    {
        if (hasActualPosition && _live.ToX != 0 && _live.ToY != 0 && _live.ToZ != 0)
        {
            Vector3 destination = WorldUtils.L2ToUnityCoords(_live.ToX, _live.ToY, 0);
            float magn = (destination - transform.position).magnitude;
            if (magn > 1 && magn < 2048)
            {
                SetDestinationLine(destination);
            }
            else
            {
                RemoveDestinationLine();
            }
        }
    }

    protected void SetDestinationLine(Vector3 target)
    {
        const float HOVER_HEIGHT = 0.09f;

        if (destinationLine == null)
            destinationLine = Instantiate(destinationLinePrefab, gameObject.transform);

        var lr = destinationLine.GetComponent<LineRenderer>();
        lr.SetPosition(0, transform.position + new Vector3(0, HOVER_HEIGHT, 0));
        lr.SetPosition(1, target + new Vector3(0, HOVER_HEIGHT, 0));
    }

    protected void RemoveDestinationLine()
    {
        if (destinationLine != null)
            Destroy(destinationLine);
    }

    protected void AttackThisCommand()
    {
        SM.userActorManager.DoForEachSelected((cl) =>
        {
            cl.SendTargetCommand(_live.OID);
            cl.SendUseActionCommand(3, SM.preferencesManager.isForceAtackEnabled);
        });
    }

    public void SetDirection()
    {
        //TODO
    }

    protected virtual void UpdateInfo(LA2Live obj)
    {
        nameText.text = obj.Name;
        SelectAppearance();
    }

    protected virtual void UpdateStats(LA2Live obj)
    {
        if (obj.Dead != wasDead)
        {
            wasDead = obj.Dead;
            SelectAppearance();
        }
    }

    protected void UpdatePosition(LA2Live obj)
    {
        _actionQueue.Enqueue(() =>
        {
            hasActualPosition = true;
            transform.position = WorldUtils.L2ToUnityCoords(obj.X, obj.Y, 0);
        });
    }

    protected virtual void OnSelectionChanged(bool isSelected)
    {
        if (isSelected)
        {
            if (!SM.selectionManager.selectedActors.Contains(this))
                SM.selectionManager.selectedActors.AddLast(this);
        }
        else
        if (SM.selectionManager.selectedActors.Contains(this))
            SM.selectionManager.selectedActors.Remove(this);

        _actionQueue.Enqueue(() =>
        {
            SetHighlight(isSelected);
            // if (isSelected)
            // {
            //     actorInfoPanel.currentActor = _live;
            //     actorInfoPanel.Show();
            // }
        });
    }

    public virtual void SelectAppearance()
    {
        if (_live.Dead)
            SetDeadAppearance();
        else
            SetDefaultAppearance();
    }

    public virtual void SetDefaultAppearance()
    {
        SetNameplateVisibility(defaultNameplateVisibility);
        GetComponent<Renderer>().material = defaultMaterial;
    }

    public virtual void SetDeadAppearance()
    {
        transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        SetNameplateVisibility(defaultNameplateVisibility);
        GetComponent<Renderer>().material = deadMaterial;
    }

    public void SetNameplateVisibility(bool isVisible)
    {
        namePlate.enabled = isVisible;
    }

    protected void OnNameplatesEnabledChange(bool isEnabled)
    {
        defaultNameplateVisibility = isEnabled;
        SelectAppearance();
    }

    protected void OnTargetLinesChange(bool isEnabled)
    {
        if (!isEnabled)
            RemoveTargetLine();
    }

    protected void OnDestinationLinesChange(bool isEnabled)
    {
        if (!isEnabled)
            RemoveDestinationLine();
    }

    public void SetHighlight(bool isHighlighted)
    {
        GetComponent<cakeslice.Outline>().enabled = isHighlighted;
    }

    public virtual void OnPointerEnter()
    {
        SetNameplateVisibility(true);
        SetHighlight(true);
    }

    public virtual void OnPointerExit()
    {
        if (!defaultNameplateVisibility)
            SetNameplateVisibility(false);
        if (!properties.isSelected)
            SetHighlight(false);
    }

    public enum OutlineColor
    {
        Friend = 0,
        Enemy = 1,
        Neutral = 2
    }

    public void SetOutlineColor(OutlineColor color)
    {
        GetComponent<cakeslice.Outline>().color = (int)color;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftMouseClick(pointerEventData);
        }
        else if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            OnRightMouseClick(pointerEventData);
        }
    }

    protected abstract void OnRightMouseClick(PointerEventData pointerEventData);

    protected abstract void OnLeftMouseClick(PointerEventData pointerEventData);

    public abstract string GetStatsJson();

    public abstract bool IsSelected();
}
