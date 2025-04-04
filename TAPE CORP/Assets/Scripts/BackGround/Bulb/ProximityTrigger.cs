using UnityEngine;
using UnityEngine.Rendering.Universal;
public class ProximityTrigger2D : MonoBehaviour
{
    public float maxDistance; // ī�޶�� �� �Ÿ����� �ָ� ����

    private Light2D light2D;
    private Camera mainCam;

    void Start()
    {
        light2D = GetComponent<Light2D>();
        mainCam = Camera.main;
        if (mainCam == null) return;

        float distance = Vector2.Distance(transform.position, mainCam.transform.position);
        light2D.enabled = distance <= maxDistance;
    }

    void FixedUpdate()
    {
        if (mainCam == null) return;

        float distance = Vector2.Distance(transform.position, mainCam.transform.position);
        light2D.enabled = distance <= maxDistance;
    }
}
