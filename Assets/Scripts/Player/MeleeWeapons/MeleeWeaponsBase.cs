using UnityEngine;

public class MeleeWeaponsBase : MonoBehaviour
{


    [HideInInspector] public bool basicAttackNow = false;
    [HideInInspector] public bool stanceAttackNow = false;
    [HideInInspector] public bool harmonyAttackNow = false;


    public GameObject basicAttackObj;
    public GameObject stanceAttackObj;
    public GameObject harmonyAttackObj;



    [HideInInspector] public Player1 player;
    [HideInInspector] public SwordAndGunCharacter playerSpesifics;
    [HideInInspector] public PlayerHealth playerHealth;
    [HideInInspector] public AudioManager audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {

        player = GetComponentInParent<Player1>();
        playerSpesifics = GetComponentInParent<SwordAndGunCharacter>();
        playerHealth = GetComponentInParent<PlayerHealth>();

        audioManager = FindAnyObjectByType<AudioManager>();

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
