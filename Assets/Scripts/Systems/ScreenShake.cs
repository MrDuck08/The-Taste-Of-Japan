using System.Reflection;
using Unity.Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] float shakeMagnitude = 0.7f;
    [SerializeField] float maxShakeDistance = 15f;

    [SerializeField] CinemachineImpulseListener impulseListener;
    [SerializeField] CinemachineImpulseSource recoilSource;
    [SerializeField] CinemachineImpulseSource standardScreenShakeSource;
    CinemachineBrain brain;

    float shakeDuration = 0f;

    bool priorityShake = false;
    bool shakeStop = true;

    public static Vector3 cameraOffset = Vector3.zero;
    public static Vector3 recoilOffset = Vector3.zero;

    Player1 player;

    private void Start()
    {
        player = FindAnyObjectByType<Player1>();
        brain = GetComponent<CinemachineBrain>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            TriggerShakeTime(5, 35f, false);
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

            Debug.Log("Screen Shake");
            brain.UpdateMethod = CinemachineBrain.UpdateMethods.LateUpdate;
            standardScreenShakeSource.GenerateImpulse(new Vector2(xMagnitude, yMagnitude));
            brain.UpdateMethod = CinemachineBrain.UpdateMethods.LateUpdate;
            shakeDuration -= Time.unscaledDeltaTime;
        }
        else if (!shakeStop)
        {
            // Shake Done
            Time.timeScale = 1;
            StopShake();
        }
    }

    public void TriggerShakeTime(float duration, float magnitude, bool isShakeImortant)
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

        priorityShake = false;

    }

    public void ScreenRecoil(float duration, float magnitude)
    {

        recoilSource.GenerateImpulse(-player.lookDirection.normalized);


    }
}
