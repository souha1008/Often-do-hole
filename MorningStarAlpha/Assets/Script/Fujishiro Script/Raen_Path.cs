using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raen_Path : MonoBehaviour
{
    public GameObject[] positions; // ウェイポイント配列

    void Awake()
    {
        Path_cart.waypoints = this;
    }

    
}
