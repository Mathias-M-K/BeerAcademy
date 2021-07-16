using System.Collections;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WelcomeScreenController : MonoBehaviour
{
    public GameObject playerCountInputPanel;
    public GameObject gameAndPlayerInfoPanel;
    public ButtonManager forwardBtn;
    public ButtonManager backBtn;

    public PlayerNamesInputPanel pnip;

    private float _nrOfPlayers;

    private int _flowPosition = 0;  //Value to determine where the user is in the flow
    // Start is called before the first frame update

    public void NextButtonPressed()
    {
        _flowPosition++;
        if (_flowPosition > 2) _flowPosition = 2;
        UpdateElementPositions();
    }

    public void BackButtonPressed()
    {
        _flowPosition--;
        if (_flowPosition < 0) _flowPosition = 0;
        UpdateElementPositions();
    }

    private void UpdateElementPositions()
    {
        if (_flowPosition == 0)
        {
            LeanTween.moveLocalX(playerCountInputPanel, 0, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.moveLocalX(gameAndPlayerInfoPanel, 2700, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.moveLocalY(forwardBtn.gameObject, -175, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.moveLocalY(backBtn.gameObject, -900, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        }
        
        if (_flowPosition == 1)
        {
            LeanTween.moveLocalX(playerCountInputPanel, -1650, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.moveLocalX(gameAndPlayerInfoPanel, 0, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.moveLocalY(forwardBtn.gameObject, -560, 0.5f).setEase(LeanTweenType.easeInOutQuad);
            LeanTween.moveLocalY(backBtn.gameObject, -644, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        }

        if (_flowPosition == 2)
        {
            GlobalValueContainer.Container.players = pnip.GetPlayerNames();
            GlobalValueContainer.Container.playerSips = pnip.GetPlayerSips();
            GlobalValueContainer.Container.playerProtectedStatus = pnip.GetPlayerProtectedStatus();
            SceneManager.LoadScene(1);
        }
    }
}
