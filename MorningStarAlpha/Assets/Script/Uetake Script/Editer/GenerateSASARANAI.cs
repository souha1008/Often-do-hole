using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// �S�Ă̎q�I�u�W�F�N�g�̃R���C�_�[��\��
/// </summary>
public class GenerateSASARANAI : MonoBehaviour
{
    const float GridSize = 3.2f;

    public enum Direction
    {
        UP,
        DOWN,
    }

    [Tooltip("�����������I�u�W�F�N�g")] public GameObject ground;
    [Tooltip("���H�̊p�I�u�W�F�N�g")] public GameObject corner;
    [Tooltip("���H�̊p�I�u�W�F�N�g")] public GameObject flat;

    public bool Road_Top = true;
    public bool Road_Bottom = true;
    public bool Road_Left = true;
    public bool Road_Light = true;

    [Tooltip("�^�񒆂̔��������̌���")] public Direction direction;

    [Range(-0.1f, 0.1f), Tooltip("�T�C�Y�����p�F ���̐��i�^�񒆂̔����������傫�߂ɕ\�������j���̐��i�^�񒆂̔��������������߂ɕ\�������j")] public float AdjustSizeX = -0.5f;
    [Range(-0.1f, 0.1f), Tooltip("�T�C�Y�����p�F ���̐��i�^�񒆂̔����������傫�߂ɕ\�������j���̐��i�^�񒆂̔��������������߂ɕ\�������j")] public float AdjustSizeY = -0.5f;
    [Range(0.5f, 1.0f), Tooltip("�T�C�Y�����p�F ���̐��i�^�񒆂̔����������傫�߂ɕ\�������j���̐��i�^�񒆂̔��������������߂ɕ\�������j")] public float AdjustSizeZ = 0.0f;

#if UNITY_EDITOR
    [CustomEditor(typeof(GenerateSASARANAI))]
    public class GeneratePlatformEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GenerateSASARANAI genPlat = target as GenerateSASARANAI;

            
            GUILayout.Space(20);
            if (GUILayout.Button("�n�`�̐���", GUILayout.Height(72)))
            {
                //��������͍̂폜
                DeleteChildObject();
                GenerateRoad();
                GenerateGround();

                genPlat.GetComponent<Renderer>().enabled = false;
            }
            if (GUILayout.Button("�폜", GUILayout.Height(72)))
            {
                DeleteChildObject();
                genPlat.GetComponent<Renderer>().enabled = true;
            }



            void GenerateGround()
            {
                //�^�񒆂̔�������
                {
                    GameObject obj = Instantiate(genPlat.ground);
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

                    if(genPlat.direction == Direction.DOWN)
                    {
                        obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                    }

                    obj.transform.parent = genPlat.transform;

                    ////�s�{�b�g�Y���ɂ��ŏI�ʒu����
                    Vector3 adjustPos = obj.GetComponent<Renderer>().bounds.center - genPlat.GetComponent<Collider>().bounds.center;
                    obj.transform.position -= adjustPos;
                }
            }

            void GenerateRoad()
            {
                Bounds bounds = genPlat.GetComponent<Renderer>().bounds;

                //�}�X���c�����ꂼ�ꂢ���A�����Ă��邩�𒲂ׂ�
                int x_lne = Mathf.RoundToInt(Mathf.Abs(genPlat.transform.lossyScale.x) / GridSize);
                int y_lne = Mathf.RoundToInt(Mathf.Abs(genPlat.transform.lossyScale.y) / GridSize);

                Debug.Log("Xlne = " + x_lne + "  : Ylen = " + y_lne);

                //�ő�l�ƍŏ��l�����߂�
                Vector3 maxPos = bounds.max;
                maxPos.z = bounds.center.z;

                Vector3 minPos = bounds.min;
                minPos.z = bounds.center.z;

                //�p�̐���
                {
                    //����
                    if (genPlat.Road_Top || genPlat.Road_Left)
                    {
                        GameObject obj = InstantiateCorner();
                        obj.transform.position = new Vector3(minPos.x - 0.2f, maxPos.y, minPos.z);
                        obj.transform.rotation = Quaternion.Euler(0, 0, 90);
                        obj.transform.parent = genPlat.transform;
                    }

                    //�E��
                    if (genPlat.Road_Top || genPlat.Road_Light)
                    {
                        GameObject obj = InstantiateCorner();
                        obj.transform.position = new Vector3(maxPos.x, maxPos.y + 0.2f, minPos.z); ;
                        obj.transform.parent = genPlat.transform;
                    }

                    //����
                    if (genPlat.Road_Bottom || genPlat.Road_Left)
                    {
                        GameObject obj = InstantiateCorner();
                        obj.transform.position = new Vector3(minPos.x, minPos.y - 0.2f, minPos.z);
                        obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                        obj.transform.parent = genPlat.transform;
                    }

                    //�E��
                    if (genPlat.Road_Bottom || genPlat.Road_Light)
                    {
                        GameObject obj = InstantiateCorner();
                        obj.transform.position = new Vector3(maxPos.x + 0.2f, minPos.y, minPos.z);
                        obj.transform.rotation = Quaternion.Euler(0, 0, 270);
                        obj.transform.parent = genPlat.transform;
                    }
                }

                //�p�ł͂Ȃ������쐬
                {  
                    if (genPlat.Road_Top)
                    {
                        for (int i = 1; i < x_lne - 1; i++)
                        {
                            //�㕔��
                            GameObject obj = InstantiateFlat();
                            float xPos = minPos.x + GridSize * i + GridSize;
                            obj.transform.position = new Vector3(xPos, maxPos.y, minPos.z);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 90);
                            obj.transform.parent = genPlat.transform;

                        }
                    }


                    if (genPlat.Road_Bottom)
                    {
                        for (int i = 1; i < x_lne - 1; i++)
                        {

                            //������
                            GameObject obj = InstantiateFlat();
                            float xPos = minPos.x + GridSize * i;
                            obj.transform.position = new Vector3(xPos, minPos.y, minPos.z);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 270);
                            obj.transform.parent = genPlat.transform;
                        }
                    }

                    if (genPlat.Road_Left)
                    {
                        for (int i = 1; i < y_lne - 1; i++)
                        {
                            //������
                            GameObject obj = InstantiateFlat();
                            float YPos = minPos.y + GridSize * i + GridSize;
                            obj.transform.position = new Vector3(minPos.x, YPos, minPos.z);
                            obj.transform.rotation = Quaternion.Euler(0, 0, 180);
                            obj.transform.parent = genPlat.transform;
                        }
                    }

                    if (genPlat.Road_Light)
                    {
                        for (int i = 1; i < y_lne - 1; i++)
                        {
                            //�E����
                            GameObject obj = InstantiateFlat();
                            float YPos = minPos.y + GridSize * i;
                            obj.transform.position = new Vector3(maxPos.x, YPos, minPos.z);
                            obj.transform.parent = genPlat.transform;
                        }
                    }
                }
            }

            GameObject InstantiateCorner()
            {
                GameObject obj = Instantiate(genPlat.corner);
                AdjustGridSize(obj);

                //�T�C�Y����
                float cornerAdjust = 1.27f;
                obj.transform.localScale = new Vector3(obj.transform.localScale.x * cornerAdjust, obj.transform.localScale.y * cornerAdjust, obj.transform.localScale.z);

                return obj;
            }

            GameObject InstantiateFlat()
            {
                GameObject obj = Instantiate(genPlat.flat);
                AdjustGridSize(obj);

                //�T�C�Y����
                float cornerAdjust = 1.27f;
                obj.transform.localScale = new Vector3(obj.transform.localScale.x * cornerAdjust, obj.transform.localScale.y, obj.transform.localScale.z);

                return obj;
            }

            void AdjustGridSize(GameObject obj)
            {
                //�T�C�Y���킹
                {
                    Vector3 newScale = Vector3.one;

                    //���Ƃ̃��f���f�[�^�Ɉˑ������ɃT�C�Y��1:1:1�ɂ���
                    newScale.x = 1.0f / obj.GetComponent<Renderer>().bounds.size.x;
                    newScale.y = 1.0f / obj.GetComponent<Renderer>().bounds.size.y;

                    //�e�X�g�p����̃T�C�Y�ɍ��킹��
                    newScale.x *= GridSize;
                    newScale.y *= GridSize;

                    obj.transform.localScale = new Vector3(newScale.x, newScale.y, obj.transform.localScale.z);
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
