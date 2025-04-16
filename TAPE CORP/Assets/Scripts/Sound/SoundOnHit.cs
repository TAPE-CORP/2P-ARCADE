using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundOnHit : MonoBehaviour
{
    public AudioClip hitSound;
    public float cooldownTime = 0.5f;
    public LayerMask playerLayer; // 플레이어 레이어만 설정

    private AudioSource audioSource;
    private float lastPlayTime = -Mathf.Infinity;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어 레이어만 통과
        if (IsInLayerMask(collision.collider.gameObject.layer, playerLayer))
        {
            TryPlaySound();
        }
    }

    // 트리거를 쓰는 경우
    /*
    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsInLayerMask(other.gameObject.layer, playerLayer))
        {
            TryPlaySound();
        }
    }
    */

    void TryPlaySound()
    {
        Debug.Log("Hit sound played: " + this.gameObject.name);
        if (Time.time - lastPlayTime >= cooldownTime && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
            lastPlayTime = Time.time;
        }
    }

    bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}
