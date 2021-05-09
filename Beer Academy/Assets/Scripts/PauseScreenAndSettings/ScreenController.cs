using UnityEngine;

namespace PauseScreenAndSettings
{
    public class ScreenController : MonoBehaviour
    {
        // Start is called before the first frame update
        public void SetFullscreen()
        {
            Resolution[] resolutions = Screen.resolutions;
            Screen.SetResolution(resolutions[resolutions.Length-1].width,resolutions[resolutions.Length-1].height,FullScreenMode.FullScreenWindow,60);
        }
    }
}
