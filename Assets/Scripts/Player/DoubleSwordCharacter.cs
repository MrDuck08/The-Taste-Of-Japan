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

    float afterSprintDeceleraton;
    [SerializeField] float timeToDecelerate = 0.7f;
    float timeToDecelerateBase;


    #endregion

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
            if (sprinting)
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

        // Kollar Om Man Kan Sprinta
        if (!attacking && !dodgeLock && sprintCheck)
        {

            sprinting = true;

            lockRotationParent = true;
            lockMoveinputParent = true;

            if(movementInput == Vector2.zero)
            {
                movementInput = lookDirection.normalized;
            }
            else
            {
                float angle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);

            }

            speed = sprintSpeed;

        }

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
}
