using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class Raen_Path : MonoBehaviour
{
    [Serializable] public struct Waypoint
    {
        [Tooltip("�E�F�C�|�C���g�̈ʒu")]
        public Vector3 position;

        [Tooltip("�E�F�C�|�C���g�ɓ��B�����Ƃ��̊p�x")]
        public float roll;
    }

    public Waypoint[] m_waypoints = new Waypoint[0];

    // �A�^�b�`�����Ƃ��ɍŏ��ɐݒ肳���
    void Reset()
    {
        m_waypoints = new Waypoint[2]
        {
            new Waypoint{position = new Vector3(0, 0, 0), roll = 0},
            new Waypoint{position = new Vector3(0, 0, 0),roll = 0}
        };
    }
}
