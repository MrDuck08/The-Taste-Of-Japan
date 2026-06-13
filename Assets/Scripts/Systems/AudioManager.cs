using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class AudioManager : MonoBehaviour
{

    public float fadeInTargetVolume = 0.7f;
    public float fadeOutTargetVolume;
    public float fadeSpeed = 0.3f;

    [Header("General")]

    [SerializeField] GameObject bulletHitWall;


    [Header("S & G Player")]

    [SerializeField] GameObject playerShlashSound;

    [SerializeField] List<GameObject> shellSoundList = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayPlayerSlashSound(Vector2 newPos)
    {

        GameObject slashSound = Instantiate(playerShlashSound);

        // Sätter den till child av player så ljudet följer med
        slashSound.transform.parent = GameObject.FindGameObjectWithTag("Player").transform;


        SoundGeneral(slashSound, newPos);

    }

    public void PlayShellSound(Vector2 newPos)
    {

        int whatShellSound = Random.Range(0, shellSoundList.Count);

        GameObject shellSound = Instantiate(shellSoundList[whatShellSound]);


        SoundGeneral(shellSound, newPos);

    }

    public void PlayBulletHitWall(Vector2 newPos)
    {

        GameObject wallHitSound = Instantiate(bulletHitWall);


        SoundGeneral(wallHitSound, newPos);

    }

    void SoundGeneral(GameObject sound, Vector2 newPos)
    {

        GameObject slashSound = Instantiate(sound);
        slashSound.transform.position = newPos;
        slashSound.SetActive(true);
        slashSound.GetComponent<AudioSource>().Play();
        float soundLenght = slashSound.GetComponent<AudioSource>().clip.length;

        StartCoroutine(DestroySound(soundLenght, slashSound));

    }


    IEnumerator DestroySound(float soundLenght, GameObject soundToDestroy)
    {


        yield return new WaitForSeconds(soundLenght);

        Destroy(soundToDestroy);
    }

    #region Fade in & Out

    public void StartFadeIn()
    {
        StopAllCoroutines();
        StartCoroutine("DoFadeIn");
    }
    public void StartFadeOut()
    {
        StopAllCoroutines();
        StartCoroutine("DoFadeOut");
    }
    private IEnumerator DoFadeIn(AudioSource source)
    {
        while (source.volume != fadeInTargetVolume)
        {
            source.volume = Mathf.MoveTowards(source.volume, fadeInTargetVolume,
            fadeSpeed * Time.deltaTime);
            yield return null;
        }
        StopAllCoroutines();
    }
    private IEnumerator DoFadeOut(AudioSource source)
    {
        while (source.volume != fadeOutTargetVolume)
        {
            source.volume = Mathf.MoveTowards(source.volume, fadeOutTargetVolume,
            fadeSpeed * Time.deltaTime);
            yield return null;
        }
        StopAllCoroutines();
    }

    #endregion
}
