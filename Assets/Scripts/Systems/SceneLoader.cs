using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public bool playerDead = false;

    private void Update()
    {

        if (playerDead && Input.GetKey(KeyCode.R))
        {
            ReloadScene();
        }

    }

    public void ChangeScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void ReloadScene()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Left game");
        Application.Quit();
    }

    public void LoadNextScene()
    {
        int nextSceneIndex = LoopBuildIndex(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene(nextSceneIndex);
    }

    private int LoopBuildIndex(int buildIndex)
    {
        if (buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            buildIndex = 0;
        }

        return buildIndex;
    }
}
