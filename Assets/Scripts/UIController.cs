using System.Security.Cryptography;
using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour
{
    public static UIController Instance; 
    public Transform MainMenuPanel;
    public Transform RestartPanel;
    public Transform Background;

    public UIToggle AiTumbler;
    public UILabel ButtonSound;
    public UILabel ScoreLabel;
    public UILabel WinLabel;
    public UILabel TurnLabel; 
    private bool _mute;

    private void Start()
    {
        Instance = this;
        _mute = false;
    }
    public void Play()
    { 
        GameManager.Instance.Lights.gameObject.SetActive(true);
        Background.gameObject.animation.Play("Splash");
        MainMenuPanel.gameObject.SetActive(false);
        GameManager.Instance.AImode = AiTumbler.value;
        RestartGame();
    }
    public void SoundTumbler()
    {
        if (!_mute)
        {
            ButtonSound.text = "Sound off";
            SoundManager.Instance.SoundTumbler();
            _mute = !_mute;
        }
        else
        {
            ButtonSound.text = "Sound on";
            SoundManager.Instance.SoundTumbler();
            _mute = !_mute;
        }
    }
    public void RestartGame()
    {
        RestartPanel.gameObject.SetActive(false);
        foreach (var item in GameManager.Instance.Items)
        {
            item.Value.gameObject.animation.Play("Item_Y_rotateBACK");
            item.Value.gameObject.GetComponent<MeshRenderer>().material.color= Color.white;
        }
        GameManager.Instance.GameMatrix = new int[GameManager.Instance.X, GameManager.Instance.Y];
        GameManager.Instance.GameStarted = true;
        GameManager.Instance.turn = true;
        GameManager.Instance.MovsCount = 0;
        UpdateTurn();
    }
    public void OpenMenu()
    {
        GameManager.Instance.GameStarted = false; 
        Background.gameObject.animation.Play("SplashBack");
        MainMenuPanel.gameObject.SetActive(true); 
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void WinPanel(int type)
    {
        if (type == 1)
        {
            WinLabel.text = "X\nWIN";
        }
        if (type == 2)
        {
            WinLabel.text = "O\nWIN";
        }
        if (type == 3)
        {
            WinLabel.text = "Draw";
        }
        RestartPanel.gameObject.SetActive(true);
        UpdateScore();
        UpdateTurn();
        GameManager.Instance.MovsCount = 0;
        GameManager.Instance.GameStarted = false;
    }
    public void ResetScore()
    {
        GameManager.Instance.Score = new int[2];
        UpdateScore();
    }
    public void UpdateScore()
    {
        ScoreLabel.text="Score: X("+GameManager.Instance.Score[0]+") O("+GameManager.Instance.Score[1]+")";
    }
    public void UpdateTurn()
    {
        TurnLabel.text = GameManager.Instance.turn ? "Turn:X" : "Turn:O";
    }
}