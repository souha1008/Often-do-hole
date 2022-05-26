using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KikenScript : MonoBehaviour
{
    public Sprite texture1;
    public Sprite texture2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int TempNum = (int)GameStateManager.GetGameTime();

        if(TempNum % 2 == 0)
        {
            GetComponent<Image>().sprite = texture1;
        }
        else
        {
            GetComponent<Image>().sprite = texture2;
        }
    }
}
