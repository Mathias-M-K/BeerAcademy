
    using System;
    using System.Collections.Generic;
    using Data_Types;
    using UnityEngine;

    public class GUIEvents : MonoBehaviour
    {
        public static GUIEvents current;
        

        private void Awake()
        {
            if (current == null) current = this;
        }

        public event Action<Player> MouseHover;
        public event Action<Player> MouseExit;

        public event Action<List<Player>> SetSelected;
        public event Action<List<Player>> SetDeselected; 


        /*
         * 
         */
        public void OnMouseHover(Player player)
        {
            MouseHover?.Invoke(player);
        }
        public void OnMouseExit(Player player)
        {
            MouseExit?.Invoke(player);
        }

        protected virtual void OnSetSelected(List<Player> obj)
        {
            SetSelected?.Invoke(obj);
        }

        protected virtual void OnSetDeselected(List<Player> obj)
        {
            SetDeselected?.Invoke(obj);
        }
    }
 