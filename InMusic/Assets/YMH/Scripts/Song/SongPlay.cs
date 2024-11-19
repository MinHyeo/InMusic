using FMODUnity;
using UnityEngine;

public class SongPlay : MonoBehaviour
{
    [SerializeField]
    private EventReference[] songs;

    public void Play()
    {
        SoundManager.instance.PlayOneShot(songs[0], transform.position);
    }
}
