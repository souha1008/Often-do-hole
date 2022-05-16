using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaManager : MonoBehaviour
{
    AlphaManager instance;

    [SerializeField] public SelectUIImageAlpha[] ImageArray;
    [SerializeField] public SelectUITextAlpha[] TextArray;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(SelectManager.instance.CanStart)
        {
            SetOriginnal();
        }
        else
        {
            SetClear();
        }
    }

    public void SetOriginnal()
    {
        for (int i = 0; i < ImageArray.Length; i++)
        {
            ImageArray[i].SetOriginnal();
        }
        for (int i = 0; i < TextArray.Length; i++)
        {
            TextArray[i].SetOriginnal();
        }
    }

    public void SetClear()
    {
        for (int i = 0; i < ImageArray.Length; i++)
        {
            ImageArray[i].SetClear();
        }
        for (int i = 0; i < TextArray.Length; i++)
        {
            TextArray[i].SetClear();
        }
    }
}
