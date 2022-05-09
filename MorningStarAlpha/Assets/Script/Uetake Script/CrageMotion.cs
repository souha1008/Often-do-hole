using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrageMotion : MonoBehaviour
{

    Animator animator;
    bool isOn;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            isOn = true;
        }
        else
        {
            isOn = false;
        }

        animator.SetBool("IsCatch", isOn);
    }
}
