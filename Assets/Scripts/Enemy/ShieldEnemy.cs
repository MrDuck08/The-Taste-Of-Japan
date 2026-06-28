using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ShieldEnemy : EnemyBase
{
    [Header("Shield Enemy Specifik")]

    [SerializeField] GameObject attackObject;
    [SerializeField] GameObject ShieldObj;

    [SerializeField] float timeForAttack = 0.2f;
    [SerializeField] float timeForAttackToDisaappear = 0.1f;

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

        if (inRangeForAttack && !attacking)
        {
            StartCoroutine(BasicAttackRoutine());
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
        ShieldObj.SetActive(false);

        Vector2 toTarget = playerObject.transform.position - transform.position;
        audioManager.PlayShieldDestroySound(transform.position);
        StartCoroutine(BeStunned(-toTarget.normalized));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.transform.CompareTag("StanceAttack"))
        {
            Vector2 direction = playerObject.transform.position - transform.position;
            float lenght = Vector2.Distance(playerObject.transform.position, transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, lenght, health.obsticleCheck);

            if (ShieldObj != null && hit)
            {
                audioManager.PlayShieldDestroySound(transform.position);
            }
            health.TakeDamage(1, 1, playerObject.transform.position);

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
