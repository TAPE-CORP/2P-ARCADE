using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [Header("공통")]
    public Camera mainCamera;
    public float parallaxFactor = 0.5f;

    [Header("반복 배경")]
    public GameObject backgroundPrefab;
    private List<GameObject> backgrounds = new List<GameObject>();
    private float backgroundWidth;
    private int backgroundCount;

    [Header("오브젝트 배치")]
    public GameObject objectPrefab;
    public int objectCount = 10;
    public float spacing = 5f;
    private List<GameObject> objects = new List<GameObject>();

    private float lastCameraWidth = -1f;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        InitBackgrounds();
        SpawnObjects();
    }

    void Update()
    {
        UpdateParallax();
        RecycleBackgrounds();

        // 카메라 사이즈가 바뀌면 배경 재설정
        float currentWidth = GetCameraWidth();
        if (!Mathf.Approximately(currentWidth, lastCameraWidth))
        {
            ResetBackgrounds();
        }
    }

    float GetCameraWidth()
    {
        return mainCamera.orthographicSize * 2f * mainCamera.aspect;
    }

    void InitBackgrounds()
    {
        SpriteRenderer sr = backgroundPrefab.GetComponent<SpriteRenderer>();
        backgroundWidth = sr.bounds.size.x;

        float viewWidth = GetCameraWidth();
        backgroundCount = Mathf.CeilToInt(viewWidth / backgroundWidth) + 2; // 여유분 +2

        for (int i = 0; i < backgroundCount; i++)
        {
            GameObject bg = Instantiate(backgroundPrefab, new Vector3(i * backgroundWidth, 0, 10), Quaternion.identity, transform);
            backgrounds.Add(bg);
        }

        lastCameraWidth = viewWidth;
    }

    void ResetBackgrounds()
    {
        // 기존 배경 삭제
        foreach (GameObject bg in backgrounds)
        {
            Destroy(bg);
        }
        backgrounds.Clear();

        InitBackgrounds();
    }

    void UpdateParallax()
    {
        foreach (GameObject bg in backgrounds)
        {
            float camPosX = mainCamera.transform.position.x;
            float offsetX = camPosX * parallaxFactor;
            Vector3 targetPos = new Vector3(offsetX, bg.transform.position.y, bg.transform.position.z);
            bg.transform.position = targetPos;
        }
    }

    void RecycleBackgrounds()
    {
        float camX = mainCamera.transform.position.x;
        float halfViewWidth = GetCameraWidth() * 0.5f;

        foreach (GameObject bg in backgrounds)
        {
            float bgX = bg.transform.position.x;

            if (camX - halfViewWidth > bgX + backgroundWidth)
            {
                bg.transform.position += Vector3.right * backgroundWidth * backgroundCount;
            }
            else if (camX + halfViewWidth < bgX - backgroundWidth)
            {
                bg.transform.position -= Vector3.right * backgroundWidth * backgroundCount;
            }
        }
    }

    void SpawnObjects()
    {
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 pos = new Vector3(i * spacing, 0, 0);
            GameObject obj = Instantiate(objectPrefab, pos, Quaternion.identity, transform);
            objects.Add(obj);
        }
    }
}
