using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField] float openSpeed = 5;

    Rigidbody2D myRigidbody2D;
    Rigidbody2D collisionRB2D;

    public static Vector3 posWhenOpened;

    public bool playerPushedDoor;

    AudioManager audioManager;

    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();

        audioManager = FindAnyObjectByType<AudioManager>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 3 = Wall
        if (collision.gameObject.layer == 3) { return; }

        if (collision.gameObject.GetComponent<Rigidbody2D>() == null)
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

            ArtificialPush(collision.transform.position, 10);

            return;

        }
        else
        {
            playerPushedDoor = true;

            StartCoroutine(playerStopDoor());
        }

        // Får positionen från när någon nuddar dörren
        posWhenOpened = transform.position;

        audioManager.PlayDoorSlamSound(transform.position, true);

        myRigidbody2D.AddForce(collisionRB2D.linearVelocity * openSpeed);


    }

    public void ArtificialPush(Vector3 collision, float Strengh)
    {

        audioManager.PlayDoorSlamSound(transform.position, false);

        Vector2 leftOrRight = collision - transform.position;


        myRigidbody2D.AddForce(leftOrRight.normalized * Strengh * -openSpeed);

    }

    IEnumerator playerStopDoor()
    {


        yield return new WaitForSeconds(1);

        playerPushedDoor = false;

    }

}
