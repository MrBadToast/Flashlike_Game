using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Platformer_Cannon : MonoBehaviour
{
    private bool playerInCannon = false;
    private AudioSource audioSource;
    public DOTweenAnimation tweenAnimation;
    public AudioClip readyClip;
    public AudioClip fireClip;
    public GameObject CannonCamera;
    public UnityEvent onCannonFire;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(playerInCannon)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            playerInCannon = true;
            audioSource.PlayOneShot(readyClip);
            CannonCamera.SetActive(true);
            StartCoroutine(Cor_CannonReady());
            Player_Platformer.Instance.ChangeState(new CannonReady());
            Player_Platformer.Instance.transform.parent = transform;
            Player_Platformer.Instance.transform.position = transform.position;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInCannon = false;
            CannonCamera.SetActive(false);
        }
    }

    IEnumerator Cor_CannonReady()
    {
        while (playerInCannon)
        {
            // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = transform.position.z;

            // ���� ���� ���
            Vector2 direction = mouseWorldPos - transform.position;

            // ���� ��� (���� �� ��)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // ȸ�� ���� (Z�� ����)
            transform.rotation = Quaternion.Euler(0, 0, angle);

            if (Input.GetMouseButtonDown(0))
            {
                audioSource.PlayOneShot(fireClip);
                tweenAnimation.DORestart();
                Player_Platformer.Instance.transform.parent = null;
                Player_Platformer.Instance.transform.rotation = Quaternion.Euler(0, 0, 0);
                Player_Platformer.Instance.CannonPlayer(transform.right);

                playerInCannon = false;
                onCannonFire?.Invoke();
                yield break;
            }
            else if(Player_Platformer.Instance.CurrentState is DeadState)
            {
                CannonCamera.SetActive(false);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                Player_Platformer.Instance.transform.rotation = Quaternion.Euler(0, 0, 0);
                playerInCannon = false;
                yield break;
            }

                yield return null;

        }
    }
}
