using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class DoubleSwordCharacter : Player1
{
    [Header("Double Sword Specifics")]

    [SerializeField] GameObject sword1Hitbox;
    [SerializeField] GameObject sword2Hitbox;
    [SerializeField] GameObject fist;

    [SerializeField] GameObject throwingSword;

    int numberOfSwordsHeld = 2;

    #region Sprint Variables

    [Header("Sprint")]

    [SerializeField] float sprintSpeedMultiplier = 1.25f;
    float sprintSpeed = 25f;
    [SerializeField] float wallCrashSpeed = 50f;

    bool sprinting = false;
    bool sprintCheck = false;
    bool gotWhereToSprint = false;
    bool stunned = false;

    float afterSprintDeceleraton;
    [SerializeField] float timeToDecelerate = 0.7f;
    float timeToDecelerateBase;


    #endregion

    #region Spin

    [Header("Spin")]

    [SerializeField] float spinSpeed = 70f;
    [SerializeField] float lookSpeed = 50;
    [SerializeField] float afterSpinPush = 1000f;

    bool spinning = false;
    bool leftSpinStart = false;
    bool rightSpinStart = false;
    bool afterSpinSprint = false;
    bool afterSpinBoost = false;

    [SerializeField] GameObject leftSpinPos;
    [SerializeField] GameObject rightSpinPos;

    Vector3 spinAroundPos;
    Vector3 lookWhenSpining;

    #endregion

    #region Basic Dodge Variables

    [Header("Dodge")]

    [SerializeField] float dodgeSpeed = 5;
    [SerializeField] float dodgeTime = 0.5f;
    [SerializeField] float dodgeRecoveryTime = 0.1f;

    GameObject dodgeCollider = null;


    #endregion

    SpriteRenderer spriteRenderer = null; // Temporärt för Debbuging

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();

        sprintSpeed = speed * sprintSpeedMultiplier;

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
            // !spining  Så den inte saktar ned när mans snurrar runt
            // !afterSpinBoost  Så man får tillbaka kontrolen om man inte ska sprinta efter spin
            if (sprinting && !spinning && !afterSpinBoost)
            {
                speed -= afterSprintDeceleraton * Time.deltaTime;

                timeToDecelerate -= Time.deltaTime;

                ModeChangeVisuall(Color.grey);

                stunned = true;

                if (timeToDecelerate <= 0)
                {

                    timeToDecelerate = timeToDecelerateBase;
                    // Dividerar med 2 så att man får kontroll när man har stannat med hälften av farten
                    afterSprintDeceleraton = (sprintSpeed / 2) / timeToDecelerateBase;

                    sprinting = false;
                    gotWhereToSprint = false;
                    stunned = false;

                    lockRotationParent = false;
                    lockMoveinputParent = false;
                    movementInput = inactiveMovementInput;

                    speed = maxSpeed;

                    NormalModeVisuall();

                }
            }
        }

        SprintCheck();

        if (dodgeLock || sprinting || stunned)
        {
            attacking = false;

            if (sprinting && !stunned)
            {

                // Gör så att man har möjligheten att spina
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

    #region Throw Sword

    void ThrowSword()
    {

        //numberOfSwordsHeld--;

        GameObject thrownSwordObject = Instantiate(throwingSword);
        thrownSwordObject.transform.position = transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        thrownSwordObject.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

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

            #region Once

            // Så den får vart den ska gå 1 gång
            // Körs 1 gång
            // Körs inte efter spin
            if (!gotWhereToSprint)
            {
                gotWhereToSprint = true;
                afterSpinBoost = false;

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

                ModeChangeVisuall(Color.cyan);
            }

            #endregion

            speed = sprintSpeed;

            // Framtida saker
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
  
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {

            }

        }

    }

    #endregion

    #region Spin

    void Spin()
    {



        #region Left spin




        // Trycker ned för första gången
        // Ger åt vilket håll den ska snurra
        // !rightSpinStart så den inte kan byta håll man snurrar åt
        if (Input.GetMouseButtonDown(0) && !rightSpinStart)
        {
            Debug.Log("LEFT!");
            ModeChangeVisuall(Color.green);

            myRigidbody.linearVelocity = Vector2.zero;

            spinAroundPos = leftSpinPos.transform.position;

            spinning = true;
            leftSpinStart = true;
            afterSpinSprint = false;
            gotWhereToSprint = false;

        }
        // Håller ned den
        // Gör så att den snurrar
        if (Input.GetMouseButton(0) && leftSpinStart)
        {
            speed = 0;
            Debug.Log("LEFT! Continius");
            lookWhenSpining = transform.position - spinAroundPos;

            float angle = Mathf.Atan2(lookWhenSpining.y, lookWhenSpining.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);

            transform.RotateAround(spinAroundPos, Vector3.forward, spinSpeed * Time.deltaTime);

        }
        // Släpper 
        // Vad den ska göra (om man ska fortsätta springa eller ej)
        if (Input.GetMouseButtonUp(0) && leftSpinStart)
        {
            Debug.Log("End RIGHT!");

            leftSpinStart = false;
            spinning = false;

            if (sprintCheck)
            {
                afterSpinSprint = true;
                ModeChangeVisuall(Color.cyan);
            }
            else
            {
                StartCoroutine(AfterSpinSpeedBoost());
            }
        }



        #endregion

        #region Right spin


        // Trycker ned för första gången
        // Ger åt vilket håll den ska snurra
        // !leftSpinStart så den inte kan byta håll man snurrar åt
        if (Input.GetMouseButtonDown(1) && !leftSpinStart)
        {
            Debug.Log("RIGHT!");
            ModeChangeVisuall(Color.green);

            myRigidbody.linearVelocity = Vector2.zero;

            spinAroundPos = rightSpinPos.transform.position; // Skillnad här

            spinning = true;
            rightSpinStart = true; // Skillnad här
            afterSpinSprint = false;
            gotWhereToSprint = false;

        }
        // Håller ned den
        // Gör så att den snurrar
        if (Input.GetMouseButton(1) && rightSpinStart)
        {
            Debug.Log("RIGHT! Continius");
            speed = 0;

            lookWhenSpining = transform.position - spinAroundPos;

            float angle = Mathf.Atan2(lookWhenSpining.y, lookWhenSpining.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, -lookSpeed * Time.deltaTime);

            transform.RotateAround(spinAroundPos, Vector3.forward, -spinSpeed * Time.deltaTime);

        }
        // Släpper 
        // Vad den ska göra (om man ska fortsätta springa eller ej)
        if (Input.GetMouseButtonUp(1) && rightSpinStart)
        {
            Debug.Log("End RIGHT!");
            rightSpinStart = false; // Skillnad här
            spinning = false;

            if (sprintCheck)
            {
                afterSpinSprint = true;
                ModeChangeVisuall(Color.cyan);
            }
            else
            {
                StartCoroutine(AfterSpinSpeedBoost());
            }
        }


        #endregion

    }

    IEnumerator AfterSpinSpeedBoost()
    {
        speed = maxSpeed * 1.3f;
        myRigidbody.AddForce(transform.up * afterSpinPush);
        afterSpinBoost = true;

        gotWhereToSprint = false;
        lockRotationParent = false;
        lockMoveinputParent = false;
        movementInput = inactiveMovementInput;
        sprinting = false;

        ModeChangeVisuall(Color.yellow);
        yield return new WaitForSeconds(2);
        NormalModeVisuall();


        // För man kan börja springa under väntan till detta så då om man sätter afterSpinBoost till falsk så förstör den inte
        if (afterSpinBoost)
        {
            afterSpinBoost = false;
            speed = maxSpeed;
        }
    }

    #endregion

    #region Collisions

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        // Träffar en väg medans man snurrar, detta behövs för annars går man igenom vägen
        if(sprinting)
        {
            StartCoroutine(WallCrash());

        }
    }

    public override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionStay2D(collision);

        if(collision.gameObject.layer == 3 && spinning)
        {
            StartCoroutine(WallCrash());

        }

    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.gameObject.layer == 6 && spinning)
        {

            collision.gameObject.GetComponent<Door>().ArtificialPush(myCollider, 15);

        }

    }

    IEnumerator WallCrash()
    {
        spinning = false;
        leftSpinStart = false;
        rightSpinStart = false;
        sprinting = false;
        stunned = true;
        spinAroundPos = transform.position;
        myRigidbody.angularVelocity = 0f;
        speed = 0;
        movementInput = Vector2.zero;

        myRigidbody.AddForce(-transform.up * wallCrashSpeed);

        ModeChangeVisuall(Color.grey);
        yield return new WaitForSeconds(0.3f);
        NormalModeVisuall();

        speed = maxSpeed;
        lockMoveinputParent = false;
        lockRotationParent = false;
        stunned = false;
        gotWhereToSprint = false;
        movementInput = inactiveMovementInput;
        timeToDecelerate = timeToDecelerateBase;
        afterSprintDeceleraton = (sprintSpeed / 2) / timeToDecelerateBase;

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        lookDirection = mousePos - myRigidbody.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90);

        transform.rotation = targetRotation;
    }

    #endregion

    #region basic Dodge

    IEnumerator basicDodge()
    {
        dodgeLock = true;
        attackObject.SetActive(false);

        bool standingStillDodge = false;

        // Om man står stilla så ska man dodga dit man kollar
        if (movementInput == Vector2.zero) 
        {
            standingStillDodge = true;

            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            movementInput = mousePos - myRigidbody.position;

        }

        myRigidbody.linearDamping = 0;

        playerVelocity = movementInput.normalized * dodgeSpeed;
        myRigidbody.linearVelocity += playerVelocity;

        dodgeCollider.SetActive(true);
        myCollider.enabled = false;


        yield return new WaitForSeconds(dodgeTime / 2);


        dodgeCollider.SetActive(false);
        myCollider.enabled = true;


        yield return new WaitForSeconds(dodgeTime / 2);

        myRigidbody.linearDamping = 40;

        if (standingStillDodge)
        {
            movementInput = inactiveMovementInput;
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

    #region Debugging help

    void ModeChangeVisuall(Color whatColour)
    {

        spriteRenderer.color = whatColour;

    }

    void NormalModeVisuall()
    {

        spriteRenderer.color = Color.white;

    }

    #endregion
}
