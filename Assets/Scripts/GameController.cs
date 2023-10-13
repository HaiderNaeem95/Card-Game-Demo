using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class CardData
{
    public GameObject card;
    public int indexPlaced;
    public bool matched;
    public Card attributes;
}
public class GameController : MonoBehaviour
{
    public GameObject card, prevCard, currCard;
    public Card[] cardVariants;
    public List<CardData> carddetails;
    public RectTransform ContainerPanel;
    public int rows, cols, counter;
    public Sprite genericTex;
    public int turnsTaken, score;
    public Text turnText, scoreText;

    private void Start()
    {
        Debug.Log(Screen.height);
        counter = 0;
        GridMaker();
    }
    
    
    public void GridMaker()
    {
        ContainerPanel.sizeDelta = new Vector2(card.GetComponent<RectTransform>().sizeDelta.x * (rows * 1.2f),ContainerPanel.sizeDelta.y);
        int index = 0;
        for (int i = 0; i < rows; i++)
        {
            for(int j=0;j<cols;j++)
            {
                var spawnedCard = Instantiate(card, transform.position, transform.rotation);
                spawnedCard.name = index.ToString();
                CardData temp = new CardData();
                temp.card = spawnedCard;
                temp.indexPlaced = index;
                carddetails.Add(temp);
                index++;
                spawnedCard.transform.SetParent(ContainerPanel.transform, false);
                spawnedCard.GetComponent<Button>().onClick.AddListener(SpriteBtn);
            }
        }

        setCardAttributes();
        Debug.Log(index);
        StartCoroutine(disableLayout());
/*
        this.gameObject.GetComponent<ContentSizeFitter>().enabled = false;
        this.gameObject.GetComponent<LayoutGroup>().enabled = false;*/
    }


    IEnumerator disableLayout()
    {
        yield return new WaitForSeconds(2f);
        this.gameObject.GetComponent<ContentSizeFitter>().enabled = false;
        this.gameObject.GetComponent<LayoutGroup>().enabled = false;
    }


    public void setCardAttributes()
    {
        
        int totalindexes = rows * cols;
        List<int> indexavailable = new List<int>();
        for(int i=0;i<totalindexes;i++)
        {
            indexavailable.Add(i);
        }
        for(int i=0;i<totalindexes/2;i++)
        {
            int index1 = indexavailable.ElementAt(UnityEngine.Random.Range(0, indexavailable.Count - 1));
            indexavailable.Remove(index1);
            int index2 = indexavailable.ElementAt(UnityEngine.Random.Range(0, indexavailable.Count - 1));
            indexavailable.Remove(index2);

            int variantIndex = UnityEngine.Random.Range(0, cardVariants.Length - 1);
            carddetails.ElementAt(index1).attributes = cardVariants[variantIndex];
            carddetails.ElementAt(index2).attributes = cardVariants[variantIndex];
        }

    }

    public void SpriteBtn()
    {
        turnsTaken++;
        turnText.text = turnsTaken.ToString();
        GameObject BtnPressed =  EventSystem.current.currentSelectedGameObject;
        BtnPressed.GetComponent<Image>().sprite = carddetails.Find(card => card.card == BtnPressed).attributes.Tex;
        StartCoroutine(revertTex(BtnPressed));
        if(currCard == null)
        {
           // prevCard = BtnPressed;
            currCard = BtnPressed;
        }
        else
        {
            prevCard = currCard;
            currCard = BtnPressed;
        }

    }

    public void checkMatch()
    {
        if(prevCard!=null && prevCard.name != currCard.name)
        {
            if (carddetails.Find(card => card.card == prevCard).attributes.Tex == carddetails.Find(card => card.card == currCard).attributes.Tex)
            {
                Debug.Log("CardsMatch");
                carddetails.Find(card => card.card == prevCard).card.SetActive(false);
                carddetails.Find(card => card.card == currCard).card.SetActive(false);
                
                prevCard = null;
                currCard = null;

                score = score + 20;
                scoreText.text = score.ToString();
            }
        }
      
    }
    IEnumerator revertTex(GameObject card)
    {
        yield return new WaitForSeconds(1f);
        card.GetComponent<Image>().sprite = genericTex;
        yield return new WaitForSeconds(1f);

        checkMatch();

    }
}
