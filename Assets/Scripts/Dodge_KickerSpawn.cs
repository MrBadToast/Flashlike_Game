using UnityEngine;

public class Dodge_KickerSpawn : MonoBehaviour
{
    public GameObject kickerPrefab;
    public float spawnRadius = 5f;
    public float spawnInterval = 2f;
    public Transform target; // ��ǥ Transform

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnKicker();
        }
    }

    private void SpawnKicker()
    {
        // �� ���� ���� ����
        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 randomPoint = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
        Vector3 spawnPosition = new Vector3(randomPoint.x, randomPoint.y, 0f) + transform.position;

        // ������Ʈ ����
        GameObject kicker = Instantiate(kickerPrefab, spawnPosition, Quaternion.identity);

        // up�� target�� ���ϵ��� ȸ�� (2D)
        if (target != null)
        {
            Vector2 dir = ((Vector2)target.position - (Vector2)spawnPosition).normalized;
            float rotAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            kicker.transform.rotation = Quaternion.Euler(0, 0, rotAngle);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
