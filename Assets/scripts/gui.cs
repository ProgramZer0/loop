using System;
using UnityEngine;
using UnityEngine.UI;

public class gui : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelMenu;
    [SerializeField] private GameObject timer;
    [SerializeField] Button start;
    [SerializeField] Button quit;
    [SerializeField] Button exit;
    [SerializeField] Button restartLevelButton;
    [SerializeField] Button[] levelSelector;
    [SerializeField] GameManger GM;
    [SerializeField] LevelController LC;
    [SerializeField] PlayerController player;

    private bool inMenu = false;
    private bool hitESC = false;
    private int levelSelected = 0;

    private void Start()
    {
        unlockCursor();

        start.onClick.AddListener(startGame);
        quit.onClick.AddListener(quitLevel);
        exit.onClick.AddListener(exitGame);
        restartLevelButton.onClick.AddListener(restartLevel);

        for (int i = 0; i < levelSelector.Length; i++)
        {
            int index = i; 
            levelSelector[i].onClick.AddListener(() => selectLevel(index));
        }
    }

    private void restartLevel()
    {
        LC.Restart();
    }

    private void exitGame()
    {
        Application.Quit();
    }

    private void quitLevel()
    {
        foreach (GameObject ghostn in GameObject.FindGameObjectsWithTag("Ghost"))
        {
            Destroy(ghostn);
        }

        levelSelected = 0;
        LC.startingArea();
        GM.resetTimer();
        closeMenu(pauseMenu);
        closeMenu(timer);
        openMenu(mainMenu);
    }

    private void startGame()
    {
        closeMenu(mainMenu);
        openMenu(levelMenu);
    }

    private void selectLevel(int level)
    {
        levelSelected = level;
        LC.stageNumber = levelSelected;
        LC.spawnLevel();
        mainMenu.SetActive(false);
        levelMenu.SetActive(false);
        timer.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) hitESC = true;

        inMenu = pauseMenu.activeSelf || mainMenu.activeSelf || levelMenu.activeSelf;

        if (inMenu)
            unlockCursor();
        else
            lockCursor();

        if (hitESC)
        {
            if (inMenu)
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
                lockCursor();
            }
            else
            {
                Time.timeScale = 0f;
                openMenu(pauseMenu);
            }
            hitESC = false;
        }
    }

    private void openMenu(GameObject menu)
    {
        menu.SetActive(true);
        unlockCursor();
    }
    private void closeMenu(GameObject menu)
    {
        menu.SetActive(false);
        lockCursor();
    }

    private void lockCursor()
    {
        player.lockMovement(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void unlockCursor()
    {
        player.lockMovement(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void setTimerGUIActive(bool val)
    {
        timer.SetActive(val);
    }
}
