using UnityEngine;
using UnityEngine.Events;

public class TriggerObject : MonoBehaviour
{
    public LayerMask targetLayer;
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public bool tirggerOnce = false;

    bool triggered = false;
    bool triggerExit = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            if (tirggerOnce && triggered) return;

            triggered = true;
            onTriggerEnter?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            if (tirggerOnce && triggerExit) return;

            triggerExit = true;
            onTriggerExit?.Invoke();
        }
    }

}
