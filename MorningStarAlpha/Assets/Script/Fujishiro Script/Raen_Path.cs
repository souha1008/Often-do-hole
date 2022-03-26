using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class Raen_Path : MonoBehaviour
{
    [Serializable] public struct Waypoint
    {
        [Tooltip("ウェイポイントの位置")]
        public Vector3 position;

        [Tooltip("ウェイポイントに到達したときの角度")]
        public float roll;
    }

    public Waypoint[] m_waypoints = new Waypoint[0];

    // アタッチしたときに最初に設定される
    void Reset()
    {
        m_waypoints = new Waypoint[2]
        {
            new Waypoint{position = new Vector3(0, 0, 0), roll = 0},
            new Waypoint{position = new Vector3(0, 0, 0),roll = 0}
        };
    }
}
