using UnityEngine;

namespace Debugging
{
    public class DebugPanelController : MonoBehaviour
    {
        private bool _active;

        public void Show()
        {
            LeanTween.moveLocalX(gameObject, 960, 0.75f).setEase(LeanTweenType.easeInOutQuad);
            _active = true;
        }

        public void Hide()
        {
            LeanTween.moveLocalX(gameObject, 1569, 0.75f).setEase(LeanTweenType.easeInOutQuad);
            _active = false;
        }

        public void Toggle()
        {
            if (_active)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    
    }
}
