using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] float shakeMagnitude = 0.7f;
    [SerializeField] float maxShakeDistance = 15f;

    private float shakeDuration = 0f;

    bool priorityShake = false;
    bool shakeStop = true;

    Vector3 ChangedPosition = Vector3.zero;

    public static Vector3 shakePosition = Vector3.zero;


    private void Update()
    {
        if (shakeDuration > 0)
        {
            float xMagnitude = Random.Range(-shakeMagnitude, shakeMagnitude);
            float yMagnitude = Random.Range(-shakeMagnitude, shakeMagnitude);

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

            // Sparar alla ändringar på objectet
            ChangedPosition += new Vector3(xMagnitude, yMagnitude, 0);

            shakePosition += new Vector3(xMagnitude, yMagnitude, 0);
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
        shakePosition -= ChangedPosition;

        ChangedPosition = Vector3.zero;

        priorityShake = false;

    }
}
