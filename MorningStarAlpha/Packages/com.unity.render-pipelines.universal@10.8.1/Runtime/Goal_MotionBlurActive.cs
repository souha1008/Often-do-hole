using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Goal_MotionBlurActive : MonoBehaviour
{
    [SerializeField] Volume PostProssece;

    public static Goal_MotionBlurActive instance;

   public void MotionBlurActive()
   {
        if (PostProssece == null) Debug.Log("volume is not loading");

        PostProssece.profile.TryGet<MotionBlur>(out var motionBlur);
        //PostProssece.GetComponent<Volume>().TryGetComponent<MotionBlur>(out var motionBlur);
        //PostProssece.TryGetComponent<MotionBlur>(out var motionBlur);
        motionBlur.active = true;
        if (!motionBlur.active) Debug.Log("motionBlur is false");
    }
}
