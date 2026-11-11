using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasManager : MonoBehaviour
{
    public GameObject GameOverPanel;
    public static event Action OnRetry;

    private void OnEnable()
    {
        PlayerStats.OnDeath += ShowGameOver;
    }

    private void OnDisable()
    {
        PlayerStats.OnDeath -= ShowGameOver;
    }

    void ShowGameOver()
    {
        GameOverPanel.SetActive(true);
    }

    public void Retry()
    {
        Cursor.lockState = CursorLockMode.Locked;
        GameOverPanel.SetActive(false);
        GameController.GetGameController().Restart();
        OnRetry?.Invoke();
    }
}
