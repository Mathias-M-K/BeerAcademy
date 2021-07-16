using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Data_Types;
using PauseScreenAndSettings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Timer = SupportClasses.Timer;



public class ChuckPanelController : MonoBehaviour
{
    public static ChuckPanelController current;

    private void Awake()
    {
        if (current == null) current = this;
    }
    
    [Header("Feedback Messages")]
    [SerializeField] private List<string> welcomeMessages = new List<string>();
    [SerializeField] private List<string> drinkingMessages = new List<string>();

    [Header("Animation of panel")] 
    [SerializeField] private LeanTweenType easeIn = LeanTweenType.easeInOutQuad;
    [SerializeField] private LeanTweenType easeOut = LeanTweenType.easeInOutQuad;
    [SerializeField] private float animationTimeIn = 0.5f;
    [SerializeField] private float animationTimeOut = 0.5f;
    
    [Header("Animation of feedback text")] 
    [SerializeField] private LeanTweenType feedbackTextEaseIn = LeanTweenType.easeInOutQuad;
    [SerializeField] private LeanTweenType feedbackTextEaseOut = LeanTweenType.easeInOutQuad;
    [SerializeField] private float feedbackTextAnimationTimeIn = 0.5f;
    [SerializeField] private float feedbackTextAnimationTimeOut = 0.5f;

    [Header("Chuck History Panel")] 
    public ChuckHistoryController chuckHistoryController;

    [Header("Audio")] 
    [SerializeField] private AudioSource audioComponent;
    [SerializeField] private List<AudioClip> audioClipsBefore;
    [SerializeField] private List<AudioClip> audioClipsAfter;
    
    [Header("Other")]
    [SerializeField] private GameObject startBtn;
    [SerializeField] private GameObject stopBtn;
    [SerializeField] private GameObject finishBtn;

    [SerializeField] private Image background;

    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Timer timer;

    public event Action ChuckPanelClose;

    
    private DateTime _startTime;
    
    public bool timerActive;    //True if the panel is active
    private bool _timerRunning; //True if the timer is running
    private bool _playerFinished;
    private bool _spaceBarReady;
    private Player _currentPlayer;
    private Suit _currentSuit;

    private float _hidePos;

    private void Start()
    {
        _hidePos = transform.localPosition.y;
    }


    public void ShowTimer(Player player, Suit suit)
    {
        _currentPlayer = player;
        _currentSuit = suit;
        
        playerName.text = player.Name;
        
        startBtn.SetActive(true);
        finishBtn.SetActive(false);
        stopBtn.SetActive(false);
        
        _spaceBarReady = false;
        _playerFinished = false;

        timer.SetTimer(TimeSpan.FromMinutes(0));


        int randomNr = Random.Range(0, welcomeMessages.Count);
        feedbackText.text = welcomeMessages[randomNr];

        GetComponent<Image>().color = player.Color;
        
        ShowPanel();

        StartCoroutine(ListenForKeypress());
    }
    
    public void StartTimer()
    {
        startBtn.SetActive(false);
        stopBtn.SetActive(true);

        _startTime = DateTime.Now;
        _timerRunning = true;
        
        int randomNr = Random.Range(0, drinkingMessages.Count);
        ChangeFeedbackText(drinkingMessages[randomNr]);

        audioComponent.Stop();
        
        StartCoroutine(WaitForPlayerToFinish());
    }

    public void StopTimer()
    {
        _timerRunning = false;
        _playerFinished = true;

        stopBtn.SetActive(false);
        finishBtn.SetActive(true);

        TimeSpan ts = DateTime.Now - _startTime;

        float time = (float)ts.TotalMilliseconds;

        ChangeFeedbackText(GetWrittenChuckTimePerformance(time));
        
        _currentPlayer.AddNewChuckTime(time);
        
        StatTracker.statTracker.LogChuckTime(ts,_currentPlayer);
        
        chuckHistoryController.AddNewChuckTime(_currentPlayer,time,_currentSuit);
        
        
        //Choosing audio clip and playing it
        int clip = Random.Range(0,audioClipsAfter.Count);
        audioComponent.clip = audioClipsAfter[clip];
        audioComponent.Play();
    }

    private IEnumerator WaitForPlayerToFinish()
    {
        while (_timerRunning)
        {
            TimeSpan we = DateTime.Now - _startTime;

            timer.SetTimer(we);
            
            yield return null;
        }
    }

    private IEnumerator ListenForKeypress()
    {
        while (timerActive)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                _spaceBarReady = true;
            }
            
            if (Input.GetKeyDown(KeyCode.Space) && _spaceBarReady && !PausePanelController.current.panelActive)
            {
                if (_playerFinished)
                {
                    HidePanel();
                }
                if (_timerRunning)
                {
                    StopTimer();
                }

                if (!_timerRunning && !_playerFinished)
                {
                    StartTimer();
                }
            }

            yield return null;
        }
    }

    private void ShowPanel()
    {
        LeanTween.moveLocalY(gameObject, 0, animationTimeIn).setEase(easeIn);
        LeanTween.color(background.rectTransform, new Color32(0, 0, 0, 134), animationTimeIn).setEase(LeanTweenType.easeInOutQuad);
        timerActive = true;

        
        //Choosing audio clip and playing it
        int clip = Random.Range(0,audioClipsBefore.Count);
        audioComponent.clip = audioClipsBefore[clip];
        audioComponent.Play();
    }

    public void HidePanel()
    {
        LeanTween.moveLocalY(gameObject, _hidePos, animationTimeOut).setEase(easeOut);
        LeanTween.color(background.rectTransform, new Color32(0, 0, 0, 0), animationTimeOut).setEase(LeanTweenType.easeInOutQuad);
        timerActive = false;
        
        OnChuckPanelClose();
    }

    private void ChangeFeedbackText(string newFeedbackText)
    {
        LeanTween.moveLocalY(feedbackText.gameObject, -50, feedbackTextAnimationTimeOut).setEase(feedbackTextEaseOut).setOnComplete(() =>
        {
            feedbackText.text = newFeedbackText;
            LeanTween.moveLocalY(feedbackText.gameObject, 0, feedbackTextAnimationTimeIn).setEase(feedbackTextEaseIn);
        });
    }

    /// <summary>
    /// Calculates how good the time is compared to the rest. 
    /// </summary>
    /// <returns></returns>
    private static string GetWrittenChuckTimePerformance(float time)
    {
        var chuckTimes = GameController.current.GetChuckTimes();
        chuckTimes.Sort();

        if (chuckTimes.Count == 0)
        {
            return "Sweet!";
        }

        var i = 1;
        foreach (float oldChuckTime in chuckTimes)
        {
            if (time < oldChuckTime)
            {
                break;
            }

            i++;
        }

        switch (i)
        {
            case 1:
                return "New Best Time!";
            case 2:
                return "2nd Best Time!";
            case 3:
                return "3rd Best Time";
        }
        
        if (i > 3)
        {
            return $"{i}th Best Time";
        }

        throw new Exception(
            $"GetChuckTimePerformance have failed. Input was ranked as {i}, but somehow no statement was returned before hitting the exception line");
    }

    protected virtual void OnChuckPanelClose()
    {
        ChuckPanelClose?.Invoke();
    }
}