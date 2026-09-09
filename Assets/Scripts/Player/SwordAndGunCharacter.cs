using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwordAndGunCharacter : Player1
{

    [Header("S&G Specifics")]

    [HideInInspector] public bool attackStance = false;

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

    [HideInInspector] public bool harmonyDoorHit = false;
    [HideInInspector] public Vector3 harmonyDoorHitPos = Vector3.zero;



    [SerializeField] GameObject fadeEffectObj;
    float harmonyFadeEffectTime;
    float maxHarmonyFadeEffectTime = 0.3f;

    #endregion

    [Header("Stance")]

    [SerializeField] int stanceAttack = 2;
    [SerializeField] int stanceSlow = 4;
    [SerializeField] float stanceLookSpeed = 1;
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



        #region Move Lock

        if (PauseScript.pause)
        {
            return;
        }

        if (dodgeLock)
        {
            attackStance = false;
            attacking = false;
            lockRotationParent = false;

            //meleeWeapon.stanceAttackObj.SetActive(false);

            speed = maxSpeed;

            return;

        }

        #endregion

        #region Harmony

        if (killWithCharge && killWithRevolver)
        {
            // Enter Harmony 
            if ((Input.GetKeyDown(KeyCode.E) || (Input.GetKeyDown(KeyCode.LeftControl)) && !dodgeLock))
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

                    // Aktiverar attacken i ens "meleeWeapon"
                    meleeWeapon.HarmonyAttack();


                    // Resetar variabler
           

                    dodgeLock = true;
                    lockRotationParent = true;

                    playerHealth.invincible = true;

                    audioManager.StopHarmonySounds();
                    ResetHarmony();
                }

                #endregion

                #region Revolver

                // Revolver attack harmony
                if (Input.GetMouseButtonDown(1) && !dodgeLock)
                {

                    bulletIgnoreLayerMask |= (1 << LayerMask.NameToLayer("Door"));
                    bulletIgnoreLayerMask |= (1 << LayerMask.NameToLayer("Wall"));
                    bulletIgnoreLayerMask |= (1 << LayerMask.NameToLayer("Shield"));
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, lookDirection, 1337, ~bulletIgnoreLayerMask);
                    bulletIgnoreLayerMask &= ~(1 << LayerMask.NameToLayer("Door"));
                    bulletIgnoreLayerMask &= ~(1 << LayerMask.NameToLayer("Wall"));
                    bulletIgnoreLayerMask |= (1 << LayerMask.NameToLayer("Shield"));

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
                        hit.transform.gameObject.GetComponent<EnemyHealth>().TakeDamage(1, 2, transform.position);
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
                    meleeWeapon.BasicAttack();

                }

                if (attackStance == true)
                {
                    meleeWeapon.StanceAttack();


                    stanceAttack--;

                    ChargeText.text = stanceAttack.ToString();
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
                audioManager.PlayShootSound(transform.position, gameObject);
                audioManager.PlayRevolverClickSound(transform.position, gameObject);

                screenShake.ScreenRecoil(0.1f, 0.3f);

                bulletText.text = bullets.ToString();

                if (hit.transform.tag == "Enemy")
                {
                    hit.transform.gameObject.GetComponent<EnemyHealth>().TakeDamage(1, 2, transform.position);
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

                speed = maxSpeed/stanceSlow;

                attackStance = true;
                lookAroundSpeed = stanceLookSpeed;

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


    #region basic Dodge

    IEnumerator basicDodge()
    {
        dodgeLock = true;
        meleeWeapon.basicAttackObj.SetActive(false);

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

    public void ThingsToFalse()
    {

        attacking = false;

        attackStance = false;
        lockRotationParent = false;
        meleeWeapon.stanceAttackObj.SetActive(false);

        speed = maxSpeed;
        lookAroundSpeed = maxLookAroundSpeed;

        basicAttacking = false;
        meleeWeapon.basicAttackObj.SetActive(false);

        audioManager.RevertWalkingPitch(gameObject);

    }

    #endregion

}
