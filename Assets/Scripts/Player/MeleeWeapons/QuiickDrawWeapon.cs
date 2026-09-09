using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuiickDrawWeapon : MeleeWeaponsBase
{
    bool lookingForDestination = false;
    bool harmonyChargingToPos = false;

    List<Vector2> posToGoToList = new List<Vector2>();

    [SerializeField] GameObject tempCameraFollowObj;
    GameObject tempObj;

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

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        #region Harmony

        if (lookingForDestination && Input.GetMouseButtonDown(0))
        {

            lookingForDestination = false;

            harmonyAttackObj.SetActive(true);


            float clickDistance = Vector2.Distance(tempObj.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - tempObj.transform.position;

            // Behöver manuelt kolla om man träffar en dörr eftersom man åker för snabbt.
            RaycastHit2D doorCheckHit = Physics2D.Raycast(tempObj.transform.position, dir, clickDistance, doorLayerMask);

            // Tar bort och lägger till door layer så man kan åka igenom den. 
            // Lägger till
            bulletIgnoreLayerMask |= (1 << LayerMask.NameToLayer("Door"));
            RaycastHit2D hit = Physics2D.Raycast(tempObj.transform.position, dir, clickDistance, ~bulletIgnoreLayerMask);
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


            posToGoToList.Add(pointToRushTo);

            harmonyChargingToPos = true;

            cam.GoBackToPlayer();

            Time.timeScale = 1.0f;
        }

        if (harmonyChargingToPos)
        {

            harmonyFadeEffectTime -= 20 * Time.deltaTime;
            if (harmonyFadeEffectTime < 0)
            {

                harmonyFadeEffectTime = maxHarmonyFadeEffectTime;
                GameObject fadeObj = Instantiate(fadeEffectObj);
                fadeObj.GetComponent<FadeEffect>().InstanciateInfo(player.gameObject.GetComponent<SpriteRenderer>(), player.transform, new Color32(170, 170, 170, 255));
            }


            //Åker mot position
            player.transform.position = Vector2.MoveTowards(player.transform.position, posToGoToList[0], rushSpeed * Time.deltaTime);


            Vector2 lookDirection = posToGoToList[0] - new Vector2(player.transform.position.x, player.transform.position.y);
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

            player.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);


            // 1.7 Så den stannar innan den kommer fram
            if (Vector2.Distance(player.transform.position, posToGoToList[0]) < 0.5f)
            {


                posToGoToList.RemoveAt(0);

                if (posToGoToList.Count <= 0)
                {

                    rushing = false;
                    harmonyFadeEffectTime = maxHarmonyFadeEffectTime;
                    player.dodgeLock = false;
                    player.lockMoveinputParent = false;
                    player.lockRotationParent = false;
                    playerHealth.invincible = false;
                    harmonyChargingToPos = false;
                    harmonyAttackObj.SetActive(false);
                    Destroy(tempObj);

  
                }
            }

        }

        #endregion

        if (lookingForDestination || harmonyChargingToPos) { return; }

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

            HarmonyFindWhereToGo();
        }

    }

    #region Basic Attack

    IEnumerator BasicAttackRoutine()
    {

        basicAttackObj.SetActive(true);

        player.attacking = true;
        player.basicAttacking = true;

        audioManager.PlayPlayerSlashSound(transform.position);

        yield return new WaitForSeconds(0.15f);

        basicAttackObj.SetActive(false);

        yield return new WaitForSeconds(0.05f);

        player.attacking = false;
        player.basicAttacking = false;

    }

    #endregion


    #region Stance Attack

    IEnumerator StanceAttackRoutine()
    {

        //cameraScript.ZoomOutAgain(0.1f);

        player.attacking = true;
        player.dodgeLock = true;
        player.lockRotationParent = true;

        stanceAttackObj.SetActive(true);


        Vector2 mousePos = player.cam.ScreenToWorldPoint(Input.mousePosition);
        player.movementInput = mousePos - player.myRigidbody.position;


        Vector2 lookDirection = mousePos - new Vector2(player.transform.position.x, player.transform.position.y);
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        player.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);


        player.myRigidbody.linearDamping = 0;

        player.playerVelocity = player.movementInput.normalized * 40; // Dodge Speed
        player.myRigidbody.linearVelocity += player.playerVelocity;



        audioManager.PlayUnsheatheSound(transform.position);

        yield return new WaitForSeconds(0.05f);
        audioManager.PlayPlayerChargeSlashSound(transform.position); // Mini paus för att spela ljud
        yield return new WaitForSeconds(0.15f);

        stanceAttackObj.SetActive(false);
        player.dodgeLock = false;

        player.myRigidbody.linearDamping = 40;
        player.movementInput = player.inactiveMovementInput;

        player.speed = player.maxSpeed / 2;
        player.lookAroundSpeed = player.maxLookAroundSpeed;


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

    void HarmonyFindWhereToGo()
    {
        


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
            //pointToRushTo = hit.point;

            // Måste kalkulera om det med mindre distance så den inte fastnar i en väg
            pointToRushTo = transform.position + (Vector3)player.lookDirection.normalized * (hit.distance - 0.3f);
        }

        // Dörr layer
        if (doorCheckHit)
        {

            playerSpesifics.harmonyDoorHit = true;

            playerSpesifics.harmonyDoorHitPos = transform.position;

        }


        tempObj = Instantiate(tempCameraFollowObj);

        tempObj.transform.position = pointToRushTo;

        tempObj.SetActive(true);

        posToGoToList.Add(tempObj.transform.position);

        cam.ChangeTargetCam(tempObj, 3);

        lookingForDestination = true;

    }

    #endregion
}
