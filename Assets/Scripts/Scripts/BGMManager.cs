using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public AudioClip GameBGM;
    public AudioClip FinishSE;
    private AudioSource audioSource;

    public void Play(GameManager.GameState state)
    {
        switch(state)
        {
            case GameManager.GameState.Play:
                Stop();
                audioSource.clip = GameBGM;
                audioSource.loop = true;
                audioSource.Play();
                break;
            case GameManager.GameState.Finished:
                Stop();
                audioSource.clip = FinishSE;
                audioSource.loop = false;
                audioSource.Play();
                break;
        }
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public void ChangePitch(float pitch)
    {
        audioSource.pitch = pitch;
    }

    void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }
}