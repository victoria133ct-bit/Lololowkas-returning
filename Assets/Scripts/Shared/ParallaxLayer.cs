using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public float parallaxFactor = 0.1f;

    private Transform cam;
    private Vector3 startOffset;

    void Start()
    {
        cam = Camera.main.transform;
        startOffset = transform.position - cam.position;
    }

    void LateUpdate()
    {
        Vector3 camPos = cam.position;
        if (CameraShake.Instance != null)
            camPos -= CameraShake.Instance.ShakeOffset;

        float lagX = camPos.x * parallaxFactor;
        transform.position = new Vector3(
            camPos.x + startOffset.x - lagX,
            startOffset.y + camPos.y,
            transform.position.z
        );
    }
}
