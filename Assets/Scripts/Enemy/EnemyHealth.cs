using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] int health;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("PlayerAttack"))
        {

            health--;

            if (health <= 0)
            {


                Destroy(gameObject);

            }

        }

        if (collision.gameObject.layer == 6 && collision.gameObject.GetComponent<Door>().playerPushedDoor == true && collision.GetComponent<Rigidbody2D>().linearVelocity.magnitude > 12) //Tr�ffad av d�rr
        {

            Destroy(gameObject); // �NDRA TILL STUNED

        }
    }


}
