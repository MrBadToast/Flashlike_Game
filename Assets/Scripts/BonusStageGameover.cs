using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BonusStageGameover : MonoBehaviour
{
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;

    public void Restart()
    {
        SceneLoader.Instance.LoadNewScene(SceneManager.GetActiveScene().name);
    }

    private void OnEnable()
    {
        currentScoreText.text = DodgePatternManager.Instance.BonusStageCurrentScore.ToString() + "Á¡";

        float highScore = PlayerPrefs.GetFloat("BonusStageHighScore", 0);

        if (DodgePatternManager.Instance.BonusStageCurrentScore > highScore)
        {
            highScore = DodgePatternManager.Instance.BonusStageCurrentScore;
            PlayerPrefs.SetFloat("BonusStageHighScore", highScore);
        }

        highScoreText.text = highScore.ToString() + "Á¡";
    }
}
