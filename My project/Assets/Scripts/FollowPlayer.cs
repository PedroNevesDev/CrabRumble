using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform followTrans;
    public Vector3 followOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = followTrans.position+followOffset;
    }
}
