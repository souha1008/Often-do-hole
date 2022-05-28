using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracePlayer : MonoBehaviour
{
    private float Distance;
    private Vector3 Angle;

    // Start is called before the first frame update
    void Start()
    {
        Distance = Vector3.Distance(this.transform.position, PlayerMain.instance.rb.position);
        Angle = this.transform.position - PlayerMain.instance.rb.position;
        Angle = Angle.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tempPos = PlayerMain.instance.rb.position + (Distance * Angle);
            transform.position = tempPos;
    }
}
