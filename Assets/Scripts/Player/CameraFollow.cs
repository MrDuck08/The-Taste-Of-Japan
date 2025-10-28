using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject target;

    // Update is called once per frame
    void Update()
    {

        if (target != null)
        {
            transform.position = target.transform.position;
        }
    }
}
