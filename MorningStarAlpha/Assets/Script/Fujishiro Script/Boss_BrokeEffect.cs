using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_BrokeEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem brokeEffect;
    [Header("���G�t�F�N�g���o����")][SerializeField] int loop_num = 3;
    [Header("�G�t�F�N�g�̋����Ԋu")][SerializeField] float effect_interval = 30.0f;
    [Header("��ڂ̃G�t�F�N�g���o���ʒu")][SerializeField] float start_y = 30;
    [Header("�T�C�Y��ύX���邩�ǂ���")][SerializeField] bool sizeChange;
    [Header("�X�P�[���̕ύX")][SerializeField] Vector3 sizeChange_V3 = new Vector3(0.7f, 0.7f, 0.7f);

    Vector3 pos;    // �G�t�F�N�g�����ꏊ
    
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
