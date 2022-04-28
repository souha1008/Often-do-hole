using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title_p2 : MonoBehaviour
{
    [Tooltip("Playerオブジェクト")] [SerializeField] GameObject Player;
    private Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        pos = Player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Player.transform.position = pos;
    }
}
