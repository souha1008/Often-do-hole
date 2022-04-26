using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GoalManager : MonoBehaviour
{
    [SerializeField] GameObject ClearCam;
    [SerializeField] GameObject MainCam;
    [SerializeField] Volume PostProssece;
    [SerializeField] RawImage RawImage;

    [SerializeField][Range(1.0f, 100.0f)] float AlphaSpeed; // �����x�̐ݒ�
    [SerializeField][Range(0.0f, 1.0f)] float CameraRotateSpeed;

    private int counter_1; // �����J�E���g�p
    [SerializeField][ReadOnly] private float alpha;

    private bool AngleChange;

    // Start is called before the first frame update
    void Start()
    {
        // �N���A�J������A�N�e�B�u
        ClearCam.SetActive(false);

        // �ϐ�������
        AngleChange = false;

        counter_1 = 0;
        alpha = 0;

        RawImage.color = new Color(255, 255, 255, 0);
    }

    void FixedUpdate()
    {

        if (AngleChange == true)
        {
            MainCam.transform.Rotate(new Vector3(-CameraRotateSpeed, 0, 0));
            counter_1++;
        }

        if (counter_1 >= 4)
        {
            alpha += AlphaSpeed / 1000000;
            RawImage.color += new Color(0, 0, 0, alpha);
        }

        // ���U���g��ʂւ̑J�ڔ���
        if (alpha > 0.01)
        {
            
            SceneManager.LoadScene("ResultScene");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AngleChange = true;
            if (PostProssece == null) Debug.Log("volume is not loading");

            PostProssece.profile.TryGet<MotionBlur>(out var motionBlur);
            motionBlur.active = true;
            if (!motionBlur.active) Debug.Log("motionBlur is false");

            ClearCam.SetActive(true);
            if (!ClearCam.activeSelf) Debug.Log("ClearCam is not Actived");

        }

        Debug.Log("Goal��������");
    }
}
