using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelFall : MonoBehaviour
{
    public GameObject[] Jewel = new GameObject[8];      // �v���n�u�i�[�p

    private class JEWEL_LIST
    {
        public GameObject Obj;   // ���������v���n�u�i�[�p
        public Rigidbody Rb;  // ���������v���n�u�̃��W�b�h�{�f�B�i�[�p
        public Vector3 Vec;   // ���������v���n�u�̃����_����]�ʊi�[�p
        public int Num;           // �W���G���ԍ�

        public JEWEL_LIST(GameObject obj, Rigidbody rb, Vector3 vec, int num)
        {
            Obj = obj;
            Rb = rb;
            Vec = vec;
            Num = num;
        }
    }

    List<JEWEL_LIST> JewelList = new List<JEWEL_LIST>();

    private static float StartPos_Y = 90.0f;        // ����Y���W
    private static float EndPos_Y = -140.0f;        // ����Y���W
    private static float FallLength_X = 160.0f;     // X���W�̍~�点��ő啝
    private static float FallLength_Z = 125.0f;     // Z���W�̍~�点��ő啝
    private static float StartVel_Y = -100.0f;      // �ŏ��ɉ�����Y�����ړ���
    private static float StartDelayTime = 1.65f;     // �ŏ��ɒx�点�鎞��(�b)
    private static float JewelFallTime = 0.02f;    // �W���G�����~�鎞��(�b)
    private static int JewelFallNum = 10;           // �W���G������x�ɍ~���
    private static float JewelFallRot = 2.0f;       // �W���G���̍ő��]��

    private bool StartDelayFlag = false;            // �ŏ��ɒx�点��p�t���O
    private float NowTime = 0.0f;                   // ���ݎ���
    //private float FallSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // �J�n�x������
        if (!StartDelayFlag)
        {
            if (NowTime >= StartDelayTime)
            {
                SoundManager.Instance.PlaySound("Result_BGM");
                StartDelayFlag = true;
                NowTime = 0.0f;
            }
            else
            {
                NowTime += Time.deltaTime;
                return;
            }
        }

        // ���ԂɒB���Ă�����~�点��
        if (NowTime >= JewelFallTime)
        {
            for (int i = 0; i < JewelFallNum; i++)
            {
                int Jewel_rand = Random.Range(0, 8);                            // �����_���ŃW���G���I��
                float Pos_rand_X = Random.Range(-FallLength_X, FallLength_X);   // X���W�w��
                float Pos_rand_Z;   // Z���W�w��

                // ���ɑ����~�炷
                if (i <= JewelFallNum * 0.3f)
                    Pos_rand_Z = Random.Range(0.0f, FallLength_Z); 
                else
                    Pos_rand_Z = Random.Range(FallLength_Z * 0.5f, FallLength_Z); // Z���W�w��

                Vector3 FallPos = new Vector3(Pos_rand_X, StartPos_Y, Pos_rand_Z);   // ���W����

                bool ReUseFlag = false;
                for (int j = 0; j < JewelList.Count; j++)
                {
                    if (!JewelList[j].Obj.activeSelf && JewelList[j].Num == Jewel_rand)
                    {
                        ReUseFlag = true;

                        JewelList[j].Obj.SetActive(true);
                        JewelList[j].Obj.transform.position = FallPos;
                        JewelList[j].Rb.velocity = Vector3.zero;
                        JewelList[j].Rb.AddForce(0.0f, StartVel_Y, 0.0f, ForceMode.VelocityChange);   // �ŏ��ɉ�����Y�����ړ���
                        JewelList[j].Vec = new Vector3(
                            Random.Range(-JewelFallRot, JewelFallRot), 
                            Random.Range(-JewelFallRot, JewelFallRot), 
                            Random.Range(-JewelFallRot, JewelFallRot)
                            ); // ��]�ʃ����_��
                        break;
                    }
                }

                if (!ReUseFlag)
                {
                    GameObject Jw = Instantiate(Jewel[Jewel_rand], FallPos, Quaternion.identity);
                    Rigidbody JwRb = Jw.GetComponent<Rigidbody>();
                    JwRb.AddForce(0.0f, StartVel_Y, 0.0f, ForceMode.VelocityChange);   // �ŏ��ɉ�����Y�����ړ���
                    Vector3 JwVec = new Vector3(
                            Random.Range(-JewelFallRot, JewelFallRot),
                            Random.Range(-JewelFallRot, JewelFallRot),
                            Random.Range(-JewelFallRot, JewelFallRot)
                            ); // ��]�ʃ����_��
                    JewelList.Add(new JEWEL_LIST(Jw, JwRb, JwVec, Jewel_rand));
                }
                
            }

            NowTime = 0.0f;
        }
        else
            NowTime += Time.deltaTime;

        // �v���n�u�I������ or �ړ�����
        for (int i = 0; i < JewelList.Count; i++)
        {
            if (JewelList[i].Obj.activeSelf)
            {
                if (JewelList[i].Obj.transform.position.y <= EndPos_Y)
                {
                    //Destroy(JewelList[i]);
                    //JewelList.RemoveAt(i);
                    //JewelRbList.RemoveAt(i);
                    //JewelVecList.RemoveAt(i);
                    //i--;
                    JewelList[i].Obj.SetActive(false);
                }
                else
                {
                    JewelList[i].Rb.rotation *= Quaternion.Euler(JewelList[i].Vec.x, JewelList[i].Vec.y, JewelList[i].Vec.z);
                }
            }
        }
    }
}
