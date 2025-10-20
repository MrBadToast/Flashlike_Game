using UnityEngine;
using UnityEngine.SceneManagement;

public class TimescaleManager : MonoBehaviour
{
    public void StopTime()
    {
        Time.timeScale = 0f;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    public void ResetGame()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadNewScene("Scene1");
    }

    public void ResetStage()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadNewScene(SceneManager.GetActiveScene().name);
    }

}
