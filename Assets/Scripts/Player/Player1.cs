using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player1 : MonoBehaviour
{
    #region Types Variables

    [Header("What Character")]

    public bool lockRotationParent = false;
    public bool lockMoveinputParent = false;

    #endregion

    #region Movment

    [Header("Ignore")]

    #region Basic Movment Variables

    public Vector2 movementInput;
    public Vector2 inactiveMovementInput;
    public Vector2 playerVelocity;

    public float maxSpeed;
    public float speed = 5;

    public Vector2 lookDirection;

    #endregion

    public bool dodgeLock = false;

    #endregion

    public float lookOffset = 0;
    [SerializeField] float lookAroundSpeed = 500f;

    #region Attack Variables

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

        Look();

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
        myRigidbody.linearVelocity += playerVelocity;


    }

    void Look()
    {

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        lookDirection = mousePos - myRigidbody.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90);


        if (lockRotationParent == false)
        {

            myRigidbody.freezeRotation = false;

            if(transform.rotation != targetRotation)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookAroundSpeed * Time.deltaTime);
            }

        }
        else
        {

            myRigidbody.freezeRotation = true;

        }


    }

    void OnMove(InputValue value)
    {
        if (!lockMoveinputParent)
        {
            movementInput = value.Get<Vector2>();
        }
        else
        {
            inactiveMovementInput = value.Get<Vector2>();
        }
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

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        


    }

}
