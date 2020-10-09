using UnityEngine;
using System.Collections;
using System;

public class MapTileManager : MonoBehaviour
{

    const int TILE_ARRAY_SIZE = 30;
    const float TILE_UPDATE_TIME_S = 0.5f;

    public GameObject tilePrefab;

    private L2RTSServerManager SM;
    private bool[,] loadedTiles = new bool[TILE_ARRAY_SIZE, TILE_ARRAY_SIZE];
    private GameObject[,] tiles = new GameObject[TILE_ARRAY_SIZE, TILE_ARRAY_SIZE];

    // Use this for initialization
    void Start()
    {
        SM = FindObjectOfType<L2RTSServerManager>();
        InvokeRepeating("UpdateTiles", TILE_UPDATE_TIME_S, TILE_UPDATE_TIME_S);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateTiles()
    {
        bool[,] tilesWithActors = new bool[TILE_ARRAY_SIZE, TILE_ARRAY_SIZE];

        foreach (var cl in SM.userActorManager.clientProperties.Keys)
        {
            var cp = SM.userActorManager.clientProperties[cl];
            if (cp != null && cp.actor != null)
            {
                int x = (int)(20 - (cp.actor.transform.position.x / 2048));
                int y = (int)(18 + (cp.actor.transform.position.z / 2048));

                for (int _x = x - 1; _x <= x + 1; _x++)
                    for (int _y = y - 1; _y <= y + 1; _y++)
                    {
                        tilesWithActors[_x, _y] = true;
                        if (!loadedTiles[_x, _y])
                        {
                            LoadTile(_x, _y);
                        }
                    }
            }
        }

        for (int x = 0; x < TILE_ARRAY_SIZE; x++)
            for (int y = 0; y < TILE_ARRAY_SIZE; y++)
            {
                if (loadedTiles[x, y] && !tilesWithActors[x, y])
                    UnloadTile(x, y);
            }
    }

    private void UnloadTile(int x, int y)
    {
        loadedTiles[x, y] = false;
        Destroy(tiles[x, y]);
    }

    private void LoadTile(int x, int y)
    {
        Vector3 tilePos = new Vector3
        {
            y = 0,
            x = (20 - x) * 2048,
            z = (y - 18) * 2048
        };
        loadedTiles[x, y] = true;
        tiles[x, y] = Instantiate(tilePrefab, tilePos, tilePrefab.transform.rotation);
        tiles[x, y].name = "Tile(" + x + "_" + y + ")";
    }
}
