using System;
using Data_Types;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ChuckHistoryElement : MonoBehaviour
    {
        [Header("Values")]
        public float time;
        public Suit suit;
        public GameObject target;

        [Header("Elements")] 
        public TextMeshProUGUI timeText;
        public TextMeshProUGUI playerNameText;
        public Image suitImage;
        

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                FindTarget();
            }
        }

        public void FindTarget()
        {
            LeanTween.move(gameObject, target.transform.position, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        }

        public void SetInfo(float time, Suit suit, Player player, GameObject target)
        {
            this.time = time;
            this.suit = suit;
            this.target = target;
            
            playerNameText.text = player.name;
            suitImage.sprite = MyResources.current.GetSuitSprite(suit);

            TimeSpan ts = TimeSpan.FromMilliseconds(Mathf.Floor(time));

            timeText.text = $"{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds:000}";

            GetComponent<Image>().color = player.color;

        }

    }

