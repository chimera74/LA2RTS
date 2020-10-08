using UnityEngine;
using System.Collections;
using System;

public class ActorProperties
{
    public event Action<bool> SelectionChangedEvent;

    private bool _isSelected;
    public bool isSelected
    {
        get
        {
            return _isSelected;
        }
        set
        {
            _isSelected = value;
            if (SelectionChangedEvent != null)
                SelectionChangedEvent(_isSelected);
        }
    }

    public GameObject actor;

}
