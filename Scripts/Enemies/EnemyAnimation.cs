using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimation : MonoBehaviour
{
    private Unit unit;

    public AnimationClip clipIdle;
    public AnimationClip clipMove;

    private Animator anim;

    void Awake()
    {
        unit = gameObject.GetComponent<Unit>();
        if (unit != null)
        {
            anim = GetComponent<Animator>();
            if (anim != null) unit.SetAnimation(this);
        }
        else return;

        //  AnimatorOverrideController overrideController = new AnimatorOverrideController();
        //overrideController.runtimeAnimatorController = anim.runtimeAnimatorController;

        //	overrideController["Idle"] = clipIdle;
        //	overrideController["Move"] = clipMove;
        //USE this when i make mutiple type of enemies.
        //  anim.runtimeAnimatorController = overrideController;
    }

    public void GotoMove()
    {
        // anim.ResetTrigger("atk");
    }

    public void GotoAtk1()
    {
        anim.SetTrigger("atk");
    }

    public void GotoDeath()
    {
        anim.SetTrigger("dead");
    }

    public void setRootMotion(bool b)
    {
        anim.applyRootMotion = b;
    }
}