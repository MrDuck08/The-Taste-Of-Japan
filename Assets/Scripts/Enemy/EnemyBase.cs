using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyBase : MonoBehaviour
{

    public NavMeshAgent agent;
    Rigidbody2D myRigidbody2D;


    GameObject playerObject;


    #region Player Detection


    bool aggro = false;


    [Header("Player Detection")]


    public bool inRangeForAttack = false;
    public bool attacking = false;


    [SerializeField] float detectRadiusInFront = 5;
    [SerializeField] float detectRadiusFromBehind = 2.5f;
    [SerializeField] float DetectionCone = 10;
    [SerializeField] float attackRange = 1;
    [SerializeField] float rotatingSpeed = 200;


    [SerializeField] LayerMask obsticleLayer;


    #endregion


    #region Idle Behavior


    [Header("Idle")]


    [SerializeField] List<Vector2> positionToCycle = new List<Vector2>();
    int atWhatPositionInIdleList = 0;


    bool idle;


    #endregion

    #region Stunned, (Removed For Now)

    //[Header("Stunned")]

    //[SerializeField] float knockbackSpeed = 200000;
    //[SerializeField] float stopSpeed = 40f;

    //[SerializeField] float knockedBackwardsTime = 0.3f;
    //[SerializeField] float stunnedTimer = 5;
    //bool stunned = false;

    #endregion


    #region Look Around For Player


    [Header("Look Around")]

    [SerializeField] float maxTimeToLookAround = 3f;
    float timeToLookAround;
    [SerializeField] float maxTimeUntilNewRotation = 1f;
    float timeUntilNewRotation;

    bool isLookAround;


    #endregion


    #region After Aggro


    [Header("After Aggro")]


    bool startLookForPlayer = false;
    [SerializeField] float maxHowLongLookForPlayer = 3;
    float howLongLookForPlayer;


    #endregion


    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;


        howLongLookForPlayer = maxHowLongLookForPlayer;


        myRigidbody2D = GetComponent<Rigidbody2D>();


    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {


        playerObject = GameObject.FindGameObjectWithTag("Player");

        positionToCycle.Add(transform.position);


    }


    // Update is called once per frame
    public virtual void Update()
    {
        //if (stunned) { return; }

        PlayerDetection();


        if (!aggro)
        {
            IddleBehavior();
        }

        LookAround();


        #region Looking For Player After Aggro


        if (startLookForPlayer)
        {


            howLongLookForPlayer -= Time.deltaTime;


            if (howLongLookForPlayer < 0)
            {
                startLookForPlayer = false;
                idle = false;
                aggro = false;


            }


        }


        #endregion


    }

    #region Idle Walk Around

    void IddleBehavior()
    {

        if (!idle) // F�r Ny Position att G� Till
        {
            idle = true;


            atWhatPositionInIdleList++;

            //Cyklar mellan de olika positionerna
            if (atWhatPositionInIdleList >= positionToCycle.Count)
            {


                atWhatPositionInIdleList = 0;


            }

            agent.SetDestination(positionToCycle[atWhatPositionInIdleList]);

            float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);

            timeToLookAround = maxTimeToLookAround; // Det här är tid för hur länge man ska vara idle på ett ställe

        }
        else if(!isLookAround) // G�r Till Nya Position
        {
            agent.SetDestination(positionToCycle[atWhatPositionInIdleList]);

            float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);

            if (Vector2.Distance(transform.position, positionToCycle[atWhatPositionInIdleList]) < 0.5f) // Nåt position
            {

                // Stå Still och kolla runt
                isLookAround = true;

            }
        }

    }

    #endregion

    #region Look Around

    void LookAround()
    {

        if (isLookAround)
        {

            timeToLookAround -= Time.deltaTime;
            timeUntilNewRotation -= Time.deltaTime;


            if (timeToLookAround < 0) // Sluta kolla runt
            {

                isLookAround = false;
                idle = false;

                // Är Om Aggro också för detta ska också inkludera efter fienden har jagat spelaren
                startLookForPlayer = false;
                aggro = false;


            }


            if (timeUntilNewRotation < 0) // Ny Rotation
            {
                timeUntilNewRotation = maxTimeUntilNewRotation;


                transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));


            }


        }

    }

    #endregion

    #region Detection


    private void PlayerDetection()
    {


        if(playerObject == null) {  return; }

        Vector2 toTarget = playerObject.transform.position - transform.position;
        float currentAngle = Vector2.Angle(transform.up, toTarget);


        float distanceToPlayer = Vector2.Distance(transform.position, playerObject.transform.position);




        Vector2 raycastDirection = playerObject.transform.position - transform.position;


        bool playerObstructed = Physics2D.Raycast(transform.position, raycastDirection.normalized, distanceToPlayer, obsticleLayer);




        #region Attack Range


        if (distanceToPlayer < attackRange && currentAngle < DetectionCone && !playerObstructed) // Kollar om spelaren �r tillr�ckligt n�ra f�r att attackera
        {


            Vector2 lookDirection = new Vector2(playerObject.transform.position.x, playerObject.transform.position.y) - myRigidbody2D.position;
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;


            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);


            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotatingSpeed * Time.deltaTime);


            inRangeForAttack = true;


            aggro = true;


            return; // S�tter return s� den inte beh�ver k�ra den nedast�ende koden


        }
        else
        {
            inRangeForAttack = false;
        }






        #endregion


        #region Back Range




        if (distanceToPlayer < detectRadiusFromBehind)
        {




            Vector2 raycastDirectionBehind = playerObject.transform.position - transform.position;


            bool playerObstructedBehind = Physics2D.Raycast(transform.position, raycastDirectionBehind.normalized, distanceToPlayer, obsticleLayer);


            if (!playerObstructedBehind) // Kollar om det �r n�gonting mellan spellaren och Fienden
            {
                //agent.SetDestination(playerObject.transform.position);


                float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);




                aggro = true;


            }


        }


        #endregion




        #region Front View


        if (!playerObstructed && currentAngle < DetectionCone && distanceToPlayer < detectRadiusInFront) // Kollar om det �r n�gonting mellan spellaren, om de �r tillr�ckligt n�ra och Fienden kollar p� spelaren
        {
            agent.SetDestination(playerObject.transform.position);


            Vector2 lookDirection = new Vector2(playerObject.transform.position.x, playerObject.transform.position.y) - myRigidbody2D.position;
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;


            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);


            startLookForPlayer = false;
            aggro = true;
        }


        #endregion


        if (aggro && myRigidbody2D.angularVelocity == 0 && !startLookForPlayer)
        {
            // Börjar Kolla Runt
            timeUntilNewRotation = maxTimeUntilNewRotation;
            timeToLookAround = maxTimeToLookAround;
            isLookAround = true;

            // Så deta bara körs 1 gång

            howLongLookForPlayer = maxHowLongLookForPlayer;
            startLookForPlayer = true;

        }


    }


    #endregion

    #region Knockback When Door, (Removed For Now)

    //public IEnumerator BeStunned(Vector2 direction)
    //{
    //    stunned = true;

    //    agent.isStopped = true;


    //    myRigidbody2D.AddForce(direction * knockbackSpeed);

    //    SpriteRenderer visuallsChild = transform.Find("Visualls").gameObject.GetComponent<SpriteRenderer>();

    //    visuallsChild.color = new Color32(128, (byte)visuallsChild.color.g, (byte)visuallsChild.color.b, 255);


    //    yield return new WaitForSeconds(knockedBackwardsTime);


    //    Debug.Log("start To Stop");
    //    myRigidbody2D.linearVelocity = Vector3.zero;

    //    //myRigidbody2D.AddForce(direction * knockbackSpeed/7);




    //    yield return new WaitForSeconds(stunnedTimer);


    //    visuallsChild.color = new Color32(255, (byte)visuallsChild.color.g, (byte)visuallsChild.color.b, 255);

    //    stunned = false;
    //}

    #endregion

    #region Gizmo

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;


        Gizmos.DrawWireSphere(transform.position, detectRadiusInFront);




        Gizmos.color = Color.black;


        Gizmos.DrawWireSphere(transform.position, detectRadiusFromBehind);




        Gizmos.color = Color.blue;


        Quaternion leftRotation = Quaternion.Euler(0, 0, DetectionCone);
        Quaternion rightRotation = Quaternion.Euler(0, 0, -DetectionCone);


        Vector2 leftBoundary = leftRotation * transform.up * 3f;
        Vector2 rightBoundary = rightRotation * transform.up * 3f;


        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);


        Gizmos.color = Color.magenta;


        Gizmos.DrawWireSphere(transform.position, attackRange);


    }

    #endregion
}

