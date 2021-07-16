using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSelectionBar : MonoBehaviour
{

    [SerializeField] private List<GameObject> buttons;
    [SerializeField] private GameObject selectionBar;


    public void ButtonPress(int btnId)
    {
        LeanTween.moveLocalX(selectionBar, 
                buttons[btnId].transform.localPosition.x, 
                0.5f)
            .setEase(LeanTweenType.easeInOutQuad);
    }
}
