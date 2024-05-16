using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Whether or not the game is currently paused.
    [SerializeField] private bool isPaused;

    #region EVENTS

    // An event called when the game is paused or unpaused.
    public delegate void Pause(bool _paused);

    // The event called when this object collides with a hitbox.
    public event Pause OnPause;

    #endregion

    // Gets the isPaused attribute.
    public bool IsPaused { get { return isPaused; } }

    // Gets called every frame.
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    // Pauses or unpauses the game.
    public void PauseGame()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1;
        }
        else
        {
            isPaused = true;
            Time.timeScale = 0;
        }

        this.OnPause(isPaused);
    }
}