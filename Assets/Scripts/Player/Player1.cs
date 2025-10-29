using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player1 : MonoBehaviour
{
    #region Types

    #region Sword & Gun

    public bool stanceBoolToParent = false;

    #endregion

    #endregion

    #region Movment

    #region Basic Movment

    Vector2 movementInput;
    Vector2 playerVelocity;

    public float speed = 5;

    [SerializeField] float lookSpeed = 2;
    public Vector2 lookDirection;

    #endregion

    #region Basic Dodge

    [SerializeField] float dodgeSpeed = 5;
    [SerializeField] float dodgeTime = 0.5f;

    public bool dodgeLock = false;

    GameObject dodgeCollider = null;


    #endregion

    #endregion

    #region Attack

    public GameObject attackObject;

    public bool attacking = false;
    public bool basicAttacking = false;

    #endregion

    Rigidbody2D myRigidbody;
    CircleCollider2D myCollider;

    Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = transform.Find("CollisionObject").gameObject.GetComponent<CircleCollider2D>();

        cam = Camera.main;

        attackObject.SetActive(false);

        dodgeCollider = transform.Find("DodgeCollider").gameObject;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //Debug.Log(myRigidbody.linearVelocity); //

        if(stanceBoolToParent == false)
        {

            myRigidbody.freezeRotation = false;
            Look();

        }
        else
        {

            myRigidbody.freezeRotation = true;

        }

        if (Input.GetKeyDown(KeyCode.Space) && dodgeLock == false)
        {

            StartCoroutine(basicDodge());


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


    #region basic Dodge

    IEnumerator basicDodge()
    {
        dodgeLock = true;
        attackObject.SetActive(false);

        Vector2 TempMovementInput = movementInput;

        if (movementInput == Vector2.zero) // Om man står stilla så ska man dodga dit man kollar
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            movementInput = mousePos - myRigidbody.position;

        }

        playerVelocity = new Vector2(movementInput.normalized.x * dodgeSpeed, movementInput.normalized.y * dodgeSpeed);
        myRigidbody.linearVelocity = playerVelocity;

        dodgeCollider.SetActive(true);
        myCollider.enabled = false;

        
        yield return new WaitForSeconds(dodgeTime);


        dodgeCollider.SetActive(false);
        myCollider.enabled = true;

        movementInput = TempMovementInput;

        dodgeLock = false;
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
