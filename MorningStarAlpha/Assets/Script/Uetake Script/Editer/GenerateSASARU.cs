using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// �S�Ă̎q�I�u�W�F�N�g�̃R���C�_�[��\��
/// </summary>
public class GenerateSASARU : MonoBehaviour
{
    [Tooltip("�����������I�u�W�F�N�g")] public GameObject platform;
    [Range(-0.1f, 0.1f), Tooltip("�T�C�Y�����p�F ���̐��i�����ڗp�I�u�W�F�N�g���傫�߂ɕ\�������j���̐��i�����ڗp�I�u�W�F�N�g�������߂ɕ\�������j")] public float AdjustSizeX = 0.0f;
    [Range(-0.1f, 0.1f), Tooltip("�T�C�Y�����p�F ���̐��i�����ڗp�I�u�W�F�N�g���傫�߂ɕ\�������j���̐��i�����ڗp�I�u�W�F�N�g�������߂ɕ\�������j")] public float AdjustSizeY = 0.0f;
    [Range(-0.1f, 1.0f), Tooltip("�T�C�Y�����p�F ���̐��i�����ڗp�I�u�W�F�N�g���傫�߂ɕ\�������j���̐��i�����ڗp�I�u�W�F�N�g�������߂ɕ\�������j")] public float AdjustSizeZ = 0.0f;

#if UNITY_EDITOR
    [CustomEditor(typeof(GenerateSASARU))]
    public class GeneratePlatformEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GenerateSASARU genPlat = target as GenerateSASARU;

            GUILayout.Space(20);
            if (GUILayout.Button("�n�`�̐���", GUILayout.Height(72)))
            {
                //��������͍̂폜
                DeleteChildObject();

                   
                GenerateGround();
            }

            if (GUILayout.Button("�n�`�̍폜", GUILayout.Height(72)))
            {
                DeleteChildObject();
            }

            void GenerateGround()
            {
                //�^�񒆂̔�������
                {
                    GameObject obj = Instantiate(genPlat.platform);
                    //�ʒu���킹
                    obj.transform.position = genPlat.transform.position;

                    //�T�C�Y���킹
                    {
                        Vector3 newScale = Vector3.zero;

                        //���Ƃ̃��f���f�[�^�Ɉˑ������ɃT�C�Y��1:1:1�ɂ���
                        newScale.x = 1.0f / obj.GetComponent<Renderer>().bounds.size.x;
                        newScale.y = 1.0f / obj.GetComponent<Renderer>().bounds.size.y;
                        newScale.z = 1.0f / obj.GetComponent<Renderer>().bounds.size.z;

                        //�e�X�g�p����̃T�C�Y�ɍ��킹��
                        newScale.x *= genPlat.transform.localScale.x;
                        newScale.y *= genPlat.transform.localScale.y;
                        newScale.z *= genPlat.transform.localScale.z;

                        newScale.x += genPlat.AdjustSizeX;
                        newScale.y += genPlat.AdjustSizeY;
                        newScale.z += genPlat.AdjustSizeZ;

                        obj.transform.localScale = newScale;
                    }

                    obj.transform.parent = genPlat.transform;

                    ////�s�{�b�g�Y���ɂ��ŏI�ʒu����
                    Vector3 adjustPos = obj.GetComponent<Renderer>().bounds.center - genPlat.GetComponent<Collider>().bounds.center;
                    obj.transform.position -= adjustPos;
                }
            }

            void DeleteChildObject()
            {
                Debug.Log(genPlat.transform.childCount);

                for (int i = genPlat.transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(genPlat.transform.GetChild(i).gameObject);    
                }
            }
        }
    }
#endif

}
