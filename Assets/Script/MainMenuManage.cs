using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject optionPanel;

    // Update is called once per frame
    public void SelectStage()
    {
        SceneManager.LoadScene("Select_stage");
    }

    public void ShowOption()
    {
        optionPanel.SetActive(true);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void CloseOptionPanel()
    {
        optionPanel.SetActive(false);
    }
}
