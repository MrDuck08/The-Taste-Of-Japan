using System.Collections;
using UnityEngine;

public class ShotEnemyBase : EnemyBase
{

    #region Shot

    [Header("Shot")]

    [SerializeField] LayerMask bulletIgnoreLayerMask;

    [SerializeField] GameObject bulletObject;
    [SerializeField] TrailRenderer bulletTrail;

    [SerializeField] float startShotingTime = 1f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float reloadTime = 3;
    [SerializeField] int maxBullets = 2;

    int bullets;

    bool reloading = false;

    #endregion

    [Header("WalkBack")]

    [SerializeField] float walkBackSpeed = 2f;


    CameraFollow cameraFollow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();

        bullets = maxBullets;

        cameraFollow = FindObjectOfType<CameraFollow>();

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (inRangeForAttack && !attacking && !reloading)
        {

            StartCoroutine(ShotRoutine());

        }

        if(reloading && inRangeForAttack)
        {
            transform.position += -transform.up * Time.deltaTime * walkBackSpeed;
        }

    }

    #region Shot + Reload

    IEnumerator ShotRoutine()
    {

        attacking = true;


        yield return new WaitForSeconds(startShotingTime); // Hur lång tid det tar att "sikta in"


        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1337, ~bulletIgnoreLayerMask);

        TrailRenderer trail = Instantiate(bulletTrail);
        trail.transform.position = transform.position;

        trail.GetComponent<BulletTrailScript>().MoveAndFadeTrail(trail.transform.position, hit.point);


        if(hit.collider.tag == "Player")
        {

            hit.collider.GetComponent<PlayerHealth>().TakeDamage(1);

        }
        if(hit.collider.tag == "PlayerAttack")
        {

            GameObject spawnedBullet = Instantiate(bulletObject);
            spawnedBullet.transform.position = hit.point;

            cameraFollow.ChangeTargetCam(spawnedBullet, 1);

        }

        bullets--;

        if (bullets <= 0)
        {

            StartCoroutine(ReloadRoutine());

        }


        yield return new WaitForSeconds(fireRate); // Paus Till nästa skot


        attacking = false;

    }

    IEnumerator ReloadRoutine()
    {
        reloading = true;

        agent.enabled = false;

        yield return new WaitForSeconds(reloadTime);

        bullets = maxBullets;

        agent.enabled = true;

        reloading = false;

    }

    #endregion

}
