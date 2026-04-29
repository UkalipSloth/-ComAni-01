using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager1 : MonoBehaviour
{
    public enum Choice { None, Scissors, Rock, Paper }
    const int WinScore = 5;

    [Header("UI Buttons - Player")]
    public Button buttonScissors;
    public Button buttonRock;
    public Button buttonPaper;

    [Header("UI Buttons - System")]
    public Button buttonRestart;
    public Button buttonExit;
    public Button buttonCloseTutorial;

    [Header("Display Images")]
    public Image imagePlayer;
    public Image imageComputer;
    public Sprite spriteQuestion;

    [Header("Sprites")]
    public Sprite spriteScissors;
    public Sprite spriteRock;
    public Sprite spritePaper;

    [Header("Movement Settings")]
    public float shakeSpeed = 5.0f;
    public float shakeAmount = 15.0f;
    private Vector3 posS, posR, posP;
    private Vector3 posImgP, posImgC;

    [Header("Gameplay Status UI")]
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI textScorePlayer;
    public TextMeshProUGUI textScoreComputer;

    [Header("UI Panels & Effects")]
    public GameObject panelResult;
    public TextMeshProUGUI textResultPanelStats;
    public GameObject panelTutorial;
    public CanvasGroup resultTextCanvasGroup;

    private int scorePlayer = 0;
    private int scoreComputer = 0;
    private int totalGames = 0;
    private bool gameEnded = false;
    private bool isAnimating = true;

    private float animationInterval = 0.1f;
    private float animationTimer = 0f;
    private int currentSpriteIndex = 0;
    private Sprite[] sprites;

    void Start()
    {
        sprites = new Sprite[] { spriteScissors, spriteRock, spritePaper };

        // 초기 위치 저장
        posS = buttonScissors.transform.localPosition;
        posR = buttonRock.transform.localPosition;
        posP = buttonPaper.transform.localPosition;
        posImgP = imagePlayer.transform.localPosition;
        posImgC = imageComputer.transform.localPosition;

        // 컴퓨터 이미지 마주보게 반전
        imageComputer.transform.localScale = new Vector3(-1, 1, 1);

        // 버튼 이벤트 연결
        buttonScissors.onClick.AddListener(() => OnPlayerChoice(Choice.Scissors));
        buttonRock.onClick.AddListener(() => OnPlayerChoice(Choice.Rock));
        buttonPaper.onClick.AddListener(() => OnPlayerChoice(Choice.Paper));
        buttonRestart.onClick.AddListener(OnRestart);
        buttonExit.onClick.AddListener(OnExit);

        if (buttonCloseTutorial != null)
            buttonCloseTutorial.onClick.AddListener(CloseTutorial);

        InitGame();
    }

    void Update()
    {
        if (gameEnded) return;

        AnimateEverything();

        // 선택 대기 중 컴퓨터 이미지 순환 애니메이션
        if (isAnimating)
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationInterval)
            {
                animationTimer = 0f;
                currentSpriteIndex = (currentSpriteIndex + 1) % 3;
                imageComputer.sprite = sprites[currentSpriteIndex];
            }
        }
    }

    // UI 요소들을 부드럽게 흔드는 애니메이션
    void AnimateEverything()
    {
        float time = Time.time * shakeSpeed;
        buttonScissors.transform.localPosition = posS + new Vector3(Mathf.Cos(time), Mathf.Sin(time), 0) * shakeAmount;
        buttonRock.transform.localPosition = posR + new Vector3(Mathf.Cos(time + 0.5f), Mathf.Sin(time + 0.5f), 0) * shakeAmount;
        buttonPaper.transform.localPosition = posP + new Vector3(Mathf.Cos(time + 1.0f), Mathf.Sin(time + 1.0f), 0) * shakeAmount;

        // 중앙 이미지는 위아래로만 둥둥 떠다님
        imagePlayer.transform.localPosition = posImgP + new Vector3(0, Mathf.Sin(time * 0.8f), 0) * (shakeAmount * 1.5f);
        imageComputer.transform.localPosition = posImgC + new Vector3(0, Mathf.Sin(time * 0.8f + 0.3f), 0) * (shakeAmount * 1.5f);
    }

    void InitGame()
    {
        gameEnded = false;
        scorePlayer = 0; scoreComputer = 0; totalGames = 0;
        UpdateScoreUI();

        if (panelResult != null) panelResult.SetActive(false);
        if (panelTutorial != null) panelTutorial.SetActive(true);

        isAnimating = true;
        imagePlayer.sprite = spriteQuestion;
        imageComputer.sprite = spriteQuestion;

        resultText.text = "가위, 바위, 보 중 하나를 선택하세요!";
        resultText.color = Color.white;
        if (resultTextCanvasGroup != null) resultTextCanvasGroup.alpha = 1f;

        SetButtonsInteractable(true);
    }

    public void CloseTutorial()
    {
        if (panelTutorial != null) panelTutorial.SetActive(false);
    }

    void OnPlayerChoice(Choice choice)
    {
        if (gameEnded) return;

        isAnimating = false;
        totalGames++;

        Choice computerChoice = (Choice)Random.Range(1, 4);
        imagePlayer.sprite = GetSpriteFromChoice(choice);
        imageComputer.sprite = GetSpriteFromChoice(computerChoice);

        // 결과 판정 및 메시지 출력
        DetermineWinner(choice, computerChoice);

        // 결과 텍스트 페이드 연출
        StopAllCoroutines();
        StartCoroutine(FadeResultEffect());

        // 게임 종료 혹은 다음 라운드 준비
        if (scorePlayer >= WinScore || scoreComputer >= WinScore)
            Invoke("EndGame", 1.2f);
        else
            Invoke("RestartCycle", 1.5f);
    }

    void RestartCycle()
    {
        if (!gameEnded)
        {
            isAnimating = true;
            resultText.text = "다시 한번 선택해주세요!";
            resultText.color = Color.white;
        }
    }

    void DetermineWinner(Choice p, Choice c)
    {
        // 무승부인 경우
        if (p == c)
        {
            resultText.text = "서로 비겼습니다!";
            resultText.color = Color.yellow;
        }
        // 플레이어 승리 조건
        else if ((p == Choice.Scissors && c == Choice.Paper) ||
                 (p == Choice.Rock && c == Choice.Scissors) ||
                 (p == Choice.Paper && c == Choice.Rock))
        {
            scorePlayer++;
            resultText.text = "와! 이겼습니다!";
            resultText.color = Color.green;
        }
        // 컴퓨터 승리 조건
        else
        {
            scoreComputer++;
            resultText.text = "앗... 졌습니다.";
            resultText.color = Color.red;
        }
        UpdateScoreUI();
    }

    IEnumerator FadeResultEffect()
    {
        if (resultTextCanvasGroup == null) yield break;
        resultTextCanvasGroup.alpha = 0f;
        while (resultTextCanvasGroup.alpha < 1f)
        {
            resultTextCanvasGroup.alpha += Time.deltaTime * 5f;
            yield return null;
        }
    }

    void UpdateScoreUI()
    {
        textScorePlayer.text = scorePlayer.ToString();
        textScoreComputer.text = scoreComputer.ToString();
    }

    void EndGame()
    {
        gameEnded = true;
        SetButtonsInteractable(false);
        if (panelResult != null)
        {
            panelResult.SetActive(true);
            // 최종 승패 제목 없이 통계와 재도전 질문만 표시
            textResultPanelStats.text = $"총 {totalGames}판의 대결이 끝났습니다.\n다시 도전하시겠습니까?";
        }
    }

    void OnRestart() => InitGame();

    void OnExit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void SetButtonsInteractable(bool state)
    {
        buttonScissors.interactable = state;
        buttonRock.interactable = state;
        buttonPaper.interactable = state;
    }

    Sprite GetSpriteFromChoice(Choice c)
    {
        switch (c)
        {
            case Choice.Scissors: return spriteScissors;
            case Choice.Rock: return spriteRock;
            case Choice.Paper: return spritePaper;
            default: return spriteQuestion;
        }
    }
}