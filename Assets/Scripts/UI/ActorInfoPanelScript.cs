using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LA2RTS.LA2Entities;

public class ActorInfoPanelScript : MonoBehaviour
{
    public Text infoText;
    public LA2Live currentActor;

    private SimpleActionQueue _actionQueue;
    private L2RTSServerManager SM;
    private CanvasGroup cg;

    public void Awake()
    {
        _actionQueue = new SimpleActionQueue();
    }

    // Use this for initialization
    protected void Start()
    {
        SM = FindObjectOfType<L2RTSServerManager>();
        cg = GetComponent<CanvasGroup>();
        SM.selectionManager.SelectionChangedEvent += OnSelectionChanged;
        Hide();
    }

    // Update is called once per frame
    protected void Update()
    {
        _actionQueue.InvokeAll();
    }

    public void SetInfoText(string iText)
    {
        infoText.text = JsonHelper.FormatJson(iText);
    }

    public void Hide()
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void Show()
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void OnSelectionChanged()
    {
        if (SM.selectionManager.selectedActors.Count > 0)
        {
            SetInfoText(SM.selectionManager.selectedActors.First.Value.GetStatsJson());
            Show();
        }
        else
        {
            Hide();
        }
    }
}
