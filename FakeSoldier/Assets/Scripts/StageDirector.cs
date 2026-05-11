using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StageDirector : MonoBehaviour
{
    [SerializeField] protected AudioClip stageBGM;

    protected PlayerController player;
    protected EventTriggerZone eventZone;

    protected virtual void Start()
    {
        player    = FindFirstObjectByType<PlayerController>();
        eventZone = FindFirstObjectByType<EventTriggerZone>();
        if (player) player.SetCanMove(false);
        if (stageBGM != null && AudioManager.Instance != null)
            AudioManager.Instance.PlayBGM(stageBGM);
        StartCoroutine(RunStage());
    }

    protected abstract IEnumerator RunStage();

    protected IEnumerator PlayDialogue(IEnumerable<DialogueLine> lines)
    {
        bool wasMovable = player != null && player.CanMove;
        LockMove();
        bool done = false;
        DialogueManager.Instance.StartDialogue(lines, () => done = true);
        yield return new WaitUntil(() => done);
        if (wasMovable) AllowMove();
    }

    protected IEnumerator ShowChoice(ChoiceData[] choices, System.Action<int> onResult, float timer = 0f)
    {
        int chosen = -1;
        ChoiceSystem.Instance.Show(choices, idx => { chosen = idx; onResult?.Invoke(idx); }, timer);
        yield return new WaitUntil(() => chosen >= 0);
    }

    protected IEnumerator WaitForEventZone()
    {
        if (eventZone == null) { yield return new WaitForSeconds(2f); yield break; }
        bool triggered = false;
        eventZone.onPlayerEnterCallback = () => triggered = true;
        yield return new WaitUntil(() => triggered);
    }

    protected IEnumerator Wait(float seconds) { yield return new WaitForSeconds(seconds); }

    protected IEnumerator WaitUntilPlayerNear(float worldX, float threshold = 2f)
    {
        yield return new WaitUntil(() => player != null && player.transform.position.x >= worldX - threshold);
    }

    // 날짜·장소 타이틀 카드를 화면에 잠깐 표시
    protected IEnumerator ShowStageTitle(string title, float holdTime = 1.8f)
    {
        if (SceneFader.Instance != null)
            yield return SceneFader.Instance.ShowTitle(title, holdTime);
    }

    // 카메라 흔들기
    protected void ShakeCamera(float duration = 0.35f, float magnitude = 0.15f)
    {
        if (CameraFollow.Instance != null)
            CameraFollow.Instance.Shake(duration, magnitude);
    }

    protected void AllowMove() { if (player) player.SetCanMove(true); }
    protected void LockMove()  { if (player) player.SetCanMove(false); }

    protected void NextScene(string sceneName)
    {
        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene(sceneName);
        else
            GameManager.Instance.LoadScene(sceneName);
    }

    protected void GoEnding()
    {
        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene(null, () => GameManager.Instance.LoadEnding());
        else
            GameManager.Instance.LoadEnding();
    }
}
