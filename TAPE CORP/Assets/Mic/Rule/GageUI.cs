using UnityEngine;
using UnityEngine.UI;

public class GageUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;  // 위에서 만든 “게이지 채움” Image

    void Update()
    {
        if (ResourceCon.instance == null) return;

        // Gage가 0~100 사이이므로 0~1로 정규화
        float t = Mathf.Clamp01(ResourceCon.instance.Gage / 100f);
        fillImage.fillAmount = t;
    }
}
