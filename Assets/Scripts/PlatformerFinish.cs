using UnityEngine;

public class PlatformerFinish : MonoBehaviour
{
    public string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneLoader.Instance.LoadNewScene(sceneToLoad);
        }
    }
}
