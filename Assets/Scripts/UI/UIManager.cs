using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject ReloadingText;
    public TextMeshProUGUI AmmoText;

    private static UIManager s_instance;

    public static UIManager Instance
    {
        get
        {
            if (s_instance == null)
                Debug.LogError("Game Manager is null");

            return s_instance;
        }
    }

    private void Awake()
    {
        s_instance = this;
    }
}
