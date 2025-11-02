using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InLevelSystems : MonoBehaviour
{

    List<GameObject> enemiesList = new List<GameObject>();

    [SerializeField] LayerMask whatLayerToIgnore;
    [SerializeField] TrailRenderer bulletTrail;

    [SerializeField] float trailSpeed = 20f;
    [SerializeField] float fadeDuration = 1f;

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

            StartCoroutine(MoveAndFadeTrail(trail, fromWhere, hit.point));

            enemiesList[whatI].GetComponent<EnemyHealth>().TakeDamage(1);

        }

    }

    #region Fade & Widen Bullet

    IEnumerator MoveAndFadeTrail(TrailRenderer trail, Vector2 fromWhere, Vector2 target)
    {
        float elapsedTime = 0f;
        Vector2 startPosition = fromWhere;

        // Cache initial alpha
        float startAlpha = trail.startColor.a;
        float endAlpha = trail.endColor.a;

        while (elapsedTime < fadeDuration)
        {
            // Move the trail towards the target
            float moveT = Mathf.Clamp01(elapsedTime * trailSpeed);
            trail.transform.position = Vector2.Lerp(startPosition, target, moveT);

            // Fade the alpha over time
            float alphaT = 1 - (elapsedTime / fadeDuration);
            float currentStartAlpha = startAlpha * alphaT;
            float currentEndAlpha = endAlpha * alphaT;

            trail.startWidth += 0.00015f;
            trail.endWidth += 0.00015f;

            trail.startColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, currentStartAlpha);
            trail.endColor = new Color(trail.endColor.r, trail.endColor.g, trail.endColor.b, currentEndAlpha);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        // Final position and cleanup
        trail.transform.position = target;
        Destroy(trail.gameObject);
    }

    #endregion

}
