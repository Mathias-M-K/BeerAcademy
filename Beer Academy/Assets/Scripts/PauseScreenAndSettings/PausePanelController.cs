using System;
using UnityEngine;
using UnityEngine.UI;

namespace PauseScreenAndSettings
{
    public sealed class PausePanelController : MonoBehaviour
    {
        public static PausePanelController current;

        private void Awake()
        {
            if (current == null) current = this;
        }

        [Header("Components")]
        public SettingsPanelController settingsPanelController;

        [Header("Elements")] 
        public Image background;

        [Header("Animation")] 
        public LeanTweenType easeIn = LeanTweenType.easeInOutQuad;
        public LeanTweenType easeOut = LeanTweenType.easeInOutQuad;
        public float timeIn = 0.5f;
        public float timeOut = 0.5f;
        
        private float _hideYPos;
        [HideInInspector] public bool panelActive;

        public event Action Unpause;
        public event Action Pause;

        private void Start()
        {
            _hideYPos = transform.position.y;
        }


        public void ShowPanel()
        {
            LeanTween.moveLocal(gameObject, new Vector3(0,0,0), timeIn).setEase(easeIn);
            LeanTween.color(background.rectTransform, new Color32(0, 0, 0, 160),0.5f).setEase(LeanTweenType.easeInOutQuad);
            
            panelActive = true;
            
            OnPause();
        }

        public void HidePanel()
        {
            LeanTween.moveY(gameObject, _hideYPos, timeOut).setEase(easeOut).setOnComplete(() =>
            {
                LeanTween.moveLocalX(gameObject, 0, 0);
            });
            LeanTween.color(background.rectTransform, new Color32(0, 0, 0, 0),0.5f).setEase(LeanTweenType.easeInOutQuad);
            panelActive = false;
            
            settingsPanelController.HidePanel();
            
            OnUnpause();
        }
        
        public void TogglePanel()
        {
            if (panelActive)
            {
                HidePanel();
            }
            else
            {
                ShowPanel();
            }
        }

        public void ShowSettings()
        {
            settingsPanelController.ShowPanel();
            LeanTween.moveX(gameObject, -1191, timeOut).setEase(easeOut);
        }

        private void OnUnpause()
        {
            Unpause?.Invoke();
        }

        private void OnPause()
        {
            Pause?.Invoke();
        }
    }
}