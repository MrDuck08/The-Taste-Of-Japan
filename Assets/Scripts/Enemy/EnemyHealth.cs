using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] int health;
    [SerializeField] LayerMask obsticleCheck;

    ScreenShake screenShake;
    SwordAndGunCharacter swordAndGun;
    Player1 player1;

    AudioManager audioManager;

    private void Start()
    {
        screenShake = FindAnyObjectByType<ScreenShake>();
        swordAndGun = FindAnyObjectByType<SwordAndGunCharacter>();
        player1 = FindAnyObjectByType<Player1>();
        audioManager = FindAnyObjectByType<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("PlayerAttack"))
        {
            Vector2 direction = player1.transform.position - transform.position;
            float lenght = Vector2.Distance(player1.transform.position, transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, lenght, obsticleCheck);

            if (!hit)
            {
                TakeDamage(1, 1);
            }

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

            audioManager.PlayEnemyDeathSound(transform.position);

            switch (whatTypeOfAttack)
            {

                case 1:

                    if(swordAndGun != null)
                    {
                        swordAndGun.RechargeBullets();
                    }

                    break;

                case 2:

                    swordAndGun.RechargeStance();
                    audioManager.PlayEnemyBulletDeathSound(transform.position);

                    break;

            }

            screenShake.TriggerShake(0.05f, 0.35f, false);
            Destroy(gameObject);

        }

    }

}
