using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] int health = 1;

    SceneLoader sceneLoader;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.transform.CompareTag("EnemyAttack"))
        {


            health--;

            if(health <= 0)
            {

                sceneLoader = FindObjectOfType<SceneLoader>();

                sceneLoader.playerDead = true;

                Destroy(gameObject);

            }


        }


    }

}
