using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickBlockingScreen : MonoBehaviour {

    public GameObject RootUpfrontObject;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ClearScreen()
    {
        Destroy(RootUpfrontObject);
        Destroy(gameObject);
    }
}
