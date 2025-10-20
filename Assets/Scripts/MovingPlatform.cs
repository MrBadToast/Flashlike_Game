using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MovingPlatform : MonoBehaviour
{
    public Transform platform;
    public Transform pointA;
    public Transform pointB;

    public AnimationCurve movingCurve;

    public bool waitUntilPlayerSteps = true;
    public float movingTime = 2f;

    bool isMoving = false;
    private float time;

    public UnityEvent onPlatformStartMove;
    public UnityEvent onPlatformReset;

    public void Start()
    {
        if (!waitUntilPlayerSteps)
        {
            isMoving = true;
            onPlatformStartMove?.Invoke();
        }
    }

    public void ResetPlatform()
    {
        onPlatformReset?.Invoke();

        if (!waitUntilPlayerSteps)
        {
            isMoving = true;
            onPlatformStartMove?.Invoke();
        }
        else
            isMoving = false;

        time = 0f;
        platform.position = pointA.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isMoving)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (waitUntilPlayerSteps)
                onPlatformStartMove?.Invoke();

            isMoving = true;
        }
    }

    public void Update()
    {
        if (!isMoving)
            return;

        time += Time.deltaTime;

        float t = Mathf.PingPong(time / movingTime, 1f);
        platform.position = Vector3.Lerp(pointA.position, pointB.position,movingCurve.Evaluate(t));

    }

}

