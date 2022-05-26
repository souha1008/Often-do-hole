using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectBox: MonoBehaviour
{
    // Start is called before the first frame update
    private bool isOpen = false;
    private Animator anim;
    public int isNumber = 0;
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.speed = 0.0f;

        Debug.Log(isNumber);

        if(SaveDataManager.Instance.GetStageData(isNumber).Clear)
        {
            isOpen = true;
        }
        else
        {
            isOpen = false;
        }

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
