using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子要素のパーティクルをすべて再生する
/// </summary>
public class PlayAllParticles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem[] particles;
        particles = gameObject.GetComponentsInChildren<ParticleSystem>();

        for (int i = 0; i < particles.Length; ++i)
        {
            particles[i].Play();
        }
    }
}
