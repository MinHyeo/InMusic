using UnityEngine;
using FMODUnity;

public class SoundManager : MonoBehaviour
{
    [Header("Instance")]
    public static SoundManager instance;
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Scene�� �������� SoundManager ����");
        }
        instance = this;
    }

    [Header("SFX")]
    FMOD.ChannelGroup sfxChannelGroup;
    FMOD.Sound[] sfsx;
    FMOD.Channel[] sfxChannels;

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
}
