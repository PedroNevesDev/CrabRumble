using System.Collections.Generic;
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

    public bool isSticky;

    public UnityEvent onDestroy;

    public int numCollisions = 1;

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
        onDestroy.Invoke();
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
    public void Explosion()
    {
        print("Explosion");
    }
    void OnCollisionEnter2D(Collision2D other) 
    {
        DVDMovement dVDMovement = other.gameObject.GetComponent<DVDMovement>();
        if(dVDMovement)
        {
            numCollisions--;
            if(numCollisions<=0)
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
