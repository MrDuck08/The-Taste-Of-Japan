using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField] float openSpeed = 5;

    Rigidbody2D myRigidbody2D;

    public static Vector3 posWhenOpened;

    public static bool playerPushedDoor;

    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.gameObject.GetComponent<Rigidbody2D>() == null)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {

            Vector2 leftOrRight = collision.transform.position - transform.position;

            myRigidbody2D.AddForce(leftOrRight.normalized * 10 * -openSpeed);

            playerPushedDoor = false;

            return;

        }
        else
        {
            playerPushedDoor = true;
        }

        // Får positionen från när någon nuddar dörren
        posWhenOpened = transform.position;

        myRigidbody2D.AddForce(collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity * openSpeed);


    }

}
