using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MapTileScript : MonoBehaviour, IPointerClickHandler
{
    public Texture2D defaultTexture;

    private L2RTSServerManager SM;

    // Use this for initialization
    void Start()
    {
        SM = FindObjectOfType<L2RTSServerManager>();
        LoadTextureByCoordinates();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadTextureByCoordinates()
    {
        int x = (int)(20 - (transform.position.x / 2048));
        int y = (int)(18 + (transform.position.z / 2048));
        string textureName = "MapTiles/" + x + "_" + y;
        Texture2D texture = Resources.Load(textureName) as Texture2D;
        if (texture != null)
        {
            GetComponent<Renderer>().material.mainTexture = texture;
        } else
        {
            GetComponent<Renderer>().material.mainTexture = defaultTexture;
        }
    }

    public void OnPointerClick(PointerEventData data)
    {
        SM.inputController.OnGroundMouseClick(data);
    }
}
