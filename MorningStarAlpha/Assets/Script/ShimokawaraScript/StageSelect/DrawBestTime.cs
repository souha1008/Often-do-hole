using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DrawBestTime : MonoBehaviour
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
            if(SaveDataManager.Instance.GetStageData(SelectManager.instance.NowSelectStage).Time == 1000)
            {
                gameObject.GetComponent<TextMeshProUGUI>().text = "- - - , - -";

            }
            else
            {
                //Debug.Log(SaveDataManager.Instance.GetStageData(SelectManager.instance.NowSelectStage).Time);
                float num = SaveDataManager.Instance.GetStageData(SelectManager.instance.NowSelectStage).Time;
               
                int tempNum = (int)(num * 100.0f);//for‚Ì’†‚Å•Ï“®‚·‚é‚æ

                int[] NumArray = {0,0,0,0,0 };

                for(int i = 0; i < 5; i++)
                {
                    int Warukazu = 1;//10‚Ìnæ
                    for(int j = 0; j< (4 - i); j++)
                    {
                        Warukazu *= 10;
                    }


                    NumArray[i] = tempNum / Warukazu;

                    tempNum -= (NumArray[i] * Warukazu);
                }

                gameObject.GetComponent<TextMeshProUGUI>().text = "" +
                    NumArray[0] + 
                    NumArray[1] + 
                    NumArray[2] + "," + 
                    NumArray[3] + 
                    NumArray[4] ;

                   

            }
        }
    }
}
