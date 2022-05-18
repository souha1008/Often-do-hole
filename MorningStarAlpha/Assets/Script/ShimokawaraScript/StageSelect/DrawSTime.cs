using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSTime : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SelectManager.instance.CanStart)
            gameObject.GetComponent<UnityEngine.UI.Text>().text = "SÅc" + (int)(GameStateManager.Instance.ClearRank[SelectManager.instance.NowSelectStage].S) + "ïbà»ì‡";
    }
}
