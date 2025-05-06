using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManage : MonoBehaviour
{
    private string lastStageName;

    void Start()
    {
        // 저장된 이전 씬 이름 불러오기
        lastStageName = PlayerPrefs.GetString("LastStage", "Stage1"); // 기본값: Stage1
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 이전 스테이지로 이동
            SceneManager.LoadScene(lastStageName);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(lastStageName);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main");
    }
}
