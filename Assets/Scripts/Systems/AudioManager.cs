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
    [SerializeField] GameObject walkingSound;


    [Header("S & G Player")]

    [SerializeField] GameObject playerShlashSound;
    [SerializeField] GameObject playerChargeShlashSound;
    [SerializeField] GameObject playerUnsheatheSound;

    [SerializeField] List<GameObject> ShootSoundList = new List<GameObject>();
    [SerializeField] List<GameObject> ClickSoundList = new List<GameObject>();
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

    public void PlayPlayerChargeSlashSound(Vector2 newPos)
    {

        GameObject chargeSlashSound = Instantiate(playerChargeShlashSound);

        // Sätter den till child av player så ljudet följer med
        chargeSlashSound.transform.parent = GameObject.FindGameObjectWithTag("Player").transform;


        SoundGeneral(chargeSlashSound, newPos);


    }

    public void PlayUnsheatheSound(Vector2 newPos)
    {

        GameObject unsheatheSound = Instantiate(playerUnsheatheSound);

        // Sätter den till child av player så ljudet följer med
        unsheatheSound.transform.parent = GameObject.FindGameObjectWithTag("Player").transform;


        SoundGeneral(unsheatheSound, newPos);


    }

    public void PlayShellSound(Vector2 newPos)
    {

        int whatShellSound = Random.Range(0, shellSoundList.Count);

        GameObject shellSound = Instantiate(shellSoundList[whatShellSound]);


        SoundGeneral(shellSound, newPos);


    }

    public void PlayShootSound(Vector2 newPos)
    {

        int whatShootSound = Random.Range(0, ShootSoundList.Count);

        GameObject shootSound = Instantiate(ShootSoundList[whatShootSound]);

        // Sätter den till child av player så ljudet följer med
        shootSound.transform.parent = GameObject.FindGameObjectWithTag("Player").transform;

        SoundGeneral(shootSound, newPos);


    }

    public void PlayRevolverClickSound(Vector2 newPos)
    {

        int whatClickSound = Random.Range(0, ClickSoundList.Count);

        GameObject revolverClickSound = Instantiate(ClickSoundList[whatClickSound]);

        // Sätter den till child av player så ljudet följer med
        revolverClickSound.transform.parent = GameObject.FindGameObjectWithTag("Player").transform;


        SoundGeneral(revolverClickSound, newPos);


    }

    public void PlayBulletHitWall(Vector2 newPos)
    {

        GameObject wallHitSound = Instantiate(bulletHitWall);


        SoundGeneral(wallHitSound, newPos);


    }

    void SoundGeneral(GameObject sound, Vector2 newPos)
    {


        sound.transform.position = newPos;
        sound.SetActive(true);
        sound.GetComponent<AudioSource>().Play();
        float soundLenght = sound.GetComponent<AudioSource>().clip.length;

        StartCoroutine(DestroySound(soundLenght, sound));

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
