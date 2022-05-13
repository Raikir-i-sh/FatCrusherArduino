using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adrenaline : Powerup
{
    public float timelimit = 4;
    public float multiplier = 1.4f;

    public override void Use()
    {
        print("Speed Boosted");
        base.Use();
        GameController.Instance.player.moveSpeedMultiplier = multiplier;
        //TODO : particle system.
        //pickfx.emit(true);
        //AudioManager.Play(pick_sound);
        Invoke("EndAbility", timelimit);
    }

    private void EndAbility()
    {
        GameController.Instance.player.moveSpeedMultiplier = 1;
        gameObject.DestroyAPS();
    }

}