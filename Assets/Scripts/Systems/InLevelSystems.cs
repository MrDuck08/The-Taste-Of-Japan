using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InLevelSystems : MonoBehaviour
{

    List<GameObject> enemiesList = new List<GameObject>();
    GameObject tempObj;
    [SerializeField] GameObject tempCameraFollowObj;
    float enemyCount = 0;
    float timeForSlowDown = 1.5f;
    public bool levelDone = false;
    bool finalEnemyKill = false;

    [SerializeField] LayerMask whatLayerToIgnore;
    [SerializeField] TrailRenderer bulletTrail;

    [SerializeField] TextMeshProUGUI timerText;
    [HideInInspector] public float currentShownTime = 0;
    float minuteCounter = 0;
    [HideInInspector] public float currentActualTime = 0;

    CameraFollow cam;
    AudioManager audioManager;
    ScoreSystem scoreSystem;

    private void Start()
    {
        cam = FindAnyObjectByType<CameraFollow>();
        audioManager = FindAnyObjectByType<AudioManager>();
        scoreSystem = FindAnyObjectByType<ScoreSystem>();

        enemiesList.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        for (int i = 0; i < enemiesList.Count; i++)
        {
            enemyCount++;
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            FindAnyObjectByType<SceneLoader>().ReloadScene();
        }

        if (finalEnemyKill)
        {

            timeForSlowDown -= Time.unscaledDeltaTime;

            if(timeForSlowDown <= 0)
            {
                finalEnemyKill = false;

                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                cam.GoBackToPlayer();
                cam.ZoomOutAgain(0.3f);
                audioManager.RevertPitch();
            }

        }

        if (!levelDone)
        {
            currentActualTime += Time.deltaTime;

            currentShownTime += Time.deltaTime;
            timerText.text = currentShownTime.ToString("00:00.00");

            // Man måste "manuelt ändra så att det ser ut som att minuter går
            minuteCounter += Time.deltaTime;
            if (minuteCounter > 60)
            {
                currentShownTime += 40;
                minuteCounter = 0f;
            }
        }
    }

    #region Bullet Deflect

    public void ShootBackDeflectedBullet(Vector2 fromWhere)
    {

        enemiesList.Clear();

        enemiesList.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        float closestDistanceToEnemy = 1337;
        int whatI = 1337;

        for (int i = 0; i < enemiesList.Count; i++)
        {

            Vector2 direction = new Vector2(enemiesList[i].transform.position.x, enemiesList[i].transform.position.y) - fromWhere;

            RaycastHit2D hit = Physics2D.Raycast(fromWhere, direction.normalized, 1337, ~whatLayerToIgnore);

            if (hit.collider.CompareTag("Enemy"))
            {

                if (Vector2.Distance(fromWhere, enemiesList[i].transform.position) < closestDistanceToEnemy) // Om en annan fiende är närmare
                {

                    whatI = i;
                    closestDistanceToEnemy = Vector2.Distance(fromWhere, enemiesList[i].transform.position);

                }

            }

        }


        if (whatI != 1337)
        {

            Vector2 direction = new Vector2(enemiesList[whatI].transform.position.x, enemiesList[whatI].transform.position.y) - fromWhere;

            RaycastHit2D hit = Physics2D.Raycast(fromWhere, direction.normalized, 1337, ~whatLayerToIgnore);


            TrailRenderer trail = Instantiate(bulletTrail);
            trail.transform.position = fromWhere;

            trail.GetComponent<BulletTrailScript>().MoveAndFadeTrail(fromWhere, hit.point);

            audioManager.PlayDeflectSound();

            enemiesList[whatI].GetComponent<EnemyHealth>().TakeDamage(1, 3, fromWhere);

        }

    }

    #endregion

    public void EnemyKilled(Transform pos)
    {

        enemyCount--;

        if (enemyCount <= 0)
        {
            levelDone = true;
            finalEnemyKill = true;
            timerText.gameObject.SetActive(false);

            Time.timeScale = 0.1f;
            Time.fixedDeltaTime = 0.016F * Time.timeScale;

            tempObj = Instantiate(tempCameraFollowObj);

            tempObj.transform.position = pos.position;

            audioManager.ChangePitchAll(0.3f);
            cam.ChangeTargetCam(tempObj, 1337);
            cam.StartZoomIn(4, 0.5f);
        }

    }
}
