using UnityEngine;
using UnityEngine.UI;

public class ButtonTexture : MonoBehaviour
{
    // �{�^���\������Image
    [SerializeField] private Sprite OnButtonImage;
    [SerializeField] private Sprite OffButtonImage;

    private Image MyImage;


    private void Awake()
    {
        MyImage = GetComponent<Image>();
        ChangeOffImage();
    }

    public void ChangeOnImage()
    {
        MyImage.sprite = OnButtonImage;
    }

    public void ChangeOffImage()
    {
        MyImage.sprite = OffButtonImage;
    }
}
