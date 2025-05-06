using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManage : MonoBehaviour
{
    private string lastStageName;

    void Start()
    {
        // ����� ���� �� �̸� �ҷ�����
        lastStageName = PlayerPrefs.GetString("LastStage", "Stage1"); // �⺻��: Stage1
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // ���� ���������� �̵�
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
