using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class EnemyBase : MonoBehaviour
{

    public NavMeshAgent agent;
    Rigidbody2D rigidbody2D;

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

        rigidbody2D = GetComponent<Rigidbody2D>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {

        playerObject = GameObject.FindGameObjectWithTag("Player");

    }

    // Update is called once per frame
    public virtual void Update()
    {

        PlayerDetection();

        if (!aggro)
        {
            if (!idle) // Får Ny Position att Gå Till
            {
                idle = true;

                atWhatPositionInIdleList++;

                if (atWhatPositionInIdleList >= positionToCycle.Count)
                {

                    atWhatPositionInIdleList = 0;

                }

                agent.SetDestination(positionToCycle[atWhatPositionInIdleList]);

                Vector2 lookDirection = positionToCycle[atWhatPositionInIdleList] - rigidbody2D.position;
                float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);

            }
            else // Går Till Nya Position
            {
                agent.SetDestination(positionToCycle[atWhatPositionInIdleList]);

                if (Vector2.Distance(transform.position, positionToCycle[atWhatPositionInIdleList]) < 0.5f)
                {

                    idle = false;

                }
            }

        }

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

    #region Non Aggro Behavior

    void IdleBehavior()
    {

        idle = true;

        if (atWhatPositionInIdleList > positionToCycle.Count)
        {

            atWhatPositionInIdleList = 0;

        }
        Debug.Log("YES");
        agent.SetDestination(positionToCycle[atWhatPositionInIdleList]);

    }

    #endregion

    #region Detection

    private void PlayerDetection()
    {

        Vector2 toTarget = playerObject.transform.position - transform.position;
        float currentAngle = Vector2.Angle(transform.up, toTarget);

        float distanceToPlayer = Vector2.Distance(transform.position, playerObject.transform.position);


        Vector2 raycastDirection = playerObject.transform.position - transform.position;

        bool playerObstructed = Physics2D.Raycast(transform.position, raycastDirection.normalized, distanceToPlayer, obsticleLayer);


        #region Attack Range

        if (distanceToPlayer < attackRange && currentAngle < DetectionCone && !playerObstructed) // Kollar om spelaren är tillräckligt nära för att attackera
        {

            Vector2 lookDirection = new Vector2(playerObject.transform.position.x, playerObject.transform.position.y) - rigidbody2D.position;
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotatingSpeed * Time.deltaTime);

            inRangeForAttack = true;

            aggro = true;

            return; // Sätter return så den inte behöver köra den nedastående koden

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

            if (!playerObstructedBehind) // Kollar om det är någonting mellan spellaren och Fienden
            {
                //agent.SetDestination(playerObject.transform.position);

                Vector2 lookDirection = new Vector2(playerObject.transform.position.x, playerObject.transform.position.y) - rigidbody2D.position;
                float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);


                aggro = true;

            }

        }

        #endregion


        #region Front View

        if (!playerObstructed && currentAngle < DetectionCone && distanceToPlayer < detectRadiusInFront) // Kollar om det är någonting mellan spellaren, om de är tillräckligt nära och Fienden kollar på spelaren
        {
            agent.SetDestination(playerObject.transform.position);

            Vector2 lookDirection = new Vector2(playerObject.transform.position.x, playerObject.transform.position.y) - rigidbody2D.position;
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);

            startLookForPlayer = false;
            aggro = true;
        }

        #endregion

        if(aggro && rigidbody2D.angularVelocity == 0 && !startLookForPlayer)
        {
            howLongLookForPlayer = maxHowLongLookForPlayer;
            Debug.Log("Maybe START AGGRO");
            startLookForPlayer = true;

        }

    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, detectRadiusInFront);


        Gizmos.color = Color.orange;

        Gizmos.DrawWireSphere(transform.position, detectRadiusFromBehind);


        Gizmos.color = Color.blue;

        Quaternion leftRotation = Quaternion.Euler(0, 0, DetectionCone);
        Quaternion rightRotation = Quaternion.Euler(0, 0, -DetectionCone);

        Vector2 leftBoundary = leftRotation * transform.up * 3f;
        Vector2 rightBoundary = rightRotation * transform.up * 3f;

        Gizmos.DrawRay(transform.position, leftBoundary);
        Gizmos.DrawRay(transform.position, rightBoundary);

        Gizmos.color = Color.pink;

        Gizmos.DrawWireSphere(transform.position, attackRange);

    }
}
