using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectManager : SingletonMonoBehaviour<EffectManager>
{
    private GameObject ShotParticle;
    private GameObject BurstParticle;
    private GameObject LandParticle;
    private GameObject TreasureOpenParticle;
    private GameObject BoostParticle;
    private GameObject CoinGetParticle;

    private void Awake()
    {
        ShotParticle = (GameObject)Resources.Load("Effect/05_stone_effect_01");
        BurstParticle = (GameObject)Resources.Load("Effect/Shot");
        BoostParticle = (GameObject)Resources.Load("Effect/Boost");
        LandParticle = (GameObject)Resources.Load("Effect/Landing");
        TreasureOpenParticle = (GameObject)Resources.Load("Effect/05_Treasure_open");
        CoinGetParticle = (GameObject)Resources.Load("Effect/05_coin");
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

    public void ShotEffect()
    {
        const float adjustDistance = 8.0f;
        Quaternion rot = Quaternion.Euler(-90, 0, 0);

        Quaternion rotateRot = Quaternion.LookRotation(PlayerMain.instance.adjustLeftStick);

        rot = rot * rotateRot;

        Vector3 vec = PlayerMain.instance.adjustLeftStick.normalized;
        vec = vec * adjustDistance;
        vec.y += 1.0f;
        Vector3 Point = PlayerMain.instance.rb.position + vec;


        GameObject shot = Instantiate(BurstParticle, Point, rotateRot);
        shot.transform.localScale = Vector3.one * 3; //サイズ変更
    }

    public GameObject BoostEffect(Vector3 BoostVel)
    {
        Vector3 forwardVel = BoostVel.normalized;
        const float adjustDistance =0.0f;
        Quaternion rot = Quaternion.Euler(-90, 0, 0);

        Quaternion rotateRot = Quaternion.LookRotation(forwardVel);


        Vector3 vec = PlayerMain.instance.adjustLeftStick.normalized;
        vec = forwardVel * adjustDistance;
        Vector3 Point = PlayerMain.instance.rb.position + vec;


        GameObject shotObject = Instantiate(BoostParticle, Point, rotateRot);
        shotObject.transform.localScale = Vector3.one * 2; //サイズ変更
        //shotObject.transform.SetParent(PlayerMain.instance.transform);


        return shotObject;
    }

    public void landEffect(Vector3 pos)
    {
        const float size = 4.0f;
        pos.y += 1.2f;
        GameObject effect = Instantiate(LandParticle, pos, Quaternion.Euler(-90, 0, 0));
        effect.transform.localScale = Vector3.one * size; //サイズ変更
    }

    public void BoxOpen(Vector3 pos)
    {
        const float size = 4.0f;
        pos.y += 1.2f;
        GameObject effect = Instantiate(TreasureOpenParticle, pos, Quaternion.identity);
        effect.transform.localScale = Vector3.one * size; //サイズ変更s
    }

    public void CoinGetEffect(Vector3 pos)
    {
        const float size = 12.0f;
        GameObject effect = Instantiate(CoinGetParticle, pos, Quaternion.identity);
        effect.transform.localScale = Vector3.one * size; //サイズ変更
    }
}
