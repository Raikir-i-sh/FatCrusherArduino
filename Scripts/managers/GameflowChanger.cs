using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameflowChanger : Singleton<GameflowChanger>
{
    public Vector2 minToMaxSpawnRate; // min=2 max=9
    public Vector2 minToMaxMovespeedRate;
    public Vector2 minToMaxShootRate;
    public Vector2 minToMaxFatScale;
    private float maxTimeScale = 1.6f;

    private float delayDecreaseRate, deltaspeed, deltaspawn, deltashoot, deltafat;

    void Start()
    {
        Init();
        if (MyUduinoManager.Instance.IsConnected)
            // call this only when in High BPM range
            StartCoroutine(hRavailable());
        else
            StartCoroutine(steadyIncrease());
    }

    void Init()
    {
        minToMaxSpawnRate = new Vector2(2f, 9f);
        minToMaxMovespeedRate = new Vector2(10f, 18f);
        minToMaxShootRate = new Vector2(0.2f, 0.8f);
        minToMaxFatScale = new Vector2(0.75f, 1f);
    }

    IEnumerator hRavailable()
    {
        GameEventArgs args = new GameEventArgs();

        while (true)
        {
            // Get heartrate limit from settings. This is used like a percentage here.
            delayDecreaseRate = (float)GetDelayRate(MyUduinoManager.Instance.HR);
            if (delayDecreaseRate > 1) delayDecreaseRate = 1;
            print("delay=" + delayDecreaseRate);
            Time.timeScale += (maxTimeScale - Time.timeScale) * delayDecreaseRate;
            // all u have to do is add required variable from subscribed methods 
            // This is done so that i don't need references of all those classes.
            deltaspeed = (minToMaxMovespeedRate.y - minToMaxMovespeedRate.x) * delayDecreaseRate;
            deltaspawn = (minToMaxSpawnRate.y - minToMaxSpawnRate.x) * delayDecreaseRate;
            deltashoot = (minToMaxShootRate.y - minToMaxShootRate.x) * delayDecreaseRate;
            deltafat = (minToMaxFatScale.y - minToMaxFatScale.x) * delayDecreaseRate;

            args.SetAllValues(delayDecreaseRate, deltaspawn, deltaspeed, deltashoot, deltafat);
            OnRaiseEvent(args);
            yield return new WaitForSeconds(5f);
            // every 5 sec check heartrate and change variables.
        }
    }

    //balances delayrate depending on which range of heartbeat it lies
    private float GetDelayRate(int currentHeartRate)
    {
        float delayR;
        //Regular High range
        if (currentHeartRate >= SettingsManager.Instance.normalHRlimit &&
            currentHeartRate < SettingsManager.Instance.highHRlimit)
        {
            delayR = (currentHeartRate - SettingsManager.Instance.normalHRlimit) / (SettingsManager.Instance.highHRlimit - SettingsManager.Instance.normalHRlimit);

            return (float)Math.Round(delayR * 100f) / 100f; //0.11222 to 0.11
        }
        //Normal range
        if (currentHeartRate >= SettingsManager.Instance.lowerHRlimit &&
             currentHeartRate < SettingsManager.Instance.normalHRlimit)
        {
            delayR = (currentHeartRate - SettingsManager.Instance.lowerHRlimit) / (SettingsManager.Instance.normalHRlimit - SettingsManager.Instance.lowerHRlimit);

            return delayR / 3;
        }

        if (currentHeartRate < SettingsManager.Instance.lowerHRlimit)
        {
            delayR = currentHeartRate / SettingsManager.Instance.lowerHRlimit;

            return delayR / 5;
        }
        //Irregular high range
        if (currentHeartRate >= SettingsManager.Instance.highHRlimit)
            return 1.1f;
        return 0;
    }

    //when heartrate not available

    IEnumerator steadyIncrease()
    {
        GameEventArgs args = new GameEventArgs();

        // the number update frequency we need for 'delayR' to reach 1 (i.e 100%)
        float waitDuration = 1 / 0.01f;

        while (true)
        {
            waitDuration = (SettingsManager.Instance.exerciseDuration * 60) / waitDuration;
            delayDecreaseRate += 0.01f;
            if (delayDecreaseRate > 1) delayDecreaseRate = 1;
            Time.timeScale += (maxTimeScale - Time.timeScale) * delayDecreaseRate;

            deltaspeed = (minToMaxMovespeedRate.y - minToMaxMovespeedRate.x) * delayDecreaseRate;
            deltaspawn = (minToMaxSpawnRate.y - minToMaxSpawnRate.x) * delayDecreaseRate;
            deltashoot = (minToMaxShootRate.y - minToMaxShootRate.x) * delayDecreaseRate;
            deltafat = (minToMaxFatScale.y - minToMaxFatScale.x) * delayDecreaseRate;
            print("fatscale = " + deltafat);
            args.SetAllValues(delayDecreaseRate, deltaspawn, deltaspeed, deltashoot, deltafat);
            OnRaiseEvent(args);

            yield return new WaitForSeconds(waitDuration);
        }
    }

    public event EventHandler<GameEventArgs> RaiseEvent;

    protected void OnRaiseEvent(GameEventArgs e)
    {
        EventHandler<GameEventArgs> raiseEvent = RaiseEvent;
        // Event will be null if there are no subscribers
        if (raiseEvent != null)
        {
            raiseEvent(this, e);
        }
    }
}

public class GameEventArgs : EventArgs
{
    //making constructor is NOT a must
    public GameEventArgs(float delayRate, float spawnDecreaseRate, float playerMovespeed, float shootDecreaseRate)
    {
        this.delayRate = delayRate;
        this.spawnDecreaseRate = spawnDecreaseRate;
        this.playerMovespeed = playerMovespeed;
        this.shootDecreaseRate = shootDecreaseRate;
    }
    public GameEventArgs()
    {
        this.delayRate = 0;
        this.spawnDecreaseRate = 0;
        this.playerMovespeed = 0;
        this.shootDecreaseRate = 0;
    }

    public void SetAllValues(float delay, float spwn, float ms, float shoot, float fat)
    {
        delayRate = delay;
        spawnDecreaseRate = spwn;
        playerMovespeed = ms;
        shootDecreaseRate = shoot;
        fatLossScale = fat;
    }
    public float delayRate { get; set; }
    public float spawnDecreaseRate { get; set; }
    public float playerMovespeed { get; set; }
    public float shootDecreaseRate { get; set; }
    public float fatLossScale { get; set; }
}