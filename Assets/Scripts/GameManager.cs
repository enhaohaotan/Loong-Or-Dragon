using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RectTransform loongTile;
    [SerializeField] private RectTransform dragonTile;

    [SerializeField] private Image background;
    [SerializeField] private Image card;
    [SerializeField] private Image frame;

    private List<RectTransform> dragonTiles;
    private List<RectTransform> loongTiles;

    private void Awake()
    {   
        dragonTiles = new List<RectTransform>();
        loongTiles = new List<RectTransform>();

        dragonTile.gameObject.SetActive(false);
        loongTile.gameObject.SetActive(false);

        for (int i = 0; i < 36; i++)
        {
            dragonTiles.Add(Instantiate(dragonTile, dragonTile.parent));
            loongTiles.Add(Instantiate(loongTile, loongTile.parent));
        }
    }

    private void Start()
    {
        int colorGroupIdx = Random.Range(0, Utilities.colors.Length);
        
        int loongPosition = Random.Range(0, 4);
        int dragonCount = 0;

        background.color = Utilities.colors[colorGroupIdx][2];
        card.color = Utilities.colors[colorGroupIdx][1];
        frame.color = Utilities.colors[colorGroupIdx][3];

        List<Vector2> positions = new List<Vector2>
        {
            new Vector2 (150, -150),
            new Vector2 (-150, 150),
            new Vector2 (-150, -150),
            new Vector2 (150, 150)
        };

        for (int i = 0; i < 4; i++)
        {
            if (i ==  loongPosition)
            {
                loongTiles[0].gameObject.SetActive (true);
                loongTiles[0].anchoredPosition = positions[i];
                loongTiles[0].GetComponent<Image>().color = Utilities.colors[colorGroupIdx][0];
            }
            else
            {
                dragonTiles[dragonCount].gameObject.SetActive (true);
                dragonTiles[dragonCount].anchoredPosition = positions[i];
                dragonTiles[dragonCount].GetComponent<Image>().color = Utilities.colors[colorGroupIdx][0];
                dragonCount++;
            }
        }
    }



    public void OnLoongClicked()
    {
        Debug.Log("Loong was clicked!");
    }

    public void OnDragonClicked()
    {
        Debug.Log("Dragon was clicked!");
    }
}
