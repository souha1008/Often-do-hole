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

    public void StartShotEffect(Vector3 Pos)
    {
        GameObject shot = Instantiate(ShotParticle, Pos, Quaternion.identity);

        ParticleSystem child = shot.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        child.Play();
    }
}
