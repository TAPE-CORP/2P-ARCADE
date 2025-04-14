using UnityEngine;
using UnityEngine.EventSystems;

public class SmoothHoverScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Ŀ�� ���")]
    public RectTransform targetToScale; // Ŀ�� ��� (���� �θ� ������Ʈ)

    [Header("ũ�� ����")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1.1f);
    public float smoothing = 10f;

    private Vector3 targetScale;

    private void Awake()
    {
        if (targetToScale == null)
            targetToScale = GetComponent<RectTransform>(); // fallback

        targetToScale.localScale = normalScale;
        targetScale = normalScale;
    }

    private void Update()
    {
        if (targetToScale != null)
        {
            targetToScale.localScale = Vector3.Lerp(targetToScale.localScale, targetScale, Time.unscaledDeltaTime * smoothing);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = normalScale;
    }
}
