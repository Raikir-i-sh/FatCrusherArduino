using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticleSystemManager : Singleton<ParticleSystemManager>
{
    public ParticleSystem blueDrop;
    public ParticleSystem redDrop;
    public ParticleSystem collectfx;

    public void PlayParticle1(Transform t)
    {
        GameObject g = Instantiate(blueDrop.gameObject, t.position, blueDrop.gameObject.transform.rotation);
        Destroy(g, 3f);
    }

    public void PlayParticle2(Transform t)
    {
        GameObject g = Instantiate(redDrop.gameObject, t.position, redDrop.gameObject.transform.rotation);
        Destroy(g, 3f);
    }

    public void PlayParticle3(Transform t)
    {
        GameObject g = Instantiate(collectfx.gameObject, t.position, collectfx.gameObject.transform.rotation);
        g.GetComponent<ParticleSystem>().Emit(1);
        Destroy(g, 3f);
    }
}