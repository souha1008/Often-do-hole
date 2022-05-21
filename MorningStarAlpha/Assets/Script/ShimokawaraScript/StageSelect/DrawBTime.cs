using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBTime : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SelectManager.instance.CanStart)
            gameObject.GetComponent<UnityEngine.UI.Text>().text = "BÅc" + (int)(GameStateManager.Instance.ClearRankTime[SelectManager.instance.NowSelectStage].B) + "ïbà»ì‡";
    }
}
