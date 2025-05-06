using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverTrigger : MonoBehaviour
{
    public void TriggerGameOver()
    {
        // 현재 씬 이름 저장
        PlayerPrefs.SetString("LastStage", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        // GameOver 씬으로 이동
        SceneManager.LoadScene("GameOver");
    }
}