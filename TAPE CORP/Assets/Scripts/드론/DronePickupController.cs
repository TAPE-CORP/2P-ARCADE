using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class DronePickupController : MonoBehaviour
{
    public Vector3 controlOffset = new Vector3(2f, 4f, 0f);
    public float curveSmooth = 2f;

    public float standbyMargin = 1f;
    public float standbyOffsetRangeY = 2f;

    public float pickupDelay = 0.5f;

    public LayerMask groundLayer;
    public string releaseKey = "Jump";

    [SerializeField] private float liftSpeed = 1f;
    [SerializeField] private float fastLiftSpeed = 3f;
    [SerializeField] private float slowLiftSpeed = 1f;

    private float t = 0f;
    private Camera mainCam;
    private LineRenderer line;
    private Transform player;
    private DronePickupManager manager;
    private float currentLiftSpeed;

    public void Initialize(Transform targetPlayer, DronePickupManager pickupManager)
    {
        mainCam = Camera.main;
        line = GetComponent<LineRenderer>();
        player = targetPlayer;
        manager = pickupManager;

        t = 0f;
        transform.position = GetDynamicStandbyPosition();

        line.positionCount = 2;
        line.enabled = false;

        StartCoroutine(LaunchWithCurve());
    }

    bool IsInsideCameraView()
    {
        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);
        return viewportPos.x >= 0f && viewportPos.x <= 1f && viewportPos.y >= 0f && viewportPos.y <= 1f;
    }

    Vector3 GetDynamicStandbyPosition()
    {
        float camHeight = 2f * mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;
        Vector3 camCenter = mainCam.transform.position;

        bool spawnLeft = Random.value < 0.5f;
        float x = camCenter.x + (spawnLeft ? -camWidth / 2f - standbyMargin : camWidth / 2f + standbyMargin);
        float y = camCenter.y + Random.Range(-standbyOffsetRangeY, standbyOffsetRangeY);

        return new Vector3(x, y, 0f);
    }

    IEnumerator LaunchWithCurve()
    {
        Vector3 start = transform.position;
        Vector3 control = (start + player.position) / 2f + controlOffset;

        while (t < 1f)
        {
            Vector3 a = Vector3.Lerp(start, control, t);
            Vector3 b = Vector3.Lerp(control, player.position, t);
            transform.position = Vector3.Lerp(a, b, t);

            Vector3 dir = b - a;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 10f);

            t += Time.deltaTime / curveSmooth;
            yield return null;
        }

        transform.position = player.position + Vector3.up * 1.5f;
        transform.rotation = Quaternion.identity;

        StartCoroutine(PickupPlayer());
    }

    IEnumerator PickupPlayer()
    {
        Debug.Log("픽업 시작");

        Rigidbody2D prb = player.GetComponent<Rigidbody2D>();
        Collider2D pcollider = player.GetComponent<Collider2D>();

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

        while (timer < pickupDelay)
        {
            player.position = Vector3.Lerp(originalPlayerPos, pickupPoint, timer / pickupDelay);
            UpdateLine(player.position);
            timer += Time.deltaTime;
            yield return null;
        }

        player.SetParent(transform);

        while (true)
        {
            currentLiftSpeed = IsInsideCameraView() ? slowLiftSpeed : fastLiftSpeed;

            transform.position += Vector3.up * currentLiftSpeed * Time.deltaTime;
            UpdateLine(player.position);

            if (Input.GetButtonDown(releaseKey) && !IsTouchingGround(pcollider))
            {
                Debug.Log("해제됨");
                break;
            }

            yield return null;
        }

        player.SetParent(null);

        if (prb)
        {
            prb.gravityScale = 1f;
            prb.simulated = true;
        }

        line.enabled = false;

        yield return new WaitForSeconds(0.5f);

        // 구조 끝났으니 매니저에 알림
        if (manager != null)
        {
            manager.UnmarkRescuedPlayer(player);
        }

        gameObject.SetActive(false);
    }

    bool IsTouchingGround(Collider2D col)
    {
        return col != null && col.IsTouchingLayers(groundLayer);
    }

    void UpdateLine(Vector3 playerPos)
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, playerPos);
    }
}
