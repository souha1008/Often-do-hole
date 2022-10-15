using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// �S�Ă̎q�I�u�W�F�N�g�̃R���C�_�[��\��
/// </summary>
public class DebugRendererONOFF : MonoBehaviour
{
    //// Start is called before the first frame update
    //void Start()
    //{
    //    if (GameStateManager.Instance.ColliderVisible == false)
    //    {
    //        foreach (Transform child in gameObject.transform)
    //        {
    //            child.gameObject.GetComponent<Renderer>().enabled = false;
    //        }
    //    }
    //    else
    //    {
    //        foreach (Transform child in gameObject.transform)
    //        {
    //            child.gameObject.GetComponent<Renderer>().enabled = true;
    //        }
    //    }
    //}

#if UNITY_EDITOR
    [CustomEditor(typeof(DebugRendererONOFF))]
    public class ColliderOnOffEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DebugRendererONOFF colOnOff = target as DebugRendererONOFF;

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("�I�u�W�F�N�g�̃����_���[�I��", GUILayout.Height(120)))
            {
                Renderer[] rens = colOnOff.GetComponentsInChildren<Renderer>();

                foreach (var ren in rens)
                {
                    ren.enabled = true;
                }
            }

            if (GUILayout.Button("�I�u�W�F�N�g�̃����_���[�I�t", GUILayout.Height(120)))
            {
                Renderer[] rens = colOnOff.GetComponentsInChildren<Renderer>();

                foreach (var ren in rens)
                {
                    ren.enabled = false;
                }
            }
            GUILayout.EndHorizontal();
        }
    }
#endif

}
