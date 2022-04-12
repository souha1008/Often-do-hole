using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raen_Path : MonoBehaviour
{
    public GameObject[] positions; // ウェイポイント配列
    public GameObject[] player_postions;

    void Awake()
    {
        Path_cart.waypoints = this;
    }

    
}
