using UnityEngine;

public class VersionText : MonoBehaviour
{
    private void Start()
    {
        var version = Application.version;
        GetComponent<TMPro.TextMeshProUGUI>().text = "v " + version;
    }
}
