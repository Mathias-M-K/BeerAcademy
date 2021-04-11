using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{

        [Header("Card Info")]
        public Suit suit;

        [Header("Card Elements")] [Tooltip("")]
        public List<Image> suits;



        private void Start()
        {
                FindSuitElements(transform);
        }

        private void Update()
        {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                        SetSuit(Suit.Diamond);
                }
        }

        private void SetSuit(Suit s)
        {
                foreach (Image image in suits)
                {
                        image.sprite = MyResources.Current.GetSprite(s);
                }
        }


        private void FindSuitElements(Transform t)
        {
                if (t.CompareTag("Suit"))
                {
                        suits.Add(t.gameObject.GetComponent<Image>());
                }

                if (t.childCount > 0)
                {
                        foreach (Transform child in t)
                        {
                                FindSuitElements(child);
                        }
                }
        }
}



public enum Suit
{
        Club,
        Heart,
        Spade,
        Diamond
}
