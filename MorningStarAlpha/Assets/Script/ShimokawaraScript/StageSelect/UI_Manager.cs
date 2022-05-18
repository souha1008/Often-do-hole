using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    RectTransform CanvasObj;
    [SerializeField] public GameObject S_RankTime;
    [SerializeField] public GameObject A_RankTime;
    [SerializeField] public GameObject B_RankTime;
    [SerializeField] public GameObject ClearRank;
    [SerializeField] public GameObject Coin;

    public RectTransform rectS_RankTime;
    public RectTransform rectA_RankTime;
    public RectTransform rectB_RankTime;
    public RectTransform rectClearRank;
    public RectTransform rectCoin;

    float CANVAS_WIDTH;
    float CANVAS_HEIGHT;

    Vector3 RankTimeSize;

    // Start is called before the first frame update
    void Start()
    {
#if false
        CanvasObj = this.GetComponent<RectTransform>();
        CANVAS_WIDTH = CanvasObj.rect.width;
        CANVAS_HEIGHT = CanvasObj.rect.height;

        rectS_RankTime = S_RankTime.GetComponent<RectTransform>(); transform.GetComponent<RectTransform>();
        rectA_RankTime = A_RankTime.GetComponent<RectTransform>(); transform.GetComponent<RectTransform>();
        rectB_RankTime = B_RankTime.GetComponent<RectTransform>(); transform.GetComponent<RectTransform>();
        rectClearRank = ClearRank.GetComponent<RectTransform>(); transform.GetComponent<RectTransform>();
        rectCoin = Coin.GetComponent<RectTransform>(); transform.GetComponent<RectTransform>();

        RankTimeSize.x = rectS_RankTime.rect.width * rectS_RankTime.localScale.x;
        RankTimeSize.y = rectS_RankTime.rect.height * rectS_RankTime.localScale.y;
        RankTimeSize.z = 0;
        SetPosition();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        //SetPosition();
    }


    void SetPosition()
    {


        //{
        //    Vector3 TempPos;
        //    TempPos = new Vector3(CANVAS_WIDTH / 2 - (RankTimeSize.x / 2), CANVAS_HEIGHT / 2 - (rectS_RankTime.rect.height / 2), 0);
        //    rectS_RankTime.localPosition = TempPos;
        //}

        {
            Vector3 TempPos;
            TempPos = new Vector3(CANVAS_WIDTH / 2 - (RankTimeSize.x / 2), CANVAS_HEIGHT / 2 - (RankTimeSize.y / 2), 0);
            rectS_RankTime.localPosition = TempPos;
        }
        {
            Vector3 TempPos = rectS_RankTime.localPosition;
            TempPos.y -= RankTimeSize.y;
            rectA_RankTime.localPosition = TempPos;
        }
        {
            Vector3 TempPos = rectA_RankTime.localPosition;
            TempPos.y -= RankTimeSize.y;
            rectB_RankTime.localPosition = TempPos;
        }

    }
}
