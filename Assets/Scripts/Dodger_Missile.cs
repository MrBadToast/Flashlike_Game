using UnityEngine;

public class Dodger_Missile : MonoBehaviour
{
    public Transform target;           // ��ǥ Ÿ��
    public float speed = 8f;           // �̻��� �ӵ�
    public float arriveDistance = 0.2f; // ���� ���� �Ÿ�
    public float maxAngularSpeed = 2f; // �ִ� ȸ�� �ӵ�(��/��)
    public float trackingTime = 3f;  // ���� ���� �ð�(��)

    private Vector2 initialPosition;
    private float elapsedTime = 0f;
    private bool isTracking = true;

    private void Awake()
    {
        initialPosition = transform.position;
        elapsedTime = 0f;
        isTracking = true;
    }

    private void OnEnable()
    {
        transform.position = initialPosition;

        Vector2 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        elapsedTime = 0f;
        isTracking = true;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        elapsedTime += Time.deltaTime;
        if (isTracking && elapsedTime >= trackingTime)
        {
            isTracking = false;
        }

        if (isTracking)
        {
            Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;

            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            float currentAngle = transform.eulerAngles.z;
            float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, maxAngularSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(0, 0, angle);
        }


        Vector2 moveDir = transform.up;
        transform.position += (Vector3)(moveDir * speed * Time.deltaTime);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTracking = false;
        }
    }
}
