using UnityEngine;
using UnityEngine.UIElements;

public class Waves : MonoBehaviour
{
    Vector3 startPos;
    public float speed = 0;
    public TrailRenderer tr;

    public float ammont;
    public AudioClip wavesSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
    }
    void OnEnable()
    {
        tr.Clear();
        tr.enabled = true;
        AudioManager.Instance.PlaySoundEffect(wavesSound);
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
        float dis = Vector3.Distance(transform.position,startPos+new Vector3(ammont,0,0));
        if(dis<40)
        {
            enabled = false;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position,startPos+new Vector3(ammont,0,0),speed*Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        other.GetComponent<Rigidbody2D>()?.AddForce((startPos+new Vector3(ammont,0,0))-transform.position*10,ForceMode2D.Impulse);
    }
}
