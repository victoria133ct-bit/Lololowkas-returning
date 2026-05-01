using UnityEngine;

// Тряска камеры. Работает даже с Cinemachine — применяет shake после него.
// Для использования: CameraShake.Instance.Shake(duration, magnitude)


[DefaultExecutionOrder(1000)] 
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    // Текущее смещение от тряски — параллакс вычитает его чтобы не дёргался
    public Vector3 ShakeOffset { get; private set; }

    private float shakeDuration;
    private float shakeMagnitude;
    private float shakeTimer;

    private void Awake()
    {
        Instance = this;
    }

    public void Shake(float duration = 0.15f, float magnitude = 0.15f)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeTimer = duration;
    }

    private void LateUpdate()
    {
        if (shakeTimer > 0)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            ShakeOffset = new Vector3(x, y, 0);

            // Прибавляем смещение к текущей позиции (которую уже задал Cinemachine)
            transform.position += ShakeOffset;

            shakeTimer -= Time.deltaTime;
        }
        else
        {
            ShakeOffset = Vector3.zero;
        }
    }
}
