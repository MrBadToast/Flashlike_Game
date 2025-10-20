using TMPro;
using UnityEngine;

public class CurrentTime : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    void Update()
    {
        string now = System.DateTime.Now.ToString("HH:mm");
        timeText.text = now;
    }
}
