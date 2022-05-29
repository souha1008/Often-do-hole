using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kujira_Effect_ONOFF : MonoBehaviour
{
    [SerializeField] GameObject fog_1;
    [SerializeField] GameObject fog_2;
 
    // Start is called before the first frame update
    void Start()
    {
        fog_1.SetActive(false);
        fog_2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider collider)
    {
        if(collider.CompareTag("Iron"))
        {
            fog_1.SetActive(true);
            fog_2.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if(collider.CompareTag("Iron"))
        {
            fog_1.SetActive(false);
            fog_2.SetActive(false);
        }
    }
}
