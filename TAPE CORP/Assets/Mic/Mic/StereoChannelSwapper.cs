using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class StereoChannelSwapper : MonoBehaviour
{
    // 이 메서드는 AudioSource가 재생할 때마다 호출됩니다.
    // data 배열은 [L,R,L,R,…] 순서로 샘플이 들어 있고,
    // channels 은 채널 수(스테레오면 2)입니다.
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (channels < 2) return;

        // 샘플 버퍼를 돌면서 L<->R을 스왑
        for (int i = 0; i < data.Length; i += channels)
        {
            // data[i]   = 왼쪽 채널
            // data[i+1] = 오른쪽 채널
            float tmp = data[i];
            data[i] = data[i + 1];
            data[i + 1] = tmp;
        }
    }
}
