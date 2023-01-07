using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //game
    public GameObject playerPrefab;
    public GameObject finishPrefab;
    public Drawer drawer;
    Levels levels;
    GameObject player;
    int lvl_id;
    public bool starCollected = false;
    public bool noMoreLines = true;
    public bool gameStarted = false;
    public List<GameObject> obsaclePrefabs = new List<GameObject>();

    //ui
    public Button startB, reloadB, undoB, reverseB, startModeB;
    public GameObject lvlCompletedPanel, pausePanel, failPanel;
    public Button nextLvLB;
    public GameObject playerPointer;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        levels = new Levels();
        lvl_id = MenuManager.LvlSelected;
        SpawnObjects();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(drawer.line_index > 0)
        {
            undoB.interactable = true;
        }
        else
        {
            undoB.interactable = false;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
        }

        if(player.transform.position.y < -20)
        {
            Time.timeScale = 0;
            failPanel.SetActive(true);
        }
    }


    //buttons ------------------------------------------------------------------------------------------
    public void StartButton()
    {
        gameStarted = true;
        startB.gameObject.SetActive(false);
        reloadB.gameObject.SetActive(true);
        reverseB.interactable = false;
        startModeB.interactable = false;
        player.GetComponent<Player>().StartPlayer();
    }

    public void ReloadButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UndoButton()
    {
        drawer.UndoLine();
    }

    public void ReverseButton()
    {
        player.GetComponent<Player>().ReverseForce();
    }

    public void StartModeButton()
    {
        player.GetComponent<Player>().StartMode *= -1;
    }

    public void BackToMenuButton()
    {
        SceneManager.LoadScene("Menu");
    }

    public void UnPauseButton()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void LoadNextLvLButton()
    {
        MenuManager.LvlSelected = lvl_id + 1;
        SceneManager.LoadScene("Game");
    }

    // game -----------------------------------------------------------------------------------------------
    void SpawnObjects()
    {
        Levels.Level level = levels.getLevel(lvl_id);

        //player:
        Vector3 playerLoc = level.player.position;
        Quaternion playerRot = level.player.rotation;
        player = Instantiate(playerPrefab, playerLoc, playerRot);

        player.GetComponent<Player>().playerPointer = playerPointer;
        playerPointer.GetComponent<PlayerPointer>().player = player;

        //finish:
        Vector3 finishLoc = level.finish.position;
        Quaternion finishRot = level.finish.rotation;
        Instantiate(finishPrefab, finishLoc, finishRot);

        //obstacles:
        foreach (Levels.Obstacle obstacle in levels.getLevel(lvl_id).obstacles)
        {
            Instantiate(obsaclePrefabs[obstacle.type], obstacle.position, obstacle.rotation);
        }
    }

    public void LvLCompleted()
    {
        Time.timeScale = 0;
        if(lvl_id < levels.LevelsCount())
        {
            PlayerPrefs.SetInt("LvL" + (lvl_id + 1).ToString() + "Status", 1);
        }
        startB.interactable = false;
        reloadB.interactable = false;
        undoB.interactable = false;
        reverseB.interactable = false;
        startModeB.interactable = false;
        lvlCompletedPanel.SetActive(true);

        if(lvl_id == (levels.LevelsCount()-1))
        {
            nextLvLB.interactable = false;
        }

        lvlCompletedPanel.GetComponent<PanelBehavior>().SetStars(1);
        if(noMoreLines || starCollected)
        {
            lvlCompletedPanel.GetComponent<PanelBehavior>().SetStars(2);
        }
        if(noMoreLines && starCollected)
        {
            lvlCompletedPanel.GetComponent<PanelBehavior>().SetStars(3);
        }
    }
}
