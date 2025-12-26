using System.Collections;
using UnityEngine;

public class SwordAndGunCharacter : Player1
{

    [Header("S&G Specifics")]

    [SerializeField] GameObject stanceAttackObject;

    bool attackStance = false;

    #region Basic Dodge

    [Header("Dodge")]

    [SerializeField] float dodgeSpeed = 5;
    [SerializeField] float dodgeTime = 0.5f;
    [SerializeField] float dodgeRecoveryTime = 0.1f;

    GameObject dodgeCollider = null;


    #endregion

    #region Revolver

    [Header("Revolver")]

    [SerializeField] TrailRenderer bulletTrail;

    [SerializeField] Vector2 bulletSpawnPos;
    [SerializeField] int bullets = 6;
    [SerializeField] LayerMask bulletIgnoreLayerMask;

    int maxBullets;

    #endregion

    #region Harmony

    [Header("Harmony")]

    [SerializeField] float decayTimeForHarmony = 7f;
    float decayTimeForHarmonyBase;
    float maxTimeInHarmony = 10f;
    float maxTimeInHarmonyBase;

    bool killWithRevolver = false;
    bool killWithCharge = false;
    bool inHarmony = false;

    [SerializeField] GameObject rushAttackObject;

    [SerializeField] float rushSpeed = 40f;

    bool rushing = false;
    bool startRushAttack = false;

    Vector2 pointToRushTo = Vector2.zero;

    #endregion

    [SerializeField] int stanceAttack = 2;
    int maxStanceAttack;

    CameraFollow cameraScript;
    PlayerHealth playerHealth;

    public override void Start()
    {
        base.Start();

        maxBullets = bullets;
        maxStanceAttack = stanceAttack;
        decayTimeForHarmonyBase = decayTimeForHarmony;
        maxTimeInHarmonyBase = maxTimeInHarmony;

        dodgeCollider = transform.Find("DodgeCollider").gameObject;

        cameraScript = FindAnyObjectByType<CameraFollow>();

        playerHealth = GetComponent<PlayerHealth>();
    }

    public override void Update()
    {
        base.Update();

        #region Rush

        if (rushing && !startRushAttack)
        {

            transform.position = Vector2.MoveTowards(transform.position, pointToRushTo, rushSpeed * Time.deltaTime);

            if(Vector2.Distance(transform.position, pointToRushTo) < 1.7f)
            {
                StartCoroutine(RushAttack());

                dodgeLock = false;
                lockRotationParent = false;
            }

        }

        #endregion

        #region Move Lock

        if (dodgeLock || rushing)
        {
            attackStance = false;
            attacking = false;
            lockRotationParent = false;

            stanceAttackObject.SetActive(false);

            speed = maxSpeed;

            return;

        }

        #endregion

        if (Input.GetKeyDown(KeyCode.Space) && dodgeLock == false)
        {

            StartCoroutine(basicDodge());


        }

        #region Harmony

        if(killWithCharge && killWithRevolver)
        {
            // Enter Harmony 
            if (Input.GetKeyDown(KeyCode.LeftControl) && !dodgeLock)
            {
                inHarmony = true;

                Time.timeScale = 0.1f;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
            }

            // In harmony now
            if (inHarmony)
            {

                // Sword attack harmony
                if (Input.GetMouseButtonDown(0) && !dodgeLock)
                {
                    float clickDistance = Vector2.Distance(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
 
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, lookDirection, clickDistance, ~bulletIgnoreLayerMask);

                    if(hit.point == Vector2.zero)
                    {
                        pointToRushTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    }
                    else
                    {
                        pointToRushTo = hit.point;
                    }

                    rushing = true;

                    dodgeLock = true;
                    lockRotationParent = true;

                    transform.LookAt(pointToRushTo);

                    ResetHarmony();
                }

                // Revolver attack harmony
                if (Input.GetMouseButtonDown(1) && !dodgeLock)
                {

                    RaycastHit2D hit = Physics2D.Raycast(transform.position, lookDirection, 1337, ~bulletIgnoreLayerMask);

                    TrailRenderer trail = Instantiate(bulletTrail);
                    trail.transform.position = transform.position;

                    //SpawnTrail(trail, hit);
                    trail.GetComponent<BulletTrailScript>().MoveAndFadeTrail(trail.transform.position, hit.point);

                    if (hit.transform.tag == "Enemy")
                    {
                        hit.transform.gameObject.GetComponent<EnemyHealth>().TakeDamage(1, 2);
                    }


                    ResetHarmony();
                }


                maxTimeInHarmony -= Time.unscaledDeltaTime;

                if(maxTimeInHarmony < 0)
                {
                    ResetHarmony();
                }

            }

            decayTimeForHarmony -= Time.deltaTime;

            if(decayTimeForHarmony < 0 && !inHarmony)
            {
                ResetHarmony();
            }


        }

        #endregion

        #region Left Click Attacks

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
                trail.GetComponent<BulletTrailScript>().MoveAndFadeTrail(trail.transform.position, hit.point);

                bullets--;

                if(hit.transform.tag == "Enemy")
                {
                    hit.transform.gameObject.GetComponent<EnemyHealth>().TakeDamage(1, 2);
                }

            }

        }

        #endregion

        #region Stance

        if (Input.GetKey(KeyCode.LeftShift) && stanceAttack > 0)
        {

            if (attacking == false)
            {

                //cameraScript.ChangeTargetCam(gameObject, 2);

                speed = 2;

                attackStance = true;
                lockRotationParent = true;

            }

        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && stanceAttack > 0)
        {

            if (attacking == false)
            {

                //cameraScript.ZoomOutAgain(0.1f);

                speed = maxSpeed;

                attackStance = false;
                lockRotationParent = false;

            }

        }

        #endregion

    }

    #region Charge Attack

    IEnumerator ChargeAttack()
    {
        cameraScript.ZoomOutAgain(0.1f);

        attacking = true;

        stanceAttack--;

        stanceAttackObject.SetActive(true);

        speed = 0;

        yield return new WaitForSeconds(0.4f);

        stanceAttackObject.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        speed = maxSpeed;

        attackStance = false;
        attacking = false;
        lockRotationParent = false;


    }

    #endregion

    #region basic Dodge

    IEnumerator basicDodge()
    {
        dodgeLock = true;
        attackObject.SetActive(false);

        Vector2 tempMovementInput = movementInput;
        bool standingStillDodge = false;

        if (movementInput == Vector2.zero) // Om man står stilla så ska man dodga dit man kollar
        {
            standingStillDodge = true;

            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            movementInput = mousePos - myRigidbody.position;

        }

        playerVelocity = new Vector2(movementInput.normalized.x * dodgeSpeed, movementInput.normalized.y * dodgeSpeed);
        myRigidbody.linearVelocity = playerVelocity;

        dodgeCollider.SetActive(true);
        myCollider.enabled = false;


        yield return new WaitForSeconds(dodgeTime / 2);


        dodgeCollider.SetActive(false);
        myCollider.enabled = true;


        yield return new WaitForSeconds(dodgeTime / 2);


        if (standingStillDodge)
        {
            movementInput = tempMovementInput;
        }
        else
        {
            playerVelocity = new Vector2(0, 0);
            myRigidbody.linearVelocity = playerVelocity;
        }

        dodgeLock = false;

        speed = maxSpeed / 2;


        yield return new WaitForSeconds(dodgeRecoveryTime);


        speed = maxSpeed;

    }

    #endregion

    IEnumerator RushAttack()
    {

        rushAttackObject.SetActive(true);

        attacking = true;
        startRushAttack = true;

        yield return new WaitForSeconds(0.5f);

        rushAttackObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        rushing = false;
        startRushAttack = false;
        attacking = false;

    }

    public void RechargeBullets()
    {
        killWithCharge = true;

        if (bullets != maxBullets)
        {
            bullets++;
        }

    }

    public void RechargeStance()
    {
        killWithRevolver = true;

        if (stanceAttack != maxStanceAttack)
        {
            stanceAttack++;
        }

    }

    void ResetHarmony()
    {


        decayTimeForHarmony = decayTimeForHarmonyBase;
        maxTimeInHarmony = maxTimeInHarmonyBase;

        killWithCharge = false;
        killWithRevolver = false;
        inHarmony = false;

        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F;
    }

    #region Things To False

    void ThingsToFalse()
    {

        attacking = false;

        attackStance = false;
        lockRotationParent = false;
        stanceAttackObject.SetActive(false);

        speed = maxSpeed;

        basicAttacking = false;
        attackObject.SetActive(false);

    }

    #endregion

}
