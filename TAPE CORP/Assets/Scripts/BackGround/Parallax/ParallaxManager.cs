using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    public Camera mainCamera;
    public ParallaxLayer backgroundLayer;
    public ParallaxLayer wallLayer;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (backgroundLayer != null)
        {
            backgroundLayer.mainCamera = mainCamera;
        }

        if (wallLayer != null)
        {
            wallLayer.mainCamera = mainCamera;
        }
    }
}
