using UnityEngine;
using System.Collections;

public class AudioFade : MonoBehaviour
{
    [SerializeField] float fadeInTargetVolume = 0.7f;
    [SerializeField] float fadeOutTargetVolume;
    [SerializeField] float fadeInSpeed = 0.3f;
    [SerializeField] float fadeOutSpeed = 0.3f;
    [SerializeField] bool fadeInStart = false;

    AudioSource audioSource = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (fadeInStart)
        {
            StartFadeIn();
        }
    }


    public void StartFadeIn()
    {
        StopAllCoroutines();
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        StartCoroutine(DoFadeIn());
    }
    public void StartFadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(DoFadeOut());
    }
    IEnumerator DoFadeIn()
    {
        while (audioSource.volume != fadeInTargetVolume)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, fadeInTargetVolume,
            fadeInSpeed * Time.deltaTime);
            yield return null;
        }
        StopAllCoroutines();
    }
    IEnumerator DoFadeOut()
    {
        while (audioSource.volume != fadeOutTargetVolume)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, fadeOutTargetVolume,
            fadeOutSpeed * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
        StopAllCoroutines();
    }
}
