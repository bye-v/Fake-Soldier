using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothSpeed = 6f;
    [SerializeField] bool followX = true;
    [SerializeField] bool followY = false;

    Camera cam;
    float minX, maxX;
    bool boundsReady;

    void Awake()
    {
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
        minX = sr.bounds.min.x + halfW;
        maxX = sr.bounds.max.x - halfW;
        boundsReady = (maxX > minX);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 pos = transform.position;

        if (followX) pos.x = target.position.x;
        if (followY) pos.y = target.position.y;

        if (boundsReady)
            pos.x = Mathf.Clamp(pos.x, minX, maxX);

        transform.position = Vector3.Lerp(transform.position, pos, smoothSpeed * Time.deltaTime);
    }
}
