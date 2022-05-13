using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour, IPickable
{
    public ParticleSystem pickfx;
    public AudioClip pick_sound;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Use();
        }
    }

    public virtual void Use()
    {
    }

}