using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SimpleAudioPlayer : MonoBehaviour
{
    public AudioClip audioClip;   // 再生したいMP3（WAVでもOK）
    [Range(0f, 1f)]
    public float volume = 1.0f;   // 音量
    public bool loop = false;     // ループ再生するか

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioClip != null)
        {
            audioSource.clip = audioClip;
        }

        audioSource.playOnAwake = false;
        audioSource.loop = loop;
        audioSource.volume = volume;
    }

    public void Play()
    {
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }

    public void SetLoop(bool shouldLoop)
    {
        loop = shouldLoop;
        audioSource.loop = loop;
    }
}
