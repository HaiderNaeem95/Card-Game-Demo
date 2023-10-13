using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CardData
{
    public int id;
    public Texture mainTex;
}
public class GameController : MonoBehaviour
{
    public GameObject card;
    public RectTransform ContainerPanel;
    public int rows, cols;
    private void Start()
    {
        Debug.Log(Screen.height);
        GridMaker();
    }

    public void GridMaker()
    {
        ContainerPanel.sizeDelta = new Vector2(card.GetComponent<RectTransform>().sizeDelta.x * (rows * 1.2f),ContainerPanel.sizeDelta.y);

        for (int i = 0; i < rows; i++)
        {
            for(int j=0;j<cols;j++)
            {
                var spawnedCard = Instantiate(card, transform.position, transform.rotation);
                spawnedCard.transform.SetParent(ContainerPanel.transform, false);
            }
        }
    }
}
