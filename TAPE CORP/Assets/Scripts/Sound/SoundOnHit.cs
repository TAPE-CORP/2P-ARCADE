using UnityEngine;

/// <summary>
/// SoundOnHit Ŭ������ ������Ʈ�� �÷��̾�� �浹�ϰų�
/// ������ Ű �Է� �� ���带 ����ϴ� ����� �����մϴ�.
/// �ν����Ϳ��� �浹 Ʈ����, Ű Ʈ����, ��� ��ٿ� ���� ������ �� �ֽ��ϴ�.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundOnHit : MonoBehaviour
{
    [Header("���� ����")]
    [Tooltip("�浹 �Ǵ� Ű �Է� �� ����� ���� Ŭ��")]
    public AudioClip hitSound;
    [Min(0f)]
    [Tooltip("���� ��� �� �ٽ� ��� �������� ������ ����� �ð�(��)")]
    public float cooldownTime = 0.5f;

    [Space(10)]
    [Header("Ʈ���� ��� ����")]
    [Tooltip("�浹 ���� �� �ڵ����� ���带 ������� ����")]
    public bool useCollisionTrigger = true;
    [Tooltip("�浹 ���¿��� Ű �Է� �� ���带 ������� ����")]
    public bool useKeyTrigger = false;

    [Space(10)]
    [Header("Ʈ���� ����")]
    [Tooltip("���� ����� ����� �÷��̾� ���̾�")]
    public LayerMask playerLayer;
    [Tooltip("����� Ű ���: �迭 �� �ϳ��� ������ ���� ��� �õ�")]
    public KeyCode[] triggerKeys = new KeyCode[] { KeyCode.Space };

    [Space(10)]
    // ���� ���� ���� (�ν����� ��ǥ��)
    [HideInInspector] private AudioSource audioSource;
    [HideInInspector] private float lastPlayTime = -Mathf.Infinity;
    [HideInInspector] private bool isCollidingWithPlayer = false;

    /// <summary>
    /// ��ũ��Ʈ �ʱ�ȭ: AudioSource �������� �� �⺻ ����
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        Debug.Log("[SoundOnHit] �ʱ�ȭ �Ϸ�");
    }

    /// <summary>
    /// �� ������ Ű �Է��� üũ�Ͽ� ���带 ����մϴ�.
    /// useKeyTrigger�� Ȱ��ȭ�ǰ� �浹 ������ ���� ����.
    /// </summary>
    void Update()
    {
        if (useKeyTrigger && isCollidingWithPlayer)
        {
            foreach (KeyCode key in triggerKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    Debug.Log($"[SoundOnHit] Ű �Է� ����: {key}");
                    TryPlaySound();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// �浹�� ���۵� �� ȣ��˴ϴ�.
    /// �÷��̾� ���̾����� Ȯ�� ��, useCollisionTrigger�� true�� ��� ���,
    /// useKeyTrigger�� true�� Ű �Է� ��� ���·� ��ȯ.
    /// </summary>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsInLayerMask(collision.collider.gameObject.layer, playerLayer))
            return;

        if (useCollisionTrigger)
        {
            Debug.Log("[SoundOnHit] �浹 ���� �� ���� ���");
            TryPlaySound();
        }
        if (useKeyTrigger)
        {
            isCollidingWithPlayer = true;
            Debug.Log("[SoundOnHit] �浹 ���� ����");
        }
    }

    /// <summary>
    /// �浹�� ���� �� ȣ��˴ϴ�.
    /// useKeyTrigger�� Ȱ��ȭ�� ��� �浹 ���� ����.
    /// </summary>
    void OnCollisionExit2D(Collision2D collision)
    {
        if (!IsInLayerMask(collision.collider.gameObject.layer, playerLayer))
            return;

        if (useKeyTrigger)
        {
            isCollidingWithPlayer = false;
            Debug.Log("[SoundOnHit] �浹 ���� ����");
        }
    }

    /// <summary>
    /// ���� ��� �õ�: Ŭ�� ����, ��ٿ� üũ �� PlayOneShot ȣ��
    /// </summary>
    void TryPlaySound()
    {
        if (hitSound == null)
        {
            Debug.Log("[SoundOnHit] ����� ���� Ŭ���� �����ϴ�");
            return;
        }
        if (Time.time - lastPlayTime < cooldownTime)
        {
            Debug.Log("[SoundOnHit] ��ٿ� �� - ��� ���");
            return;
        }
        audioSource.PlayOneShot(hitSound);
        lastPlayTime = Time.time;
        Debug.Log("[SoundOnHit] ���� ��� �Ϸ�");
    }

    /// <summary>
    /// ���̾� ����ũ�� Ư�� ���̾ ���ԵǾ� �ִ��� Ȯ���մϴ�.
    /// </summary>
    bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}
