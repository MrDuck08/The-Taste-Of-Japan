using Unity.VisualScripting;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] float shakeMagnitude = 0.7f;
    [SerializeField] float maxShakeDistance = 15f;
    float recoilMagnitude = 0f;

    float shakeDuration = 0f;
    float recoilDuration = 0f;

    bool priorityShake = false;
    bool shakeStop = true;

    Vector3 changedShakePos = Vector3.zero;

    public static Vector3 cameraOffset = Vector3.zero;
    public static Vector3 recoilOffset = Vector3.zero;

    Player1 player;

    private void Start()
    {
        player = FindAnyObjectByType<Player1>();
    }


    private void Update()
    {
        if(recoilDuration > 0f)
        {

            recoilOffset = Vector2.Lerp(recoilOffset, -player.lookDirection.normalized * recoilMagnitude, 0.1f);


            recoilDuration -= Time.deltaTime;

        }
        else if(recoilOffset.x != 0 || recoilOffset.y != 0)
        {

            recoilOffset = Vector2.Lerp(recoilOffset, Vector2.zero, 0.2f);

        }

        if (shakeDuration > 0)
        {
            float xMagnitude = Random.Range(-shakeMagnitude, shakeMagnitude);
            float yMagnitude = Random.Range(-shakeMagnitude, shakeMagnitude);

            // Kollar så att de inte går över max
            if (xMagnitude > maxShakeDistance)
            {
                xMagnitude = maxShakeDistance;
            }
            if (xMagnitude < -maxShakeDistance)
            {
                xMagnitude = -maxShakeDistance;
            }
            if (yMagnitude > maxShakeDistance)
            {
                yMagnitude = shakeMagnitude;
            }
            if (yMagnitude < -maxShakeDistance)
            {
                yMagnitude = -maxShakeDistance;
            }

            // Sparar alla ändringar på objectet, om vi vill ha flera shakes som går tillbaka till sin position
            changedShakePos += new Vector3(xMagnitude, yMagnitude, 0);

            // Ändreing på postiion körs i "CameraFollow" Script
            cameraOffset += new Vector3(xMagnitude, yMagnitude, 0);
            shakeDuration -= Time.deltaTime;
        }
        else if (!shakeStop)
        {
            // Shake Done

            StopShake();
        }
    }

    public void TriggerShake(float duration, float magnitude, bool isShakeImortant)
    {

        // En shake som har prioritet körs
        if (priorityShake)
        {
            return;
        }

        shakeDuration = -1;

        shakeStop = false;

        priorityShake = isShakeImortant;

        shakeMagnitude = magnitude;
        shakeDuration = duration;
    }

    public void StopShake()
    {
        shakeStop = true;

        // Ändrar tillbaka alla ändringar
        cameraOffset -= changedShakePos;

        changedShakePos = Vector3.zero;

        priorityShake = false;

    }

    public void ScreenRecoil(float duration, float magnitude)
    {



        recoilDuration = duration;
        recoilMagnitude = magnitude;

    }
}
