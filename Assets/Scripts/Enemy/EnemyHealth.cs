using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField] int health;
    public LayerMask obsticleCheck;
    [SerializeField] GameObject fadeEffectObj;
    [SerializeField] GameObject objBloodAnimation;
    [SerializeField] ParticleSystem bloodParticles;
    [SerializeField] List<GameObject> bloodSpreadObject = new List<GameObject>();

    bool startDoubleCheck = false;
    float doubleCheckTime;
    float maxDoubleCheckTime = 1f;

    ScreenShake screenShake;
    SwordAndGunCharacter swordAndGun;
    Player1 player1;

    AudioManager audioManager;

    private void Start()
    {
        screenShake = FindAnyObjectByType<ScreenShake>();
        swordAndGun = FindAnyObjectByType<SwordAndGunCharacter>();
        player1 = FindAnyObjectByType<Player1>();
        audioManager = FindAnyObjectByType<AudioManager>();

        doubleCheckTime = maxDoubleCheckTime;
    }

    private void Update()
    {
        if (startDoubleCheck)
        {
            doubleCheckTime -= Time.deltaTime;

            if (doubleCheckTime < 0)
            {
                startDoubleCheck = false;
                doubleCheckTime = maxDoubleCheckTime;

            }


            Vector2 direction = player1.transform.position - transform.position;
            float lenght = Vector2.Distance(player1.transform.position, transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, lenght, obsticleCheck);

            if (!hit)
            {
                TakeDamage(1, 1);
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("PlayerAttack") || collision.transform.CompareTag("StanceAttack"))
        {
            Vector2 direction = player1.transform.position - transform.position;
            float lenght = Vector2.Distance(player1.transform.position, transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, lenght, obsticleCheck);

            if (!hit)
            {
                TakeDamage(1, 1);
            }
            else if(hit.transform.name != "Sheild") // Dubbelkollar så länge spelaren inte träffade en sköld (annars kan man springa igenom och döda)
            {
                doubleCheckTime = maxDoubleCheckTime;
                startDoubleCheck = true;
            }


        }

        //Träffad av dörr, Kollar också hur snabb dörren var
        if (collision.gameObject.layer == 6 && collision.GetComponent<Door>().playerPushedDoor == true && collision.GetComponent<Rigidbody2D>().linearVelocity.magnitude > 12)
        {
            #region For Knockback, (Removed For Now)

            //// Får vinkeln åt Vart den ska åka
            //Vector2 direction = transform.position - Door.posWhenOpened;

            //StartCoroutine(enemyBase.BeStunned(direction.normalized));

            #endregion

            TakeDamage(1, 0);

        }
    }

    public void TakeDamage(int takeDamage, int whatTypeOfAttack)
    {
        // whatTypeOfAttack
        // 0 = Door hit
        // 1 = S&R Basic Hit
        // 2 = S&R Revolver Hit

        health -= takeDamage;

        startDoubleCheck = false;

        if (health <= 0)
        {

            audioManager.PlayEnemyDeathSound(transform.position);

            switch (whatTypeOfAttack)
            {

                case 1:

                    if(swordAndGun != null)
                    {
                        swordAndGun.RechargeBullets();
                    }

                    break;

                case 2:

                    swordAndGun.RechargeStance();
                    audioManager.PlayEnemyBulletDeathSound(transform.position);

                    break;

            }

            GameObject fadeObj = Instantiate(fadeEffectObj);
            SpriteRenderer playerSpriterenderer = player1.gameObject.GetComponent<SpriteRenderer>();
            fadeObj.GetComponent<FadeEffect>().InstanciateInfo(playerSpriterenderer, player1.transform, playerSpriterenderer.color);

            // Hur många blood animationer det finns
            int whatBloodAnimation = Random.Range(0, 3);

            GameObject bloodAnimationObj = Instantiate(objBloodAnimation);
            bloodAnimationObj.transform.position = transform.position;
            bloodAnimationObj.GetComponent<Animator>().SetInteger("WhatAnimation", whatBloodAnimation);

            ParticleSystem spawnedBloodParticles = Instantiate(bloodParticles);
            spawnedBloodParticles.transform.position = transform.position;
            bloodParticles.Play();
            float totalDuration = bloodParticles.duration + bloodParticles.startLifetime;
            Destroy(spawnedBloodParticles.gameObject, totalDuration);

            int howManySpawns = Random.Range(6, 8);

            while (howManySpawns > 0) 
            {
                int whatBloodSpread = Random.Range(0, bloodSpreadObject.Count);

                GameObject bloodSpreadObj = Instantiate(bloodSpreadObject[whatBloodSpread]);
                bloodSpreadObj.transform.position = transform.position;

                Vector2 dir = Vector2.zero;
                float angle = 0;
                float spd = Random.Range(20, 25);

                if (whatTypeOfAttack == 1)
                {
                    dir = Random.onUnitSphere;

                    angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    bloodSpreadObj.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
                }
                else
                {
                    dir = transform.position - player1.transform.position;
                    angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                    float randomOffset = Random.Range(-30f, 30f);
                    bloodSpreadObj.transform.rotation = Quaternion.Euler(0, 0, angle - 90 + randomOffset);

                    spd *= 1.5f;
                    dir = Quaternion.Euler(0, 0, randomOffset) * dir.normalized;
                }

                bloodSpreadObj.GetComponent<Rigidbody2D>().linearVelocity = dir * spd;

                howManySpawns--; 
            }

            screenShake.TriggerShakeTime(0.05f, 0.25f, false);
            Destroy(gameObject);

        }

    }

}
