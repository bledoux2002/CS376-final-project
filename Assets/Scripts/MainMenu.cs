using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private GameObject MainMenuObject;
    [SerializeField] private GameObject InstructionsMenu;
    
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void DisplayInstructions()
    {
        MainMenuObject.SetActive(false);
        InstructionsMenu.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        InstructionsMenu.SetActive(false);
        MainMenuObject.SetActive(true);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
