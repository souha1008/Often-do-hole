using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// �S�Ă̎q�I�u�W�F�N�g�̃R���C�_�[��\��
/// </summary>
public class GeneratePlatform : MonoBehaviour
{
    [Tooltip("�����������I�u�W�F�N�g")] public GameObject platform;
    [Range(-0.1f, 0.1f), Tooltip("�T�C�Y�����p�F ���̐��i�����ڗp�I�u�W�F�N�g���傫�߂ɕ\�������j���̐��i�����ڗp�I�u�W�F�N�g�������߂ɕ\�������j")] public float AdjustSizeX = 0.0f;
    [Range(-0.1f, 0.1f), Tooltip("�T�C�Y�����p�F ���̐��i�����ڗp�I�u�W�F�N�g���傫�߂ɕ\�������j���̐��i�����ڗp�I�u�W�F�N�g�������߂ɕ\�������j")] public float AdjustSizeY = 0.0f;
    [Range(-0.1f, 0.1f), Tooltip("�T�C�Y�����p�F ���̐��i�����ڗp�I�u�W�F�N�g���傫�߂ɕ\�������j���̐��i�����ڗp�I�u�W�F�N�g�������߂ɕ\�������j")] public float AdjustSizeZ = 0.0f;

#if UNITY_EDITOR
    [CustomEditor(typeof(GeneratePlatform))]
    public class GeneratePlatformEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GeneratePlatform genPlat = target as GeneratePlatform;

            GUILayout.Space(20);
            if (GUILayout.Button("�n�`�̐���", GUILayout.Height(72)))
            {
                GameObject parent = new GameObject(genPlat.name + "_MODELS");
                Collider[] cols = genPlat.GetComponentsInChildren<Collider>();

                foreach (var col in cols)
                {
                    if(col.gameObject == genPlat.gameObject)
                    {
                        continue;
                    }
                    GameObject TestPlat = col.gameObject;

                    //�I�u�W�F�N�g�����Ɩ��O�ύX
                    GameObject obj = Instantiate(genPlat.platform);
                    obj.name = "Model_" + TestPlat.name;
                    //�ʒu���킹
                    obj.transform.position = TestPlat.transform.position;

                    //�T�C�Y���킹
                    {
                        Vector3 newScale = Vector3.zero;

                        //���Ƃ̃��f���f�[�^�Ɉˑ������ɃT�C�Y��1:1:1�ɂ���
                        newScale.x = 1.0f / obj.GetComponent<Renderer>().bounds.size.x;
                        newScale.y = 1.0f / obj.GetComponent<Renderer>().bounds.size.y;
                        newScale.z = 1.0f / obj.GetComponent<Renderer>().bounds.size.z;

                        //�e�X�g�p����̃T�C�Y�ɍ��킹��
                        newScale.x *= TestPlat.transform.localScale.x;
                        newScale.y *= TestPlat.transform.localScale.y;
                        newScale.z *= TestPlat.transform.localScale.z;

                        newScale.x += genPlat.AdjustSizeX;
                        newScale.y += genPlat.AdjustSizeY;
                        newScale.z += genPlat.AdjustSizeZ;

                        obj.transform.localScale = newScale;
                    }

                    ////�s�{�b�g�Y���ɂ��ŏI�ʒu����
                    Vector3 adjustPos = obj.GetComponent<Renderer>().bounds.center - col.bounds.center;
                    obj.transform.position -= adjustPos;

                    obj.transform.parent = parent.gameObject.transform;
                }
            }
        }
    }
#endif

}
