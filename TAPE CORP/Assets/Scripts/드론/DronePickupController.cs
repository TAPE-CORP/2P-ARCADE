using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class DronePickupController : MonoBehaviour
{
    [Header("Curve 이동 설정")]
    public Vector3 controlOffset = new Vector3(2f, 4f, 0f);
    public float curveSmooth = 2f;
    public float pickupDelay = 0.5f;

    [Header("이동 속도 설정")]
    public float horizontalMoveSpeed = 2f;
    public float fastLiftSpeed = 3f;
    public float slowLiftSpeed = 1f;

    [Header("지면 레이어")]
    public LayerMask groundLayer;

    [Header("Player 부모로 사용할 Transform")]
    public Transform Parent;  // Player 태그 오브젝트를 이 아래로 옮깁니다.

    private Camera mainCam;
    private LineRenderer line;
    private Transform player;
    private Rigidbody2D prb;
    private Collider2D pcollider;
    private DronePickupManager manager;
    private bool forceReleaseRequested = false;

    private float t = 0f;
    private float currentLiftSpeed;

    /// <summary>
    /// 외부에서 호출하여 드론을 초기화하고 출발시킵니다.
    /// </summary>
    public void Initialize(Transform targetPlayer, DronePickupManager droneManager)
    {
        StopAllCoroutines();
        gameObject.SetActive(true);

        t = 0f;
        forceReleaseRequested = false;
        line = GetComponent<LineRenderer>();
        mainCam = Camera.main;

        player = targetPlayer;
        manager = droneManager;
        prb = player.GetComponent<Rigidbody2D>();
        pcollider = player.GetComponent<Collider2D>();

        transform.position = GetStandbyPosition();

        line.positionCount = 2;
        line.enabled = false;

        StartCoroutine(LaunchWithCurve());
    }

    /// <summary>
    /// 카메라 밖 대기 위치 중 랜덤 좌/우에서 시작
    /// </summary>
    private Vector3 GetStandbyPosition()
    {
        float camHeight = 2f * mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;
        Vector3 camCenter = mainCam.transform.position;

        float x = camCenter.x + (Random.value < 0.5f ? -camWidth : camWidth);
        float y = camCenter.y;

        return new Vector3(x, y, 0f);
    }

    /// <summary>
    /// 곡선으로 플레이어에게 접근
    /// </summary>
    private IEnumerator LaunchWithCurve()
    {
        Vector3 start = transform.position;
        Vector3 control = (start + player.position) / 2f + controlOffset;

        while (t < 1f)
        {
            Vector3 a = Vector3.Lerp(start, control, t);
            Vector3 b = Vector3.Lerp(control, player.position, t);
            transform.position = Vector3.Lerp(a, b, t);

            t += Time.deltaTime / curveSmooth;
            yield return null;
        }

        transform.position = player.position + Vector3.up * 1.5f;
        StartCoroutine(PickupPlayer());
    }

    /// <summary>
    /// 플레이어 픽업, 이동, 해제 로직
    /// </summary>
    private IEnumerator PickupPlayer()
    {
        Debug.Log("픽업 시작");

        // 물리 비활성화
        if (prb)
        {
            prb.velocity = Vector2.zero;
            prb.gravityScale = 0f;
            prb.simulated = false;
        }

        line.enabled = true;
        Vector3 pickupPoint = transform.position + Vector3.down * 0.5f;
        Vector3 originalPlayerPos = player.position;
        float timer = 0f;

        // 픽업 애니메이션
        while (timer < pickupDelay)
        {
            player.position = Vector3.Lerp(originalPlayerPos, pickupPoint, timer / pickupDelay);
            UpdateLine(player.position);
            timer += Time.deltaTime;
            yield return null;
        }

        player.SetParent(transform);

        // 초기 상승 속도 결정
        bool hasEnteredCamera = IsInsideCameraView();
        currentLiftSpeed = hasEnteredCamera ? slowLiftSpeed : fastLiftSpeed;

        // 상승 및 조작 반복
        while (true)
        {
            bool isInsideNow = IsInsideCameraView();
            if (!hasEnteredCamera && isInsideNow)
            {
                hasEnteredCamera = true;
                currentLiftSpeed = slowLiftSpeed;
            }

            float moveInput = GetHorizontalInputForPlayer(player.gameObject.layer);
            Vector3 move = new Vector3(moveInput * horizontalMoveSpeed * Time.deltaTime,
                                       currentLiftSpeed * Time.deltaTime, 0f);

            transform.position += move;
            UpdateLine(player.position);

            // 지정 y 좌표 넘어가면 종료
            if (transform.position.y > mainCam.transform.position.y + 10f)
            {
                TryScoreAndDestroy(player.gameObject);
                break;
            }

            // 강제 해제 요청 또는 키 입력 시
            if (forceReleaseRequested ||
                (Input.GetKeyDown(KeyCode.LeftShift) && player.gameObject.layer == LayerMask.NameToLayer("Grab2P")) ||
                (Input.GetKeyDown(KeyCode.RightShift) && player.gameObject.layer == LayerMask.NameToLayer("Grab1P")))
            {
                if (!IsTouchingGround(pcollider))
                    break;
            }

            yield return null;
        }

        // 해제 처리
        player.SetParent(null);
        if (prb)
        {
            prb.gravityScale = 1f;
            prb.simulated = true;
        }

        line.enabled = false;
        manager.UnmarkPickup(player);

        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 외부에서 강제 해제 호출용
    /// </summary>
    public void ForceRelease()
    {
        forceReleaseRequested = true;
    }

    /// <summary>
    /// 카메라 뷰포트 안에 있는지 확인
    /// </summary>
    private bool IsInsideCameraView()
    {
        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);
        return viewportPos.x >= 0f && viewportPos.x <= 1f &&
               viewportPos.y >= 0f && viewportPos.y <= 1f;
    }

    /// <summary>
    /// 지면에 닿았는지 확인
    /// </summary>
    private bool IsTouchingGround(Collider2D col)
    {
        return col != null && col.IsTouchingLayers(groundLayer);
    }

    /// <summary>
    /// 플레이어별 좌/우 입력 반환
    /// </summary>
    private float GetHorizontalInputForPlayer(int layer)
    {
        if (layer == LayerMask.NameToLayer("Grab2P"))
            return Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.A) ? -1f : 0f;
        else if (layer == LayerMask.NameToLayer("Grab1P"))
            return Input.GetKey(KeyCode.RightArrow) ? 1f : Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f;
        return 0f;
    }

    /// <summary>
    /// 드론과 플레이어를 연결하는 라인 업데이트
    /// </summary>
    private void UpdateLine(Vector3 playerPos)
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, playerPos);
    }

    /// <summary>
    /// y 좌표 한계 초과 시 점수 처리 또는 부모 변경 후 파괴/해제
    /// </summary>
    private void TryScoreAndDestroy(GameObject obj)
    {
        // Player 태그면 파괴 대신 부모 변경
        if (obj.CompareTag("Player"))
        {
            obj.transform.SetParent(Parent);
            return;
        }

        // Packed 태그면 점수 처리
        if (obj.CompareTag("Packed"))
        {
            BoxObject box = obj.GetComponent<BoxObject>();
            if (box != null)
                ScoreSystem.Instance?.AddScore(Mathf.RoundToInt(box.areaSize));
        }

        // 그 외는 파괴
        Destroy(obj);
    }
}
