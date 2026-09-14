using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RainbowEffect : MonoBehaviour
{

    [SerializeField] private Gradient gradient;
    [SerializeField] private float speed = 1f;

    SpriteRenderer spriteRenderer;
    Image image;
    TextMeshProUGUI text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(spriteRenderer != null)
        {
            float t = Mathf.Repeat(Time.deltaTime * speed, 1f);

            //  Har inte den första eftersom det förstör fade effekten
            //spriteRenderer.color = gradient.Evaluate(t);
            spriteRenderer.color = new Color(gradient.Evaluate(t).r, gradient.Evaluate(t).g, gradient.Evaluate(t).b, spriteRenderer.color.a);
        }

        if (image != null)
        {
            float t = Mathf.Repeat(Time.time * speed, 1f);

            //image.color = gradient.Evaluate(t);
            image.color = new Color(gradient.Evaluate(t).r, gradient.Evaluate(t).g, gradient.Evaluate(t).b, image.color.a);
        }

        if (text != null)
        {
            float t = Mathf.Repeat(Time.time * speed, 1f);

            //text.color = gradient.Evaluate(t);
            text.color = new Color(gradient.Evaluate(t).r, gradient.Evaluate(t).g, gradient.Evaluate(t).b, text.color.a);
        }
    }
}
