using System.Collections;
using UnityEngine;

// Мигает спрайт красным когда Health получает урон
[RequireComponent(typeof(Health))]
public class HitFlash : MonoBehaviour
{
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    private Health hp;
    private SpriteRenderer sr;
    private Color originalColor;
    private int lastHp;

    private void Awake()
    {
        hp = GetComponent<Health>();
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
    }

    private void OnEnable()
    {
        hp.OnHealthChanged += OnHealthChanged;
    }

    private void Start()
    {
        // Берём актуальный HP после Awake всех компонентов
        lastHp = hp.Current;
    }

    private void OnDisable()
    {
        hp.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int current, int max)
    {
        // Мигаем только при получении урона, не при лечении
        if (current < lastHp)
            StartCoroutine(FlashRoutine());
        lastHp = current;
    }

    private IEnumerator FlashRoutine()
    {
        if (sr == null) yield break;
        sr.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }
}
