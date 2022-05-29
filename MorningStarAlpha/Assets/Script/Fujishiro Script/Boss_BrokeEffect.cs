using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_BrokeEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem brokeEffect;
    [Header("何個エフェクトを出すか")][SerializeField] int loop_num = 3;
    [Header("エフェクトの距離間隔")][SerializeField] float effect_interval = 30.0f;
    [Header("一個目のエフェクトを出す位置")][SerializeField] float start_y = 30;
    [Header("サイズを変更するかどうか")][SerializeField] bool sizeChange;
    [Header("スケールの変更")][SerializeField] Vector3 sizeChange_V3 = new Vector3(0.7f, 0.7f, 0.7f);

    Vector3 pos;    // エフェクト発生場所
    
    // Start is called before the first frame update
    void Start()
    {
        pos = this.gameObject.transform.position;
        pos.y += start_y;
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
                ParticleSystem newParticle = Instantiate(brokeEffect);
                if(sizeChange)
                {
                    newParticle.gameObject.transform.localScale = sizeChange_V3;
                }
                newParticle.gameObject.transform.position = new Vector3(pos.x, pos.y - effect_interval * i, 0);

                newParticle.Play();
            }
        }
    }
}
