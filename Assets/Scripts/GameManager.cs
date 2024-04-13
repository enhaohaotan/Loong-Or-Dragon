using DG.Tweening;
using Nobi.UiRoundedCorners;
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

    [SerializeField] private AudioSource successSound;
    [SerializeField] private AudioSource failureSound;

    private List<RectTransform> dragonTiles;
    private List<RectTransform> loongTiles;

    private int halfFrameSize = 360;
    private int correctCount = 0;

    private void Awake()
    {   
        dragonTiles = new List<RectTransform>();
        loongTiles = new List<RectTransform>();

        dragonTile.gameObject.SetActive(false);
        loongTile.gameObject.SetActive(false);

        for (int i = 0; i < 36; i++)
        {
            dragonTiles.Add(Instantiate(dragonTile, dragonTile.parent));
        }
        loongTiles.Add(Instantiate(loongTile, loongTile.parent));

    }

    private void Start()
    {
        SetLayout(2);
    }



    public void OnLoongClicked(RectTransform currentTile)
    {
        //Debug.Log(currentTile.name);
        successSound.Play();
        Vector2 originalSize = currentTile.sizeDelta;
        Vector2 scaleSize = new Vector2(currentTile.sizeDelta.x - 10, currentTile.sizeDelta.y - 10);
        float duration = 0.1f;
        correctCount++;

        if (correctCount < 8)
        {
            currentTile.DOSizeDelta(scaleSize, duration)
                .OnComplete(() =>
                {
                    currentTile.DOSizeDelta(originalSize, duration)
                        .OnComplete(() => { SetLayout(correctCount / 2 + 2); });
                }
            );
        }
        else
        {
            currentTile.DOSizeDelta(scaleSize, duration)
                .OnComplete(() =>
                {
                    currentTile.DOSizeDelta(originalSize, duration)
                        .OnComplete(() => { SetLayout(6); });
                }
            );
        }
    }

    public void OnDragonClicked(RectTransform currentTile)
    {
        //Debug.Log(currentTile.name);
        failureSound.Play();
        Vector2 originalSize = currentTile.sizeDelta;
        Vector2 scaleSize = new Vector2(currentTile.sizeDelta.x - 10, currentTile.sizeDelta.y - 10);
        float duration = 0.1f;

        currentTile.DOSizeDelta(scaleSize, duration)
                .OnComplete(() =>
                {
                    currentTile.DOSizeDelta(originalSize, duration);
                }
            );
    }

    private void SetLayout(int level)
    {
        int colorGroupIdx = Random.Range(0, Colors.colors.Length);
        int loongPosition = Random.Range(0, level * level);
        int dragonCount = 0;

        background.color = Colors.colors[colorGroupIdx][2];
        card.color = Colors.colors[colorGroupIdx][1];
        frame.color = Colors.colors[colorGroupIdx][3];

        int halfTileSize = halfFrameSize / level;

        List<Vector2> positions = new List<Vector2>();

        for (int i = 0; i < level; i++)
        {
            for (int j = 0; j < level; j++)
            {
                int x = -halfFrameSize + halfTileSize + 2 * i * halfTileSize;
                int y = -halfFrameSize + halfTileSize + 2 * j * halfTileSize;

                Vector2 position = new Vector2 (x, y);
                positions.Add(position);
            }
        }


        for (int i = 0; i < level * level; i++)
        {
            if (i == loongPosition)
            {
                loongTiles[0].GetComponent<ImageWithRoundedCorners>().radius = (halfTileSize * 2 - 10) / 10f;
                loongTiles[0].gameObject.SetActive(true);
                loongTiles[0].anchoredPosition = positions[i];
                loongTiles[0].GetComponent<Image>().color = Colors.colors[colorGroupIdx][0];
                loongTiles[0].sizeDelta = new Vector2(halfTileSize * 2 - 10, halfTileSize * 2 - 10);
            }
            else
            {
                dragonTiles[dragonCount].GetComponent<ImageWithRoundedCorners>().radius = (halfTileSize * 2 - 10) / 10f;
                dragonTiles[dragonCount].gameObject.SetActive(true);
                dragonTiles[dragonCount].anchoredPosition = positions[i];
                dragonTiles[dragonCount].GetComponent<Image>().color = Colors.colors[colorGroupIdx][0];
                dragonTiles[dragonCount].sizeDelta = new Vector2(halfTileSize * 2 - 10, halfTileSize * 2 - 10);
                dragonCount++;
            }
        }
    }
}
