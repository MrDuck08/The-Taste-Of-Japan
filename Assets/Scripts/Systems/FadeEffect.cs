using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - 0.5f * Time.deltaTime);
        if(spriteRenderer.color.a < 0.01f)
        {
            Destroy(gameObject);
        }
    }

    public void InstanciateInfo(SpriteRenderer newSprite, Transform newTransform, Color32 newColor)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = newSprite.sprite;
        spriteRenderer.color = newColor;

        transform.position = newTransform.position;
        transform.localScale = newTransform.localScale;
        transform.rotation = newTransform.rotation;

    }
}
