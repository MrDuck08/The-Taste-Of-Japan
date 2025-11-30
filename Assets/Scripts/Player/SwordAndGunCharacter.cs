using System.Collections;
using UnityEngine;

public class SwordAndGunCharacter : Player1
{

    bool attackStance = false;

    [Header("S&G Specifics")]

    [SerializeField] GameObject stanceAttackObject;

    #region Basic Dodge

    [Header("Dodge")]

    [SerializeField] float dodgeSpeed = 5;
    [SerializeField] float dodgeTime = 0.5f;

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

    CameraFollow cameraScript;

    public override void Start()
    {
        base.Start();

        maxBullets = bullets;

        dodgeCollider = transform.Find("DodgeCollider").gameObject;

        cameraScript = FindAnyObjectByType<CameraFollow>();
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

        if (Input.GetKeyDown(KeyCode.Space) && dodgeLock == false)
        {

            StartCoroutine(basicDodge());


        }

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
                trail.GetComponent<BulletTrailScript>().MoveAndFadeTrail(trail.transform.position, hit.point);

                //bullets--;

                if(hit.transform.tag == "Enemy")
                {
                    hit.transform.gameObject.GetComponent<EnemyHealth>().TakeDamage(1);
                }

            }

        }

        #endregion

        #region Stance

        if (Input.GetKey(KeyCode.LeftShift))
        {

            if (attacking == false)
            {

                cameraScript.ChangeTargetCam(gameObject, 2);

                speed = 2;

                attackStance = true;
                stanceBoolToParent = true;

            }

        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {

            if (attacking == false)
            {

                cameraScript.ZoomOutAgain(0.1f);

                speed = 20;

                attackStance = false;
                stanceBoolToParent = false;

            }

        }

        #endregion

    }

    #region Charge Attack

    IEnumerator ChargeAttack()
    {
        cameraScript.ZoomOutAgain(0.1f);

        attacking = true;

        stanceAttackObject.SetActive(true);

        speed = 0;

        yield return new WaitForSeconds(0.4f);

        stanceAttackObject.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        speed = 20;

        attackStance = false;
        attacking = false;
        stanceBoolToParent = false;


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
    }

    #endregion

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
