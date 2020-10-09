using LA2RTS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GUIManager : MonoBehaviour {


    [Header("Global References")]
    public GameObject ClientCountLabel;
    public GameObject ClientList;

    [Header("Prefabs")]
    public GameObject ClientPanelPrefab;

    private SimpleActionQueue _actionQueue;
    private L2RTSServerManager SM;

    private void OnEnable()
    {
        SM = FindObjectOfType<L2RTSServerManager>();
        SM.ServerStartedEvent += () =>
        {
            RegisterToEvents();
        };
    }

    void Awake()
    {
        _actionQueue = new SimpleActionQueue();
    }
    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        _actionQueue.InvokeAll();
	}

    private void RegisterToEvents()
    {
        SM.Server.ClientConnectedEvent += UpdateClientCounter;
        SM.Server.ClientDisconnectedEvent += UpdateClientCounter;

        SM.Server.ClientConnectedEvent += AddClientToList;
        SM.Server.ClientDisconnectedEvent += RemoveClientFromList;
    }

    private void UpdateClientCounter(RTSClient cl)
    {   
        _actionQueue.Enqueue(() =>
        {   
            ClientCountLabel.GetComponent<Text>().text = "Connected clients: " + SM.Server.clients.Count;
        });
    }

    private void AddClientToList(RTSClient cl)
    {
        _actionQueue.Enqueue(() =>
        {
            GameObject newClientPanel = Instantiate(ClientPanelPrefab, ClientList.transform);
            var cps = newClientPanel.GetComponent<ClientPanelScript>();
            cps.client = cl;
            cps.SetHighlight(false);
            
        });
    }

    private void RemoveClientFromList(RTSClient cl)
    {
        _actionQueue.Enqueue(() =>
        {
            var clSearchRes = ClientList.GetComponentsInChildren<ClientPanelScript>().SingleOrDefault(c => c.client.clientID == cl.clientID);
            if (clSearchRes != null)
                Destroy(clSearchRes.gameObject);
        });
    }

    public void ToggleSelectAll()
    {
        if (SM.selectionManager.selectedActors.Count > 0)
            SM.selectionManager.ClearSelection();
        else
            SM.selectionManager.SelectMultipleUsers(SM.userActorManager.clientProperties.Keys);
    }
}
