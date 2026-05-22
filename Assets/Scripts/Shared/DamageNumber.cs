using System.Threading;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public TextMeshPro text;
    public float lifeTime = 1f;
    public float floatSpeed = 1.5f;
    public float fadeStartAt = 0.3f;
    private float timer;
    private Color startColor;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>();
        startColor = text.color;
    }
    public static void Spawn(GameObject prefab, Vector3 position, int damage)
    {
        if (prefab == null) return;

        // Случайное смещение чтобы числа не наслаивались
        Vector3 offset = new Vector3(
            Random.Range(-0.7f, 0.3f),
            Random.Range(0f, 0.2f),
            0
        );

        var obj = Instantiate(prefab, position + offset, Quaternion.identity);
        var dn = obj.GetComponent<DamageNumber>();
        if (dn != null) dn.SetDamage(damage);
    }

    private void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        float time = timer / lifeTime;

        if (time > fadeStartAt && text != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, (time - fadeStartAt) / (1f - fadeStartAt));
            var c = startColor;
            c.a = alpha;
            text.color = c;
        }
        if(timer >= lifeTime) { Destroy(gameObject); }
    }

    private void SetDamage(int damage)
    {
        text.text = damage.ToString();
    }
}
