using DG.Tweening;
using UnityEngine;

public class PlatformerCheckpoint : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite active;

    bool activated = false;
    AudioSource audioSource;
    DOTweenAnimation tweenAnimation;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) return;

        if (collision.CompareTag("Player"))
        {
            Player_Platformer.Instance.lastCheckPoint = transform;
            Activate();
        }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        tweenAnimation = GetComponent<DOTweenAnimation>();
    }

    public void Activate()
    {
        spriteRenderer.sprite = active;
        activated = true;
        audioSource.Play();
        tweenAnimation.DOPlay();;
    }
}


