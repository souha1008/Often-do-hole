using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;
    [SerializeField]  GameObject ShotParticle;
    [SerializeField] GameObject BurstParticle;
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

    public void ShotEffect(Vector3 Pos, Vector3 angle)
    {
        const float adjustDistance = 4.0f;
        Quaternion rot = Quaternion.identity;

        Vector3 vec = PlayerMain.instance.adjustLeftStick.normalized;
        vec = vec * adjustDistance;
        vec.y += 1.0f;
        Vector3 Point = PlayerMain.instance.rb.position + vec;

     

        GameObject shot = Instantiate(BurstParticle, Point, rot);

        shot.transform.localScale = Vector3.one * 3;
    }
}
