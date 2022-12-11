using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrielPlotexiaPlayer : MonoBehaviour
{
    [SerializeField]
    private Player player;

    private void GameOver()
    {
        player.GameOver();
    }
}
