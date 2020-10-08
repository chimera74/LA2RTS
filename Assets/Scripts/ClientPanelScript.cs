using LA2RTS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using LA2RTS.LA2Entities;

public class ClientPanelScript : MonoBehaviour
{

    public RTSClient client;

    [Header("Prefabs")]
    public GameObject contextMenu;

    [Header("Subitems")]
    public Text nameLabel;
    public Text statusLabel;
    public Image selectionOutline;
    public ActorProperties properties;

    private SimpleActionQueue _actionQueue;
    private CameraMovement _mainCamera;
    private L2RTSServerManager SM;


    void Awake()
    {
        _actionQueue = new SimpleActionQueue();
    }

    // Use this for initialization
    void Start()
    {
        SM = FindObjectOfType<L2RTSServerManager>();

        client.SelfInfoPacketEvent += UpdateClientInfo;
        client.StatusPacketEvent += UpdateStatus;
        client.FirstPositionPacketEvent += UpdateClientInfo;

        properties = SM.userActorManager.clientProperties[client];
        properties.SelectionChangedEvent += OnSelectionChanged;

        _mainCamera = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        _actionQueue.InvokeAll();
    }

    public void SetName(string nameStr)
    {
        nameLabel.text = nameStr;
    }

    public void SetStatus(string statusStr)
    {
        statusLabel.text = statusStr;

        switch (client.UserChar.Status)
        {
            case LA2UserChar.ClientStatus.Off:
                statusLabel.color = Color.red;
                break;
            case LA2UserChar.ClientStatus.CharSelect:
                statusLabel.color = Color.yellow;
                break;
            case LA2UserChar.ClientStatus.InGame:
                statusLabel.color = new Color(0, (float)(190.0 / 255.0), 0); //green
                break;
            default:
                statusLabel.color = Color.red;
                break;
        }
    }

    public void OnPointerClick(BaseEventData data)
    {
        PointerEventData pointerEventData = data as PointerEventData;
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftMouseClick(pointerEventData);
            if (pointerEventData.clickCount > 1)
                OnLeftMouseDoubleClick(pointerEventData);
        }
        else if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            if (!properties.isSelected)
                OnLeftMouseClick(pointerEventData);

            OnRightMouseClick(pointerEventData);
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

    private void OnLeftMouseDoubleClick(PointerEventData pointerEventData)
    {
        if (properties.actor != null)
            _mainCamera.LookAt(properties.actor.transform.position);
    }


    private void OnRightMouseClick(PointerEventData pointerEventData)
    {
        //instantiate as child of canvas (root)
        var cm = Instantiate(contextMenu, gameObject.transform.parent.parent);

        LinkedList<RTSClient> selectedClients = new LinkedList<RTSClient>();
        foreach (var kv in SM.userActorManager.clientProperties)
        {
            if (kv.Value.isSelected)
                selectedClients.AddLast(kv.Key);
        }
        cm.GetComponent<ClientContextMenuScript>().Clients = selectedClients;

        cm.transform.position = pointerEventData.pressPosition;
    }

    private void UpdateClientInfo(RTSClient cl)
    {
        _actionQueue.Enqueue(() =>
        {
            SetName(cl.UserChar.Name);
            SetStatus(cl.UserChar.Status.ToString());
        });
    }


    private void UpdateStatus(RTSClient cl)
    {
        _actionQueue.Enqueue(() =>
        {
            SetStatus(cl.UserChar.Status.ToString());
        });
    }

    public void SetHighlight(bool isHighlighted)
    {
        selectionOutline.enabled = isHighlighted;
    }

    private void OnSelectionChanged(bool isSelected)
    {
        _actionQueue.Enqueue(() => SetHighlight(isSelected));
    }
}
