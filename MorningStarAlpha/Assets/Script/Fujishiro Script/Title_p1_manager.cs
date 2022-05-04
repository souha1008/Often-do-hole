using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Title_p1_manager : MonoBehaviour
{
    [Tooltip("���C���J����")][SerializeField] Camera camera;
    [Tooltip("Player�I�u�W�F�N�g")][SerializeField] GameObject Player;

    [Tooltip("�J�����������Ă���t���[��")][SerializeField] int camera_motion = 30;
    [Tooltip("�J�������������x")][Range(0.0f, 10.0f)] [SerializeField] float camera_speed = 5.4f;
    [Tooltip("�J�������Z�b�g�p�x")][SerializeField] float camera_resetRot = 281.0f;
    [Tooltip("�J�������Z�b�g�ʒu")][SerializeField] Vector3 camera_RestPos = new Vector3(3.09f, -1, -9.29f);
    [Tooltip("���񎞊�")][SerializeField] float duration = 10.0f;
    [Tooltip("����I�����牽�t���[���o������")][SerializeField] int derayFlame = 120;

    private bool transition_1;    // ��ԖڃJ�����̃��[�V�����g���K�[
    private bool transition_2;    // ��ԖڃJ�������[�V�����g���K�[
    private Vector3 pos;                // �v���C���[�ʒu�␳�p
    private int flame_count;            // �t���[���J�E���g�p

    private bool once_press;
    private bool sceneCange;    //�V�[���`�F���W�ϐ����V���O���g�������p



    void Awake()
    {
        // DOTween�L���p�V�e�B�x�����p
        DOTween.SetTweensCapacity(tweenersCapacity: 800, sequencesCapacity: 200);

        // Player�|�W�V�����Œ�p
        pos = Player.transform.position;

        // �ϐ�������
        flame_count = 0;
        transition_1 = false;
        transition_2 = false;
        once_press = false;
        sceneCange = false;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Player��Z���␳
        if (transition_1 == false && transition_2 == false)
        {
            Player.transform.position = pos;
        }

        // PressAnyButton
        if(Input.GetButton("Fire1") || Input.GetButton("Fire2")
            || Input.GetButton("Fire3") || Input.GetButton("Jump") && once_press == false)
        {
            transition_1 = true;
            once_press = true;
        }

        // �J������]����
        if(transition_1 == true)
        {
            flame_count++;
            Debug.Log(flame_count);

            camera.transform.Rotate(new Vector3(0, camera_speed, 0));

            // ���t���[�����B��J�����A�j���[�V�����I��
            if(flame_count >= camera_motion)
            {
                CameraReset();

                Player.SetActive(false);

                transition_1 = false;
                transition_2 = true;
                flame_count = 0;
            }
        }

        if(transition_2 == true)
        {
            camera.transform.DORotate(new Vector3(0, -1, 0), duration, RotateMode.Fast)
                .SetLink(camera.gameObject)
                .SetEase(Ease.OutCirc);

            flame_count++;
            if(flame_count >= derayFlame)
            {
                sceneCange = true;
                DOTween.Kill(camera);
            }

            if(sceneCange == true)
            {
                Debug.Log("�V�[���؂�ւ�");
                SceneManager.LoadScene("Title_part2");
            }
        }
    }

    void CameraReset()
    {
        camera.transform.rotation = new Quaternion(0, camera_resetRot, 0, 0);
        camera.transform.position = camera_RestPos;
    }
}
