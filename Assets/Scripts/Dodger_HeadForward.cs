using UnityEngine;

public class Dodger_HeadForward : MonoBehaviour
{
    public Transform target;
    public bool Enabled = true;
    public float additionalOffset = 0f;
    public float rotationLerpSpeed = 10f;

    public void Toggle(bool enable)
    {
        Enabled = enable;
    }

    void Update()
    {
        if (target == null) return;
        if (!Enabled) return;

        Vector2 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f + additionalOffset;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);


        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationLerpSpeed * Time.deltaTime);
    }
}
