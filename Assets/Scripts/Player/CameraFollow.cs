using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject playerTarget;
    GameObject temporaryTarget;
    GameObject targetThatIsFollowed;

    [SerializeField] float startZoom = 10;

    Camera cam;

    private void Start()
    {
        targetThatIsFollowed = playerTarget;

        cam = Camera.main; // Det här objectet

        cam.orthographicSize = startZoom;
    }

    // Update is called once per frame
    void Update()
    {

        if (playerTarget != null)
        {
            transform.position = targetThatIsFollowed.transform.position;
        }
    }

    #region New Target

    public void ChangeTargetCam(GameObject newTarget)
    {

        temporaryTarget = newTarget;
        targetThatIsFollowed = temporaryTarget;

    }

    public void TargetPlayerAgainCam()
    {

        targetThatIsFollowed = playerTarget;

    }

    #endregion
}
