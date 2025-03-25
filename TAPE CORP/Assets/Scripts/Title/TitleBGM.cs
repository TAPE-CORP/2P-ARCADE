using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TitleBGMPlayer : MonoBehaviour
{
    public AudioClip bgmClip;
    [Range(0f, 1f)] public float volume = 0.5f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = bgmClip;
        audioSource.volume = volume;
        audioSource.loop = true;

        // AudioSource Ȱ��ȭ �� ���
        audioSource.enabled = true;
        audioSource.Play();
    }
}
