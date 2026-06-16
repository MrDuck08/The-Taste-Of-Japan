using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class AudioManager : MonoBehaviour
{

    List<AudioSource> allAudioSource = new List<AudioSource>();
    List<GameObject> objThatHaveInfSoundOnList = new List<GameObject>();
    List<GameObject> infSoundList = new List<GameObject>();
    GameObject playerObj;

    [Header("General")]

    [SerializeField] GameObject bulletHitWall;
    [SerializeField] GameObject walkingSound;
    [SerializeField] List<GameObject> doorSlamSound = new List<GameObject>();


    [Header("S & G Player")]

    [SerializeField] GameObject playerShlashSound;
    [SerializeField] GameObject playerChargeShlashSound;
    [SerializeField] GameObject playerUnsheatheSound;

    [SerializeField] GameObject harmonyWindSound;
    GameObject currentHarmonyWindSound;
    [SerializeField] GameObject harmonyChoirSound;
    GameObject currentharmonyChoirSound;

    [SerializeField] List<GameObject> ShootSoundList = new List<GameObject>();
    [SerializeField] List<GameObject> ClickSoundList = new List<GameObject>();
    [SerializeField] List<GameObject> shellSoundList = new List<GameObject>();


    [Header("Enemy")]

    [SerializeField] List<GameObject> enemyDeathSoundList = new List<GameObject>();
    [SerializeField] List<GameObject> enemyBulletDeathSoundList = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Walking

    public void playWalkingSound(Vector2 newPos, GameObject parent)
    {

        GameObject walkingSoundObj = Instantiate(walkingSound);
        // Behöver ändra namnet eftersom annars kommer den ha "clone" i sig som blir problem eftersom jag kollar på namn senare
        walkingSoundObj.name = walkingSound.name;

        // Sätter den till child av player så ljudet följer med
        walkingSoundObj.transform.parent = parent.transform;

        objThatHaveInfSoundOnList.Add(parent);
        infSoundList.Add(walkingSoundObj);


        SoundGeneral(walkingSoundObj, newPos, true);


    }

    public void StopWalkingSound(GameObject whoCalled)
    {

        StopInfiniteSounds(whoCalled, walkingSound);

    }

    public void ChangeWalkingPtch(GameObject whoCalled, float newPitch)
    {

        ChangePith(whoCalled, walkingSound, newPitch, true);

    }

    public void RevertWalkingPitch(GameObject whoCalled)
    {
        // 
        ChangePith(whoCalled, walkingSound, 1337, false);
    }

    #endregion

    public void PlayPlayerSlashSound(Vector2 newPos)
    {

        GameObject slashSound = Instantiate(playerShlashSound);

        // Sätter den till child av player så ljudet följer med
        slashSound.transform.parent = playerObj.transform;


        SoundGeneral(slashSound, newPos, false);


    }

    public void PlayDoorSlamSound(Vector2 newPos)
    {

        int whatDoorSound = Random.Range(0, doorSlamSound.Count);

        GameObject doorSound = Instantiate(doorSlamSound[whatDoorSound]);


        SoundGeneral(doorSound, newPos, false);


    }

    #region S & G Charge Slash

    public void PlayPlayerChargeSlashSound(Vector2 newPos)
    {

        GameObject chargeSlashSound = Instantiate(playerChargeShlashSound);

        // Sätter den till child av player så ljudet följer med
        chargeSlashSound.transform.parent = playerObj.transform;


        SoundGeneral(chargeSlashSound, newPos, false);


    }

    public void PlayUnsheatheSound(Vector2 newPos)
    {

        GameObject unsheatheSound = Instantiate(playerUnsheatheSound);

        // Sätter den till child av player så ljudet följer med
        unsheatheSound.transform.parent = playerObj.transform;


        SoundGeneral(unsheatheSound, newPos, false);


    }

    #endregion

    #region Harmony
    public void PlayHarmonySounds()
    {

        allAudioSource.Clear();
        allAudioSource.AddRange(FindObjectsByType<AudioSource>(FindObjectsSortMode.None));

        for(int i = 0; i < allAudioSource.Count; i++ )
        {
            allAudioSource[i].pitch = 0.3f;
        }

        currentHarmonyWindSound = Instantiate(harmonyWindSound);
        currentharmonyChoirSound = Instantiate(harmonyChoirSound);

        AudioSource windSource = currentHarmonyWindSound.GetComponent<AudioSource>();
        AudioSource choirSource = currentharmonyChoirSound.GetComponent<AudioSource>();

        windSource.GetComponent<AudioSource>().time = Random.Range(0f, windSource.GetComponent<AudioSource>().clip.length);
        choirSource.GetComponent<AudioSource>().time = Random.Range(0f, choirSource.GetComponent<AudioSource>().clip.length);

        windSource.pitch = 1f;
        choirSource.pitch = 1f;

        currentHarmonyWindSound.GetComponent<AudioFade>().StartFadeIn();
        currentharmonyChoirSound.GetComponent<AudioFade>().StartFadeIn();

    }

    public void StopHarmonySounds()
    {

        allAudioSource.Clear();
        allAudioSource.AddRange(FindObjectsByType<AudioSource>(FindObjectsSortMode.None));

        for (int i = 0; i < allAudioSource.Count; i++)
        {
            allAudioSource[i].pitch = 1f;
        }

        currentHarmonyWindSound.GetComponent<AudioFade>().StartFadeOut();
        currentharmonyChoirSound.GetComponent<AudioFade>().StartFadeOut();

    }

    #endregion

    #region Shoot Sounds

    public void PlayShellSound(Vector2 newPos)
    {

        int whatShellSound = Random.Range(0, shellSoundList.Count);

        GameObject shellSound = Instantiate(shellSoundList[whatShellSound]);


        SoundGeneral(shellSound, newPos, false);


    }

    public void PlayShootSound(Vector2 newPos)
    {

        int whatShootSound = Random.Range(0, ShootSoundList.Count);

        GameObject shootSound = Instantiate(ShootSoundList[whatShootSound]);

        // Sätter den till child av player så ljudet följer med
        shootSound.transform.parent = playerObj.transform;

        SoundGeneral(shootSound, newPos, false);


    }

    public void PlayRevolverClickSound(Vector2 newPos)
    {

        int whatClickSound = Random.Range(0, ClickSoundList.Count);

        GameObject revolverClickSound = Instantiate(ClickSoundList[whatClickSound]);

        // Sätter den till child av player så ljudet följer med
        revolverClickSound.transform.parent = playerObj.transform;


        SoundGeneral(revolverClickSound, newPos, false);


    }

    public void PlayBulletHitWall(Vector2 newPos)
    {

        GameObject wallHitSound = Instantiate(bulletHitWall);


        SoundGeneral(wallHitSound, newPos, false);


    }

    #endregion

    public void PlayEnemyDeathSound(Vector2 newPos)
    {

        int whatDeathSound = Random.Range(0, enemyDeathSoundList.Count);

        GameObject deathSound = Instantiate(enemyDeathSoundList[whatDeathSound]);


        SoundGeneral(deathSound, newPos, false);


    }

    public void PlayEnemyBulletDeathSound(Vector2 newPos)
    {

        int whatBulletDeathSound = Random.Range(0, enemyBulletDeathSoundList.Count);

        GameObject bulletDeathSound = Instantiate(enemyBulletDeathSoundList[whatBulletDeathSound]);


        SoundGeneral(bulletDeathSound, newPos, false);


    }

    void SoundGeneral(GameObject sound, Vector2 newPos, bool infinite)
    {


        sound.transform.position = newPos;
        sound.SetActive(true);
        sound.GetComponent<AudioSource>().Play();
        float soundLenght = sound.GetComponent<AudioSource>().clip.length;

        if(infinite == false)
        {
            StartCoroutine(DestroySound(soundLenght, sound));
        }

    }


    IEnumerator DestroySound(float soundLenght, GameObject soundToDestroy)
    {


        yield return new WaitForSeconds(soundLenght);

        Destroy(soundToDestroy);
    }


    void StopInfiniteSounds(GameObject whatObj, GameObject whatSound)
    {

        for (var i = 0; i < infSoundList.Count; i++)
        {

            // Kollar så att det är rätt ljud på rätt object (plats) sitter på samma ställe och sedan tar bort dem
            if (objThatHaveInfSoundOnList[i].name == whatObj.name && infSoundList[i].name == whatSound.name)
            {
                Destroy(infSoundList[i]);
                infSoundList.RemoveAt(i);
                objThatHaveInfSoundOnList.RemoveAt(i);

            }

        }

    }

    void ChangePith(GameObject whatObj, GameObject whatSound, float whatPitch, bool newPitch)
    {

        for (var i = 0; i < infSoundList.Count; i++)
        {

            // Kollar så att det är rätt ljud på rätt object (plats) sitter på samma ställe och sedan tar bort dem
            if (objThatHaveInfSoundOnList[i].name == whatObj.name && infSoundList[i].name == whatSound.name)
            {
                if (newPitch)
                {
                    infSoundList[i].GetComponent<AudioSource>().pitch = whatPitch;
                }
                else
                {
                    infSoundList[i].GetComponent<AudioSource>().pitch = 1;
                }

            }

        }

    }


}
