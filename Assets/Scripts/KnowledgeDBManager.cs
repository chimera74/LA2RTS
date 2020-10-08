using UnityEngine;
using System.Collections;
using System.Linq;

public class KnowledgeDBManager : MonoBehaviour
{
    private int[] raidBossIds =
    {
        25357,
        25188
    };

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsRaidBoss(int id)
    {
        return raidBossIds.Contains(id);
    }
}
