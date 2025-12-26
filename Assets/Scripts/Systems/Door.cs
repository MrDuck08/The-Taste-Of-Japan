using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField] float openSpeed = 5;

    Rigidbody2D myRigidbody2D;
    Rigidbody2D collisionRB2D;

    public static Vector3 posWhenOpened;

    public bool playerPushedDoor;

    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.gameObject.GetComponent<Rigidbody2D>() == null)
        {

            // Finns en Rigdidbody på parent (Gjord för dodge collider är på Child)
            if(collision.GetComponentInParent<Rigidbody2D>() != null)
            {
                collisionRB2D = collision.GetComponentInParent<Rigidbody2D>();
            }
            else
            {
                return;
            }
        }
        else
        {
            // Finns en Rigidbody på objectet
            collisionRB2D = collision.GetComponent<Rigidbody2D>();
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 leftOrRight = collision.transform.position - transform.position;

            myRigidbody2D.AddForce(leftOrRight.normalized * 10 * -openSpeed);

            return;

        }
        else
        {
            playerPushedDoor = true;

            StartCoroutine(playerStopDoor());
        }

        // Får positionen från när någon nuddar dörren
        posWhenOpened = transform.position;

        myRigidbody2D.AddForce(collisionRB2D.linearVelocity * openSpeed);


    }

    IEnumerator playerStopDoor()
    {


        yield return new WaitForSeconds(1);

        playerPushedDoor = false;

    }

}
