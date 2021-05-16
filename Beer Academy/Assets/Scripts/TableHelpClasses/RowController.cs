using System.Collections.Generic;
using Data_Types;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TableHelpClasses
{
    public class RowController : MonoBehaviour
    {
        [SerializeField] private Sprite defaultFrame;
        [SerializeField] private Sprite focusFrame;
        

        public void SetImageWidth(float width)
        {

            foreach (RectTransform child in GetAllRectTransforms())
            {
                child.sizeDelta = new Vector2(width, child.sizeDelta.y);
            }
        }

        public void ClearAllFields()
        {
            foreach (TextMeshProUGUI childText in GetAllTextFields())
            {
                childText.text = "";
            }
        }

        public void SetValue(int field, float value)
        {
            GetAllTextFields()[field].text = value.ToString();
        }

        public void SetRound(int round)
        {
            transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = round.ToString();
        }

        public void SetFrameColor(int field, Color32 color, bool focus)
        {
            Image frame = GetAllTextFields()[field].gameObject.transform.parent.GetComponent<Image>();
            frame.color = color;
            
            frame.sprite = focus ? focusFrame : defaultFrame;
        }

        public void SetTextColor(int field, Color32 color)
        {
            GetAllTextFields()[field].color = color;
        }


        private List<TextMeshProUGUI> GetAllTextFields()
        {
            List<TextMeshProUGUI> textFields = new List<TextMeshProUGUI>();
            
            bool firstChildSkipped = false;
            foreach (RectTransform child in transform)
            {
                if (!firstChildSkipped)
                {
                    firstChildSkipped = true;
                    continue;
                }
                
                textFields.Add(child.GetChild(0).GetComponent<TextMeshProUGUI>());
            }

            return textFields;
        }
        private List<RectTransform> GetAllRectTransforms()
        {
            List<RectTransform> rectTransforms = new List<RectTransform>();
            
            bool firstChildSkipped = false;
            foreach (RectTransform child in transform)
            {
                if (!firstChildSkipped)
                {
                    firstChildSkipped = true;
                    continue;
                }

                rectTransforms.Add(child);
            }

            return rectTransforms;
        }
    }
}
