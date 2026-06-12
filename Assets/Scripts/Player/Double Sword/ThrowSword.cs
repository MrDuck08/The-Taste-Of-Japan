using System;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowSword : MonoBehaviour
{
    Rigidbody2D myRigidbody2D;

    bool hitSomething = false;
    bool hitEnemy = false;

    [SerializeField] float speed = 50;
    [SerializeField] float distanceAfterHit = 0.4f;
    [SerializeField] LayerMask ignoreMask;

    GameObject collisionObject = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        #region Flying

        if (!hitSomething)
        {

            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1337, ~ignoreMask);
            

            transform.position = Vector2.MoveTowards(transform.position, hit.point, speed * Time.deltaTime);

            float distance = Vector2.Distance(hit.point, transform.position);

            if (distance < distanceAfterHit)
            {
                hitSomething = true;
                myRigidbody2D.linearVelocity = Vector2.zero;

                collisionObject = hit.transform.gameObject;
                if (collisionObject.tag == "Enemy")
                {
                    hitEnemy = true;
                }
                else
                {
                    
                    Vector2 reflectDir = Vector2.Reflect(transform.up, hit.normal);

                    // Förvandlar riktningen till grader
                    float angle = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;

                    transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

                    hitSomething = false;

                }
            }

        }

        #endregion

        if(hitSomething && !hitEnemy)
        {



        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        //if(collision.transform.tag == "Player" && hitSomething)
        //{

        //    Destroy(gameObject);

        //}

    }
}
