using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Bubble buburoPrefab;
    public float bubbleShootForce;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;          // Maximum movement speed
    public float acceleration = 15f;     // How quickly the player accelerates
    public float deceleration = 20f;     // How quickly the player slows down
    public float inputResponsiveness = 10f; // How quickly input changes are applied

    private Vector2 currentVelocity;     // Current velocity of the player
    public Rigidbody2D rb;    
        
    [Header("Rotation Settings")]
    public float rotationSpeed = 360f;   // Rotation speed in degrees per second

    public void OnMove(InputAction.CallbackContext context) =>  inputVector = context.action.ReadValue<Vector2>();

    public AudioClip shootBubbleSound;


    Vector2 inputVector;
    // Update is called once per frame
    void Update()
    {
        // Normalize input to prevent diagonal speed boosts
        if (inputVector.magnitude > 1f)
        {
            inputVector.Normalize();
        }

        // Calculate the target velocity based on input
        Vector2 targetVelocity = inputVector * moveSpeed;

        // Smoothly interpolate velocity based on acceleration or deceleration
        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity,
            (inputVector != Vector2.zero ? acceleration : deceleration) * Time.deltaTime);

        // Update Rigidbody velocity with current velocity
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, currentVelocity, inputResponsiveness * Time.deltaTime);

        if (inputVector != Vector2.zero)
        {
            RotateTowardsDirection(inputVector);
        }
    }
    private void RotateTowardsDirection(Vector2 direction)
    {
        // Calculate the target rotation based on the input direction
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Get the current rotation angle
        float currentAngle = transform.rotation.eulerAngles.z;

        // Smoothly rotate toward the target angle
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime / 360f);

        // Apply the new rotation to the object
        transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }
    public void OnShoot(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            AudioManager.Instance.PlaySoundEffect(shootBubbleSound);
            Quaternion offsetRotation = Quaternion.Euler(0f, 0f, -90f);
            Quaternion combinedRotation = transform.rotation * offsetRotation;
            Instantiate(buburoPrefab, transform.position, combinedRotation);
        }
    }

    public void OnUseSpecial(InputAction.CallbackContext context)
    {
        if(context.performed)
        print("Special was used");
    }
}
