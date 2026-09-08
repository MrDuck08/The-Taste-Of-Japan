using System.Collections;
using UnityEngine;

public class SpearWeapon : MeleeWeaponsBase
{
    #region Harmony Variables

    [Header("Harmony")]

    [SerializeField] LayerMask bulletIgnoreLayerMask;
    [SerializeField] LayerMask doorLayerMask;

    [SerializeField] float rushSpeed = 40f;
    bool rushing = false;
    bool rushAttackHasStarted = false;
    Vector2 pointToRushTo = Vector2.zero;

    [SerializeField] GameObject fadeEffectObj;
    float harmonyFadeEffectTime;
    float maxHarmonyFadeEffectTime = 0.3f;

    #endregion


    public override void Start()
    {
        base.Start();


    }


    public override void Update()
    {
        base.Update();

        if (basicAttackNow)
        {
            basicAttackNow = false;

            StartCoroutine(BasicAttackRoutine());
        }

        if (stanceAttackNow)
        {
            stanceAttackNow = false;

            StartCoroutine(StanceAttackRoutine());
        }

        if (harmonyAttackNow)
        {
            harmonyAttackNow = false;

            StartCoroutine(HarmonyFindWhereToGo());
        }

        #region Harmony

        if (rushing)
        {

            // BARA VISUELLT
            // Gör en after image med mellanrum

            //harmonyFadeEffectTime -= 0.1f;
            harmonyFadeEffectTime -= 20 * Time.deltaTime;
            if (harmonyFadeEffectTime < 0)
            {

                harmonyFadeEffectTime = maxHarmonyFadeEffectTime;
                GameObject fadeObj = Instantiate(fadeEffectObj);
                fadeObj.GetComponent<FadeEffect>().InstanciateInfo(player.gameObject.GetComponent<SpriteRenderer>(), player.transform, new Color32(170, 170, 170, 255));
            }


            //Åker mot position
            player.transform.position = Vector2.MoveTowards(player.transform.position, pointToRushTo, rushSpeed * Time.deltaTime);


            // 1.7 Så den stannar innan den kommer fram
            if (Vector2.Distance(player.transform.position, pointToRushTo) < 1.7f)
            {
                StartCoroutine(RushAttack());
                rushing = false;
                harmonyFadeEffectTime = maxHarmonyFadeEffectTime;
                player.dodgeLock = false;
                player.lockRotationParent = false;
            }

        }

        #endregion
    }

    #region Basic Attack

    IEnumerator BasicAttackRoutine()
    {

        basicAttackObj.SetActive(true);

        player.attacking = true;
        player.basicAttacking = true;

        player.lookAroundSpeed = 15f;

        audioManager.PlayPlayerSlashSound(transform.position);


        yield return new WaitForSeconds(0.2f);


        basicAttackObj.SetActive(false);
        player.lookAroundSpeed = player.maxLookAroundSpeed;


        yield return new WaitForSeconds(0.1f);

        player.attacking = false;
        player.basicAttacking = false;

    }

    #endregion

    #region Stance Attack

    IEnumerator StanceAttackRoutine()
    {

        //cameraScript.ZoomOutAgain(0.1f);

        player.attacking = true;

        stanceAttackObj.SetActive(true);

        audioManager.PlayUnsheatheSound(transform.position);

        yield return new WaitForSeconds(0.05f);
        audioManager.PlayPlayerChargeSlashSound(transform.position); // Mini paus för att spela ljud
        yield return new WaitForSeconds(0.35f);

        stanceAttackObj.SetActive(false);



        yield return new WaitForSeconds(0.05f);

        player.speed = player.maxSpeed;
        player.lookAroundSpeed = player.maxLookAroundSpeed;

        playerSpesifics.attackStance = false;
        player.attacking = false;
        player.lockRotationParent = false;
        audioManager.RevertWalkingPitch(gameObject);

    }

    #endregion

    #region Harmony



    IEnumerator HarmonyFindWhereToGo()
    {

        harmonyAttackObj.SetActive(true);


        yield return new WaitForSeconds(0.2f);

        harmonyAttackObj.SetActive(false);

        yield return new WaitForSeconds(0.1f);


        float clickDistance = Vector2.Distance(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

        // Behöver manuelt kolla om man träffar en dörr eftersom man åker för snabbt.
        RaycastHit2D doorCheckHit = Physics2D.Raycast(transform.position, player.lookDirection, clickDistance, doorLayerMask);

        // Tar bort och lägger till door layer så man kan åka igenom den. 
        // Lägger till
        bulletIgnoreLayerMask |= (1 << LayerMask.NameToLayer("Door"));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.lookDirection, clickDistance, ~bulletIgnoreLayerMask);
        // tar bort
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

        // Dörr layer
        if (doorCheckHit)
        {

            playerSpesifics.harmonyDoorHit = true;

            playerSpesifics.harmonyDoorHitPos = transform.position;

        }

        player.transform.LookAt(pointToRushTo);

        rushing = true;

    }

    IEnumerator RushAttack()
    {

        yield return new WaitForSeconds(0.1f);

        harmonyAttackObj.SetActive(true);

        player.attacking = true;
        rushAttackHasStarted = true;

        yield return new WaitForSeconds(0.5f);

        harmonyAttackObj.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        playerHealth.invincible = false;
        rushing = false;
        rushAttackHasStarted = false;
        player.attacking = false;

    }

    #endregion
}
