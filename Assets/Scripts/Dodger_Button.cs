using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Dodger_Button : MonoBehaviour
{
    public Sprite activeSprite;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D buttonCollider;
    public AudioClip acitveSound;
    public AnimationCurve attackAnimationCurve;
    public GameObject textObject;

    private bool active = false;
    private AudioSource audioSource;

    private DOTweenAnimation tween;

    private void Awake()
    {
        tween = GetComponent<DOTweenAnimation>();
        audioSource = GetComponent<AudioSource>();
    }

    public void DestroyButton()
    {
        DodgePatternManager.buttonSpawned = null;
        Destroy(gameObject);
    }

    public void ButtonAttack()
    {
        buttonCollider.enabled = false;
        StartCoroutine(Cor_AnimateAttack());
        DodgePatternManager.Instance.AttackSuccesful();

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!active) return;
            ButtonAttack();
        }

        if(collision.gameObject.CompareTag("CodeObjects"))
        {
            if (active) return;
            active = true;

            textObject.SetActive(false);

            collision.rigidbody.gameObject.GetComponent<Dodger_Codeshot>().DisableObject();

            spriteRenderer.sprite = activeSprite;
            audioSource.PlayOneShot(acitveSound);
            tween.DOPlayAllById("AttackButtonActive");
        }

    }

    IEnumerator Cor_AnimateAttack()
    {
        float elapsed = 0f;
        float duration = 1f;

        Vector3 originPosition = transform.position;
        float targetRotation = 2000f;

        while (elapsed < duration)
        {
            transform.position = Vector3.LerpUnclamped(originPosition, Vector3.zero , attackAnimationCurve.Evaluate(elapsed / duration));
            transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpUnclamped(0, targetRotation, attackAnimationCurve.Evaluate(elapsed / duration)));

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        DestroyButton();
    }

}
