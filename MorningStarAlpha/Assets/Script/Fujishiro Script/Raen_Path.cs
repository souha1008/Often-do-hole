using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raen_Path : MonoBehaviour
{
    public Vector3[] Path; // �E�F�C�|�C���g�z��

    void Awake()
    {
        Path_cart.waypoints = this;
    }

    
}
