using UnityEngine;
using UnityEngine.UI;

public class ButtonTexture : MonoBehaviour
{
    // ƒ{ƒ^ƒ“•\Ž¦Žž‚ÌImage
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
