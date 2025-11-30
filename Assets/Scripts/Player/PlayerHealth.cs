using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] int health = 1;

    SceneLoader sceneLoader;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.transform.CompareTag("EnemyAttack"))
        {

            TakeDamage(1);

        }


    }

    public void TakeDamage(int damageTaken)
    {



        health -= damageTaken;

        if (health <= 0)
        {

            sceneLoader = FindAnyObjectByType<SceneLoader>();

            sceneLoader.playerDead = true;

            Destroy(gameObject);

        }


    }

}
