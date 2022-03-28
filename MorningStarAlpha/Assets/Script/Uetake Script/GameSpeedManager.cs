using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedManager : SingletonMonoBehaviour<GameSpeedManager>
{
    // Start is called before the first frame update
    private int FrameCounter;
    private bool isHitStop;
    private int stopTime;

    void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject); // シーンが変わっても死なない

        //ヒットストップ
        FrameCounter = 0;
        isHitStop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHitStop)
        {
            FrameCounter += 1;

            if(FrameCounter > stopTime)
            {
                isHitStop = false;
                Time.timeScale = 1;
                FrameCounter = 0;
            }
        } 
    }

    public void StartHitStop()
    {
        isHitStop = true;
        FrameCounter = 0;
        Time.timeScale = 0;

        stopTime = 30;
    }

    public void StartHitStop(int StopTime)
    {
        isHitStop = true;
        FrameCounter = 0;
        Time.timeScale = 0;

        stopTime = StopTime;
    }


    
}
