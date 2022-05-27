using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Result_WantedChange : MonoBehaviour
{
    [SerializeField] private Sprite StageWantedSprite;
    [SerializeField] private Sprite NoStageWantedSprite;
    void Start()
    {
        //if (GameStateManager.GetNowStage() == 0 || GameStateManager.GetNowStage() == 7)
        //{
        //    this.gameObject.GetComponent<Image>().sprite = NoStageWantedSprite;
        //}
        //else
        //{
        //    this.gameObject.GetComponent<Image>().sprite = StageWantedSprite;
        //}

        if (ResultManager.instance.debug_stageNo == 0 || ResultManager.instance.debug_stageNo == 7)
        {
            this.gameObject.GetComponent<Image>().sprite = NoStageWantedSprite;
        }
        else
        {
            this.gameObject.GetComponent<Image>().sprite = StageWantedSprite;
        }
    }
}
