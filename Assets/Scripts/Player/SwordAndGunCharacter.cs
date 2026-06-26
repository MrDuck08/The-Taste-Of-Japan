using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SwordAndGunCharacter : Player1
{

    [Header("S&G Specifics")]

    [SerializeField] GameObject stanceAttackObject;

    bool attackStance = false;

    #region Basic Dodge Variables

    [Header("Dodge")]

    [SerializeField] float dodgeSpeed = 5;
    [SerializeField] float dodgeTime = 0.5f;
    [SerializeField] float dodgeRecoveryTime = 0.1f;

    GameObject dodgeCollider = null;


    #endregion

    #region Revolver Variables

    [Header("Revolver")]

    [SerializeField] TrailRenderer bulletTrail;

    [SerializeField] Vector2 bulletSpawnPos;
    [SerializeField] int bullets = 6;
    [SerializeField] LayerMask bulletIgnoreLayerMask;

    int maxBullets;

    #endregion

    #region Harmony Variables

    [Header("Harmony")]

    [SerializeField] float decayTimeForHarmony = 7f;
    float decayTimeForHarmonyBase;
    float maxTimeInHarmony = 10f;
    float maxTimeInHarmonyBase;

    bool killWithRevolver = false;
    bool killWithCharge = false;
    bool inHarmony = false;

    bool harmonyDoorHit = false;
    Vector3 harmonyDoorHitPos = Vector3.zero;

    [SerializeField] GameObject rushAttackObject;

    [SerializeField] float rushSpeed = 40f;
    bool rushing = false;
    bool rushAttackHasStarted = false;
    Vector2 pointToRushTo = Vector2.zero;

    [SerializeField] GameObject fadeEffectObj;
    float harmonyFadeEffectTime;
    float maxHarmonyFadeEffectTime = 0.3f;

    #endregion

    [SerializeField] int stanceAttack = 2;
    int maxStanceAttack;

    #region UI Variables

    [Header("UI")]

    [SerializeField] TextMeshProUGUI bulletText;
    [SerializeField] TextMeshProUGUI ChargeText;

    [SerializeField] Image bulletKillImage;
    [SerializeField] Image ChargeKillImage;

    #endregion

    ScreenShake screenShake;
    PlayerHealth playerHealth;

    public override void Start()
    {
        base.Start();

        maxBullets = bullets;
        maxStanceAttack = stanceAttack;
        decayTimeForHarmonyBase = decayTimeForHarmony;
        maxTimeInHarmonyBase = maxTimeInHarmony;
        harmonyFadeEffectTime = maxHarmonyFadeEffectTime;

        bulletText.text = bullets.ToString();
        ChargeText.text = stanceAttack.ToString();
        bulletKillImage.gameObject.SetActive(false);
        ChargeKillImage.gameObject.SetActive(false);

        dodgeCollider = transform.Find("DodgeCollider").gameObject;

        screenShake = FindAnyObjectByType<ScreenShake>();

        playerHealth = GetComponent<PlayerHealth>();
    }

    public override void Update()
    {
        base.Update();
        Debug.Log(Time.fixedDeltaTime);
        #region Rush

        if (rushing && !rushAttackHasStarted)
        {

            // BARA VISUELLT
            // Gör en after image med mellanrum

            //harmonyFadeEffectTime -= 0.1f;
            harmonyFadeEffectTime -= 20 * Time.deltaTime;
            if (harmonyFadeEffectTime < 0)
            {

                harmonyFadeEffectTime = maxHarmonyFadeEffectTime;
                GameObject fadeObj = Instantiate(fadeEffectObj);
                fadeObj.GetComponent<FadeEffect>().InstanciateInfo(gameObject.GetComponent<SpriteRenderer>(), transform, new Color32(170, 170, 170, 255));
            }

            //Åker mot position
            transform.position = Vector2.MoveTowards(transform.position, pointToRushTo, rushSpeed * Time.deltaTime);


            // 1.7 Så den stannar innan den kommer fram
            if(Vector2.Distance(transform.position, pointToRushTo) < 1.7f)
            {
                StartCoroutine(RushAttack());

                harmonyFadeEffectTime = maxHarmonyFadeEffectTime;
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

        #region Harmony

        if (killWithCharge && killWithRevolver)
        {
            // Enter Harmony 
            if (Input.GetKeyDown(KeyCode.LeftControl) && !dodgeLock)
            {
                inHarmony = true;

                Time.timeScale = 0.1f;
                // Måste ändra fixedDeltaTime annars laggar allting
                Time.fixedDeltaTime = 0.016F * Time.timeScale;

                bulletKillImage.fillAmount = 1;
                ChargeKillImage.fillAmount = 1;

                audioManager.PlayHarmonySounds();

                ThingsToFalse();
            }

            // In harmony now
            if (inHarmony)
            {
                #region Rush

                // Sword attack harmony
                if (Input.GetMouseButtonDown(0) && !dodgeLock)
                {
                    float clickDistance = Vector2.Distance(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

                    // Behöver manuelt kolla om man träffar en dörr eftersom man åker för snabbt.
                    RaycastHit2D doorCheckHit = Physics2D.Raycast(transform.position, lookDirection, clickDistance, ~bulletIgnoreLayerMask);

                    // Tar bort och lägger till door layer så man kan åka igenom den. 
                    bulletIgnoreLayerMask |= (1 << LayerMask.NameToLayer("Door"));
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, lookDirection, clickDistance, ~bulletIgnoreLayerMask);
                    bulletIgnoreLayerMask &= ~(1 << LayerMask.NameToLayer("Door"));

                    // Objekt var för långt bort
                    if (hit.point == Vector2.zero)
                    {
                        pointToRushTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    }
                    else
                    {
                        pointToRushTo = hit.point;
                    }

                    if (hit)
                    {
                        // Dörr layer
                        if (doorCheckHit.transform.gameObject.layer == 6)
                        {

                            harmonyDoorHit = true;

                            harmonyDoorHitPos = transform.position;

                        }
                    }

                    rushing = true;

                    dodgeLock = true;
                    lockRotationParent = true;

                    transform.LookAt(pointToRushTo);

                    playerHealth.invincible = true;

                    audioManager.StopHarmonySounds();
                    ResetHarmony();
                }

                #endregion

                #region Revolver

                // Revolver attack harmony
                if (Input.GetMouseButtonDown(1) && !dodgeLock)
                {

                    RaycastHit2D hit = Physics2D.Raycast(transform.position, lookDirection, 1337, ~bulletIgnoreLayerMask);

                    TrailRenderer trail = Instantiate(bulletTrail);
                    trail.transform.position = transform.position;

                    //SpawnTrail(trail, hit);
                    trail.GetComponent<BulletTrailScript>().MoveAndFadeTrail(trail.transform.position, hit.point);

                    screenShake.ScreenRecoil(0.1f, 0.3f);

                    audioManager.StopHarmonySounds();
                    ResetHarmony();

                    // Sätter den under reset så att man kan börja bygga harmoni av denna attack
                    if (hit.transform.tag == "Enemy")
                    {
                        hit.transform.gameObject.GetComponent<EnemyHealth>().TakeDamage(1, 2);
                    }
                }

                #endregion

                // Gör en after image med mellanrum
                harmonyFadeEffectTime -= Time.unscaledDeltaTime;
                if(harmonyFadeEffectTime < 0 )
                {
                    harmonyFadeEffectTime = maxHarmonyFadeEffectTime;
                    GameObject fadeObj = Instantiate(fadeEffectObj);
                    fadeObj.GetComponent<FadeEffect>().InstanciateInfo(gameObject.GetComponent<SpriteRenderer>(), transform, new Color32(170, 170, 170,255));
                }

                maxTimeInHarmony -= Time.unscaledDeltaTime;

                bulletKillImage.fillAmount = maxTimeInHarmony / maxTimeInHarmonyBase;
                ChargeKillImage.fillAmount = maxTimeInHarmony / maxTimeInHarmonyBase;

                if (maxTimeInHarmony < 0)
                {
                    audioManager.StopHarmonySounds();
                    ResetHarmony();
                }

            }
            else // Har inte gått in i Harmoni ännu
            {
                decayTimeForHarmony -= Time.deltaTime;

                bulletKillImage.fillAmount = decayTimeForHarmony / decayTimeForHarmonyBase;
                ChargeKillImage.fillAmount = decayTimeForHarmony / decayTimeForHarmonyBase;

                if (decayTimeForHarmony < 0)
                {
                    ResetHarmony();
                }
            }

        }

        if (inHarmony) { return; }

        #endregion

        if (Input.GetKeyDown(KeyCode.Space))
        {

            StartCoroutine(basicDodge());


        }

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
                    StartCoroutine(StanceAttack());
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

                audioManager.PlayShellSound(transform.position);
                audioManager.PlayShootSound(transform.position);
                audioManager.PlayRevolverClickSound(transform.position);

                screenShake.ScreenRecoil(0.1f, 0.3f);

                bulletText.text = bullets.ToString();

                if (hit.transform.tag == "Enemy")
                {
                    hit.transform.gameObject.GetComponent<EnemyHealth>().TakeDamage(1, 2);
                }
                else
                {

                    audioManager.PlayBulletHitWall(hit.point);

                }

                if(hit.transform.gameObject.layer == 9)
                {

                    hit.transform.GetComponentInParent<ShieldEnemy>().ShieldRemove();

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

                audioManager.ChangeWalkingPtch(gameObject, 0.3f);

                speed = maxSpeed/10;

                attackStance = true;
                lookAroundSpeed = 1;

            }

        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && stanceAttack > 0)
        {

            if (attacking == false)
            {

                //cameraScript.ZoomOutAgain(0.1f);

                audioManager.RevertWalkingPitch(gameObject);

                speed = maxSpeed;
                lookAroundSpeed = maxLookAroundSpeed;

                attackStance = false;
                lockRotationParent = false;

            }

        }

        #endregion

    }

    #region Stance Attack

    IEnumerator StanceAttack()
    {
        //cameraScript.ZoomOutAgain(0.1f);

        attacking = true;

        stanceAttack--;

        ChargeText.text = stanceAttack.ToString();

        stanceAttackObject.SetActive(true);

        speed = 0;

        audioManager.PlayUnsheatheSound(transform.position);

        yield return new WaitForSeconds(0.05f);
        audioManager.PlayPlayerChargeSlashSound(transform.position); // Mini paus för att spela ljud
        yield return new WaitForSeconds(0.35f);

        stanceAttackObject.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        speed = maxSpeed;
        lookAroundSpeed = maxLookAroundSpeed;

        attackStance = false;
        attacking = false;
        lockRotationParent = false;
        audioManager.RevertWalkingPitch(gameObject);


    }

    #endregion

    #region basic Dodge

    IEnumerator basicDodge()
    {
        dodgeLock = true;
        attackObject.SetActive(false);

        audioManager.StopWalkingSound(gameObject);
        audioManager.PlayDashSound();

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
        lookAroundSpeed = maxLookAroundSpeed;

        if(movementInput != Vector2.zero && !lockMoveinputParent)
        {
            audioManager.playWalkingSound(transform.position, gameObject);
        }


        yield return new WaitForSeconds(dodgeRecoveryTime);


        speed = maxSpeed;

    }

    #endregion

    IEnumerator RushAttack()
    {

        rushAttackObject.SetActive(true);

        attacking = true;
        rushAttackHasStarted = true;

        yield return new WaitForSeconds(0.5f);

        rushAttackObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        playerHealth.invincible = false;
        rushing = false;
        rushAttackHasStarted = false;
        attacking = false;

    }

    public void RechargeBullets()
    {
        killWithCharge = true;
        ChargeKillImage.gameObject.SetActive(true);

        if (bullets != maxBullets)
        {
            bullets++;

            bulletText.text = bullets.ToString();
        }

    }

    public void RechargeStance()
    {
        killWithRevolver = true;
        bulletKillImage.gameObject.SetActive(true);

        if (stanceAttack != maxStanceAttack)
        {
            stanceAttack++;

            ChargeText.text = stanceAttack.ToString();
        }

    }

    void ResetHarmony()
    {
        

        decayTimeForHarmony = decayTimeForHarmonyBase;
        maxTimeInHarmony = maxTimeInHarmonyBase;
        harmonyFadeEffectTime = maxHarmonyFadeEffectTime;

        killWithCharge = false;
        killWithRevolver = false;
        inHarmony = false;

        bulletKillImage.fillAmount = 1;
        ChargeKillImage.fillAmount = 1;
        bulletKillImage.gameObject.SetActive(false);
        ChargeKillImage.gameObject.SetActive(false);


        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.016F;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        // Dörr Layer, behövs för att i harmony Rush så åker man för snabbt för att dörren ska få rätt pos
        if(collision.transform.gameObject.layer == 6 && harmonyDoorHit)
        {
            harmonyDoorHit = false;

            collision.GetComponent<Door>().ArtificialPush(harmonyDoorHitPos, 15);

        }

    }


    #region Reset

    void ThingsToFalse()
    {

        attacking = false;

        attackStance = false;
        lockRotationParent = false;
        stanceAttackObject.SetActive(false);

        speed = maxSpeed;
        lookAroundSpeed = maxLookAroundSpeed;

        basicAttacking = false;
        attackObject.SetActive(false);

        audioManager.RevertWalkingPitch(gameObject);

    }

    #endregion

}
