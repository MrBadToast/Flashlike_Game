using UnityEngine;

public class Autodestroy : MonoBehaviour
{
    public bool startTimerOnAwake = true;
    public float lifetime = 1f;

    private float timer;

    private void Awake()
    {
        if (startTimerOnAwake)
        {
            timer = lifetime;
        }
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
