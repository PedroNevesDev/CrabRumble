using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

public class Bubble : MonoBehaviour
{
    public AnimationCurve speedOverLifetimeCurve;
    float currrentSpeedTime = 1;
    public float maxVelocityTime = 1;
    public float maxLifetime = 1;
    float currrentLifeTime = 1;
    public float speed;
    public Rigidbody2D myRigidbody;
    List<Bubble> bubblesConnectedToMe = new List<Bubble>();

    public AudioClip popSound;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>(); // Assuming 3D Rigidbody
        currrentSpeedTime = maxVelocityTime;
        currrentLifeTime = maxLifetime;
    }




    void Update()
    {
        currrentSpeedTime -= Time.deltaTime;
        if (currrentSpeedTime < 0)
        {
            currrentLifeTime -= Time.deltaTime;
            if(currrentLifeTime < 0)
            {
                OnDestruction();
            }
        }

        // Apply velocity to Rigidbody based on force and speed curve
        float speedFactor = speedOverLifetimeCurve.Evaluate(currrentSpeedTime/maxVelocityTime);
        myRigidbody.linearVelocity = transform.up * speedFactor * speed;
    }
    public void OnDestruction()
    {
        AudioManager.Instance.PlaySoundEffect(popSound);
        Destroy(gameObject);
        Invoke("DestroyBuburo",0.5f);
    }

    void DestroyBuburo()
    {
        bubblesConnectedToMe.ForEach(bubble=>bubble.OnDestruction());
    }
    public void AddBubble(Bubble childBubble)
    {
        bubblesConnectedToMe.Add(childBubble);
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        DVDMovement dVDMovement = other.gameObject.GetComponent<DVDMovement>();
        if(dVDMovement)
        {
            OnDestruction();
            return;
        }

        Bubble bubble = other.collider.GetComponent<Bubble>();

        if (bubble!=null)
        {
            currrentSpeedTime = 0;
            bubble.AddBubble(this);
        }

        Rigidbody2D rb = other.collider.GetComponent<Rigidbody2D>();
        if(rb)
        {
            gameObject.AddComponent<HingeJoint2D>().connectedBody = rb;
        }

        
    }
}
