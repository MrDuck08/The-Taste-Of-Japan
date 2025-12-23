using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] int health;

    [SerializeField] GameObject rechargeBulletObject;
    [SerializeField] GameObject rechargeStanceObject;

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

            TakeDamage(1, 1);

        }

        //Träffad av dörr, Kollar också hur snabb dörren var
        if (collision.gameObject.layer == 6 && collision.GetComponent<Door>().playerPushedDoor == true && collision.GetComponent<Rigidbody2D>().linearVelocity.magnitude > 12)
        {
            #region For Knockback, (Removed For Now)

            //// Får vinkeln åt Vart den ska åka
            //Vector2 direction = transform.position - Door.posWhenOpened;

            //StartCoroutine(enemyBase.BeStunned(direction.normalized));

            #endregion

            TakeDamage(1, 0);

        }
    }

    public void TakeDamage(int takeDamage, int whatTypeOfAttack)
    {
        // whatTypeOfAttack
        // 0 = Door hit
        // 1 = S&R Basic Hit
        // 2 = S&R Revolver Hit

        health -= takeDamage;

        if (health <= 0)
        {

            switch (whatTypeOfAttack)
            {

                case 1:

                    Instantiate(rechargeBulletObject, transform.position, Quaternion.identity);

                    break;

                case 2:

                    Instantiate(rechargeStanceObject, transform.position, Quaternion.identity);

                    break;

            }

            screenShake.TriggerShake(0.05f, 0.7f, false);
            Destroy(gameObject);

        }

    }

}
