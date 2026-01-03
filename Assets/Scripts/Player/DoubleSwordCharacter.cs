using UnityEngine;
using System.Collections;

public class DoubleSwordCharacter : Player1
{
    [Header("Double Sword Specifics")]

    [SerializeField] GameObject sword1Hitbox;
    [SerializeField] GameObject sword2Hitbox;
    [SerializeField] GameObject fist;

    [SerializeField] GameObject sword1;
    [SerializeField] GameObject sword2;

    int numberOfSwordsHeld = 2;

    #region Sprint Variables

    [Header("Sprint")]

    [SerializeField] float sprintSpeed = 25f;

    bool sprinting = false;
    bool sprintCheck = false;
    bool gotWhereToSprint = false;

    float afterSprintDeceleraton;
    [SerializeField] float timeToDecelerate = 0.7f;
    float timeToDecelerateBase;


    #endregion

    [Header("Spin")]

    [SerializeField] float spinSpeed = 70f;
    [SerializeField] float lookSpeed = 50;
    [SerializeField] float wallCrashSpeed = 50f;

    bool spinning = false;
    bool afterSpinSprint = false;
    bool stunned = false;

    [SerializeField] GameObject leftSpinPos;
    [SerializeField] GameObject rightSpinPos;

    Vector3 spinAroundPos;
    Vector3 lookWhenSpining;

    #region Basic Dodge Variables

    [Header("Dodge")]

    [SerializeField] float dodgeSpeed = 5;
    [SerializeField] float dodgeTime = 0.5f;
    [SerializeField] float dodgeRecoveryTime = 0.1f;

    GameObject dodgeCollider = null;


    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();

        timeToDecelerateBase = timeToDecelerate;
        // Dividerar med 2 så att man får kontroll när man har stannat med hälften av farten
        afterSprintDeceleraton = (sprintSpeed / 2) / timeToDecelerateBase;

        dodgeCollider = transform.Find("DodgeCollider").gameObject;

    }

    // Update is called once per frame
    public override void Update()
    {
       base.Update();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            sprintCheck = true;
        }
        else
        {

            sprintCheck = false;

            // Gör så att spelaren stannar långsamt
            // !spining så den inte saktar ned när mans snurrar runt
            if (sprinting && !spinning)
            {
                speed -= afterSprintDeceleraton * Time.deltaTime;

                timeToDecelerate -= Time.deltaTime;

                if (timeToDecelerate <= 0)
                {

                    movementInput = inactiveMovementInput;

                    timeToDecelerate = timeToDecelerateBase;
                    // Dividerar med 2 så att man får kontroll när man har stannat med hälften av farten
                    afterSprintDeceleraton = (sprintSpeed / 2) / timeToDecelerateBase;

                    sprinting = false;
                    gotWhereToSprint = false;

                    lockRotationParent = false;
                    lockMoveinputParent = false;

                    speed = maxSpeed;

                }
            }
        }

        SprintCheck();

        if (dodgeLock || sprinting)
        {
            attacking = false;

            if (sprinting)
            {


                Spin();

            }

            return;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(basicDodge());
        }

        #region Attack Checks

        if (Input.GetMouseButtonDown(0))
        {
            if (!attacking)
            {
                StartCoroutine(DoubleSwordBasicAttack());
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!attacking && numberOfSwordsHeld != 0)
            {
                ThrowSword();
            }
        }

        #endregion
    }

    #region Throw Sword // Nothing yet

    void ThrowSword()
    {



    }

    #endregion 

    #region Basic Attack

    IEnumerator DoubleSwordBasicAttack()
    {

        switch (numberOfSwordsHeld)
        {

            case 0:

                fist.SetActive(true);

                attacking = true;
                basicAttacking = true;

                yield return new WaitForSeconds(0.1f);

                fist.SetActive(false);

                break;

            case 1:

                sword1Hitbox.SetActive(true);

                attacking = true;
                basicAttacking = true;

                yield return new WaitForSeconds(0.1f);

                sword1Hitbox.SetActive(false);

                break;

            case 2:

                sword1Hitbox.SetActive(true);

                attacking = true;
                basicAttacking = true;

                yield return new WaitForSeconds(0.1f);


                sword1Hitbox.SetActive(false);


                yield return new WaitForSeconds(0.1f);

                sword1Hitbox.SetActive(false);

                break;

        }


        yield return new WaitForSeconds(0.1f);

        attacking = false;
        basicAttacking = false;

    }

    #endregion

    #region Sprinting

    void SprintCheck()
    {

        if (spinning || stunned) { return; }

        // Kollar Om Man Kan Sprinta
        if (!attacking && !dodgeLock && sprintCheck)
        {
            sprinting = true;

            // Låser var man kollar och moveinput som tcingar den att springa
            lockRotationParent = true;
            lockMoveinputParent = true;


            // Så den får vart den ska gå 1 gång
            if (!gotWhereToSprint)
            {
                gotWhereToSprint = true;

                if (afterSpinSprint)
                {
                    movementInput = transform.up; 

                    return;
                }

                // Tvingar den att srpinga vart den kollar
                if (movementInput == Vector2.zero)
                {
                    movementInput = lookDirection.normalized;
                }
                else // Tvingar den att springa åt hållet den ville senast gå
                {
                    float angle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);

                }
            }

            speed = sprintSpeed;

        }

    }

    #endregion

    void Spin()
    {

        #region Left spin

        if (Input.GetMouseButtonDown(0))
        {
            myRigidbody.linearVelocity = Vector2.zero;

            spinAroundPos = leftSpinPos.transform.position;

            afterSpinSprint = false;
            gotWhereToSprint = false;

        }
        if (Input.GetMouseButton(0))
        {
            speed = 0;

            lookWhenSpining = transform.position - spinAroundPos;

            float angle = Mathf.Atan2(lookWhenSpining.y, lookWhenSpining.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);

            spinning = true;
            transform.RotateAround(spinAroundPos, Vector3.forward, spinSpeed * Time.deltaTime);

        }
        if (Input.GetMouseButtonUp(0))
        {

            spinning = false;

            if (sprintCheck)
            {
                afterSpinSprint = true;
            }
            else
            {
                speed = maxSpeed;
            }
        }

        #endregion

        #region Right spin

        // Samma sak som vänster men ändrar att den åker runt en annan position
        if (Input.GetMouseButtonDown(1))
        {
            myRigidbody.linearVelocity = Vector2.zero;

            spinAroundPos = rightSpinPos.transform.position; // Skillnad här

            afterSpinSprint = false;
            gotWhereToSprint = false;

        }
        if (Input.GetMouseButton(1))
        {
            speed = 0;

            lookWhenSpining = transform.position - spinAroundPos;

            float angle = Mathf.Atan2(lookWhenSpining.y, lookWhenSpining.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, -lookSpeed * Time.deltaTime);

            spinning = true;
            transform.RotateAround(spinAroundPos, Vector3.forward, -spinSpeed * Time.deltaTime);

        }
        if (Input.GetMouseButtonUp(1))
        {

            spinning = false;

            if (sprintCheck)
            {
                afterSpinSprint = true;
            }
            else
            {
                speed = maxSpeed;
            }
        }

        #endregion

    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        // Träffar en väg medans man snurrar, detta behövs för annars går man igenom vägen
        if(collision.gameObject.layer == 3 && spinning || sprinting)
        {
            StartCoroutine(WallCrash());

        }
    }

    IEnumerator WallCrash()
    {
        spinning = false;
        sprinting = false;
        stunned = true;
        myRigidbody.angularVelocity = 0f;
        speed = 0;
        movementInput = Vector2.zero;

        myRigidbody.AddForce(-transform.up * wallCrashSpeed);


        yield return new WaitForSeconds(0.3f);


        speed = maxSpeed;
        lockMoveinputParent = false;
        lockRotationParent = false;
        stunned = false;
        gotWhereToSprint = false;
        movementInput = inactiveMovementInput;
    }

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
}
