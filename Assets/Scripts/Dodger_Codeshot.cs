using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Dodger_Codeshot : MonoBehaviour
{
    private Dodger_HeadForward headForward;
    public DOTweenAnimation tween;
    public GameObject subObject;

    private void Awake()
    {
        headForward = GetComponent<Dodger_HeadForward>();
    }

    private void OnEnable()
    {
        subObject.SetActive(true);
        headForward.Toggle(false);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        StartCoroutine(Cor_Shot());
    }
    private IEnumerator Cor_Shot()
    {
        yield return new WaitForSeconds(1f);
        headForward.Toggle(true);
        yield return new WaitForSeconds(1f);
        headForward.Toggle(false);
        yield return new WaitForSeconds(1f);
        tween.DORestart();


    }

    public void DisableObject()
    {
        subObject.SetActive(false);
    }

}
