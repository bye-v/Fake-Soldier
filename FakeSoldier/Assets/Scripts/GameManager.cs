using UnityEngine;
using UnityEngine.SceneManagement;

public enum EndingType { Bad, Normal, True }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int ConscienceScore { get; private set; } = 0;
    public bool HasRefusedFiring { get; private set; } = false;

    // 엔딩 기준 (수정 가능)
    const int BAD_MAX = 2;
    const int NORMAL_MAX = 6;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore(int amount) => ConscienceScore += amount;

    public void SetFiringRefused() => HasRefusedFiring = true;

    public EndingType GetEnding()
    {
        // 발포를 직접 거부한 경우 최소 Normal 엔딩 보장
        if (ConscienceScore <= BAD_MAX)
            return HasRefusedFiring ? EndingType.Normal : EndingType.Bad;
        if (ConscienceScore <= NORMAL_MAX) return EndingType.Normal;
        return EndingType.True;
    }

    public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

    public void LoadEnding()
    {
        var ending = GetEnding();
        switch (ending)
        {
            case EndingType.Bad:    SceneManager.LoadScene("Ending_Bad");    break;
            case EndingType.Normal: SceneManager.LoadScene("Ending_Normal"); break;
            case EndingType.True:   SceneManager.LoadScene("Ending_True");   break;
        }
    }

    public void ResetGame()
    {
        ConscienceScore = 0;
        HasRefusedFiring = false;
    }
}
