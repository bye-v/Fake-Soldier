using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button quitButton;
    [SerializeField] AudioClip menuBGM;

    void Start()
    {
        startButton?.onClick.AddListener(OnStart);
        quitButton?.onClick.AddListener(OnQuit);

        if (GameManager.Instance != null) GameManager.Instance.ResetGame();
        if (menuBGM != null && AudioManager.Instance != null)
            AudioManager.Instance.PlayBGM(menuBGM);
    }

    void OnStart()
    {
        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene("Stage_01");
        else
            SceneManager.LoadScene("Stage_01");
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
