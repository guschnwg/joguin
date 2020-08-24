using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    public GameState gameState;

    // Start is called before the first frame update
    void Start()
    {
        gameState.time = 90f;
        gameState.score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState.time > 0)
        {
            gameState.time -= Time.deltaTime;
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
