using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; // <- Cinemachine ‚ð@Using

public enum KujiraSpeed
{
    High,
    Low
}

public class KujiraCenterPos : MonoBehaviour
{
    public static KujiraCenterPos instance;

    CinemachineDollyCart myDolly;
    float dollySpeed = 0;

    public KujiraSpeed MyKujiraSpeed = KujiraSpeed.Low;

    void Start()
    {
        instance = this;
        MyKujiraSpeed = KujiraSpeed.Low;
    }

    private void Awake()
    {
        myDolly = GetComponent<CinemachineDollyCart>();
    }

    private void Update()
    {
        switch (MyKujiraSpeed)
        {
            case KujiraSpeed.Low:
                myDolly.m_Speed = 20;
                break;

            case KujiraSpeed.High:
                myDolly.m_Speed = 25;
                break;

        }

    }
}
