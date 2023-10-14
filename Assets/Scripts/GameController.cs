using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SimpleJSON;
using System.IO;
using UnityEngine.SceneManagement;

[Serializable]
public class CardData
{
    public GameObject card;
    public bool matched;
    public Sprite Tex;
}
public class GameController : MonoBehaviour
{
    [Header("Cards")]
    public GameObject card;
    GameObject prevCard, currCard;
    [Header("Card Textures")]
    public List<Sprite> cardVariants;
    [Header("Cards List")]
    public List<CardData> carddetails;
    public RectTransform ContainerPanel;
    [Header("Rows and Columns")]
    public int rows, cols;
    [Header("Generic Card Sprite")]
    public Sprite genericTex;
    [Header("UserInterface")]
    int turnsTaken, score;
    public Text turnText, scoreText;
    public GameObject loadingPanel;
    [Header("Sound")]
    public AudioClip[] sounds;
    int counter;
    bool check;

    public void Update()
    {
        int maxScore = ((rows * cols) / 2)*20;
        if(score >= maxScore && check==false)
        {
            Debug.Log("Game Over");
            // Sounds.playSound(2);
            this.gameObject.GetComponent<AudioSource>().clip = sounds[2];
            this.gameObject.GetComponent<AudioSource>().Play();
            check = true;
            SceneManager.LoadSceneAsync(0);

        }
    }
    private void Start()
    {
        loadingPanel.SetActive(true);
        rows = PlayerPrefs.GetInt("rows");
        cols = PlayerPrefs.GetInt("cols");
        string filepath = Application.persistentDataPath + "/GameData.json";
        if (File.Exists(filepath))
        {
            loadGameState();
            Debug.Log("File Exists");
        }
        else
        {
            Debug.Log(Screen.height);
            GridMaker();
        }
       
    }
    public void GridMaker()
    {
        this.gameObject.GetComponent<GridLayoutGroup>().constraintCount = cols;

        ContainerPanel.sizeDelta = new Vector2(card.GetComponent<RectTransform>().sizeDelta.x * (rows * 1.2f),ContainerPanel.sizeDelta.y);
        int index = 0;
        for (int i = 0; i < cols; i++)
        {
            for(int j=0;j<rows;j++)
            {
                var spawnedCard = Instantiate(card, transform.position, transform.rotation);
                spawnedCard.name = index.ToString();
                CardData temp = new CardData();
                temp.card = spawnedCard;
                carddetails.Add(temp);
                index++;
                spawnedCard.transform.SetParent(ContainerPanel.transform, false);
                spawnedCard.GetComponent<Button>().onClick.AddListener(SpriteBtn);
            }
        }

        setCardAttributes();

        Debug.Log(index);
       StartCoroutine(disableLayout());

    }


    IEnumerator disableLayout()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.GetComponent<ContentSizeFitter>().enabled = false;
        this.gameObject.GetComponent<LayoutGroup>().enabled = false;

        foreach (CardData card in carddetails)
        {
            if (card.matched)
                card.card.SetActive(false);
        }

        loadingPanel.SetActive(false);

        // saveGameState();

    }

    private void OnApplicationQuit()
    {
        saveGameState();
    }
    public void setCardAttributes(bool load = false)
    {
        if(load == false)
        {
            int totalindexes = rows * cols;
            List<int> indexavailable = new List<int>();
            for (int i = 0; i < totalindexes; i++)
            {
                indexavailable.Add(i);
            }
            for (int i = 0; i < totalindexes / 2; i++)
            {
                int index1 = indexavailable.ElementAt(UnityEngine.Random.Range(0, indexavailable.Count - 1));
                indexavailable.Remove(index1);
                int index2 = indexavailable.ElementAt(UnityEngine.Random.Range(0, indexavailable.Count - 1));
                indexavailable.Remove(index2);

                int variantIndex = UnityEngine.Random.Range(0, cardVariants.Count - 1);
                carddetails.ElementAt(index1).Tex = cardVariants[variantIndex];
                carddetails.ElementAt(index2).Tex = cardVariants[variantIndex];
            }
        }

        setTexts();
    }

    public void SpriteBtn()
    {
        setTexts();
        this.gameObject.GetComponent<AudioSource>().clip = sounds[0];
        this.gameObject.GetComponent<AudioSource>().Play();
       // Sounds.transform.GetChild(0).GetComponent<AudioSource>().Play();
        turnText.text = turnsTaken.ToString();
        GameObject BtnPressed =  EventSystem.current.currentSelectedGameObject;
        BtnPressed.GetComponent<Image>().sprite = carddetails.Find(card => card.card == BtnPressed).Tex;
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
            if (carddetails.Find(card => card.card == prevCard).Tex == carddetails.Find(card => card.card == currCard).Tex)
            {
                Debug.Log("CardsMatch");
                carddetails.Find(card => card.card == prevCard).matched = true;
                carddetails.Find(card => card.card == currCard).matched = true;

                carddetails.Find(card => card.card == prevCard).card.SetActive(false);
                carddetails.Find(card => card.card == currCard).card.SetActive(false);
                
                prevCard = null;
                currCard = null;

                score = score + 20;
                setTexts();

                this.gameObject.GetComponent<AudioSource>().clip = sounds[1];
                this.gameObject.GetComponent<AudioSource>().Play();
            }
            else
            {
                this.gameObject.GetComponent<AudioSource>().clip = sounds[3];
                this.gameObject.GetComponent<AudioSource>().Play();
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

    public void setTexts()
    {
        turnText.text = turnsTaken.ToString();
        scoreText.text = score.ToString();
    }
    public void saveGameState()
    {
        JSONObject gameData = new JSONObject();

        gameData.Add("rows", rows);
        gameData.Add("cols", cols);
        gameData.Add("turns",turnsTaken);
        gameData.Add("score", score);

        int i = 0;

        foreach (CardData card in carddetails)
        {
            JSONObject carddata = new JSONObject();

            carddata.Add("objName", card.card.name);
            carddata.Add("bool", card.matched);
            carddata.Add("tex", card.Tex.name);
            
            gameData.Add("cardData"+i,carddata);
            i++;
        }

        Debug.Log(gameData.ToString());

        string filepath = Application.persistentDataPath + "/GameData.json";
        File.WriteAllText(filepath,gameData.ToString());

    }

    public void loadGameState()
    {
        string filepath = Application.persistentDataPath + "/GameData.json";
        string gameData = File.ReadAllText(filepath);

        JSONObject gameJson = (JSONObject)JSON.Parse(gameData);

        rows = gameJson["rows"];
        cols = gameJson["cols"];
        turnsTaken = gameJson["turns"];
        score = gameJson["score"];

        this.gameObject.GetComponent<ContentSizeFitter>().enabled = true;
        this.gameObject.GetComponent<LayoutGroup>().enabled = true;
        GridMaker();
        int i = 0;
        foreach(CardData card in carddetails)
        {
            card.Tex = cardVariants.Find(c=> c.name == gameJson["cardData" + i]["tex"]);
            card.matched = gameJson["cardData" + i]["bool"];
           /*if(card.matched == true)
            {
                card.card.SetActive(false);
            }*/
            i++;
        }

        /*foreach (CardData card in carddetails)
        {
            if (card.matched)
                card.card.SetActive(false);
        }*/
    }

}
