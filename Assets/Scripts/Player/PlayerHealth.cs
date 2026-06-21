using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] int health = 1;
    [SerializeField] GameObject objBloodAnimation;

    public bool invincible = false;

    SceneLoader sceneLoader;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (invincible) { return; }

        if (collision.transform.CompareTag("EnemyAttack"))
        {

            TakeDamage(1);

        }


    }

    public void TakeDamage(int damageTaken)
    {

        if (invincible) { return; }


        health -= damageTaken;

        if (health <= 0)
        {

            sceneLoader = FindAnyObjectByType<SceneLoader>();

            sceneLoader.playerDead = true;

            Camera.main.GetComponent<AudioListener>().enabled = true;

            // Hur många blood animationer det finns
            int whatBloodAnimation = Random.Range(0, 3);

            GameObject bloodAnimationObj = Instantiate(objBloodAnimation);
            bloodAnimationObj.transform.position = transform.position;
            bloodAnimationObj.GetComponent<Animator>().SetInteger("WhatAnimation", whatBloodAnimation);

            Destroy(gameObject);

        }


    }

}
