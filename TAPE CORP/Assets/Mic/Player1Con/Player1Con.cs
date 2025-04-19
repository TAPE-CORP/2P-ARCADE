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

    [Header("클릭 자원 감소 설정")]
    [Tooltip("클릭 홀드 시작 시 기본 초당 감소량")]
    public float clickDrainBase = 1f;
    [Tooltip("클릭 홀드 누적시간당 추가 초당 감소량")]
    public float clickDrainAccel = 1f;

    private Vector3 _originalScale;
    private Color _originalColor;
    private float _originalOuterRadius;

    // 클릭 누적 시간
    private float clickHoldTime = 0f;

    void Start()
    {
        _originalScale = transform.localScale;

        if (flashlight == null)
            Debug.LogError("Flashlight Transform을 할당하세요.");
        if (flashlightLight == null)
            Debug.LogError("Light2D 컴포넌트를 할당하세요.");
        else
        {
            _originalColor = flashlightLight.color;
            _originalOuterRadius = flashlightLight.pointLightOuterRadius;
        }
    }

    void Update()
    {
        if (flashlight == null) return;

        // 1) 마우스 월드 좌표 & 방향
        Vector3 mouseWorld = P1Cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = flashlight.position.z;
        Vector3 dir = (mouseWorld - flashlight.position).normalized;

        // 2) 손전등 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        flashlight.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        // 3) 캐릭터 좌우 반전
        bool faceLeft = dir.x < 0f;
        transform.localScale = new Vector3(
            faceLeft ? -Mathf.Abs(_originalScale.x) : Mathf.Abs(_originalScale.x),
            _originalScale.y,
            _originalScale.z
        );

        // 4) 클릭 중 라이트 반경 확장
        if (Input.GetMouseButton(0))
        {
            flashlightLight.pointLightOuterRadius = _originalOuterRadius * 2f;
        }
        else
        {
            flashlightLight.pointLightOuterRadius = _originalOuterRadius;
        }

        // 5) 클릭 중 자원(Gage) 감소
        if (Input.GetMouseButton(0))
        {
            clickHoldTime += Time.deltaTime;
            float clickDrainRate = clickDrainBase + clickDrainAccel * clickHoldTime;
            if (ResourceCon.instance != null)
            {
                ResourceCon.instance.Gage -= clickDrainRate * Time.deltaTime;
            }
        }
        else
        {
            clickHoldTime = 0f;
        }

        // 6) 클릭 순간 전기 효과
        if (Input.GetMouseButtonDown(0))
            StartCoroutine(ElectricFlash());
    }

    private IEnumerator ElectricFlash()
    {
        // 라이트 색상 변경
        flashlightLight.color = electricColor;

        // Electric 태그 오브젝트 처리
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

        // 대기 후 색상 복원
        yield return new WaitForSeconds(1f);
        flashlightLight.color = _originalColor;
    }

    private IEnumerator ActivateUserComponents(GameObject obj, float duration)
    {
        var components = obj
            .GetComponents<MonoBehaviour>()
            .Where(c =>
            {
                if (c == null) return false;
                var ns = c.GetType().Namespace;
                return string.IsNullOrEmpty(ns) || !ns.StartsWith("UnityEngine");
            })
            .ToArray();

        foreach (var comp in components)
        {
            if (comp.GetType().Name == nameof(Conveyor))
            {
                var conveyor = comp as Conveyor;
                if (conveyor != null)
                    conveyor.SetDirection();
            }
            comp.enabled = true;
        }

        yield return new WaitForSeconds(duration);

        foreach (var comp in components)
        {
            comp.enabled = false;
        }
    }
}
