using UnityEngine;

// �C���X�^���X���P�������Ȃ��N���X�쐬�p(�����}�l�[�W���[�쐬�p)
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    Debug.LogError(typeof(T) + "is nothing");
                }
            }

            return instance;
        }
    }

}
