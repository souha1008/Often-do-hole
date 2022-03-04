using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [System.NonSerialized] public Rigidbody rb;
    public PlayerState mode;

    void Awake()
    {
        PlayerState.player = this; //PlayerStateë§Ç≈PlayerMoveéQè∆Ç≈Ç´ÇÈÇÊÇ§Ç…Ç∑ÇÈ
        rb = GetComponent<Rigidbody>();
        mode = new PlayerNone();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
