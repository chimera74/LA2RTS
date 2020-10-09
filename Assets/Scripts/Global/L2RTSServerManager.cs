using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LA2RTS;
using UnityEngine.UI;
using System;

public class L2RTSServerManager : MonoBehaviour {
    
    public RTSServer Server;
    public List<RTSClient> clients;

    public event Action ServerStartedEvent;
    //public event Action ServerStoppedEvent;

    [HideInInspector]
    public PreferencesManager preferencesManager;
    [HideInInspector]
    public UserActorManager userActorManager;
    [HideInInspector]
    public PlayerActorManager playerActorManager;
    [HideInInspector]
    public NPCActorManager npcActorManager;
    [HideInInspector]    
    public SelectionManager selectionManager;
    [HideInInspector]
    public InputController inputController;
    [HideInInspector]
    public KnowledgeDBManager knowledgeDBManager;

    protected void Awake()
    {
        preferencesManager = FindObjectOfType<PreferencesManager>();
        userActorManager = FindObjectOfType<UserActorManager>();
        playerActorManager = FindObjectOfType<PlayerActorManager>();
        npcActorManager = FindObjectOfType<NPCActorManager>();
        selectionManager = FindObjectOfType<SelectionManager>();
        inputController = FindObjectOfType<InputController>();
        knowledgeDBManager = FindObjectOfType<KnowledgeDBManager>();
    }

    protected void Start () {
        Application.runInBackground = true;
        Server = new RTSServer(RTSServer.PORT);
        Server.Start();
        clients = Server.clients;
        ServerStartedEvent?.Invoke();
    }
	
    private void OnApplicationQuit()
    {
        Server.Stop();
    }


}
