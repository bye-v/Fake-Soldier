using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StageDirector : MonoBehaviour
{
    protected PlayerController player;
    protected EventTriggerZone eventZone;

    protected virtual void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        eventZone = FindFirstObjectByType<EventTriggerZone>();
        if (player) player.SetCanMove(false);
        StartCoroutine(RunStage());
    }

    protected abstract IEnumerator RunStage();

    protected IEnumerator PlayDialogue(IEnumerable<DialogueLine> lines)
    {
        bool done = false;
        DialogueManager.Instance.StartDialogue(lines, () => done = true);
        yield return new WaitUntil(() => done);
    }

    protected IEnumerator ShowChoice(ChoiceData[] choices, System.Action<int> onResult, float timer = 0f)
    {
        int chosen = -1;
        ChoiceSystem.Instance.Show(choices, idx => { chosen = idx; onResult?.Invoke(idx); }, timer);
        yield return new WaitUntil(() => chosen >= 0);
    }

    // 플레이어가 EventTriggerZone에 진입할 때까지 대기
    protected IEnumerator WaitForEventZone()
    {
        if (eventZone == null) { yield return new WaitForSeconds(2f); yield break; }
        bool triggered = false;
        eventZone.GetComponent<UnityEngine.Events.UnityEvent>();
        // EventTriggerZone 이벤트 리스너로 감지
        var zoneTrigger = eventZone.GetComponent<EventTriggerZone>();
        if (zoneTrigger != null)
            zoneTrigger.onPlayerEnterCallback = () => triggered = true;
        yield return new WaitUntil(() => triggered);
    }

    protected IEnumerator Wait(float seconds) { yield return new WaitForSeconds(seconds); }

    protected void AllowMove() { if (player) player.SetCanMove(true); }
    protected void LockMove()  { if (player) player.SetCanMove(false); }

    protected void NextScene(string sceneName) => GameManager.Instance.LoadScene(sceneName);
    protected void GoEnding() => GameManager.Instance.LoadEnding();
}
