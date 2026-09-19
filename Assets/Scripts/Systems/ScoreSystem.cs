using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI comboText;
    [SerializeField] GameObject pointScoreEffect;

    float currentScore = 0;

    [Header("Values")]
    [SerializeField] float pointForNormalKill = 100;
    [SerializeField] float pointForSpecialKill = 125;
    [SerializeField] float pointForHarmonyKill = 200;
    [SerializeField] float pointForDoorKill = 150;
    [SerializeField] float pointForExplosionKill = 200;
    [SerializeField] float pointForDestroyShield = 25;
    [SerializeField] float pointsForDeflect = 200;

    [Header("Combo")]
    float currentCombo = 0;
    float currentComboMultiplier = 1;
    [SerializeField] float comboIncrease = 0.5f;
    float timeUntilComboDrop;
    [SerializeField] float maxTimeUntilComboDrop = 3;
    [SerializeField] float varietyBonus = 50;

    int lastKill = 1337; 
    // 1 = Basic
    // 2 = Stance
    // 3 = Ranged
    // 4 = Harmony
    // 5 = Door
    // 6 Explosion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeUntilComboDrop = maxTimeUntilComboDrop;
    }

    // Update is called once per frame
    void Update()
    {
        
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
            variety = true;
        }
        lastKill = 1;


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
                variety = true;
            }
            lastKill = 2;
        }
        else
        {
            if (lastKill != 3 && lastKill != 1337)
            {
                howMuchPoints += varietyBonus;
                variety = true;
            }
            lastKill = 3;
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
            variety = true;
        }
        lastKill = 4;


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
            variety = true;
        }
        lastKill = 5;


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
            variety = true;
        }
        lastKill = 6;


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

        currentCombo++;
        currentComboMultiplier += comboIncrease;
        timeUntilComboDrop = maxTimeUntilComboDrop;
        comboText.text = currentCombo.ToString();

    }
}
