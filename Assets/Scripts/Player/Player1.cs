using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player1 : MonoBehaviour
{
    #region Types

    [Header("What Character")]

    public bool lockRotationParent = false;

    #endregion

    #region Movment

    [Header("Ignore")]

    #region Basic Movment

    public Vector2 movementInput;
    public Vector2 playerVelocity;

    public float maxSpeed;
    public float speed = 5;

    public Vector2 lookDirection;

    #endregion

    public bool dodgeLock = false;

    #endregion

    #region Attack

    [Header("Basic Attack")]

    public GameObject attackObject;

    [Header("Ignore")]

    public bool attacking = false;
    public bool basicAttacking = false;

    #endregion

    public Rigidbody2D myRigidbody;
    public CircleCollider2D myCollider;

    public Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CircleCollider2D>();

        cam = Camera.main;

        attackObject.SetActive(false);

        speed = maxSpeed;

    }

    // Update is called once per frame
    public virtual void Update()
    {
        //Debug.Log(myRigidbody.linearVelocity); //

        if(lockRotationParent == false)
        {

            myRigidbody.freezeRotation = false;
            Look();

        }
        else
        {

            myRigidbody.freezeRotation = true;

        }

    }

    public virtual void FixedUpdate()
    {

        if(dodgeLock == false)
        {

            Moving();

        }

    }

    #region Moving

    void Moving()
    {
        playerVelocity = new Vector2(movementInput.x * speed, movementInput.y * speed);
        myRigidbody.linearVelocity = playerVelocity;

    }

    void Look()
    {

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        lookDirection = mousePos - myRigidbody.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);


    }

    void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    #endregion

    public IEnumerator BasicAttack()
    {

        attackObject.SetActive(true);

        attacking = true;
        basicAttacking = true;

        yield return new WaitForSeconds(0.2f);

        attackObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        attacking = false;
        basicAttacking = false;

    }



}
