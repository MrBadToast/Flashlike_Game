using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public bool FollowX = true;
    public bool FollowY = true;

    public Transform target;

    public Vector3 offset;

    void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }
    void Update()
    {
        if (target != null)
        {
            Vector3 newPosition = transform.position;
            if (FollowX)
            {
                newPosition.x = target.position.x + offset.x;
            }
            if (FollowY)
            {
                newPosition.y = target.position.y + offset.y;
            }
            transform.position = newPosition;
        }
    }

}
