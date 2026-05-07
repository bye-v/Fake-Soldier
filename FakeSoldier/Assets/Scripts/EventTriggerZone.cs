using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class EventTriggerZone : MonoBehaviour
{
    [SerializeField] UnityEvent onPlayerEnter;
    [SerializeField] bool triggerOnce = true;

    // StageDirector가 등록하는 코드 콜백
    public Action onPlayerEnterCallback;

    bool triggered;

    void Awake() => GetComponent<Collider2D>().isTrigger = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggerOnce && triggered) return;
        triggered = true;
        onPlayerEnter?.Invoke();
        onPlayerEnterCallback?.Invoke();
    }

    public void Reset() => triggered = false;
}
