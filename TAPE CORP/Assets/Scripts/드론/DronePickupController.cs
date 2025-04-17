using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class DronePickupController : MonoBehaviour
{
    [Header("Curve �̵� ����")]
    public Vector3 controlOffset = new Vector3(2f, 4f, 0f);
    public float curveSmooth = 2f;
    public float pickupDelay = 0.5f;

    [Header("�̵� �ӵ� ����")]
    public float horizontalMoveSpeed = 2f;
    public float fastLiftSpeed = 3f;
    public float slowLiftSpeed = 1f;

    [Header("���� ���̾�")]
    public LayerMask groundLayer;

    [Header("Player �θ�� ����� Transform")]
    public Transform Parent;  // Player �±� ������Ʈ�� �� �Ʒ��� �ű�ϴ�.

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
    /// �ܺο��� ȣ���Ͽ� ����� �ʱ�ȭ�ϰ� ��߽�ŵ�ϴ�.
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
    /// ī�޶� �� ��� ��ġ �� ���� ��/�쿡�� ����
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
    /// ����� �÷��̾�� ����
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
    /// �÷��̾� �Ⱦ�, �̵�, ���� ����
    /// </summary>
    private IEnumerator PickupPlayer()
    {
        Debug.Log("�Ⱦ� ����");

        // ���� ��Ȱ��ȭ
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

        // �Ⱦ� �ִϸ��̼�
        while (timer < pickupDelay)
        {
            player.position = Vector3.Lerp(originalPlayerPos, pickupPoint, timer / pickupDelay);
            UpdateLine(player.position);
            timer += Time.deltaTime;
            yield return null;
        }

        player.SetParent(transform);

        // �ʱ� ��� �ӵ� ����
        bool hasEnteredCamera = IsInsideCameraView();
        currentLiftSpeed = hasEnteredCamera ? slowLiftSpeed : fastLiftSpeed;

        // ��� �� ���� �ݺ�
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

            // ���� y ��ǥ �Ѿ�� ����
            if (transform.position.y > mainCam.transform.position.y + 10f)
            {
                TryScoreAndDestroy(player.gameObject);
                break;
            }

            // ���� ���� ��û �Ǵ� Ű �Է� ��
            if (forceReleaseRequested ||
                (Input.GetKeyDown(KeyCode.LeftShift) && player.gameObject.layer == LayerMask.NameToLayer("Grab2P")) ||
                (Input.GetKeyDown(KeyCode.RightShift) && player.gameObject.layer == LayerMask.NameToLayer("Grab1P")))
            {
                if (!IsTouchingGround(pcollider))
                    break;
            }

            yield return null;
        }

        // ���� ó��
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
    /// �ܺο��� ���� ���� ȣ���
    /// </summary>
    public void ForceRelease()
    {
        forceReleaseRequested = true;
    }

    /// <summary>
    /// ī�޶� ����Ʈ �ȿ� �ִ��� Ȯ��
    /// </summary>
    private bool IsInsideCameraView()
    {
        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);
        return viewportPos.x >= 0f && viewportPos.x <= 1f &&
               viewportPos.y >= 0f && viewportPos.y <= 1f;
    }

    /// <summary>
    /// ���鿡 ��Ҵ��� Ȯ��
    /// </summary>
    private bool IsTouchingGround(Collider2D col)
    {
        return col != null && col.IsTouchingLayers(groundLayer);
    }

    /// <summary>
    /// �÷��̾ ��/�� �Է� ��ȯ
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
    /// ��а� �÷��̾ �����ϴ� ���� ������Ʈ
    /// </summary>
    private void UpdateLine(Vector3 playerPos)
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, playerPos);
    }

    /// <summary>
    /// y ��ǥ �Ѱ� �ʰ� �� ���� ó�� �Ǵ� �θ� ���� �� �ı�/����
    /// </summary>
    private void TryScoreAndDestroy(GameObject obj)
    {
        // Player �±׸� �ı� ��� �θ� ����
        if (obj.CompareTag("Player"))
        {
            obj.transform.SetParent(Parent);
            return;
        }

        // Packed �±׸� ���� ó��
        if (obj.CompareTag("Packed"))
        {
            BoxObject box = obj.GetComponent<BoxObject>();
            if (box != null)
                ScoreSystem.Instance?.AddScore(Mathf.RoundToInt(box.areaSize));
        }

        // �� �ܴ� �ı�
        Destroy(obj);
    }
}
