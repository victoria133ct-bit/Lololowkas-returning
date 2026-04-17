using UnityEngine;

public class PlayerGameOver : MonoBehaviour
{
    public GameObject gameOverCanvas;

    private void Start()
    {
        gameOverCanvas.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverCanvas.SetActive(true);
    }
}