using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class PreferencesManager : MonoBehaviour
{

    [Serializable]
    public class BoolPropertyChangedEvent : UnityEvent<bool>
    {
    }

    public enum AutoTargetSelectionMode
    {
        Monsters,
        PvP_PK,
        Everyone
    }

    public bool isForceAtackEnabled;
    public AutoTargetSelectionMode autoTargetSelectionMode;

    private bool isUserMovementLinesEabled;
    private bool isPlayerMovementLinesEnabled;
    private bool isNPCMovementLinesEnabled;

    private bool isUserTargetLinesEnabled;
    private bool isPlayerTargetLinesEnabled;
    private bool isNPCTargetLinesEnabled;

    private bool isUserNameplatesEnabled;
    private bool isPlayerNameplatesEnabled;
    private bool isNPCNameplatesEnabled;

    public bool IsUserMovementLinesEnabled
    {
        get
        {
            return isUserMovementLinesEabled;
        }

        set
        {
            isUserMovementLinesEabled = value;
            if (onUMLChanged != null)
                onUMLChanged.Invoke(value);
            SaveProperties();
        }
    }
    public void ToggleIsUserMovementLinesEabled()
    {
        IsUserMovementLinesEnabled = !IsUserMovementLinesEnabled;
    }
    public BoolPropertyChangedEvent onUMLChanged;

    public void ToggleIsPlayerMovementLinesEnabled()
    {
        IsPlayerMovementLinesEnabled = !IsPlayerMovementLinesEnabled;
    }
    public bool IsPlayerMovementLinesEnabled
    {
        get
        {
            return isPlayerMovementLinesEnabled;
        }

        set
        {
            isPlayerMovementLinesEnabled = value;
            if (onPMLChanged != null)
                onPMLChanged.Invoke(value);
            SaveProperties();
        }
    }
    public BoolPropertyChangedEvent onPMLChanged;

    public void ToggleIsNPCMovementLinesEnabled()
    {
        IsNPCMovementLinesEnabled = !IsNPCMovementLinesEnabled;
    }
    public bool IsNPCMovementLinesEnabled
    {
        get
        {
            return isNPCMovementLinesEnabled;
        }

        set
        {
            isNPCMovementLinesEnabled = value;
            if (onNMLChanged != null)
                onNMLChanged.Invoke(value);
            SaveProperties();
        }
    }
    public BoolPropertyChangedEvent onNMLChanged;

    public void ToggleIsUserTargetLinesEnabled()
    {
        IsUserTargetLinesEnabled = !IsUserTargetLinesEnabled;
    }
    public bool IsUserTargetLinesEnabled
    {
        get
        {
            return isUserTargetLinesEnabled;
        }

        set
        {
            isUserTargetLinesEnabled = value;
            if (onUTLChanged != null)
                onUTLChanged.Invoke(value);
            SaveProperties();
        }
    }
    public BoolPropertyChangedEvent onUTLChanged;

    public void ToggleIsPlayerTargetLinesEnabled()
    {
        IsPlayerTargetLinesEnabled = !IsPlayerTargetLinesEnabled;
    }
    public bool IsPlayerTargetLinesEnabled
    {
        get
        {
            return isPlayerTargetLinesEnabled;
        }

        set
        {
            isPlayerTargetLinesEnabled = value;
            if (onPTLChanged != null)
                onPTLChanged.Invoke(value);
            SaveProperties();
        }
    }
    public BoolPropertyChangedEvent onPTLChanged;

    public void ToggleIsNPCTargetLinesEnabled()
    {
        IsNPCTargetLinesEnabled = !IsNPCTargetLinesEnabled;
    }
    public bool IsNPCTargetLinesEnabled
    {
        get
        {
            return isNPCTargetLinesEnabled;
        }

        set
        {
            isNPCTargetLinesEnabled = value;
            if (onNTLChanged != null)
                onNTLChanged.Invoke(value);
            SaveProperties();
        }
    }
    public BoolPropertyChangedEvent onNTLChanged;

    public void ToggleIsUserNameplatesEnabled()
    {
        IsUserNameplatesEnabled = !IsUserNameplatesEnabled;
    }
    public bool IsUserNameplatesEnabled
    {
        get
        {
            return isUserNameplatesEnabled;
        }

        set
        {
            isUserNameplatesEnabled = value;
            if (onUNChanged != null)
                onUNChanged.Invoke(value);
            SaveProperties();
        }
    }
    public BoolPropertyChangedEvent onUNChanged;

    public void ToggleIsPlayerNameplatesEnabled()
    {
        IsPlayerNameplatesEnabled = !IsPlayerNameplatesEnabled;
    }
    public bool IsPlayerNameplatesEnabled
    {
        get
        {
            return isPlayerNameplatesEnabled;
        }

        set
        {
            isPlayerNameplatesEnabled = value;
            if (onPNChanged != null)
                onPNChanged.Invoke(value);
            SaveProperties();
        }
    }
    public BoolPropertyChangedEvent onPNChanged;

    public void ToggleIsNPCNameplatesEnabled()
    {
        IsNPCNameplatesEnabled = !IsNPCNameplatesEnabled;
    }
    public bool IsNPCNameplatesEnabled
    {
        get
        {
            return isNPCNameplatesEnabled;
        }

        set
        {
            isNPCNameplatesEnabled = value;
            if (onNNChanged != null)
                onNNChanged.Invoke(value);
            SaveProperties();
        }
    }
    public BoolPropertyChangedEvent onNNChanged;
    
    void Start()
    {
        LoadProperties();
    }

    public void LoadProperties()
    {
        SetDefaults();
    }

    public void SetDefaults()
    {
        IsUserTargetLinesEnabled = true;
        IsPlayerTargetLinesEnabled = true;
        IsNPCTargetLinesEnabled = true;

        IsPlayerMovementLinesEnabled = true;
        IsUserMovementLinesEnabled = true;
        IsNPCMovementLinesEnabled = true;

        IsUserNameplatesEnabled = true;
        IsPlayerNameplatesEnabled = true;
        IsNPCNameplatesEnabled = false;
    }

    public void SaveProperties()
    {

    }
}
