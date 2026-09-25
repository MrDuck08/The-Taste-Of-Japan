using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject harmonyTutorial;

    bool harmonyTutorialDone = false;

    SwordAndGunCharacter swordAndGunCharacter;
    PauseScript pauseScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseScript = FindAnyObjectByType<PauseScript>();
        swordAndGunCharacter = FindAnyObjectByType<SwordAndGunCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (swordAndGunCharacter.harmonyAvalibleEffect == false && !harmonyTutorialDone)
        {

            harmonyTutorialDone = true;

            harmonyTutorial.SetActive(true);

            harmonyTutorial.transform.position = new Vector2(swordAndGunCharacter.transform.position.x, harmonyTutorial.transform.position.y);

            Time.timeScale = 0;
        }
    }
}
