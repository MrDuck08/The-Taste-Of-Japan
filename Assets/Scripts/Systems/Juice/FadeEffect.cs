using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 0.5f;
    [SerializeField] bool destroyParent = false;
    [SerializeField] bool goUpp = false;
    [SerializeField] float goUppSpeed = 1.0f;

    SpriteRenderer spriteRenderer;
    Image image;
    TextMeshProUGUI textCanvas;
    TextMeshPro text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
        textCanvas = GetComponent<TextMeshProUGUI>();
        text = GetComponent<TextMeshPro>();

    }

    // Update is called once per frame
    void Update()
    {
        #region Different Types of Fade

        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - fadeSpeed * Time.deltaTime);
            if (spriteRenderer.color.a < 0.01f)
            {
                if (destroyParent)
                {
                    Destroy(transform.parent.gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        if(image != null)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - fadeSpeed * Time.deltaTime);
            if (image.color.a < 0.01f)
            {
                // Vissa Objekt sitter på en canvas som behövs förstöras istället
                if (destroyParent)
                {
                    Destroy(transform.parent.gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        if (textCanvas != null)
        {
            textCanvas.color = new Color(textCanvas.color.r, textCanvas.color.g, textCanvas.color.b, textCanvas.color.a - fadeSpeed * Time.deltaTime);
            if (textCanvas.color.a < 0.01f)
            {
                if (destroyParent)
                {
                    Destroy(transform.parent.gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        if (text != null)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - fadeSpeed * Time.deltaTime);
            if (text.color.a < 0.01f)
            {
                if (destroyParent)
                {
                    Destroy(transform.parent.gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        #endregion

        if (goUpp)
        {
            transform.position += new Vector3(0, goUppSpeed * Time.deltaTime);
        }
    }

    public void InstanciateInfo(SpriteRenderer newSprite, Transform newTransform, Color32 newColor)
    {
        // Om man behöver ändra på den på något sätt
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = newSprite.sprite;
        spriteRenderer.color = newColor;

        transform.position = newTransform.position;
        transform.localScale = newTransform.localScale;
        transform.rotation = newTransform.rotation;

    }
}
