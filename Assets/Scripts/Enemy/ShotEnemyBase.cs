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
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] float trailSpeed = 20f;

    [SerializeField] int maxBullets = 2;
    int bullets;

    bool reloading = false;

    #endregion

    [Header("WalkBack")]

    [SerializeField] float walkBackSpeed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();

        bullets = maxBullets;

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (inRangeForAttack && !attacking && !reloading)
        {

            StartCoroutine(ShotRoutine());

        }

        //if(reloading && inRangeForAttack)
        //{
        //    Debug.Log("GO BACJ");
        //    transform.position += -transform.up * Time.deltaTime * 10f;

        //}

    }

    #region Shot + Reload

    IEnumerator ShotRoutine()
    {

        attacking = true;

        agent.SetDestination(transform.position); // �r h�r f�r om spelaren kommer backifr�n s� s�tter den destinationen p� spelaren, den �r h�r f�r om spelaren kommer backifr�n s� g�r skjutaren ingen stans


        yield return new WaitForSeconds(startShotingTime); // Hur l�ng tid det tar att "sikta in"


        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1337, ~bulletIgnoreLayerMask);

        TrailRenderer trail = Instantiate(bulletTrail);
        trail.transform.position = transform.position;

        StartCoroutine(MoveAndFadeTrail(trail, hit.point));

        Debug.Log(hit.collider.gameObject.name);

        if(hit.collider.tag == "Player")
        {

            //Debug.Log(hit.collider.gameObject);

        }
        if(hit.collider.tag == "PlayerAttack")
        {

            Debug.Log("WALLAI IM FINISHED!!!");

        }

        bullets--;

        if (bullets <= 0)
        {

            StartCoroutine(ReloadRoutine());

        }


        yield return new WaitForSeconds(fireRate); // Paus Till n�sta skot


        attacking = false;

    }

    IEnumerator ReloadRoutine()
    {
        reloading = true;

        yield return new WaitForSeconds(reloadTime);

        bullets = maxBullets;

        reloading = false;

    }

    #endregion

    #region Fade & Widen Bullet

    IEnumerator MoveAndFadeTrail(TrailRenderer trail, Vector2 target)
    {
        float elapsedTime = 0f;
        Vector2 startPosition = transform.position;

        // Cache initial alpha
        float startAlpha = trail.startColor.a;
        float endAlpha = trail.endColor.a;

        while (elapsedTime < fadeDuration)
        {
            // Move the trail towards the target
            float moveT = Mathf.Clamp01(elapsedTime * trailSpeed);
            trail.transform.position = Vector2.Lerp(startPosition, target, moveT);

            // Fade the alpha over time
            float alphaT = 1 - (elapsedTime / fadeDuration);
            float currentStartAlpha = startAlpha * alphaT;
            float currentEndAlpha = endAlpha * alphaT;

            trail.startWidth += 0.00015f;
            trail.endWidth += 0.00015f;

            trail.startColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, currentStartAlpha);
            trail.endColor = new Color(trail.endColor.r, trail.endColor.g, trail.endColor.b, currentEndAlpha);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        // Final position and cleanup
        trail.transform.position = target;
        Destroy(trail.gameObject);
    }

    #endregion
}
