using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] int health = 1;

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

            Destroy(gameObject);

        }


    }

}
