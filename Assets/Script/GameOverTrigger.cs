using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverTrigger : MonoBehaviour
{
    public void TriggerGameOver()
    {
        // ���� �� �̸� ����
        PlayerPrefs.SetString("LastStage", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        // GameOver ������ �̵�
        SceneManager.LoadScene("GameOver");
    }
}