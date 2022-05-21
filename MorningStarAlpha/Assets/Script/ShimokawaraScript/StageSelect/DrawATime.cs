using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawATime : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SelectManager.instance.CanStart)
            gameObject.GetComponent<UnityEngine.UI.Text>().text = "AÅc" + (int)(GameStateManager.Instance.ClearRankTime[SelectManager.instance.NowSelectStage].A) + "ïbà»ì‡";
    }
}
