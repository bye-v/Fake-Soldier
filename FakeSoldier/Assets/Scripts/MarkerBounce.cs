using UnityEngine;

public class MarkerBounce : MonoBehaviour
{
    [SerializeField] float bounceHeight = 0.25f;
    [SerializeField] float bounceSpeed  = 2.5f;
    [SerializeField] float pulseMin     = 0.75f;

    Vector3 basePos;
    SpriteRenderer sr;

    void Start()
    {
        basePos = transform.position;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float t = Mathf.Sin(Time.time * bounceSpeed);
        transform.position = basePos + Vector3.up * (t * bounceHeight);
        if (sr != null)
        {
            float alpha = Mathf.Lerp(pulseMin, 1f, (t + 1f) * 0.5f);
            var c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
}
