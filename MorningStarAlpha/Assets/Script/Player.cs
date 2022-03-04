using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [System.NonSerialized] public Rigidbody rb;
    public PlayerState mode;

    void Awake()
    {
        PlayerState.player = this; //PlayerState側でPlayerMove参照できるようにする
        rb = GetComponent<Rigidbody>();
        mode = new PlayerNone();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
