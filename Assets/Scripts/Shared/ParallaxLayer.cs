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
        float lagX = cam.position.x * parallaxFactor;
        float lagY = cam.position.y * parallaxFactor;
        transform.position = new Vector3(
            cam.position.x + startOffset.x - lagX,
            startOffset.y + cam.position.y,
            transform.position.z
        );
    }
}
