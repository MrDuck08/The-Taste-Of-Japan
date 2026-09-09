using System.Collections;
using UnityEngine;

public class Singleton : MonoBehaviour
{

    [SerializeField] GameObject normalSword;
    [SerializeField] GameObject spear;
    [SerializeField] GameObject quickDraw;

    int currentPlayerWeapon = 1337;


    Player1 player;

    static Singleton instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {

            instance.ArtificialStart();


            Destroy(gameObject);
        }
    }


    void Start()
    {
        player = FindAnyObjectByType<Player1>();

        if(currentPlayerWeapon == 1337)
        {
            if (FindAnyObjectByType<BasicSword>() != null)
            {
                currentPlayerWeapon = 1;
            }

            if (FindAnyObjectByType<SpearWeapon>() != null)
            {
                currentPlayerWeapon = 2;
            }

            if (FindAnyObjectByType<QuiickDrawWeapon>() != null)
            {
                currentPlayerWeapon = 3;
            }
        }
    }


    public void ArtificialStart()
    {

        StartCoroutine(DelayStart());
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(0.1f);


        player = FindAnyObjectByType<Player1>();

        ChangePlayerWeapon(currentPlayerWeapon);
    }


    void Update()
    {
        
    }

    public void ChangePlayerWeapon(int toWhat)
    {
        Destroy(FindAnyObjectByType<MeleeWeaponsBase>().gameObject);

        GameObject spawnedWeapon = null;

        switch (toWhat)
        {

            case 1:

                spawnedWeapon = Instantiate(normalSword);
                currentPlayerWeapon = 1;

                break;

            case 2:

                spawnedWeapon = Instantiate(spear);
                currentPlayerWeapon = 2;

                break;

            case 3:

                spawnedWeapon = Instantiate(quickDraw);
                currentPlayerWeapon = 3;

                break;

        }

        player = FindFirstObjectByType<Player1>();

        spawnedWeapon.transform.parent = player.gameObject.transform;
        spawnedWeapon.transform.position = player.gameObject.transform.position;
        spawnedWeapon.transform.rotation = player.gameObject.transform.rotation;
        player.meleeWeapon = spawnedWeapon.GetComponent<MeleeWeaponsBase>();

    }
}
