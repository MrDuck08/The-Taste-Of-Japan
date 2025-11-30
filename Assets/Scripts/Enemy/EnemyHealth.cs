using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] int health;

    ScreenShake screenShake;
    EnemyBase enemyBase;

    private void Start()
    {
        enemyBase = GetComponent<EnemyBase>();

        screenShake = FindAnyObjectByType<ScreenShake>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("PlayerAttack"))
        {

            TakeDamage(1);

        }

        //Träffad av dörr, Kollar också hur snabb dörren var
        if (collision.gameObject.layer == 6 && Door.playerPushedDoor == true && collision.GetComponent<Rigidbody2D>().linearVelocity.magnitude > 12)
        {
            // Får vinkeln åt Vart den ska åka
            Vector2 direction = transform.position - Door.posWhenOpened;

            StartCoroutine(enemyBase.BeStunned(direction.normalized));

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
