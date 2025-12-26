using UnityEngine;
using System.Collections;

public class DoubleSwordCharacter : Player1
{
    [SerializeField] GameObject sword1;
    [SerializeField] GameObject sword2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    IEnumerator DoubleSwordBasicAttack()
    {

        sword1.SetActive(true);

        attacking = true;
        basicAttacking = true;

        yield return new WaitForSeconds(0.2f);

        sword1.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        attacking = false;
        basicAttacking = false;

    }
}
