using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] int health;

    ScreenShake screenShake;

    private void Start()
    {
        screenShake = FindObjectOfType<ScreenShake>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("PlayerAttack"))
        {

            TakeDamage(1);

        }

        if (collision.gameObject.layer == 6 && collision.gameObject.GetComponent<Door>().playerPushedDoor == true && collision.GetComponent<Rigidbody2D>().linearVelocity.magnitude > 12) //Träffad av dörr
        {

            Destroy(gameObject); // ÄNDRA TILL STUNED

        }
    }

    public void TakeDamage(int takeDamage)
    {

        health -= takeDamage;

        if (health <= 0)
        {

            screenShake.TriggerShake(0.1f, 0.7f);
            Destroy(gameObject);

        }

    }

}
