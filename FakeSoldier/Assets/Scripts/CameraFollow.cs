using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    [SerializeField] Transform target;
    [SerializeField] float smoothSpeed = 6f;
    [SerializeField] bool followX = true;
    [SerializeField] bool followY = false;

    Camera cam;
    float minX, maxX, minY, maxY;
    bool boundsReady;

    Vector3 shakeOffset;
    Coroutine shakeCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        if (target == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) target = p.transform;
        }
        RefreshBounds();
    }

    public void RefreshBounds()
    {
        var bg = GameObject.Find("Background");
        if (bg == null) return;
        var sr = bg.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        float halfW = cam.orthographicSize * cam.aspect;
        float halfH = cam.orthographicSize;
        minX = sr.bounds.min.x + halfW;
        maxX = sr.bounds.max.x - halfW;
        minY = sr.bounds.min.y + halfH;
        maxY = sr.bounds.max.y - halfH;
        boundsReady = (maxX > minX);
    }

    public void Shake(float duration = 0.35f, float magnitude = 0.15f)
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(DoShake(duration, magnitude));
    }

    IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float strength = Mathf.Lerp(magnitude, 0f, elapsed / duration);
            var offset = Random.insideUnitCircle * strength;
            shakeOffset = new Vector3(offset.x, offset.y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        shakeOffset = Vector3.zero;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 pos = transform.position;
        if (followX) pos.x = target.position.x;
        if (followY) pos.y = target.position.y;
        if (boundsReady) pos.x = Mathf.Clamp(pos.x, minX, maxX);

        var basePos = Vector3.Lerp(transform.position, pos, smoothSpeed * Time.deltaTime);

        float finalX = basePos.x + shakeOffset.x;
        float finalY = basePos.y + shakeOffset.y;

        // 쉐이크 포함 최종 위치도 배경 범위 안으로 클램프
        if (boundsReady)
        {
            finalX = Mathf.Clamp(finalX, minX, maxX);
            // Y 범위가 존재할 때만 클램프 (배경이 카메라보다 클 때)
            if (maxY > minY) finalY = Mathf.Clamp(finalY, minY, maxY);
            else             finalY = basePos.y; // 여유 없으면 Y 쉐이크 무시
        }

        transform.position = new Vector3(finalX, finalY, basePos.z);
    }
}
