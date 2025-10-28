using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Physics2D.IgnoreLayerCollision(3, 6, true); // Ignore collision betwen wall and door

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
