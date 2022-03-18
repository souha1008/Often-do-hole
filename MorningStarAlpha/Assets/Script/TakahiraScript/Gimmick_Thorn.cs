using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Thorn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // �R���W����
        this.gameObject.GetComponent<Collider>().isTrigger = true; // �g���K�[�ɂ���
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            // �v���C���[�����S��ԂɕύX
            PlayerMain.instance.mode = new PlayerStateDeath();
            // �v���C���[�Ƀ_���[�W�G�t�F�N�g

            // �t�F�[�h����
            FadeManager.Instance.SetNextFade(FADE_STATE.FADE_OUT, FADE_KIND.FADE_GAMOVER);
        }
    }
}
