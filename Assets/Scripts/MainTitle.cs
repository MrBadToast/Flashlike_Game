using UnityEngine;

public class MainTitle : MonoBehaviour
{
    public GameObject bonusmodeButton;

    private void Start()
    {
        if (PlayerPrefs.GetInt("BonusModeUnlocked", 0) == 1)
        {
            bonusmodeButton.SetActive(true);
        }
        else
        {
            bonusmodeButton.SetActive(false);
        }
    }

    public void StartBonusGame()
    {
        SceneLoader.Instance.LoadNewScene("Scene_Bonus");
    }

    public void StartGame()
    {
        SceneLoader.Instance.LoadNewScene("Scene2");
    }

}
