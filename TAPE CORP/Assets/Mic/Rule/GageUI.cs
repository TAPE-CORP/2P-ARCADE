using UnityEngine;
using UnityEngine.UI;

public class GageUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;  // ������ ���� �������� ä�� Image

    void Update()
    {
        if (ResourceCon.instance == null) return;

        // Gage�� 0~100 �����̹Ƿ� 0~1�� ����ȭ
        float t = Mathf.Clamp01(ResourceCon.instance.Gage / 100f);
        fillImage.fillAmount = t;
    }
}
