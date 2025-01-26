using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DVDMovement : MonoBehaviour
{
    public float speed = 5f; // Movement speed
    public  Rigidbody2D rb;
    private Vector2 lastVelocity;

    // Distance at which we apply the "unstick" force (adjust this if needed)
    public float stuckDistance = 0.1f;
    public float unstuckForce = 2f;

    void Start()
    {

        // Set the Rigidbody2D settings for no drag or friction
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;

        // Initialize velocity in a random direction
        Vector2 initialDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        rb.linearVelocity = initialDirection * speed;  // Use velocity instead of linearVelocity
    }

    void Update()
    {
        // Store the current velocity for use in collision handling
        lastVelocity = rb.linearVelocity;

        // Add rotation based on velocity (align the object with movement direction)
        float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Check if the ball is stuck (if it's moving very slowly or too close to the player/wall)
        if (rb.linearVelocity.magnitude < 0.1f)
        {
            // Apply a small force to push the ball out of a stuck position
            rb.AddForce(Vector2.right * unstuckForce, ForceMode2D.Impulse); // Change direction as needed
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Get the collision normal
        Vector2 normal = collision.contacts[0].normal;

        // Reflect the velocity based on the normal
        Vector2 reflectedVelocity = Vector2.Reflect(lastVelocity, normal);

        // Ensure the ball maintains the same speed after the bounce
        rb.linearVelocity = reflectedVelocity.normalized * speed;

        // Check if the ball collided with a player or another object
        Rigidbody2D otherRb = collision.rigidbody;
        if (otherRb != null)
        {
            // Apply a small force to the player to simulate a realistic bounce effect (optional)
            if (collision.gameObject.CompareTag("Player"))
            {
                // Reflect the velocity without transferring too much momentum to the player
                Vector2 playerReflection = Vector2.Reflect(lastVelocity, normal);
                rb.linearVelocity = playerReflection.normalized * speed;

                // Optional: Apply a small force to the player for a realistic bounce effect
                // This is optional and can be adjusted or removed if not needed
                otherRb.AddForce(-normal * speed * 0.5f, ForceMode2D.Impulse);  // Apply a small force to the player
            }
        }

        // Optional: Check for situations where the ball is stuck between the player and a wall
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Apply a small force to push the ball out if it's stuck between the player and the wall
            Vector2 pushOutForce = normal * unstuckForce;
            rb.AddForce(pushOutForce, ForceMode2D.Impulse);
        }
    }
}
