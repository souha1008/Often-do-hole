using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameTimeDisplay : MonoBehaviour
{
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string displayTime;
        displayTime = ((int)GameStateManager.GetGameTime()).ToString("f0");
        displayTime = displayTime.PadLeft(3, '0');
        Debug.LogWarning(displayTime);

        text.text = displayTime;
    }
}
