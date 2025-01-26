using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Bubble buburoPrefab;

    public Bubble metalBubblePrefab;
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

    public List<Special> specials = new List<Special>();
    Special currentSpecial = null;

    public void OnMove(InputAction.CallbackContext context) =>  inputVector = context.action.ReadValue<Vector2>();

    public AudioClip shootBubbleSound;

    public float speedMultiplayer = 1;

    public bool shouldUseMetalBalls;

    public float maxPowerUpCd;
    float currentPowerUpCd;

    public Image fillAmmount;
    public Image abilitieImage; 

    public Text cdText;


    Vector2 inputVector;
    void Start()
    {
        currentPowerUpCd = maxPowerUpCd;
    }
    // Update is called once per frame
    void Update()
    {   
        UpdatePowerUp();
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
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, currentVelocity*speedMultiplayer, inputResponsiveness * Time.deltaTime);

        if (inputVector != Vector2.zero)
        {
            RotateTowardsDirection(inputVector);
        }
    }

    void UpdatePowerUp()
    {
        if(currentSpecial==null)
        if(currentPowerUpCd>0)
        {
            abilitieImage.enabled = false;
            fillAmmount.gameObject.SetActive(true);
            currentPowerUpCd-=Time.deltaTime;
            fillAmmount.fillAmount = currentPowerUpCd/maxPowerUpCd;
            cdText.text = currentPowerUpCd.ToString(); 
        }
        else
        {
            currentSpecial = specials[UnityEngine.Random.Range(0, specials.Count)];
            abilitieImage.enabled = true;
            abilitieImage.sprite = currentSpecial.abilitySprite;
            fillAmmount.gameObject.SetActive(false);
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
            Instantiate(shouldUseMetalBalls?metalBubblePrefab:buburoPrefab, transform.position, combinedRotation);
        }
    }

public void OnUseSpecial(InputAction.CallbackContext context)
{
    if (context.performed && currentSpecial != null)
    {
        // Store the current special in a temporary variable
        Special specialToUse = currentSpecial;

        // Reset the current special and start cooldown
        currentPowerUpCd = maxPowerUpCd;
        currentSpecial = null;

        // Activate the special effect
        if (specialToUse.isBoost)
        {
            specialToUse.BoostSpeed(this);
        }
        else if (specialToUse.isMetal)
        {
            specialToUse.StartSpawnMetal(this);
        }
        else
        {
            Quaternion offsetRotation = Quaternion.Euler(0f, 0f, -90f);
            Quaternion combinedRotation = transform.rotation * offsetRotation;
            specialToUse.Spawn(transform.position, combinedRotation);
        }

        print("Special was used");
    }
}
    public IEnumerator TemporarySpeedBoost()
    {
        speedMultiplayer = 1.5f;
        yield return new WaitForSeconds(3);
        speedMultiplayer = 1;
    } 

    public IEnumerator UseMetalBalls()
    {
        shouldUseMetalBalls = true;
        yield return new WaitForSeconds(3);
        shouldUseMetalBalls = false;
    } 


    [System.Serializable]
    public class Special 
    {
        public Sprite abilitySprite;
        public GameObject abilitiePrefab;

        public AudioClip audioClip;

        public bool isBoost;
        public bool isMetal;

        public void Spawn(Vector3 pos, Quaternion rotation)
        {
            AudioManager.Instance.PlaySoundEffect(audioClip);
            Instantiate(abilitiePrefab, pos, rotation);
        }

        public void BoostSpeed(Player p)
        {
            AudioManager.Instance.PlaySoundEffect(audioClip);
            p.StartCoroutine(p.TemporarySpeedBoost());
        }

        public void StartSpawnMetal(Player p)
        {
            AudioManager.Instance.PlaySoundEffect(audioClip);
            p.StartCoroutine(p.UseMetalBalls());
        }
    }
}
