using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    [Header("Settings")]

    [SerializeField] GameObject normalSword;
    [SerializeField] GameObject spear;
    [SerializeField] GameObject quickDraw;

    bool optionsActive = false;
    bool inforActive = false;
    public static bool pause = false;

    SceneLoader sceneLoader;
    Player1 player;
    Singleton singleton;

    private void Start()
    {
        pauseMenu.SetActive(false);
        //optionsMenu.SetActive(false);
        //gameInfoMenu.SetActive(false);

        sceneLoader = FindFirstObjectByType<SceneLoader>();
        player = FindFirstObjectByType<Player1>();
    }

    private void Update()
    {
        #region esc Functions

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            //if (optionsActive)
            //{

            //    optionsActive = false;
            //    optionsMenu.SetActive(false);

            //    pause = true;
            //    pauseMenu.SetActive(true);

            //    return;

            //}

            //if (inforActive)
            //{

            //    inforActive = false;
            //    gameInfoMenu.SetActive(false);

            //    pause = true;
            //    pauseMenu.SetActive(true);

            //    return;

            //}

            if (!pause)
            {
                //if (Cursor.lockState == CursorLockMode.None)
                //{
                //    activeCursor = true;
                //}
                //else
                //{
                //    activeCursor = false;
                //}
                //Cursor.lockState = CursorLockMode.None;
                //Cursor.visible = true;

                Time.timeScale = 0;
                pauseMenu.SetActive(true);
                pause = true;

            }
            else
            {
                //if (!activeCursor)
                //{
                //    Cursor.visible = false;
                //}

                Time.timeScale = 1;
                pauseMenu.SetActive(false);
                pause = false;

            }
        }

        #endregion
    }

    public void Home()
    {
        sceneLoader.ChangeScene(0);
    }

    public void Resume()
    {
        //if (!activeCursor)
        //{
        //    Cursor.visible = false;
        //}

        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        pause = false;

    }

    #region Options

    //public void Options()
    //{

    //    optionsActive = true;
    //    optionsMenu.SetActive(true);

    //    pause = false;
    //    pauseMenu.SetActive(false);

    //}

    public void ChangeToNormalSword()
    {

        singleton = FindAnyObjectByType<Singleton>();
        singleton.ChangePlayerWeapon(1);

    }

    public void ChangeToSpear()
    {

        singleton = FindAnyObjectByType<Singleton>();
        singleton.ChangePlayerWeapon(2);

    }

    public void ChangeToQuickDraw()
    {

        singleton = FindAnyObjectByType<Singleton>();
        singleton.ChangePlayerWeapon(3);

    }

    #endregion

    //public void Info()
    //{

    //    inforActive = true;
    //    gameInfoMenu.SetActive(true);

    //    pause = false;
    //    pauseMenu.SetActive(false);

    //}

    //public void goBackToPauseMenu()
    //{

    //    optionsMenu.SetActive(false);
    //    optionsActive = false; //L�gg till om mer menyer

    //    inforActive = false;


    //    pauseMenu.SetActive(true);
    //    pause = true;

    //}

    public void ResetScene()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        pause = false;

        sceneLoader.ReloadScene();
    }
}
