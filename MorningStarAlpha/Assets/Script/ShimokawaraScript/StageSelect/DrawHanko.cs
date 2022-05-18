using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawHanko : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SelectManager.instance.CanStart)
        {
            //ÉNÉäÉA
            if(SaveDataManager.Instance.GetStageData(SelectManager.instance.NowSelectStage).Clear == false)
            {
                this.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/ClearRankStump/Toumei");
            }
            else
            {
                switch (SaveDataManager.Instance.GetStageData(SelectManager.instance.NowSelectStage).Rank)
                {
                    case GAME_RANK.S:
                        this.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/ClearRankStump/Stump_S");
                        break;

                    case GAME_RANK.A:
                        this.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/ClearRankStump/Stump_A");
                        break;

                    case GAME_RANK.B:
                        this.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/ClearRankStump/Stump_B");
                        break;
                }
            }
        }           
    }
}
