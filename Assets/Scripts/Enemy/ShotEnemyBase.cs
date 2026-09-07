using System.Collections;
using UnityEngine;

public class ShotEnemyBase : EnemyBase
{

    #region Shot Variables

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

        cameraFollow = FindAnyObjectByType<CameraFollow>();

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (inRangeForAttack && !attacking && !reloading && playerObject != null)
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
        agent.SetDestination(transform.position);

        //audioManager.PlayRevolverClickSound(transform.position, gameObject);

        yield return new WaitForSeconds(startShotingTime - 0.4f); // Hur lång tid det tar att "sikta in"

        audioManager.PlayRevolverClickSound(transform.position, gameObject);

        yield return new WaitForSeconds(0.4f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1337, ~bulletIgnoreLayerMask);

        TrailRenderer trail = Instantiate(bulletTrail);
        trail.transform.position = transform.position;

        trail.GetComponent<BulletTrailScript>().MoveAndFadeTrail(trail.transform.position, hit.point);


        if(hit.collider.tag == "Player")
        {
            hit.collider.GetComponent<PlayerHealth>().TakeDamage(1, false, transform);

        }
        if(hit.collider.tag == "PlayerAttack" || hit.collider.tag == "StanceAttack")
        {

            GameObject spawnedBullet = Instantiate(bulletObject);
            spawnedBullet.transform.position = hit.point;

            cameraFollow.ChangeTargetCam(spawnedBullet, 1);

        }

        audioManager.PlayShellSound(transform.position);
        audioManager.PlayShootSound(transform.position, gameObject);
        audioManager.PlayRevolverClickSound(transform.position, gameObject);

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
