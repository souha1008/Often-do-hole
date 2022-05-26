using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectBox: MonoBehaviour
{
    // Start is called before the first frame update
    private bool isOpen = false;
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.speed = 0.0f;

        if (isOpen)
        {
            anim.Play("BoxAnim", -1, 0.9f);
        }
        else
        {
            anim.Play("BoxAnim", -1, 0.0f);
        }
    }
}
