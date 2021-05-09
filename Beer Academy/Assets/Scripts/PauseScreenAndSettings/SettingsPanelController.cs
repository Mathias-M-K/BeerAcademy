using System;
using UnityEngine;

namespace PauseScreenAndSettings
{
    public class SettingsPanelController : MonoBehaviour
    {
        public PausePanelController pausePanelController;
        
        [Header("Animation")] 
        public LeanTweenType easeIn = LeanTweenType.easeInOutQuad;
        public LeanTweenType easeOut = LeanTweenType.easeInOutQuad;
        public float timeIn = 0.5f;
        public float timeOut = 0.5f;
        
        private float _hideXPos;

        private void Start()
        {
            _hideXPos = transform.position.x;
        }

        public void ShowPanel()
        {
            LeanTween.moveLocalX(gameObject, 0, timeIn).setEase(easeIn);
        }

        public void HidePanel()
        {
            LeanTween.moveX(gameObject, _hideXPos, timeOut).setEase(easeOut);
        }
        
        public void Back()
        {
            pausePanelController.ShowPanel();
            HidePanel();
        }
    }
}