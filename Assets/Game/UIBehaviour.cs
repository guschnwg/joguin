using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIBehaviour : MonoBehaviour
{
    public TextMeshProUGUI score;
    public TextMeshProUGUI time;

    public GameState gameState;

    // Update is called once per frame
    void Update()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(gameState.time);
        time.text = ((int)gameState.time).ToString();

        score.text = gameState.score.ToString();
    }
}
