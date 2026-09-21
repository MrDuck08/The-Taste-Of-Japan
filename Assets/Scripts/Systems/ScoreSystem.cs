using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI comboText;
    [SerializeField] Image comboImage;
    [SerializeField] GameObject pointScoreEffect;

    float currentScore = 0;

    [Header("Values")]
    [SerializeField] float pointForNormalKill = 100;
    int numberOfNormalKills = 0;
    [SerializeField] float pointForSpecialKill = 125;
    int numberOfStancedKills = 0;
    int numberOfRangedKills = 0;
    [SerializeField] float pointForHarmonyKill = 200;
    int numberOfHarmonyKills = 0;
    [SerializeField] float pointForDoorKill = 150;
    int numberOfDoorKills = 0;
    [SerializeField] float pointForExplosionKill = 200;
    int numberOfExplosionKills = 0;
    [SerializeField] float pointForDestroyShield = 25;
    int numberOfShieldsDestroyed = 0;
    [SerializeField] float pointsForDeflect = 200;
    int numberOfDeflects = 0;

    [Header("Combo")]
    float currentCombo = 0;
    float currentComboMultiplier = 1;
    [SerializeField] float comboIncrease = 0.5f;
    float timeUntilComboDrop;
    [SerializeField] float maxTimeUntilComboDrop = 3;
    [SerializeField] float varietyBonus = 50;
    float highestCombo = 0;

    [Header("Final Score")]
    [SerializeField] GameObject finalScoreFolder;
    [SerializeField] TextMeshProUGUI finalScoreText;

    [SerializeField] GameObject additionalScoreObj;
    [SerializeField] float additionalScoreMoveSpeed = 2;
    List<GameObject> allCurrentAdditionalScoreObjList = new List<GameObject>();
    List<Vector3> additionalScoreObjTargetPosList = new List<Vector3>();
    List<bool> whatAdditionalScoreIsDoneList = new List<bool>();
    int howManyAdditionalScoreDone = 0;
    bool additionalScoresAdded = false;

    [SerializeField] GameObject nextLevelButtons;
    float finalScoreSoundPitch = 1;

    float finalScoreCounter = 0;
    [SerializeField] float countSpeed = 5;
    float timeUntilScoreSound = 0.3f;
    float maxTimeUntilScoreSound = 0.15f;
    float fasterSound = 2;
    bool startFinalCountdown = false;

    int numberOfVariety = 0;
    int lastKill = 1337;
    // 1 = Basic
    // 2 = Stance
    // 3 = Ranged
    // 4 = Harmony
    // 5 = Door
    // 6 Explosion

    Animator animator;

    AudioManager audioManager;
    InLevelSystems inLevelSystems;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
        inLevelSystems = FindAnyObjectByType<InLevelSystems>();

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeUntilComboDrop > 0)
        {

            timeUntilComboDrop -= Time.deltaTime;

            comboImage.fillAmount = timeUntilComboDrop / maxTimeUntilComboDrop;

            if (timeUntilComboDrop <= 0)
            {
                if(currentCombo > highestCombo && !startFinalCountdown)
                {
                    highestCombo = currentCombo;
                }

                currentCombo = 0;
                currentComboMultiplier = 1;

                comboText.text = "";
            }

        }


        // Sätter up det så den inte behöver kolla i ett annat script hela tiden
        if(!startFinalCountdown)
        {
            if (inLevelSystems.levelDone)
            {
                startFinalCountdown = true;

                finalScoreFolder.SetActive(true);


                if (currentCombo > highestCombo)
                {
                    highestCombo = currentCombo;
                }
            }
        }

        if (startFinalCountdown && finalScoreCounter < currentScore)
        {

            finalScoreCounter += countSpeed * Time.deltaTime;
            countSpeed += (countSpeed/2) * Time.deltaTime;

            timeUntilScoreSound -= (fasterSound + 1) * Time.deltaTime;
            // fasterSound är samma som 2
            fasterSound = 2 * (finalScoreCounter / currentScore);

            finalScoreText.text = Mathf.Round(finalScoreCounter).ToString();


            if(timeUntilScoreSound < 0)
            {
                timeUntilScoreSound = maxTimeUntilScoreSound;

                // Tillslut så blir pitchen 2
                finalScoreSoundPitch = 1 + (finalScoreCounter / currentScore);
                audioManager.playFinalScoreSound(finalScoreSoundPitch);

            }


            if (finalScoreCounter >= currentScore)
            {
                audioManager.PlayLastPointSound();
                nextLevelButtons.SetActive(true);
            }
        }

        if (startFinalCountdown && !additionalScoresAdded)
        {
            // Använder return så måste vara under

            for (int i = 0; i < allCurrentAdditionalScoreObjList.Count; i++)
            {

                allCurrentAdditionalScoreObjList[i].transform.position = Vector3.MoveTowards(allCurrentAdditionalScoreObjList[i].transform.position, additionalScoreObjTargetPosList[i], additionalScoreMoveSpeed * Time.deltaTime);

                if (Vector3.Distance(allCurrentAdditionalScoreObjList[i].transform.position, additionalScoreObjTargetPosList[i]) < 0.5f && !whatAdditionalScoreIsDoneList[i])
                {
                    whatAdditionalScoreIsDoneList[i] = true;
                    howManyAdditionalScoreDone++;
                }
            }

            if (howManyAdditionalScoreDone == allCurrentAdditionalScoreObjList.Count)
            {
                howManyAdditionalScoreDone = 0;

                // highestCombo är den sista som läggs till
                if (highestCombo == 0)
                {
                    additionalScoresAdded = true;
                    return;
                }


                for (int i = 0; i < additionalScoreObjTargetPosList.Count; i++)
                {
                    additionalScoreObjTargetPosList[i] -= new Vector3(0, 80, 0);
                    whatAdditionalScoreIsDoneList[i] = false;
                }

                GameObject scoreObj = Instantiate(additionalScoreObj);
                scoreObj.transform.parent = gameObject.transform;
                RectTransform rect = scoreObj.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(-582, 243);

                allCurrentAdditionalScoreObjList.Add(scoreObj);
                additionalScoreObjTargetPosList.Add(rect.position);
                whatAdditionalScoreIsDoneList.Add(false);

                audioManager.PlayAdditionalScoreSound();

                #region Check What New Text

                if (numberOfNormalKills != 0)
                {
                    scoreObj.GetComponent<TextMeshProUGUI>().text = "x" + numberOfNormalKills + " Basic Kills";
                    numberOfNormalKills = 0;
                    return;
                }

                if (numberOfStancedKills != 0)
                {
                    scoreObj.GetComponent<TextMeshProUGUI>().text = "x" + numberOfStancedKills + " Stance Kills";
                    numberOfStancedKills = 0;
                    return;
                }

                if (numberOfRangedKills != 0)
                {
                    scoreObj.GetComponent<TextMeshProUGUI>().text = "x" + numberOfRangedKills + " Range Kills";
                    numberOfRangedKills = 0;
                    return;
                }

                if (numberOfHarmonyKills != 0)
                {
                    scoreObj.GetComponent<TextMeshProUGUI>().text = "x" + numberOfHarmonyKills + " Harmony Kills";
                    numberOfHarmonyKills = 0;
                    return;
                }

                if (numberOfDoorKills != 0)
                {
                    scoreObj.GetComponent<TextMeshProUGUI>().text = "x" + numberOfDoorKills + " Door Kills";
                    numberOfDoorKills = 0;
                    return;
                }

                if (numberOfExplosionKills != 0)
                {
                    scoreObj.GetComponent<TextMeshProUGUI>().text = "x" + numberOfExplosionKills + " Explosion Kills";
                    numberOfExplosionKills = 0;
                    return;
                }

                if (numberOfShieldsDestroyed != 0)
                {
                    scoreObj.GetComponent<TextMeshProUGUI>().text = "x" + numberOfShieldsDestroyed + " Shields Destroyed";
                    numberOfShieldsDestroyed = 0;
                    return;
                }

                if (numberOfDeflects != 0)
                {
                    scoreObj.GetComponent<TextMeshProUGUI>().text = "x" + numberOfDeflects + " Parries";
                    numberOfDeflects = 0;
                    return;
                }

                if (numberOfVariety != 0)
                {
                    scoreObj.GetComponent<TextMeshProUGUI>().text = "x" + numberOfVariety + " Variety";
                    numberOfVariety = 0;
                    return;
                }

                if (highestCombo != 0)
                {
                    scoreObj.GetComponent<TextMeshProUGUI>().text = highestCombo + " Highest Kill Combo";
                    highestCombo = 0;
                }

                #endregion
            }
        }
    }

    #region Kills

    public void NomralKill(Vector3 pos)
    {
        GameObject scoreJuiceText = Instantiate(pointScoreEffect);
        scoreJuiceText.transform.position = pos;

        float howMuchPoints = pointForNormalKill;

        bool variety = false;
        // 1337 är så att man inte får en bonus på första killet
        if (lastKill != 1 && lastKill != 1337)
        {
            howMuchPoints += varietyBonus;
            numberOfVariety++;
            variety = true;
        }
        lastKill = 1;
        numberOfNormalKills++;


        howMuchPoints *= currentComboMultiplier;
        howMuchPoints = Mathf.Round(howMuchPoints);

        currentScore += howMuchPoints;

        if (!variety)
        {
            scoreJuiceText.GetComponent<TextMeshPro>().text = howMuchPoints.ToString() + "+";
        }
        else
        {
            scoreJuiceText.GetComponent<TextMeshPro>().text = howMuchPoints.ToString() + " \n+Variety";
        }

        ComboIncrease();

    }

    public void SpecialKill(Vector3 pos, bool rangedKill)
    {
        GameObject scoreJuiceText = Instantiate(pointScoreEffect);
        scoreJuiceText.transform.position = pos;

        float howMuchPoints = pointForSpecialKill;

        bool variety = false;
        if (!rangedKill)
        {
            if (lastKill != 2 && lastKill != 1337)
            {
                howMuchPoints += varietyBonus;
                numberOfVariety++;
                variety = true;
            }
            lastKill = 2;
            numberOfStancedKills++;
        }
        else
        {
            if (lastKill != 3 && lastKill != 1337)
            {
                howMuchPoints += varietyBonus;
                numberOfVariety++;
                variety = true;
            }
            lastKill = 3;
            numberOfRangedKills++;
        }


        howMuchPoints *= currentComboMultiplier;
        howMuchPoints = Mathf.Round(howMuchPoints);

        currentScore += howMuchPoints * currentComboMultiplier;

        if (!variety)
        {
            scoreJuiceText.GetComponent<TextMeshPro>().text = howMuchPoints.ToString() + "+";
        }
        else
        {
            scoreJuiceText.GetComponent<TextMeshPro>().text = howMuchPoints.ToString() + " \n+Variety";
        }

        ComboIncrease();

    }

    public void HarmonyKill(Vector3 pos)
    {
        GameObject scoreJuiceText = Instantiate(pointScoreEffect);
        scoreJuiceText.transform.position = pos;

        float howMuchPoints = pointForHarmonyKill;

        bool variety = false;
        // 1337 är så att man inte får en bonus på första killet
        if (lastKill != 4 && lastKill != 1337)
        {
            howMuchPoints += varietyBonus;
            numberOfVariety++;
            variety = true;
        }
        lastKill = 4;
        numberOfHarmonyKills++;


        howMuchPoints *= currentComboMultiplier;
        howMuchPoints = Mathf.Round(howMuchPoints);

        currentScore += howMuchPoints * currentComboMultiplier;

        if (!variety)
        {
            scoreJuiceText.GetComponent<TextMeshPro>().text = howMuchPoints.ToString() + "+";
        }
        else
        {
            scoreJuiceText.GetComponent<TextMeshPro>().text = howMuchPoints.ToString() + " \n+Variety";
        }

        ComboIncrease();

    }

    public void DoorKill(Vector3 pos)
    {
        GameObject scoreJuiceText = Instantiate(pointScoreEffect);
        scoreJuiceText.transform.position = pos;

        float howMuchPoints = pointForDoorKill;

        bool variety = false;
        // 1337 är så att man inte får en bonus på första killet
        if (lastKill != 5 && lastKill != 1337)
        {
            howMuchPoints += varietyBonus;
            numberOfVariety++;
            variety = true;
        }
        lastKill = 5;
        numberOfDoorKills++;


        howMuchPoints *= currentComboMultiplier;
        howMuchPoints = Mathf.Round(howMuchPoints);

        currentScore += howMuchPoints * currentComboMultiplier;

        if (!variety)
        {
            scoreJuiceText.GetComponent<TextMeshPro>().text = howMuchPoints.ToString() + "+";
        }
        else
        {
            scoreJuiceText.GetComponent<TextMeshPro>().text = howMuchPoints.ToString() + " \n+Variety";
        }

        ComboIncrease();

    }

    public void ExplosionKill(Vector3 pos)
    {
        GameObject scoreJuiceText = Instantiate(pointScoreEffect);
        scoreJuiceText.transform.position = pos;

        float howMuchPoints = pointForExplosionKill;

        bool variety = false;
        // 1337 är så att man inte får en bonus på första killet
        if (lastKill != 6 && lastKill != 1337)
        {
            howMuchPoints += varietyBonus;
            numberOfVariety++;
            variety = true;
        }
        lastKill = 6;
        numberOfExplosionKills++;


        howMuchPoints *= currentComboMultiplier;
        howMuchPoints = Mathf.Round(howMuchPoints);

        currentScore += howMuchPoints * currentComboMultiplier;

        if (!variety)
        {
            scoreJuiceText.GetComponent<TextMeshPro>().text = howMuchPoints.ToString() + "+";
        }
        else
        {
            scoreJuiceText.GetComponent<TextMeshPro>().text = howMuchPoints.ToString() + " \n+Variety";
        }

        ComboIncrease();

    }

    #endregion

    public void PointsForDeflect(Vector3 pos)
    {
        numberOfDeflects++;

        GameObject scoreJuiceText = Instantiate(pointScoreEffect);
        scoreJuiceText.transform.position = pos;

        float howMuchPoints = pointsForDeflect;


        howMuchPoints *= currentComboMultiplier;
        howMuchPoints = Mathf.Round(howMuchPoints);

        currentScore += howMuchPoints * currentComboMultiplier;

        scoreJuiceText.GetComponent<TextMeshPro>().text = howMuchPoints.ToString() + " \n+Deflect";

    }

    public void PointsForShieldDestroy(Vector3 pos)
    {

        numberOfShieldsDestroyed++;

        GameObject scoreJuiceText = Instantiate(pointScoreEffect);
        scoreJuiceText.transform.position = pos;

        float howMuchPoints = pointForDestroyShield;


        howMuchPoints *= currentComboMultiplier;
        howMuchPoints = Mathf.Round(howMuchPoints);

        currentScore += howMuchPoints * currentComboMultiplier;

        scoreJuiceText.GetComponent<TextMeshPro>().text = howMuchPoints.ToString() + " \n+Shield";

    }

    void ComboIncrease()
    {

        animator.SetTrigger("ComboUp");

        currentCombo++;
        currentComboMultiplier += comboIncrease;
        timeUntilComboDrop = maxTimeUntilComboDrop;
        comboText.text = currentComboMultiplier.ToString() + "x";

    }
}
