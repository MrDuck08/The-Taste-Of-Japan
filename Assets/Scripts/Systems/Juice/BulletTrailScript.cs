using System.Collections;
using UnityEngine;

public class BulletTrailScript : MonoBehaviour
{
    [SerializeField] float trailSpeed = 20f;
    [SerializeField] float fadeDuration = 1f;

    public void MoveAndFadeTrail(Vector2 fromWhere, Vector2 target)
    {
        // Behövs göras på detta vis för om den som kallar på Ienumeratorn dör fungerar den inte
        StartCoroutine(MoveAndFadeTrailRoutine(fromWhere, target));

    }

    public IEnumerator MoveAndFadeTrailRoutine(Vector2 fromWhere, Vector2 target)
    {

        TrailRenderer trail = GetComponent<TrailRenderer>();

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

            trail.startWidth += 0.0003f * elapsedTime;
            trail.endWidth += 0.0003f * elapsedTime;

            trail.startColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, currentStartAlpha);
            trail.endColor = new Color(trail.endColor.r, trail.endColor.g, trail.endColor.b, currentEndAlpha);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        // Final position and cleanup
        trail.transform.position = target;
        Destroy(trail.gameObject);
    }
}
