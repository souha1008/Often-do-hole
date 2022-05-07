using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField] GameObject imageCanvas;
    [System.NonSerialized] public bool anim_end;
    [System.NonSerialized] public static ResultManager instance;



    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim_end = false;
        imageCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(anim_end == true && imageCanvas.activeSelf == false)
        {
            imageCanvas.SetActive(true);
        }

        
    }
}
