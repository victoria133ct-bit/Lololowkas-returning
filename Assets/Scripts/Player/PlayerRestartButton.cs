using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRestartButton : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}