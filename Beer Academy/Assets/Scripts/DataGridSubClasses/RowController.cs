
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;


    public class RowController : MonoBehaviour
    {
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
