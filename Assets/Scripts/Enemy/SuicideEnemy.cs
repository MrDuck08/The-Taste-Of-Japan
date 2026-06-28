using System.Collections;
using UnityEngine;

public class SuicideEnemy : EnemyBase
{

    [Header("Obj")]
    [SerializeField] GameObject warningVisuallObj;
    [SerializeField] GameObject explosionObj;

    [Header("Variables")]
    [SerializeField] float maxTimeUntilExplosion = 0.5f;
    float timeUntilExplosion = 0.5f;

    bool litFuse = false;
    bool hasBeenKnockedBack = false;

    EnemyHealth enemyHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();

        enemyHealth = GetComponent<EnemyHealth>();

        timeUntilExplosion = maxTimeUntilExplosion;

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (inRangeForAttack && !litFuse && playerObject != null)
        {
            litFuse = true;
            lockMovement = true;
            agent.isStopped = true;

            StartCoroutine(audioManager.WarningExplosionSound(transform.position, gameObject));

        }


        if (litFuse)
        {
            warningVisuallObj.SetActive(true);

            timeUntilExplosion -= Time.deltaTime;

            if(timeUntilExplosion < 0)
            {

                StartCoroutine(Explode());

            }

        }

    }

    public IEnumerator Explode()
    {


        explosionObj.SetActive(true);

        // Behöver tid så att collisionen kan registreras
        yield return new WaitForSeconds(0.1f);

        audioManager.ExplosionSound(transform.position);

        enemyHealth.KillEffects();
        enemyHealth.BloodEffects(1, transform.position);
        enemyHealth.BloodEffects(1, transform.position);
        enemyHealth.BloodEffects(1, transform.position);
        enemyHealth.BloodEffects(1, transform.position);
        enemyHealth.BloodEffects(1, transform.position);


        Destroy(gameObject);

    }

    public void KnockBack()
    {

        if (hasBeenKnockedBack)
        {
            StartCoroutine(Explode());

            return;
        }
        hasBeenKnockedBack = true;

        lockMovement = true;
        agent.isStopped = true;

        Vector2 playerLookDir = playerObject.GetComponent<Player1>().lookDirection;
        StartCoroutine(BeStunned(playerLookDir.normalized));

        if (!litFuse)
        {
            StartCoroutine(audioManager.WarningExplosionSound(transform.position, gameObject));
        }

        timeUntilExplosion = maxTimeUntilExplosion;
        litFuse = true;



    }
}
