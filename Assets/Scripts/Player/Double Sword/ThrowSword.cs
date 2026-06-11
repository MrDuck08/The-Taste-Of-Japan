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
                    float _whatSideOfObj = 1;

                    if(Mathf.Abs(hit.point.y - collisionObject.transform.position.y) > Mathf.Abs(hit.point.x - collisionObject.transform.position.x))
                    {
                        _whatSideOfObj = -1;
                        Debug.Log("Closer To X");
                        
                    }

                    // Väggar är roterade så man kan inte ta scala x som exempel
                    Debug.Log("X Hit Pos: " + hit.point.x);
                    Debug.Log("Obj X + Scale: " + (Mathf.Abs(collisionObject.transform.position.x) + collisionObject.transform.localScale.y / 2));
                    float distanceToXWall = Mathf.Abs(hit.point.x - collisionObject.transform.position.x) + collisionObject.transform.localScale.x/2;
                    Debug.Log(Mathf.Abs(distanceToXWall));


                    float angle = Mathf.Atan2((hit.point.y - transform.position.y) * _whatSideOfObj, (hit.point.x - transform.position.x) * _whatSideOfObj) * Mathf.Rad2Deg;

                    //float angle = Mathf.Atan2(transform.position.y - hit.point.y, transform.position.x - hit.point.x) * Mathf.Rad2Deg;
                    //transform.rotation = Quaternion.Euler(0, 0, -angle - 90);
                    //transform.localEulerAngles *= -1;
                    transform.localEulerAngles = new Vector3(0, 0, -angle - 90);

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
