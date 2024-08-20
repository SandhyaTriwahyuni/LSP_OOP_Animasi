using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private SoundLibrary sfxLibrary;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlaySound3D(string soundName, Vector3 pos)
    {
        AudioClip clip = sfxLibrary.GetClipFromName(soundName);
        if (clip != null)
        {
            AudioSource source = AudioSourcePool.Instance.GetAudioSource();
            source.transform.position = pos;
            source.clip = clip;
            source.Play();
            StartCoroutine(ReturnSourceToPool(source, clip.length));
        }
    }

    public void PlaySound2D(string soundName)
    {
        AudioClip clip = sfxLibrary.GetClipFromName(soundName);
        if (clip != null)
        {
            AudioSource source = AudioSourcePool.Instance.GetAudioSource();
            source.transform.position = Camera.main.transform.position;
            source.clip = clip;
            source.Play();
            StartCoroutine(ReturnSourceToPool(source, clip.length));
        }
    }

    private IEnumerator ReturnSourceToPool(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        AudioSourcePool.Instance.ReturnAudioSource(source);
    }
}