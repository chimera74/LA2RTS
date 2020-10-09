using UnityEngine;
using System.Collections;
using System;

public class ActorProperties
{
    public event Action<bool> SelectionChangedEvent;

    private bool _isSelected;
    public bool isSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            SelectionChangedEvent?.Invoke(_isSelected);
        }
    }

    public GameObject actor;

}
