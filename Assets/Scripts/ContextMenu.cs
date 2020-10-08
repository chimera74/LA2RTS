using UnityEngine;
using System.Collections;

public class ContextMenu : MonoBehaviour
{

    public bool IsRootElement;
    public GameObject BlockingScreenPrefab;

    private GameObject blockingScreen;
    internal UserActorManager userActorManager;

    // Use this for initialization
    public virtual void Start()
    {
        if (IsRootElement)
            CreateBlockingScreen();

        userActorManager = FindObjectOfType<UserActorManager>();
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

    public void CreateBlockingScreen()
    {
        blockingScreen = Instantiate(BlockingScreenPrefab, gameObject.transform.parent);
        blockingScreen.transform.SetSiblingIndex(transform.GetSiblingIndex());
        blockingScreen.GetComponent<ClickBlockingScreen>().RootUpfrontObject = gameObject;
    }

    public void CloseMenu()
    {
        Destroy(blockingScreen);
        Destroy(gameObject);
    }
}
