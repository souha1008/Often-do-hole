using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGoal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerChangeClearState()
    {
        if (PlayerMain.instance.refState != EnumPlayerState.CLEAR)
        {
            PlayerMain.instance.mode = new PlayerState_Clear();
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerChangeClearState();
        }
        else
        {
            Debug.Log("ˆá‚¤‚à‚Ì");
        }
    }
}
