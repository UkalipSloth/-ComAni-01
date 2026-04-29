using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public enum Choice { None, Scissors, Rock, Paper }

    [Header("Display Images")]
    public Image imagePlayer;
    public Image imageComputer;

    [Header("Sprites")]
    public Sprite spriteScissors;
    public Sprite spriteRock;
    public Sprite spritePaper;

    [Header("UI Texts")]
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    [Header("UI Buttons")]
    public Button buttonScissors;
    public Button buttonRock;
    public Button buttonPaper;

    [Header("Game Over UI (Panel)")]
    public GameObject gameOverPanel;
    public Button buttonRestartRound; // 추가: 점수를 유지하고 "다시하기"
    public Button buttonResetAll;    // "전체 리셋" (점수까지 초기화)
    public Button buttonQuit;

    [Header("Game Settings")]
    public float timeLimit = 5f;

    private bool isAnimating = true;
    private float animationInterval = 0.1f;
    private float animationTimer = 0f;
    private int currentSpriteIndex = 0;
    private Sprite[] sprites;

    private int playerScore = 0;
    private int computerScore = 0;
    private float currentTimer;
    private bool isGameOver = false;

    void Start()
    {
        sprites = new Sprite[] { spriteScissors, spriteRock, spritePaper };

        // 버튼 리스너 연결
        buttonScissors.onClick.AddListener(() => OnPlayerChoice(Choice.Scissors));
        buttonRock.onClick.AddListener(() => OnPlayerChoice(Choice.Rock));
        buttonPaper.onClick.AddListener(() => OnPlayerChoice(Choice.Paper));

        // [다시하기] 버튼: 점수 유지
        if (buttonRestartRound != null) buttonRestartRound.onClick.AddListener(RestartRound);

        // [리셋하기] 버튼: 점수 초기화
        if (buttonResetAll != null) buttonResetAll.onClick.AddListener(ResetFullGame);

        if (buttonQuit != null) buttonQuit.onClick.AddListener(QuitGame);

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateScoreUI();
        RestartRound();
    }

    void Update()
    {
        if (isAnimating && !isGameOver)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationInterval)
            {
                animationTimer = 0f;
                currentSpriteIndex = (currentSpriteIndex + 1) % 3;
                imagePlayer.sprite = sprites[currentSpriteIndex];
                imageComputer.sprite = sprites[(currentSpriteIndex + 1) % 3];
            }

            currentTimer -= Time.deltaTime;
            timerText.text = $"시간: {Mathf.Max(0, currentTimer):F1}s";

            if (currentTimer <= 0) TimeOut();
        }
    }

    void OnPlayerChoice(Choice choice)
    {
        if (!isAnimating || isGameOver) return;
        isAnimating = false;

        Choice computerChoice = GetComputerChoice();
        imagePlayer.sprite = GetSpriteFromChoice(choice);
        imageComputer.sprite = GetSpriteFromChoice(computerChoice);

        string result = DetermineWinner(choice, computerChoice);
        resultText.text = result;

        if (result == "플레이어 승리!") playerScore++;
        else if (result == "컴퓨터 승리!") computerScore++;

        UpdateScoreUI();
        Invoke("ShowGameOverUI", 0.5f);
    }

    void TimeOut()
    {
        isAnimating = false;
        isGameOver = true;
        resultText.text = "시간 초과!";
        computerScore++;
        UpdateScoreUI();
        ShowGameOverUI();
    }

    void ShowGameOverUI()
    {
        isGameOver = true;
        SetButtonsInteractable(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    // 모든 게임 초기화 (점수 0)
    public void ResetFullGame()
    {
        playerScore = 0;
        computerScore = 0;
        UpdateScoreUI();
        RestartRound();
    }

    // ★ 다시하기 (점수 유지)
    public void RestartRound()
    {
        isAnimating = true;
        isGameOver = false;
        currentTimer = timeLimit;
        resultText.text = "안내면 진거 가위바위보!";

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        SetButtonsInteractable(true);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    Sprite GetSpriteFromChoice(Choice choice)
    {
        switch (choice)
        {
            case Choice.Scissors: return spriteScissors;
            case Choice.Rock: return spriteRock;
            case Choice.Paper: return spritePaper;
            default: return spriteRock;
        }
    }

    Choice GetComputerChoice()
    {
        return (Choice)(Random.Range(0, 3) + 1);
    }

    string DetermineWinner(Choice player, Choice computer)
    {
        if (player == computer) return "무승부!";
        bool playerWins = (player == Choice.Scissors && computer == Choice.Paper) ||
                          (player == Choice.Rock && computer == Choice.Scissors) ||
                          (player == Choice.Paper && computer == Choice.Rock);
        return playerWins ? "플레이어 승리!" : "컴퓨터 승리!";
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Player: {playerScore} | CPU: {computerScore}";
    }

    void SetButtonsInteractable(bool state)
    {
        buttonScissors.interactable = buttonRock.interactable = buttonPaper.interactable = state;
    }
}