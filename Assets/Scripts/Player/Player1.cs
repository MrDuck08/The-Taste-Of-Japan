using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player1 : MonoBehaviour
{
    #region Types Variables

    [Header("What Character")]

    [HideInInspector] public bool lockRotationParent = false;
    [HideInInspector] public bool lockMoveinputParent = false;

    #endregion

    #region Movment


    #region Basic Movment Variables

    [HideInInspector] public Vector2 movementInput;
    [HideInInspector] public Vector2 inactiveMovementInput;
    [HideInInspector] public Vector2 playerVelocity;

    public float maxSpeed;
    public float speed = 5;

    [HideInInspector] public Vector2 lookDirection;

    bool walking = false;

    #endregion

    [HideInInspector] public bool dodgeLock = false;


    #endregion

    [HideInInspector] public float lookOffset = 0;
    [HideInInspector] public float lookAroundSpeed = 500f;
    public float maxLookAroundSpeed = 500f;

    #region Attack Variables

    [Header("Basic Attack")]

    public GameObject attackObject;

    [HideInInspector] public bool attacking = false;
    [HideInInspector] public bool basicAttacking = false;

    #endregion

    [HideInInspector] public Rigidbody2D myRigidbody;
    [HideInInspector] public CircleCollider2D myCollider;

    [HideInInspector] public AudioManager audioManager;

    public Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CircleCollider2D>();

        audioManager = FindAnyObjectByType<AudioManager>();

        cam = Camera.main;

        attackObject.SetActive(false);

        speed = maxSpeed;
        lookAroundSpeed = maxLookAroundSpeed;

    }

    // Update is called once per frame
    public virtual void Update()
    {
        //Debug.Log(myRigidbody.linearVelocity); //

        Look();

        if(movementInput == Vector2.zero)
        {

            if (walking)
            {
                audioManager.StopWalkingSound(gameObject);
            }
            walking = false;
        }
        else if (!lockMoveinputParent)
        {
            if (!walking)
            {
                audioManager.playWalkingSound(transform.position, gameObject);
            }
            walking = true;
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
        myRigidbody.linearVelocity += playerVelocity * Time.fixedDeltaTime;


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

        inactiveMovementInput = value.Get<Vector2>();
    }

    #endregion

    public IEnumerator BasicAttack()
    {

        attackObject.SetActive(true);

        attacking = true;
        basicAttacking = true;

        audioManager.PlayPlayerSlashSound(transform.position);

        yield return new WaitForSeconds(0.2f);

        attackObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        attacking = false;
        basicAttacking = false;

    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        


    }

    public virtual void OnCollisionStay2D(Collision2D collision)
    {
        


    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        


    }

}
