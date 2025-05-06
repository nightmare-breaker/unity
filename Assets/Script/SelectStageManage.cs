using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectStageManage : MonoBehaviour
{
    public void SelectFirst()
    {
        SceneManager.LoadScene("Stage1");
    }
    public void SelectFinal()
    {
        SceneManager.LoadScene("Final_stage");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main");
    }

}
