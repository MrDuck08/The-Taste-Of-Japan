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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();

        timeUntilExplosion = maxTimeUntilExplosion;

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (inRangeForAttack && !litFuse && playerObject != null)
        {
            litFuse = true;
        }


        if (litFuse)
        {
            warningVisuallObj.SetActive(true);

            timeUntilExplosion -= Time.deltaTime;

            if(timeUntilExplosion < 0)
            {

                Explode();

            }

        }

    }

    IEnumerator ExplodeFuse()
    {
        attacking = true;

        warningVisuallObj.SetActive(true);

        yield return new WaitForSeconds(timeUntilExplosion);

        Explode();

    }

    public void Explode()
    {


        explosionObj.SetActive(true);

        Destroy(gameObject);

    }

    public void KnockBack()
    {

        if (hasBeenKnockedBack)
        {
            Explode();
        }

        Vector2 toTarget = playerObject.transform.position - transform.position;
        StartCoroutine(BeStunned(-toTarget.normalized));

        hasBeenKnockedBack = true;

        timeUntilExplosion = maxTimeUntilExplosion;
        litFuse = true;



    }
}
