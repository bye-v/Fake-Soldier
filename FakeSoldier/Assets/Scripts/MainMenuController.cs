using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button quitButton;
    [SerializeField] GameObject settingsPanel;

    void Start()
    {
        if (settingsPanel) settingsPanel.SetActive(false);

        startButton?.onClick.AddListener(OnStart);
        settingsButton?.onClick.AddListener(OnSettings);
        quitButton?.onClick.AddListener(OnQuit);

        // GameManager가 있으면 점수 초기화
        if (GameManager.Instance != null) GameManager.Instance.ResetGame();
    }

    void OnStart()
    {
        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene("Stage_01");
        else
            SceneManager.LoadScene("Stage_01");
    }

    void OnSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
