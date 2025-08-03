using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private Vector3[] stagePostions;
    [SerializeField] private GameObject[] level;
    [SerializeField] private GameManger GM;
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject[] levelObjs;

    public int stageNumber = 0;

    public bool unlocked = false;
    
    public void spawnLevel()
    {
        player.transform.position = stagePostions[stageNumber];
        unlocked = false;
        level[stageNumber].SetActive(true);
        GM.resetTimer();
        GM.startTimer();
    }


    public void startingArea()
    {
        player.transform.position = stagePostions[0];
    }

    public void Restart()
    {
        foreach (GameObject obj in levelObjs)
        {
            obj.SendMessage("Restart", SendMessageOptions.DontRequireReceiver);
        }

        player.transform.position = stagePostions[stageNumber];
        GM.resetTimer();
    }

    public void NextArea()
    {
        if(stageNumber == 11)
        {
            //end
        } 
        else
        {
            level[stageNumber].SetActive(false);
            stageNumber++;
            spawnLevel();
        }
    }
}
