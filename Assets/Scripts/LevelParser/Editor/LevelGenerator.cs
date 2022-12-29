using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[System.Serializable]
public class CardGroup
{
   public string colorName;
   [HideInInspector]
   public Transform cardGroupParent;  // State variable
   public GameObject[] possibleCards;
    // State variable
    int minimumIndex = int.MaxValue;
    List<Card> cardsInGroup = new List<Card>();

    public void AddCard(Card card)
    {
        cardsInGroup.Add(card);
    }

    public void ClearCards()
    {
        cardsInGroup.Clear();
    }

    public void SetCardStackIndices()
    {
        int minIndex = int.MaxValue;
        foreach(Card card in cardsInGroup)
        {
            if(card != null)
            {
                if (card.indexInStack < minIndex)
                {
                    minIndex = card.indexInStack;
                }
            }
            
        }
        foreach (Card card in cardsInGroup)
        {
            if(card != null)
            {
                card.indexInStack -= minIndex - 1;
                Debug.Log("CardName: " + card.name + ", CardIndex: " + card.indexInStack);
            }
        }

    }
}

public class LevelGenerator : MonoBehaviour
{
    //Constants
    const int ANN_COLOR_INDEX = 0;
    const int ANN_CARDINDEX_INDEX = 1;
    const int ANN_ANCHOR_INDEX = 2;
    //Settings
    public string levelFilePath;
    // Connections
    public GameObject templateLevelPrefab;
    public CardGroup[] cardGroups;
    // State Variables
    public GameObject levelGO;
    public LevelConnections levelConnections;
    private Transform origin;
    private Grid gameGrid;

    // Start is called before the first frame update
    void Start()
    {
        //InitConnections();
        //InitState();
    }
    void InitConnections(){
    }
    void InitState(){
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateLevelFromFile(string path = null)
    {
        if (path == null) path = levelFilePath;


        // Create the level gameobject
        levelGO = Instantiate(templateLevelPrefab);
        levelConnections = levelGO.GetComponent<LevelConnections>();
        ConnectToLevel(levelConnections);
        // Read the level information file line by line
        StreamReader reader = new StreamReader(path);
        string currentLine = reader.ReadLine();

        int rowIndex = 0;
        while(currentLine != null && currentLine != "")
        {
            // Split each line using comma seperator
            string[] cards = currentLine.Split(',');
            for(int columnIndex = 0; columnIndex < cards.Length; columnIndex++)
            {
                // According to the values inside comma, apply card placement
                PlaceCard(rowIndex, columnIndex, cards[columnIndex]);
            }
            rowIndex++;
            currentLine = reader.ReadLine();
        }
        reader.Close();

      

        foreach (CardGroup group in cardGroups)
        {
            group.SetCardStackIndices();
        }
    }

    private void ConnectToLevel(LevelConnections levelConnections)
    {
        origin = levelConnections.origin;
        for(int i=0; i<cardGroups.Length; i++)
        {
            cardGroups[i].cardGroupParent = levelConnections.cardParentPoints[i];
        }
        gameGrid = origin.GetComponent<Grid>();
    }


    private void PlaceCard(int rowIndex, int columnIndex, string cardAnnotation)
    {
        //Debug.Log("R:" + rowIndex + "C:" + columnIndex + ":" + cardAnnotation); // For only test: TODO: Will be changed with prefab instantiation
        //GameObject cardGO = (GameObject) PrefabUtility.InstantiatePrefab(cardGroups[0].possibleCards[0]);
        if(cardAnnotation != "")
        {
            GameObject cardGO = CreateObjectFromAnnotation(cardAnnotation);
            Vector3 cardPlacePosition = gameGrid.CellToWorld(new Vector3Int(columnIndex, -rowIndex, 0));
            cardGO.transform.position = cardPlacePosition;
        }
    }

    private GameObject CreateObjectFromAnnotation(string annotation)
    {
        GameObject result = null;
        string[] annotationParts = annotation.Split(' ');
        string colorName = annotationParts[ANN_COLOR_INDEX];
        string indexStr = annotationParts[ANN_CARDINDEX_INDEX];
        bool isAnchored = false;
        if(ANN_ANCHOR_INDEX < annotationParts.Length)
        {
            if (annotationParts[ANN_ANCHOR_INDEX].CompareTo("a") == 0)
                isAnchored = true;
        }

        CardGroup group = FindCardGroup(colorName);
        if (group != null)
        {

            int cardIndex = System.Convert.ToInt32(indexStr);
            result = (GameObject)PrefabUtility.InstantiatePrefab(group.possibleCards[cardIndex]);
            Card card = result.GetComponent<Card>();
            card.isAnchor = isAnchored;
            group.AddCard(card);
            card.indexInStack = cardIndex;
            card.cardParent = group.cardGroupParent; // TODO: Ugur bu adima gerek olmayabilir mi //Burasi redundant gorunuyor olabilir ama Card scripti uzerinde gamebreaking bug engellemede kullaniliyor
            result.transform.parent = group.cardGroupParent;
        }

        return result; 
      
    }

    private CardGroup FindCardGroup(string colorName)
    {
        CardGroup group = null;
        for (int i = 0; i < cardGroups.Length; i++)
        {
            if (cardGroups[i].colorName == colorName)
            {
                group = cardGroups[i];
            }
        }

        Grid grid = GetComponent<Grid>() ;
        

        return group;
    }

    public void ClearLevel()
    {
        for(int i=0; i<cardGroups.Length; i++)
        {
            Transform currentCardParent = cardGroups[i].cardGroupParent;
            // Destroy all childs of the parent
            
           
            for(int j= currentCardParent.childCount-1; j >= 0; j--)
            {
                DestroyImmediate(currentCardParent.GetChild(j).gameObject);
            }

            cardGroups[i].ClearCards();
        }
    }
    public void SaveLevel(string prefabPath,string levelName)
    {
        levelGO.name = levelName;
        bool success = false;
        PrefabUtility.SaveAsPrefabAsset(levelGO, prefabPath + "/" + levelName + ".prefab",out success);
        if (success)
        {
            DestroyImmediate(levelGO);
        }
        else
        {
            Debug.LogError("Could not save level, an error occured");
        }

    }
}

