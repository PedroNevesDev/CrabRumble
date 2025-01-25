using UnityEngine;
using UnityEngine.UIElements;

public class Waves : MonoBehaviour
{
    Vector2 startPos;
    public float speed = 0;
    public TrailRenderer tr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }
    void OnEnable()
    {
        tr.enabled = true;
        tr.ResetLocalBounds();
    }
    void OnDisable() 
    {
        tr.enabled = false;
        transform.position = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x>-(startPos.x*2))
        {
            transform.position = startPos;
            enabled = false;
        }
        else
        {
            transform.position = Vector2.Lerp(transform.position, -startPos*3, Time.deltaTime*speed);
        }
    }
}
