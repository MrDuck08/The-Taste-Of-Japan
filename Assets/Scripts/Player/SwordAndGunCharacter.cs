using System.Collections;
using UnityEngine;

public class SwordAndGunCharacter : Player1
{

    bool attackStance = false;

    [Header("S&G Specifics")]

    [SerializeField] GameObject stanceAttackObject;

    #region Revolver

    [SerializeField] TrailRenderer bulletTrail;

    [SerializeField] Vector2 bulletSpawnPos;
    [SerializeField] int bullets = 6;
    [SerializeField] LayerMask bulletIgnoreLayerMask;

    [SerializeField] float trailSpeed = 20f;
    [SerializeField] float fadeDuration = 1f;
    int maxBullets;

    #endregion

    public override void Start()
    {
        base.Start();

        maxBullets = bullets;
    }

    public override void Update()
    {
        base.Update();

        #region Dodge Lock

        if (dodgeLock)
        {
            attackStance = false;
            attacking = false;
            stanceBoolToParent = false;

            stanceAttackObject.SetActive(false);

            speed = 20;

            return;

        }

        #endregion

        #region Left Click Attack

        if (Input.GetMouseButtonDown(0))
        {

            if(attacking == false)
            {
                if(attackStance == false)
                {
                    StartCoroutine(BasicAttack());
                }

                if (attackStance == true)
                {

                    StartCoroutine(ChargeAttack());

                }


            }

        }

        #endregion

        #region Revolver Attack

        if (Input.GetMouseButtonDown(1))
        {

            if (bullets > 0)
            {

                ThingsToFalse();

                RaycastHit2D hit = Physics2D.Raycast(transform.position, lookDirection, 1337, ~bulletIgnoreLayerMask);

                TrailRenderer trail = Instantiate(bulletTrail);
                trail.transform.position = transform.position;

                //SpawnTrail(trail, hit);
                StartCoroutine(MoveAndFadeTrail(trail, hit.point));

                //bullets--;

            }

        }

        #endregion

        #region Stance

        if (Input.GetKey(KeyCode.LeftShift))
        {

            if (attacking == false)
            {

                speed = 2;

                attackStance = true;
                stanceBoolToParent = true;

            }

        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {

            if (attacking == false)
            {

                speed = 20;

                attackStance = false;
                stanceBoolToParent = false;

            }

        }

        #endregion

    }

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

    IEnumerator ChargeAttack()
    {
        attacking = true;

        stanceAttackObject.SetActive(true);

        speed = 0;

        yield return new WaitForSeconds(0.4f);

        speed = 20;

        stanceAttackObject.SetActive(false);

        attackStance = false;
        attacking = false;
        stanceBoolToParent = false;


    }

    void ThingsToFalse()
    {

        attacking = false;

        attackStance = false;
        stanceBoolToParent = false;
        stanceAttackObject.SetActive(false);

        speed = 20;

        basicAttacking = false;
        attackObject.SetActive(false);

    }

}
