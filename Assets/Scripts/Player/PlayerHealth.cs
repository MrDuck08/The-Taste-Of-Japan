using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] int health = 1;
    [SerializeField] LayerMask obsticleCheck;

    [SerializeField] GameObject objBloodAnimation;
    [SerializeField] ParticleSystem bloodParticles;
    [SerializeField] List<GameObject> bloodSpreadObject = new List<GameObject>();

    public bool invincible = false;

    SceneLoader sceneLoader;
    AudioManager audioManager;

    private void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (invincible) { return; }

        Vector2 direction = collision.transform.position - transform.position;
        float lenght = Vector2.Distance(collision.transform.position, transform.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, lenght, obsticleCheck);

        if (!hit)
        {
            if (collision.transform.CompareTag("EnemyAttack"))
            {

                TakeDamage(1, true, collision.transform);

            }

            if (collision.transform.CompareTag("Explosion"))
            {

                TakeDamage(1, false, collision.transform);

                BloodEffcts(false, collision.transform);
                BloodEffcts(false, collision.transform);
                BloodEffcts(false, collision.transform);

                audioManager.ExplosionKillSound(transform.position);

            }
        }


    }

    public void TakeDamage(int damageTaken, bool randomSpreadBlood, Transform fromWhere)
    {

        if (invincible) { return; }


        health -= damageTaken;

        if (health <= 0)
        {

            sceneLoader = FindAnyObjectByType<SceneLoader>();

            sceneLoader.playerDead = true;

            Camera.main.GetComponent<AudioListener>().enabled = true;

            BloodEffcts(randomSpreadBlood, fromWhere);

            Destroy(gameObject);

        }


    }

    void BloodEffcts(bool randomSpreadBlood, Transform fromWhere)
    {


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

            if (randomSpreadBlood)
            {
                dir = Random.onUnitSphere;

                angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                bloodSpreadObj.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            }
            else
            {
                dir = transform.position - fromWhere.position;
                angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                float randomOffset = Random.Range(-30f, 30f);
                bloodSpreadObj.transform.rotation = Quaternion.Euler(0, 0, angle - 90 + randomOffset);

                spd *= 1.5f;
                dir = Quaternion.Euler(0, 0, randomOffset) * dir.normalized;
            }

            bloodSpreadObj.GetComponent<Rigidbody2D>().linearVelocity = dir * spd;

            howManySpawns--;
        }


    }

}
