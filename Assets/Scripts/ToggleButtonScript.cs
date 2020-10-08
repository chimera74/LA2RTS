using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ToggleButtonScript : MonoBehaviour
{
    public Color enabledColor = new Color(57f / 255f, 1f, 0f);
    public Color disabledColor = new Color(113f / 255f, 113f / 255f, 113f / 255f);

    private Image image;
    
    // Use this for initialization
    protected void Awake()
    {   
        image = GetComponentInChildren<Image>();
    }

    public void OnStateChanged(bool isEnabled)
    {
        if (isEnabled)
            image.color = enabledColor;
        else
            image.color = disabledColor;
    }
}
