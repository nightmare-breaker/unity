using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManage_stage1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject menuPanel;

    public void ShowMenu()
    {
        menuPanel.SetActive(true);
    }
    public void StartGame()
    {
        string savedScene = PlayerPrefs.GetString("SavedScene", "");

        if (!string.IsNullOrEmpty(savedScene))
        {
            SceneManager.LoadScene(savedScene);
        }
        else
        {
            SceneManager.LoadScene("Stage1");
        }
    }

    // Update is called once per frame
    public void ExitGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void SaveGame() // 호출 시 현재 씬 저장
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("SavedScene", currentScene);
        PlayerPrefs.Save();
    }
}
