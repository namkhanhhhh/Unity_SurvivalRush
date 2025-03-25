using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetGameState(GameState.MENU);
    }
    public void StartGame() => SetGameState(GameState.GAME);
    public void StartWeaponSelection() => SetGameState(GameState.WEAPONSELECTION);
    public void StartShop() => SetGameState(GameState.SHOP);


    public void SetGameState(GameState gameState)
    {
        // Log the game state change
        Debug.Log($"Changing game state to: {gameState}");
        
        IEnumerable<IGameStateListener> gameStateListeners =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IGameStateListener>();

        foreach (IGameStateListener gameStateListener in gameStateListeners)
            gameStateListener.GameStateChangedCallback(gameState);

    }

    public void ManageGameOver()
    {
        SceneManager.LoadScene(0);
    }

    public void WaveCompletedCallback()
    {
        // Reset game state to ensure clean transition
        if (Player.instance.HasLeveledUp())
        {
            SetGameState(GameState.WAVETRANSITION);
        }
        else if (WaveTransitionManager.instance.HasCollectedChest())
        {
            SetGameState(GameState.WAVETRANSITION);
        }
        else
        {
            SetGameState(GameState.SHOP);
        }
    }
}
public interface IGameStateListener
{
    void GameStateChangedCallback(GameState gameState);
}
