using UnityEngine;

public class Zone : MonoBehaviour
{
    public bool isHere;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.GetComponent<DVDMovement>())
            isHere = true;
    }
    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.GetComponent<DVDMovement>())
            isHere = false;
    }
}
