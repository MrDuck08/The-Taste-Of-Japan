using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyBase : MonoBehaviour
{

    NavMeshAgent agent;
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
    [SerializeField] LayerMask obsticleLayer;

    #endregion

    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

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

    }

    #region Detection

    private void PlayerDetection()
    {

        Vector2 toTarget = playerObject.transform.position - transform.position;
        float currentAngle = Vector2.Angle(transform.up, toTarget);

        float distanceToPlayer = Vector2.Distance(transform.position, playerObject.transform.position);

        #region Attack Range

        if (distanceToPlayer < attackRange && currentAngle < DetectionCone) // Kollar om spelaren är tillräckligt nära för att attackera
        {

            
            inRangeForAttack = true;

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
                agent.SetDestination(playerObject.transform.position);

                Vector2 lookDirection = new Vector2(playerObject.transform.position.x, playerObject.transform.position.y) - rigidbody2D.position;
                float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);

                aggro = true;

                return;
            }

        }

        #endregion


        #region Front View

        if (distanceToPlayer > detectRadiusInFront) // Kollar om spelaren är tillräckligt nära
        {
            return;
        }

        Vector2 raycastDirection = playerObject.transform.position - transform.position;

        bool playerObstructed = Physics2D.Raycast(transform.position, raycastDirection.normalized, distanceToPlayer, obsticleLayer);

        if (!playerObstructed && currentAngle < DetectionCone) // Kollar om det är någonting mellan spellaren och Fienden kollar på spelaren
        {
            agent.SetDestination(playerObject.transform.position);

            Vector2 lookDirection = new Vector2(playerObject.transform.position.x, playerObject.transform.position.y) - rigidbody2D.position;
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);

            aggro = true;
        }

        #endregion

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
