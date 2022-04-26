using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;
    [SerializeField]  GameObject ShotParticle;

    private void Awake()
    {
        instance = this;
    }

    public void StartShotEffect(Vector3 Pos, Quaternion rot)
    {
        GameObject shot = Instantiate(ShotParticle, Pos, rot);

        ParticleSystem[] particles;
        particles = shot.gameObject.GetComponentsInChildren<ParticleSystem>();


        for (int i = 0; i < particles.Length; ++i)
        {
           particles[i].Play();
        }
    }
}
