using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour, IPickable
{
    public float healthBonus = 1f;
    private bool used = false;

    void OnEnable()
    {
        used = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Use();
        }
    }

    public void Use()
    {
        if (used) return;
        GameController.Instance.player.AddHealth(healthBonus);
        SoundInstance.InstantiateOnPos(SoundInstance.GetClipFromLibrary("collect"), transform.position, 0.4f, false, SoundInstance.Randomization.Low);
        ParticleSystemManager.Instance.PlayParticle3(this.transform);
        used = true;
        gameObject.DestroyAPS();
    }

}