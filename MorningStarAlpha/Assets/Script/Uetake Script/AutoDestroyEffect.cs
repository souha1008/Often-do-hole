using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyEffect : MonoBehaviour
{
    [SerializeField, Tooltip("消え始めまでにかかる時間")] private float StartDisableTime;
    [SerializeField, Tooltip("完全に消えるまでの時間")] private float FadeTime;

    private Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
