using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_BrokeEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem brokeEffect;
    [SerializeField] GameObject _brokeEffect;
    [SerializeField] int loop_num = 3;
    [SerializeField] float effect_interval = 30.0f;
    [SerializeField] Vector3 scale_change = new Vector3(0.3f, 0.3f, 0.3f);

    Vector3 pos;    // エフェクト発生場所
    
    // Start is called before the first frame update
    void Start()
    {
        pos = this.gameObject.transform.position;
        pos.y += 30;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Boss")
        {
            this.gameObject.SetActive(false);
            
            for(int i = 0; i < loop_num; i++)
            {
                //ParticleSystem newParticle = Instantiate(brokeEffect);
                //newParticle.gameObject.transform.localScale = scale_change;
                //newParticle.gameObject.transform.position = new Vector3(pos.x, pos.y - effect_interval * i, 0);

                //newParticle.Play();

                GameObject particle = Instantiate(_brokeEffect);
                particle.transform.localScale = scale_change;
                particle.gameObject.transform.position = new Vector3(pos.x, pos.y - effect_interval * i, 0);
                //Destroy(newParticle.gameObject, 5.0f);
            }
        }
    }
}
