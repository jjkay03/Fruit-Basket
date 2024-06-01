using UnityEngine;
using TMPro;

public class GameVersionDisplay : MonoBehaviour {
    public TextMeshProUGUI textGameVersion;
    void Start() { textGameVersion.text = Application.productName + " " + Application.version; }
}
