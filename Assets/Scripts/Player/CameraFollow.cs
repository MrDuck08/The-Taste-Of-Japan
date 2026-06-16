using Unity.Cinemachine;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject playerTarget;
    [SerializeField] CinemachineCamera cinemachine;
    GameObject temporaryTarget;
    GameObject targetThatIsFollowed;

    [SerializeField] float startZoomValue = 10;

    CinemachineBrain brain;

    bool nonPlayerZoom = false;

    #region Deflect Zoom Variables

    [Header("Deflect Zoom")]

    bool startDeflectZoom = false;
    bool zoomingInOnBullet = false;
    bool zoomedOnBullet = false;
    bool zoomingOutOffBullet = false;

    [SerializeField] float maxHowMuchZoom = 3;
    [SerializeField] float maxZoomInTime = 0.3f;
    float zoomInTime;
    [SerializeField] float maxTimeToBeZommedIn = 1f;
    float timeToBeZommedIn;
    [SerializeField] float maxZoomOutTime = 0.3f;
    float zoomOutTime;

    float speedToGoToBullet;
    float speedToZoom;

    #endregion

    #region Nomral Zoom Variables

    bool startZommingInB = false;
    float timeToZoomInB;
    float speedToZoomInB;
    float speedToGoToTargetB;

    bool startZoomingOutB = false;
    float timeToZoomOutB;
    float speedToZoomOutB;
    float speedToGoBackB;

    #endregion

    Camera cam;

    ScreenShake screenShake;
    InLevelSystems inLevelSystems;

    private void Start()
    {
        targetThatIsFollowed = playerTarget;

        cam = Camera.main; // Det här objectet
        brain = GetComponent<CinemachineBrain>(); 
        brain.IgnoreTimeScale = true;


        screenShake = GetComponent<ScreenShake>();

        inLevelSystems = FindAnyObjectByType<InLevelSystems>();

        cam.orthographicSize = startZoomValue;

        zoomInTime = maxZoomInTime;
        timeToBeZommedIn = maxTimeToBeZommedIn;
        zoomOutTime = maxZoomOutTime;
    }


    // Update is called once per frame
    void Update()
    {


        #region Normal Zoom In & Out

        if (startZommingInB)
        {
            timeToZoomInB -= Time.unscaledDeltaTime;

            transform.position = Vector2.Lerp(transform.position, targetThatIsFollowed.transform.position, 1337 * Time.unscaledDeltaTime);
            cam.orthographicSize -= speedToZoomInB * Time.unscaledDeltaTime;

            if (timeToZoomInB <= 0)
            {

                startZommingInB = false;

            }
        }

        if (startZoomingOutB)
        {
            timeToZoomOutB -= Time.unscaledDeltaTime;

            transform.position = Vector2.Lerp(transform.position, targetThatIsFollowed.transform.position, 1337 * Time.unscaledDeltaTime);
            cam.orthographicSize += speedToZoomOutB * Time.unscaledDeltaTime;

            if (timeToZoomOutB <= 0)
            {

                startZoomingOutB = false;

                nonPlayerZoom = false;

            }
        }

        #endregion

        #region Deflect Zoom

        if (startDeflectZoom)
        {
            if (zoomingInOnBullet)
            {
                zoomInTime -= Time.unscaledDeltaTime;

                cinemachine.Lens.OrthographicSize = Mathf.Lerp(cinemachine.Lens.OrthographicSize, 2, speedToZoom * Time.unscaledDeltaTime);

                if (zoomInTime <= 0)
                {
                    zoomInTime = maxZoomInTime;

                    zoomingInOnBullet = false;
                    zoomedOnBullet = true;

                    screenShake.TriggerShakeTime(maxTimeToBeZommedIn, 0.01f, true);

                }
            }

            if (zoomedOnBullet)
            {

                timeToBeZommedIn -= Time.unscaledDeltaTime;

                if(timeToBeZommedIn <= 0)
                {
                    timeToBeZommedIn = maxTimeToBeZommedIn;

                    zoomedOnBullet = false;
                    zoomingOutOffBullet = true;


                }

            }

            if (zoomingOutOffBullet)
            {

                zoomOutTime -= Time.unscaledDeltaTime;


                cinemachine.Lens.OrthographicSize = Mathf.Lerp(cinemachine.Lens.OrthographicSize, 10, speedToZoom * Time.unscaledDeltaTime);


                if (zoomOutTime <= 0)
                {

                    zoomOutTime = maxZoomOutTime;

                    zoomingOutOffBullet = false;
                    startDeflectZoom = true;

                    nonPlayerZoom = false;

                    targetThatIsFollowed = playerTarget;
                    cinemachine.Follow = playerTarget.transform;
                    brain.UpdateMethod = CinemachineBrain.UpdateMethods.FixedUpdate;
                    cinemachine.Lens.OrthographicSize = 10;

                    inLevelSystems.ShootBackDeflectedBullet(temporaryTarget.transform.position);
                    
                    Destroy(temporaryTarget);

                    Time.timeScale = 1;

                }

            }
        }

        #endregion

    }

    #region New Target

    public void ChangeTargetCam(GameObject newTarget, int forWhat)
    {
        // forWhat 
        // 1 = Deflect Bullet
        // 2 = R&S Charge Zoom

        nonPlayerZoom = true;

        temporaryTarget = newTarget;
        targetThatIsFollowed = temporaryTarget;
        cinemachine.Follow = targetThatIsFollowed.transform;

        switch (forWhat)
        {

            case 1:

                float distanceToBullet = Vector2.Distance(transform.position, targetThatIsFollowed.transform.position);

                speedToGoToBullet = distanceToBullet / maxZoomInTime;

                speedToZoom = (startZoomValue - maxHowMuchZoom) / maxZoomInTime;
                speedToZoom /= 3;

                startDeflectZoom = true;
                zoomingInOnBullet = true;
                brain.UpdateMethod = CinemachineBrain.UpdateMethods.LateUpdate;

                Time.timeScale = 0;
                //Time.fixedDeltaTime = 0.02F * Time.timeScale;

                screenShake.TriggerShakeTime(maxTimeToBeZommedIn + maxZoomInTime, 0.005f, false);

                break;

            case 2:

                StartZoomIn(4, 0.3f);

                break;

        }

    }

    void StartZoomIn(float howMuchZoom, float whatTimeToZoom)
    {

        timeToZoomInB = whatTimeToZoom;

        speedToZoomInB = (cam.orthographicSize - howMuchZoom) / timeToZoomInB;

        startZoomingOutB = false;
        startZommingInB = true;

    }

    public void ZoomOutAgain(float whatTimeToZoom)
    {

        targetThatIsFollowed = playerTarget;

        timeToZoomOutB = whatTimeToZoom;

        speedToZoomOutB = (startZoomValue - cam.orthographicSize) / timeToZoomOutB;

        startZommingInB = false;

        startZoomingOutB = true;

    }

    #endregion

}
