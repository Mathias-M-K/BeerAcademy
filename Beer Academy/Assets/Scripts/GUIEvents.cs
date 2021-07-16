
    using System;
    using System.Collections.Generic;
    using Data_Types;
    using UnityEngine;

    public class GUIEvents : MonoBehaviour
    {
        public static GUIEvents current;

        private readonly List<Player> _selectedPlayers = new List<Player>();
        private bool _selectionActive;

        private void Awake()
        {
            if (current == null) current = this;
        }

        private void Start()
        {
            GameController.current.SpacePressed += OnSpacePressed;
        }

        private void OnSpacePressed()
        {
            OnSetDeselected(GenerateListWithout(new List<Player>(_selectedPlayers)));
            OnSetSelected(new List<Player>(_selectedPlayers));
            OnMouseExit();
        }


        public event Action<List<Player>> SetSelected;
        public event Action<List<Player>> SetDeselected;
        public event Action<List<Player>> ClearSelection;


        /*
         * 
         */
        public void OnMouseHover(Player player)
        {
            OnSetDeselected(GenerateListWithout(new List<Player>(_selectedPlayers){player}));
            OnSetSelected(new List<Player>(_selectedPlayers){player});
        }
        
        public void OnMouseExit()
        {
            if (_selectionActive)
            {
                OnMouseHover(_selectedPlayers[0]);
                //OnClearSelection(GenerateListWithout(_selectedPlayers));
            }
            else
            {
                OnClearSelection(GameController.current.GetAllPlayers());
            }
        }

        public void OnMouseClick(Player player, bool selection)
        {
            if (selection)
            {
                if (_selectedPlayers.Contains(player))
                {
                    _selectedPlayers.Remove(player);
                }
                else
                {
                    _selectedPlayers.Add(player);
                    _selectionActive = true; 
                }

                if (_selectedPlayers.Count == 0)
                {
                    _selectionActive = false;
                }
            }
            else
            {
                _selectedPlayers.Clear();
                _selectionActive = false;
                
                OnClearSelection(GenerateListWithout(new List<Player>(){player}));
                OnMouseHover(player);
            }
        }

        public void OnSelectorClick(Player player, bool selected)
        {
            
        }

        protected virtual void OnSetSelected(List<Player> obj)
        {
            SetSelected?.Invoke(obj);
        }

        protected virtual void OnSetDeselected(List<Player> obj)
        {
            SetDeselected?.Invoke(obj);
        }
        
        protected virtual void OnClearSelection(List<Player> obj)
        {
            ClearSelection?.Invoke(obj);
        }
        
        
        
        /*
         * Other
         */
        private List<Player> GenerateListWithout(List<Player> excludedPlayers)
        {
            List<Player> players = new List<Player>(GameController.current.GetAllPlayers());

            foreach (Player excludedPlayer in excludedPlayers)
            {
                players.Remove(excludedPlayer);
            }

            return players;
        }


    }
 