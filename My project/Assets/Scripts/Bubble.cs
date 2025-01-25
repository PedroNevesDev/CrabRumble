using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public AnimationCurve speedOverLifetimeCurve;
    float currrentSpeedTime = 1;
    public float maxVelocityTime = 1;
    public float maxLifetime = 1;
    float currrentLifeTime = 1;
    Vector3 force;
    public Rigidbody2D myRigidbody;
    List<Bubble> bubblesConnectedToMe = new List<Bubble>();

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>(); // Assuming 3D Rigidbody
        currrentSpeedTime = maxVelocityTime;
        currrentLifeTime = maxLifetime;
    }

    public void Init(Vector3 dir)
    {
        print(dir);
        force = dir.normalized;  // Ensure the force is normalized

        GetComponent<Rigidbody2D>().AddForce(force*30,ForceMode2D.Impulse);
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
                // Apply the force to the Rigidbody in the direction of force
        if (myRigidbody != null)
        {
            myRigidbody.AddForce(force, ForceMode2D.Force); // Apply the force to move the bubble
        }
        // Apply velocity to Rigidbody based on force and speed curve
        float speedFactor = speedOverLifetimeCurve.Evaluate(currrentSpeedTime / maxVelocityTime);
        myRigidbody.linearVelocity = force * speedFactor;
    }
    public void OnDestruction()
    {
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
        force *= 0;
        if(dVDMovement)
        {
            OnDestruction();
            return;
        }

        Bubble bubble = other.collider.GetComponent<Bubble>();

        if (bubble!=null)
        {
            bubble.AddBubble(this);
        }

        Rigidbody2D rb = other.collider.GetComponent<Rigidbody2D>();
        if(rb)
        {
            gameObject.AddComponent<HingeJoint2D>().connectedBody = rb;
        }

        
    }
}
