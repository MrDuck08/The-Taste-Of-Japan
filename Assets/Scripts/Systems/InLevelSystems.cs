using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InLevelSystems : MonoBehaviour
{

    List<GameObject> enemiesList = new List<GameObject>();

    [SerializeField] LayerMask whatLayerToIgnore;
    [SerializeField] TrailRenderer bulletTrail;

    public void ShootBackDeflectedBullet(Vector2 fromWhere)
    {

        enemiesList.Clear();

        enemiesList.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        float closestDistanceToEnemy = 1337;
        int whatI = 1337;

        for (int i = 0; i < enemiesList.Count; i++)
        {

            Vector2 direction = new Vector2(enemiesList[i].transform.position.x, enemiesList[i].transform.position.y) - fromWhere;

            RaycastHit2D hit = Physics2D.Raycast(fromWhere, direction.normalized, 1337, ~whatLayerToIgnore);

            if (hit.collider.CompareTag("Enemy"))
            {

                if (Vector2.Distance(fromWhere, enemiesList[i].transform.position) < closestDistanceToEnemy) // Om en annan fiende är närmare
                {

                    whatI = i;
                    closestDistanceToEnemy = Vector2.Distance(fromWhere, enemiesList[i].transform.position);

                }

            }

        }


        if (whatI != 1337)
        {

            Vector2 direction = new Vector2(enemiesList[whatI].transform.position.x, enemiesList[whatI].transform.position.y) - fromWhere;

            RaycastHit2D hit = Physics2D.Raycast(fromWhere, direction.normalized, 1337, ~whatLayerToIgnore);


            TrailRenderer trail = Instantiate(bulletTrail);
            trail.transform.position = fromWhere;

            trail.GetComponent<BulletTrailScript>().MoveAndFadeTrail(fromWhere, hit.point);

            enemiesList[whatI].GetComponent<EnemyHealth>().TakeDamage(1, 2);

        }

    }

}
