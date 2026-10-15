using System.Collections;
using UnityEngine;

public class ShieldEnemy : EnemyBase
{
    [Header("Shield Enemy Specifik")]

    [SerializeField] GameObject attackObject;
    [SerializeField] GameObject ShieldObj;
    [SerializeField] ParticleSystem shieldDestroyParticles;

    [SerializeField] float timeForAttack = 0.2f;
    [SerializeField] float timeForAttackToDisaappear = 0.1f;

    bool startDoubleCheck = false;
    float doubleCheckTime;
    float maxDoubleCheckTime = 1f;
    int whatTypeOfAttackDoubleCheck = 1;

    EnemyHealth health;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();

        health = GetComponent<EnemyHealth>();

        attackObject.SetActive(false);

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (inRangeForAttack && !attacking && playerObject != null)
        {
            StartCoroutine(BasicAttackRoutine());
        }

        if (startDoubleCheck)
        {
            doubleCheckTime -= Time.deltaTime;

            if (doubleCheckTime < 0)
            {
                startDoubleCheck = false;
                doubleCheckTime = maxDoubleCheckTime;

            }


            Vector2 direction = playerObject.transform.position - transform.position;
            float lenght = Vector2.Distance(playerObject.transform.position, transform.position);

            health.obsticleCheck &= ~(1 << LayerMask.NameToLayer("Shield"));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, lenght - 0.3f, health.obsticleCheck);
            // Lägger tillbaks
            health.obsticleCheck |= (1 << LayerMask.NameToLayer("Shield"));

            if (!hit)
            {
             // Om det itne finns någon sköld så ska den inte köra sköld ljud + effekter
                if (ShieldObj != null)
                {
                    audioManager.PlayShieldDestroySound(transform.position);

                    ParticleSystem spawnedShieldParticles = Instantiate(shieldDestroyParticles);
                    spawnedShieldParticles.transform.position = ShieldObj.transform.position;
                    scoreSystem.PointsForShieldDestroy(ShieldObj.transform.position);

                    shieldDestroyParticles.Play();
                }

                health.TakeDamage(1, whatTypeOfAttackDoubleCheck, playerObject.transform.position);
            }

        }

    }

    IEnumerator BasicAttackRoutine()
    {
        attacking = true;

        yield return new WaitForSeconds(timeForAttack);

        attackObject.SetActive(true);

        yield return new WaitForSeconds(timeForAttackToDisaappear);

        attackObject.SetActive(false);

        attacking = false;

    }

    public void ShieldRemove()
    {
        scoreSystem.PointsForShieldDestroy(ShieldObj.transform.position);
        ShieldObj.SetActive(false);

        attacking = false;
        attackObject.SetActive(false);

        ParticleSystem spawnedShieldParticles = Instantiate(shieldDestroyParticles);
        spawnedShieldParticles.transform.position = ShieldObj.transform.position;
        shieldDestroyParticles.Play();

        Vector2 toTarget = playerObject.transform.position - transform.position;
        audioManager.PlayShieldDestroySound(transform.position);
        StartCoroutine(BeStunned(-toTarget.normalized));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.transform.CompareTag("StanceAttack") || collision.transform.CompareTag("HarmonyAttack") || collision.transform.CompareTag("Explosion"))
        {
            Vector2 direction = playerObject.transform.position - transform.position;
            float lenght = Vector2.Distance(playerObject.transform.position, transform.position);

            // Dessa attacker ska gå igenom skölden
            // tar bort layer
            health.obsticleCheck &= ~(1 << LayerMask.NameToLayer("Shield"));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, lenght - 0.3f, health.obsticleCheck);
            // Lägger tillbaks
            health.obsticleCheck |= (1 << LayerMask.NameToLayer("Shield"));


            if (!hit)
            {
                // Om det itne finns någon sköld så ska den inte köra sköld ljud + effekter
                if (ShieldObj != null)
                {
                    audioManager.PlayShieldDestroySound(transform.position);

                    ParticleSystem spawnedShieldParticles = Instantiate(shieldDestroyParticles);
                    spawnedShieldParticles.transform.position = ShieldObj.transform.position;
                    scoreSystem.PointsForShieldDestroy(ShieldObj.transform.position);

                    shieldDestroyParticles.Play();
                }

                if (collision.transform.CompareTag("StanceAttack"))
                {
                    health.TakeDamage(1, 2, playerObject.transform.position);
                }
                if (collision.transform.CompareTag("HarmonyAttack"))
                {
                    health.TakeDamage(1, 4, playerObject.transform.position);
                }
                if (collision.transform.CompareTag("Explosion"))
                {
                    health.TakeDamage(1, 5, collision.transform.position);
                }
            }
            else // Dubbelkollar så länge spelaren inte träffade en sköld (annars kan man springa igenom och döda)
            {
                doubleCheckTime = maxDoubleCheckTime;
                startDoubleCheck = true;

                if (collision.transform.CompareTag("StanceAttack"))
                {
                    whatTypeOfAttackDoubleCheck = 2;
                }
                if (collision.transform.CompareTag("HarmonyAttack"))
                {
                    whatTypeOfAttackDoubleCheck = 4;
                }
                if (collision.transform.CompareTag("Explosion"))
                {
                    whatTypeOfAttackDoubleCheck = 5;
                }
            }

        }

        if (collision.transform.CompareTag("PlayerAttack"))
        {
            Vector2 direction = playerObject.transform.position - transform.position;
            float lenght = Vector2.Distance(playerObject.transform.position, transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, lenght, health.obsticleCheck);

            if (hit)
            {

                audioManager.PlayShieldDeflectSound(transform.position);

            }


        }

    }
}
