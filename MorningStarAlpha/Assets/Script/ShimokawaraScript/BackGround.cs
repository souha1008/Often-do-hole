using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{

    public RectTransform CanvasObj;
    // Start is called before the first frame update
    void Start()
    {
        TraceObj();
    }

    // Update is called once per frame
    void Update()
    {
        TraceObj();
    }

    void TraceObj()
    {
        transform.localScale = new Vector3(CanvasObj.rect.width, CanvasObj.rect.height , 0) ;
    }
}
