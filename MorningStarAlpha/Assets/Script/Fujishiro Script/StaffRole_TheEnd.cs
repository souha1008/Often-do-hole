using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffRole_TheEnd : MonoBehaviour
{
    //　テキストのスクロールスピード
    [SerializeField]
    private float textScrollSpeed = 30;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < 500)
        transform.position = new Vector2(transform.position.x, transform.position.y + textScrollSpeed * Time.deltaTime);
    }
}
