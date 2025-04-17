using UnityEngine;

/// <summary>
/// SoundOnHit 클래스는 오브젝트가 플레이어와 충돌하거나
/// 지정된 키 입력 시 사운드를 재생하는 기능을 제공합니다.
/// 인스펙터에서 충돌 트리거, 키 트리거, 재생 쿨다운 등을 설정할 수 있습니다.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundOnHit : MonoBehaviour
{
    [Header("사운드 설정")]
    [Tooltip("충돌 또는 키 입력 시 재생할 사운드 클립")]
    public AudioClip hitSound;
    [Min(0f)]
    [Tooltip("사운드 재생 후 다시 재생 가능해질 때까지 대기할 시간(초)")]
    public float cooldownTime = 0.5f;

    [Space(10)]
    [Header("트리거 사용 여부")]
    [Tooltip("충돌 감지 시 자동으로 사운드를 재생할지 여부")]
    public bool useCollisionTrigger = true;
    [Tooltip("충돌 상태에서 키 입력 시 사운드를 재생할지 여부")]
    public bool useKeyTrigger = false;

    [Space(10)]
    [Header("트리거 설정")]
    [Tooltip("사운드 재생을 허용할 플레이어 레이어")]
    public LayerMask playerLayer;
    [Tooltip("사용할 키 목록: 배열 중 하나라도 눌리면 사운드 재생 시도")]
    public KeyCode[] triggerKeys = new KeyCode[] { KeyCode.Space };

    [Space(10)]
    // 내부 사용용 변수 (인스펙터 비표시)
    [HideInInspector] private AudioSource audioSource;
    [HideInInspector] private float lastPlayTime = -Mathf.Infinity;
    [HideInInspector] private bool isCollidingWithPlayer = false;

    /// <summary>
    /// 스크립트 초기화: AudioSource 가져오기 및 기본 설정
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        Debug.Log("[SoundOnHit] 초기화 완료");
    }

    /// <summary>
    /// 매 프레임 키 입력을 체크하여 사운드를 재생합니다.
    /// useKeyTrigger가 활성화되고 충돌 상태일 때만 동작.
    /// </summary>
    void Update()
    {
        if (useKeyTrigger && isCollidingWithPlayer)
        {
            foreach (KeyCode key in triggerKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    Debug.Log($"[SoundOnHit] 키 입력 감지: {key}");
                    TryPlaySound();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 충돌이 시작될 때 호출됩니다.
    /// 플레이어 레이어인지 확인 후, useCollisionTrigger가 true면 즉시 재생,
    /// useKeyTrigger가 true면 키 입력 대기 상태로 전환.
    /// </summary>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsInLayerMask(collision.collider.gameObject.layer, playerLayer))
            return;

        if (useCollisionTrigger)
        {
            Debug.Log("[SoundOnHit] 충돌 감지 시 사운드 재생");
            TryPlaySound();
        }
        if (useKeyTrigger)
        {
            isCollidingWithPlayer = true;
            Debug.Log("[SoundOnHit] 충돌 상태 진입");
        }
    }

    /// <summary>
    /// 충돌이 끝날 때 호출됩니다.
    /// useKeyTrigger가 활성화된 경우 충돌 상태 해제.
    /// </summary>
    void OnCollisionExit2D(Collision2D collision)
    {
        if (!IsInLayerMask(collision.collider.gameObject.layer, playerLayer))
            return;

        if (useKeyTrigger)
        {
            isCollidingWithPlayer = false;
            Debug.Log("[SoundOnHit] 충돌 상태 해제");
        }
    }

    /// <summary>
    /// 사운드 재생 시도: 클립 유무, 쿨다운 체크 후 PlayOneShot 호출
    /// </summary>
    void TryPlaySound()
    {
        if (hitSound == null)
        {
            Debug.Log("[SoundOnHit] 재생할 사운드 클립이 없습니다");
            return;
        }
        if (Time.time - lastPlayTime < cooldownTime)
        {
            Debug.Log("[SoundOnHit] 쿨다운 중 - 재생 대기");
            return;
        }
        audioSource.PlayOneShot(hitSound);
        lastPlayTime = Time.time;
        Debug.Log("[SoundOnHit] 사운드 재생 완료");
    }

    /// <summary>
    /// 레이어 마스크에 특정 레이어가 포함되어 있는지 확인합니다.
    /// </summary>
    bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}
