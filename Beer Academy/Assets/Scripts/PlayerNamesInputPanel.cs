using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = System.Object;

public class PlayerNamesInputPanel : MonoBehaviour
{
    public GameObject playerInputLineElement;
    
    private int _currentPlayerFields = 1;
    private float _nrOfPlayers = 1;
    private float rawValue;
    public List<TMP_InputField> inputFields = new List<TMP_InputField>();
    public int selectedInput = -1;

    public GameObject selected;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                selectedInput--;
            }
            else
            {
                selectedInput++;
            }
            
            
            if (selectedInput > inputFields.Count-1) selectedInput = 0;
            if (selectedInput < 0) selectedInput = inputFields.Count - 1;

            inputFields[selectedInput].Select();
        }
    }

    public void SliderChange(float value)
    {
        _nrOfPlayers = Mathf.Round(value);
        rawValue = value;
    }
    
    public void AdjustPlayerFields()
    {
        if (_currentPlayerFields < _nrOfPlayers)
        {
            int difference = (int)(_nrOfPlayers - _currentPlayerFields);

            for (int i = 0; i < difference; i++)
            {
                GameObject go = Instantiate(playerInputLineElement,gameObject.transform);
            }

            _currentPlayerFields = (int)_nrOfPlayers;
        }

        if (_currentPlayerFields > _nrOfPlayers)
        {
            int difference = (int)(_currentPlayerFields - _nrOfPlayers);

            for (int i = 0; i < difference; i++)
            {
                Debug.Log($"Deleting | {transform.childCount}");
                //Destroy(transform.GetChild(transform.childCount-1).gameObject);
                
                DestroyImmediate(transform.GetChild(transform.childCount-1).gameObject);
            }
            
            _currentPlayerFields = (int)_nrOfPlayers;
        }

        inputFields = new List<TMP_InputField>();

        bool skipFirst = false;
        foreach (Transform child in transform)
        {
            if (!skipFirst)
            {
                skipFirst = true;
                continue;
            }
            inputFields.Add(child.GetChild(0).GetComponent<TMP_InputField>());
        }


    }

    public List<string> GetPlayerNames()
    {
        List<string> playerNames = new List<string>();
        
        foreach (TMP_InputField field in inputFields)
        {
            playerNames.Add(field.text);
            Debug.Log(field.text);
        }

        return playerNames;
    }

    public List<int> GetPlayerSips()
    {
        List<int> playerSipsSettings = new List<int>();
        
        bool firstObjectSkipped = false;
        foreach (Transform transform1 in transform)
        {
            if (!firstObjectSkipped)
            {
                firstObjectSkipped = true;
                continue;
            }

            string text = transform1.GetChild(1).GetComponent<TMP_InputField>().text;

            if (text == "")
            {
                playerSipsSettings.Add(14);
            }
            else
            {
                playerSipsSettings.Add(Int32.Parse(text));
            }
        }

        return playerSipsSettings;
    }

    public List<bool> GetPlayerProtectedStatus()
    {
        List<bool> playerProtectedStatus = new List<bool>();
        
        bool firstObjectSkipped = false;
        foreach (Transform transform1 in transform)
        {
            if (!firstObjectSkipped)
            {
                firstObjectSkipped = true;
                continue;
            }

            string text = transform1.GetChild(2).GetComponent<TMP_InputField>().text;

            if (text.ToLower().Trim() == "yes" || text.ToLower().Trim() == "y" || text.ToLower().Trim() == "true" )
            {
                playerProtectedStatus.Add(true);
            }
            else
            {
                playerProtectedStatus.Add(false);
            }
        }

        return playerProtectedStatus;
    }
}
