using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Raen_Path : MonoBehaviour
{
    public Vector3[] Waypoint;
    // public Raen_Path instance; // �p�X���

    // �A�^�b�`�����Ƃ��ɍŏ��ɐݒ肳���
    void Reset()
    {
        
    }

    void Awake()
    {
        Path_cart.waypoint = this;
    }
}
