using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField] float openSpeed = 5;

    Rigidbody2D rigidbody2D;

    public bool playerPushedDoor;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

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

            rigidbody2D.AddForce(leftOrRight.normalized * 10 * -openSpeed);

            playerPushedDoor = false;

            return;

        }
        else
        {
            playerPushedDoor = true;
        }

        rigidbody2D.AddForce(collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity * openSpeed);


    }

}
