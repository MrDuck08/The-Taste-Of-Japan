using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] float shakeMagnitude = 0.7f;
    [SerializeField] Transform playerTarget;
    Transform tempTarget;

    private float shakeDuration = 0f;
    Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        if (playerTarget == null)
        {
            return;
        }

        transform.position = new Vector3(playerTarget.position.x, playerTarget.position.y, transform.position.z);

        if (shakeDuration > 0)
        {
            transform.position += new Vector3(Random.Range(-shakeMagnitude, shakeMagnitude), Random.Range(-shakeMagnitude, shakeMagnitude), 0);
            shakeDuration -= Time.unscaledDeltaTime;
        }
    }

    public void TriggerShake(float duration)
    {
        shakeDuration = duration;
    }
}
