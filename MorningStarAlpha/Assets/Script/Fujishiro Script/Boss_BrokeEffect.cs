using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_BrokeEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem brokeEffect;

    Vector3 pos;    // エフェクト発生場所
    
    // Start is called before the first frame update
    void Start()
    {
        pos = this.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Boss"))
        {
            this.gameObject.SetActive(false);
            
            for(int i = 0; i < 6; i++)
            {
                ParticleSystem newParticle = Instantiate(brokeEffect);

                newParticle.gameObject.transform.position = new Vector3(pos.x, pos.y - 10 * i, pos.z);

                newParticle.Play();
            }
        }
    }
}
