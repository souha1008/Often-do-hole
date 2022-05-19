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
    public GameObject platform;

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
                GameObject parent = new GameObject("PLATFORM_MODELS");
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
                    Vector3 newScale = Vector3.zero;
                    newScale.x = 1.0f / obj.GetComponent<Renderer>().bounds.size.x;
                    newScale.y = 1.0f / obj.GetComponent<Renderer>().bounds.size.y;
                    newScale.z = 1.0f / obj.GetComponent<Renderer>().bounds.size.z;

                    newScale.x *= TestPlat.transform.localScale.x;
                    newScale.y *= TestPlat.transform.localScale.y;
                    newScale.z *= TestPlat.transform.localScale.z;

                    obj.transform.localScale = newScale;

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
