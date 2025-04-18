using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player1Con : MonoBehaviour
{
    public Camera P1Cam;
    [Header("손전등 Transform (Player 자식으로 두세요)")]
    public Transform flashlight;

    [Header("Light2D 할당 & 전기 플래시 컬러")]
    public Light2D flashlightLight;
    public Color electricColor = Color.cyan;

    [Header("Electric 태그 검사 반경")]
    [Tooltip("Light2D의 Outer Radius를 사용할 경우 0으로 설정")]
    public float detectRadius = 0f;

    [Header("전기 활성화 기간 (초)")]
    public float electricActiveTime = 3f;

    private Vector3 _originalScale;
    private Color _originalColor;

    void Start()
    {
        _originalScale = transform.localScale;

        if (flashlight == null)
            Debug.LogError("Flashlight Transform을 할당하세요.");
        if (flashlightLight == null)
            Debug.LogError("Light2D 컴포넌트를 할당하세요.");
        else
            _originalColor = flashlightLight.color;
    }

    void Update()
    {
        if (flashlight == null) return;

        // 1) 마우스 월드 좌표
        Vector3 mouseWorld = P1Cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = flashlight.position.z;

        // 2) 방향 벡터 계산
        Vector3 dir = (mouseWorld - flashlight.position).normalized;

        // 3) 손전등 회전 (기본 위쪽 보정)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        flashlight.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        // 4) 캐릭터 좌우 반전
        bool faceLeft = dir.x < 0f;
        transform.localScale = new Vector3(
            faceLeft ? -Mathf.Abs(_originalScale.x) : Mathf.Abs(_originalScale.x),
            _originalScale.y,
            _originalScale.z
        );

        // 5) 마우스 클릭 시 전기 플래시 효과
        if (Input.GetMouseButtonDown(0))
            StartCoroutine(ElectricFlash());
    }

    private IEnumerator ElectricFlash()
    {
        // 1) 라이트 컬러 변경
        flashlightLight.color = electricColor;

        // 2) 반경 내 Electric 태그 오브젝트의 Conveyor만 한 번씩 반전
        float radius = detectRadius > 0f
            ? detectRadius
            : flashlightLight.pointLightOuterRadius;

        Collider2D[] hits = Physics2D.OverlapCircleAll(flashlight.position, radius);
        var toggledConveyors = new HashSet<Conveyor>();

        foreach (var col in hits)
        {
            if (!col.CompareTag("Electric"))
                continue;

            Conveyor conveyor = col.GetComponent<Conveyor>();
            if (conveyor != null && toggledConveyors.Add(conveyor))
            {
                conveyor.SetDirection();
            }
        }

        // 3) 잠깐 대기 후 색상 복원
        yield return new WaitForSeconds(1f);
        flashlightLight.color = _originalColor;
    }

    private IEnumerator ActivateUserComponents(GameObject obj, float duration)
    {
        // UnityEngine 네임스페이스가 아닌, 사용자 정의 MonoBehaviour만 선택
        var components = obj
            .GetComponents<MonoBehaviour>()
            .Where(c =>
            {
                if (c == null) return false;
                var ns = c.GetType().Namespace;
                // 네임스페이스가 없거나 UnityEngine으로 시작하지 않으면 사용자 스크립트
                return string.IsNullOrEmpty(ns) || !ns.StartsWith("UnityEngine");
            })
            .ToArray();

        // 활성화
        foreach (var comp in components)
        {
            // Conveyor 스크립트라면 isRight 반전
            if (comp.GetType().Name == nameof(Conveyor))
            {
                var conveyor = comp as Conveyor;
                if (conveyor != null)
                    conveyor.SetDirection();  // 내부에서 ApplyFlip까지 처리됩니다.
            }

            comp.enabled = true;
        }

        yield return new WaitForSeconds(duration);

        // 비활성화
        foreach (var comp in components)
        {
            comp.enabled = false;
        }
    }
}
