using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// 1 for heuristic , 2 for AI . This is acc. to docs , don't change this.
public enum InputMode { Manual = 1, AI = 2 };

public class SettingsManager : Singleton<SettingsManager>
{
    public float lowerHRlimit, normalHRlimit, highHRlimit;
    public InputMode inputmode;
    public int exerciseDuration; // in minute, change later to sec 
    [HideInInspector] public FatPlayerSettings playersetting;

    protected override void Awake()
    {
        playersetting = FindObjectOfType<FatPlayerSettings>();
        //RestoreDefaults();
        //TODO : Load previous setting from SaveManager or 
        //current setting from UI manager
    }
    //called from UImanager when 'save' pressed
    public void SaveSettings(int l, int n, int h, int inputm, int ed)
    {
        lowerHRlimit = (float)l;
        normalHRlimit = (float)n;
        highHRlimit = (float)h;
        inputmode = (InputMode)inputm;
        exerciseDuration = ed;
    }

    public void RestoreDefaults()
    {
        lowerHRlimit = 60f;
        normalHRlimit = 90f;
        highHRlimit = 150f;
        inputmode = InputMode.AI;
        exerciseDuration = 5;
    }

}