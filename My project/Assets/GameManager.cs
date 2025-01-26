using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float totalSeconds;
    public Text timer;

    public Text enemyScore1;
    public Text enemyScore2;

    public Zone[] zones;

    int player1Cont;
    int player2Cont;

    public GameObject endPannel;

    public GameObject hudPanned;
    public Text which;

    public int maxPointDiference;

    public Waves[] waves;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        totalSeconds -= Time.deltaTime;

        int minutes = (int)totalSeconds / 60; // Get the number of minutes
        int seconds = (int)totalSeconds % 60; // Get the remaining seconds

        string timeFormatted = $"{minutes:D2}:{seconds:D2}"; // Format into mm:ss

        timer.text = timeFormatted;

        if(totalSeconds<=0)
            EndGame();
        
        if(Time.timeScale == 0 && Input.anyKey)
            RestartGame();
    }

    void FixedUpdate() 
    {
        if(zones[0].isHere)
        {
            player1Cont++;
            enemyScore1.text = player1Cont +"";
        }
        if(zones[1].isHere)
        {
            player2Cont++;
            enemyScore2.text = player2Cont +"";
        }

        if(player1Cont-player2Cont>maxPointDiference)
            waves[1].enabled = true;
        else if(player2Cont-player1Cont>maxPointDiference)
            waves[0].enabled = true;
    }

    void EndGame()
    {
        hudPanned.SetActive(false);
        which.text = player1Cont>player2Cont?"Crab 1 won with " + player1Cont:"Crab 2 won with " + player2Cont;
        endPannel.SetActive(true);
        Time.timeScale = 0;
    }


    void RestartGame()
    {
        //END
        Time.timeScale = 1;
;
        SceneManager.LoadScene(0);

    }
}
