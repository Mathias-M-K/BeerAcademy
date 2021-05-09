using System;
using System.Collections;
using System.Collections.Generic;
using Data_Types;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

#endif

public class ChuckHistoryController : MonoBehaviour
{
    public GameObject chuckTimeObj;
    public GameObject chuckTimeObjTarget;
    public GameObject targetContainer;

    public List<ChuckHistoryElement> chuckHistoryElements = new List<ChuckHistoryElement>();


    public void AddNewChuckTime(Player player, float time, Suit suit)
    {
        GameObject target = Instantiate(chuckTimeObjTarget, targetContainer.transform);
        GameObject newChuckHistoryElement = Instantiate(chuckTimeObj,target.transform.position-new Vector3(500,0,0),new Quaternion(0,0,0,0),gameObject.transform);
        

        ChuckHistoryElement che = newChuckHistoryElement.GetComponent<ChuckHistoryElement>();
        
        che.SetInfo(time,suit,player,target);
        
        chuckHistoryElements.Add(che);
        
        chuckHistoryElements.Sort((p1,p2)=>p1.time.CompareTo(p2.time));

        int i = 0;
        foreach (ChuckHistoryElement chuckHistoryElement in chuckHistoryElements)
        {
            chuckHistoryElement.target.transform.SetSiblingIndex(i);
            i++;
        }

        StartCoroutine(PromptElementsToSeekTargets());


    }

    public IEnumerator PromptElementsToSeekTargets()
    {
        yield return new WaitForSeconds(1);
        foreach (ChuckHistoryElement chuckHistoryElement in chuckHistoryElements)
        {
            float yPos = chuckHistoryElement.target.transform.position.y;
            Vector3 pos = new Vector3(gameObject.transform.localPosition.x, yPos, 0);
            //gameObject.transform.localPosition = pos;
            
            
            chuckHistoryElement.FindTarget();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ChuckHistoryController))]
public class ChuckTimesPanelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ChuckHistoryController chuckHistoryPanel = (ChuckHistoryController) target;

        if (GUILayout.Button("Add new chuck"))
        {
            chuckHistoryPanel.AddNewChuckTime(new Player(1,"Mathias",new Color32(255,0,0,255)),2400,Suit.Spade);
        }

    }
}
#endif