using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Raen_Path : MonoBehaviour
{
    public Vector3[] Waypoint;
    // public Raen_Path instance; // パス情報

    // アタッチしたときに最初に設定される
    void Reset()
    {
        
    }

    void Awake()
    {
        Path_cart.waypoint = this;
    }
}
