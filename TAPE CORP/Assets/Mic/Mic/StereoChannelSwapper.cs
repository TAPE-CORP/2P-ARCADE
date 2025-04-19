using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class StereoChannelSwapper : MonoBehaviour
{
    // �� �޼���� AudioSource�� ����� ������ ȣ��˴ϴ�.
    // data �迭�� [L,R,L,R,��] ������ ������ ��� �ְ�,
    // channels �� ä�� ��(���׷����� 2)�Դϴ�.
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (channels < 2) return;

        // ���� ���۸� ���鼭 L<->R�� ����
        for (int i = 0; i < data.Length; i += channels)
        {
            // data[i]   = ���� ä��
            // data[i+1] = ������ ä��
            float tmp = data[i];
            data[i] = data[i + 1];
            data[i + 1] = tmp;
        }
    }
}
