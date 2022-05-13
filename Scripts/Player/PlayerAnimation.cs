using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void GotoRun()
    {
        anim.SetBool("run", true);
    }

    public void GotoIdle()
    {
        anim.SetBool("run", false);
    }

    public void GotoMeleeAtk()
    {
        anim.SetTrigger("melee_atk");
    }

    public void GotoRangedAtk()
    {
    }

    public void GotoHurt()
    {
    }

    public void GotoJump()
    {
    }

    public void Reset_Trigger(string s)
    {
        anim.ResetTrigger(s);
    }

    public void AE_MeleeSound()
    {
        SoundInstance.InstantiateOnTransform(SoundInstance.GetClipFromLibrary("atk"), transform, 0.5f, false, SoundInstance.Randomization.Low);
    }
}