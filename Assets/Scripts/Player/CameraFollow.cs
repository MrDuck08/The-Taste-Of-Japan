using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject playerTarget;
    GameObject temporaryTarget;
    GameObject targetThatIsFollowed;

    [SerializeField] float startZoom = 10;

    bool nonPlayerZoom = false;

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
    float speedToZoomIn;



    Camera cam;

    ScreenShake screenShake;
    InLevelSystems inLevelSystems;

    private void Start()
    {
        targetThatIsFollowed = playerTarget;

        cam = Camera.main; // Det här objectet

        screenShake = GetComponent<ScreenShake>();
        inLevelSystems = FindObjectOfType<InLevelSystems>();

        cam.orthographicSize = startZoom;

        zoomInTime = maxZoomInTime;
        timeToBeZommedIn = maxTimeToBeZommedIn;
        zoomOutTime = maxZoomOutTime;
    }

    // Update is called once per frame
    void Update()
    {

        if (targetThatIsFollowed != null && !nonPlayerZoom)
        {
            transform.position = targetThatIsFollowed.transform.position;
        }

        #region Deflect Zoom

        if (startDeflectZoom)
        {
            if (zoomingInOnBullet)
            {
                zoomInTime -= Time.unscaledDeltaTime;

                transform.position = Vector2.Lerp(transform.position, targetThatIsFollowed.transform.position, speedToGoToBullet * Time.unscaledDeltaTime);
                cam.orthographicSize -= speedToZoomIn * Time.unscaledDeltaTime;

                if (zoomInTime <= 0)
                {
                    zoomInTime = maxZoomInTime;

                    zoomingInOnBullet = false;
                    zoomedOnBullet = true;

                    screenShake.TriggerShake(maxTimeToBeZommedIn, 0.01f);

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

                transform.position = Vector2.Lerp(targetThatIsFollowed.transform.position, transform.position, speedToGoToBullet * Time.unscaledDeltaTime);
                cam.orthographicSize += speedToZoomIn * Time.unscaledDeltaTime;


                if (zoomOutTime <= 0)
                {

                    zoomOutTime = maxZoomOutTime;

                    zoomingOutOffBullet = false;
                    startDeflectZoom = true;

                    nonPlayerZoom = false;

                    targetThatIsFollowed = playerTarget;

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

        nonPlayerZoom = true;

        temporaryTarget = newTarget;
        targetThatIsFollowed = temporaryTarget;

        switch(forWhat)
        {

            case 1:

                float distanceToBullet = Vector2.Distance(transform.position, targetThatIsFollowed.transform.position);

                speedToGoToBullet = distanceToBullet / maxZoomInTime;

                speedToZoomIn = (cam.orthographicSize - maxHowMuchZoom) / maxZoomInTime;

                startDeflectZoom = true;
                zoomingInOnBullet = true;

                Time.timeScale = 0;

                break;

        }

    }

    public void TargetPlayerAgainCam()
    {

        targetThatIsFollowed = playerTarget;

    }

    #endregion

}
