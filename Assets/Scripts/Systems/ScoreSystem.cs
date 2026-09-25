using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI comboText;
    [SerializeField] Image comboImage;
    [SerializeField] GameObject pointScoreEffect;

    float currentScore = 0;

    #region Values

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
    [SerializeField] float varietyBonus = 50;

    [Header("Time")]
    [SerializeField] float mission1MaxTime = 45;
    [SerializeField] float mission1MaxPoints = 2000;
    [SerializeField] float mission2MaxTime = 45;
    [SerializeField] float mission2MaxPoints = 4000;
    [SerializeField] float mission3MaxTime = 90;
    [SerializeField] float mission3MaxPoints = 20000;
    float pointsForTime;

    #endregion

    [Header("Combo")]
    [SerializeField] float comboIncrease = 0.5f;
    float timeUntilComboDrop;
    [SerializeField] float maxTimeUntilComboDrop = 3;
    float currentCombo = 0;
    float currentComboMultiplier = 1;
    float highestCombo = 0;

    #region Final Score counting

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

    #endregion

    int numberOfVariety = 0;
    int lastKill = 1337;
    // 1 = Basic
    // 2 = Stance
    // 3 = Ranged
    // 4 = Harmony
    // 5 = Door
    // 6 Explosion

    #region Ranks

    [Header("Ranks")]
    [SerializeField] TextMeshProUGUI rankText;
    [SerializeField] List<float> tutorialMissionRankList = new List<float>();
    [SerializeField] List<float> mission1RanksList = new List<float>();
    [SerializeField] List<float> mission2RanksList = new List<float>();
    [SerializeField] List<float> mission3RanksList = new List<float>();

    int whatMission = 1;
    int whatRank = 0;

    #endregion

    Animator animator;

    AudioManager audioManager;
    InLevelSystems inLevelSystems;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
        inLevelSystems = FindAnyObjectByType<InLevelSystems>();

        animator = GetComponent<Animator>();

        Scene currentScene = SceneManager.GetActiveScene();

        switch (currentScene.name)
        {

            case "TutorialScene":

                whatMission = 0;

                break;

            case "Mission 1":

                whatMission = 1;

                break;

            case "Mission 2":

                whatMission = 2;

                break;

            case "Mission 3":

                whatMission = 3;

                break;

        }
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

                #region Time Calculation

                float howMuchPointsTime = 1337;

                switch (whatMission)
                {

                    case 1:

                        howMuchPointsTime = (inLevelSystems.currentActualTime / mission1MaxTime) - 1;
                        mission1MaxPoints *= Mathf.Abs(howMuchPointsTime);
                        pointsForTime = mission1MaxPoints;

                        break;

                    case 2:

                        howMuchPointsTime = (inLevelSystems.currentActualTime / mission2MaxTime) - 1;
                        mission2MaxPoints *= Mathf.Abs(howMuchPointsTime);
                        pointsForTime = mission2MaxPoints;

                        break;

                    case 3:

                        howMuchPointsTime = (inLevelSystems.currentActualTime / mission3MaxTime) - 1;
                        mission3MaxPoints *= Mathf.Abs(howMuchPointsTime);
                        pointsForTime = mission3MaxPoints;

                        break;
                }

                currentScore += pointsForTime;

                #endregion

                // Om man slutar på en högre kombo
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
                DetermineRank();
            }
        }

        #region Additional Score

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
                rect.anchoredPosition = new Vector2(-582, 333);

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

                if (inLevelSystems.currentShownTime != 0)
                {
                    scoreObj.GetComponent<TextMeshProUGUI>().text = inLevelSystems.currentShownTime.ToString("00:00") + " (" + pointsForTime.ToString("0") + "p)";
                    inLevelSystems.currentShownTime = 0;
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

        #endregion
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
        // F1 gör så att den vissar 1 decimal
        comboText.text = currentComboMultiplier.ToString("F1") + "x";

    }

    void DetermineRank()
    {

        switch (whatMission)
        {

            case 0:

                // I listan står alla poäng, om den går över en av den får den +1, sedan kollar jag bara vad den ligger på och ger en rank
                for (int i = 0; i < tutorialMissionRankList.Count; i++)
                {

                    if (currentScore > tutorialMissionRankList[i])
                    {

                        whatRank++;

                    }
                }

                break;

            case 1:

                // I listan står alla poäng, om den går över en av den får den +1, sedan kollar jag bara vad den ligger på och ger en rank
                for (int i = 0; i < mission1RanksList.Count; i++)
                {

                    if(currentScore > mission1RanksList[i])
                    {

                        whatRank++;

                    }
                }

                break;

            case 2:

                for (int i = 0; i < mission2RanksList.Count; i++)
                {

                    if (currentScore > mission2RanksList[i])
                    {

                        whatRank++;

                    }
                }

                break;

            case 3:

                for (int i = 0; i < mission3RanksList.Count; i++)
                {

                    if (currentScore > mission3RanksList[i])
                    {

                        whatRank++;

                    }
                }

                break;

        }

        rankText.gameObject.SetActive(true);
        switch (whatRank)
        {
            // D rank
            case 1:

                rankText.text = "D";

                break;

            // C Rank
            case 2:

                rankText.text = "C";

                break;

            // B Rank
            case 3:

                rankText.text = "B";

                break;

            // A Rank
            case 4:

                rankText.text = "A";

                break;

            // S Rank
            case 5:

                rankText.text = "S";

                break;

            // SSushi Rank
            case 6:

                rankText.text = "SSushi";
                rankText.fontSize = 68;
                rankText.gameObject.transform.position -= new Vector3(0, 115, 0);

                break;
        }

    }
}
