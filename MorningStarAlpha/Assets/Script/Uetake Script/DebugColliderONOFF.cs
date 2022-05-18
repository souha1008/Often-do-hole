using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全ての子オブジェクトのコライダーを表示
/// </summary>
public class DebugColliderONOFF : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameStateManager.Instance.ColliderVisible == false)
        {
            foreach (GameObject child in gameObject.transform)
            {
                child.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
