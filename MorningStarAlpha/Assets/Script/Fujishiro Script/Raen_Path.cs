using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raen_Path : MonoBehaviour
{
    public GameObject[] positions; // �E�F�C�|�C���g�z��

    void Awake()
    {
        Path_cart.waypoints = this;
    }

    
}
