using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{
    #region Public Values
    
    public GameObject cardShow;
    public GameObject cardSpawn;
    
    #endregion

    #region Private Values
    
    private Vector3 _cardShowPos;
    private Vector3 _cardSpawnPos;
    
    #endregion
    
    
    // Start is called before the first frame update
    void Start()
    {
        _cardShowPos = cardShow.transform.position;
        _cardSpawnPos = cardSpawn.transform.position;
        
        Destroy(cardShow);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            //SpawnCard(cardSpawn, _cardSpawnPos,_cardShowPos);

            StartCoroutine(SpawnCards(2,40,250,0.2f));
        }
    }
    /// <summary>
    /// Spawns a card at spawnPosition and animates it in to endPosition
    /// </summary>
    /// <param name="card"></param>
    /// <param name="spawnPosition"></param>
    /// <param name="endPosition"></param>
    private void SpawnCard(GameObject card, Vector3 spawnPosition, Vector3 endPosition)
    {
        GameObject g = Instantiate(card,spawnPosition,new Quaternion(0,0,0,0));
        g.transform.SetParent(cardSpawn.transform.parent);
        g.transform.localScale = new Vector3(1, 1, 1);

        LeanTween.move(g, endPosition, 1).setEase(LeanTweenType.easeInOutQuad);
    }

    private IEnumerator SpawnCards(int players,int horizontalSpacing, int verticalSpacing, float spawnDelay)
    {
        for (int i = 0; i < players; i++)
        {
            float vertical = _cardSpawnPos.y - (i * verticalSpacing);
            
            Vector3 spawnPos = new Vector3(_cardSpawnPos.x, vertical, 0);

            StartCoroutine(SpawnRow(spawnPos, horizontalSpacing, vertical, spawnDelay));
            
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator SpawnRow(Vector3 spawnPos, int horizontalSpacing, float vertical, float spawnDelay)
    {
        for (int y = 13; y > 0; y--)
        {
            Vector3 endPos = new Vector3(_cardShowPos.x+(horizontalSpacing*y), vertical, 0);
            
            SpawnCard(cardSpawn,spawnPos,endPos);

            yield return new WaitForSeconds(spawnDelay);
        }
    }
    
  

    
}
