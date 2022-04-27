using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title_p1_manager : MonoBehaviour
{
    [Tooltip("���C���J����")][SerializeField] Camera camera;
    [Tooltip("Player�I�u�W�F�N�g")][SerializeField] GameObject Player;

    [Tooltip("�J�����������Ă���t���[��")][SerializeField] int camera_motion = 30;

    private bool transition;    // �J�����̃��[�V�����g���K�[
    Vector3 pos;                // �v���C���[�ʒu�␳�p
    int flame_count;

    void Awake()
    {
        pos = Player.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        transition = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        // Player��Z���␳
        Player.transform.position = pos;

        if(Input.GetButton("Fire1") || Input.GetButton("Fire2")
            || Input.GetButton("Fire3") || Input.GetButton("Jump"))
        {
            transition = true;
        }

        if(transition == true)
        {
            flame_count++;
            camera.transform.Rotate(new Vector3(0, 5.4f, 0));
            if(flame_count >= camera_motion)
            {
                transition = false;
            }
        }
    }
}
