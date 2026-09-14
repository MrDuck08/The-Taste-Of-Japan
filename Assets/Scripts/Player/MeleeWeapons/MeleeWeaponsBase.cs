using UnityEngine;

public class MeleeWeaponsBase : MonoBehaviour
{


    [HideInInspector] public bool basicAttackNow = false;
    [HideInInspector] public bool stanceAttackNow = false;
    [HideInInspector] public bool harmonyAttackNow = false;


    public GameObject basicAttackObj;
    public GameObject stanceAttackObj;
    public GameObject harmonyAttackObj;



    #region Harmony Variables

    [Header("Harmony")]

    public LayerMask bulletIgnoreLayerMask;
    public LayerMask doorLayerMask;

    public float rushSpeed = 40f;
    [HideInInspector] public bool rushing = false;
    [HideInInspector] public bool rushAttackHasStarted = false;
    [HideInInspector] public Vector2 pointToRushTo = Vector2.zero;

    public GameObject fadeEffectObj;
    [HideInInspector] public float harmonyFadeEffectTime;
    [HideInInspector] public float maxHarmonyFadeEffectTime = 0.3f;

    #endregion


    [HideInInspector] public Player1 player;
    [HideInInspector] public SwordAndGunCharacter playerSpesifics;
    [HideInInspector] public PlayerHealth playerHealth;
    [HideInInspector] public CameraFollow cam;
    [HideInInspector] public AudioManager audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {

        player = GetComponentInParent<Player1>();
        playerSpesifics = GetComponentInParent<SwordAndGunCharacter>();
        playerHealth = GetComponentInParent<PlayerHealth>();

        audioManager = FindAnyObjectByType<AudioManager>();
        cam = FindAnyObjectByType<CameraFollow>();

    }

    public virtual void Update()
    {
        
    }


    public void BasicAttack()
    {
        basicAttackNow = true;
    }

    public void StanceAttack()
    {
        stanceAttackNow = true;
    }

    public void HarmonyAttack()
    {
        harmonyAttackNow = true;
    }

}
