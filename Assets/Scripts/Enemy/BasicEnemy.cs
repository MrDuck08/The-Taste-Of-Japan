using System.Collections;
using UnityEngine;

public class BasicEnemy : EnemyBase
{

    [Header("Basic Enemy Specifik")]

    [SerializeField] GameObject attackObject;

    [SerializeField] float timeForAttack = 0.2f;
    [SerializeField] float timeForAttackToDisaappear = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();

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

}
