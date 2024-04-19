using DG.Tweening;
using Nobi.UiRoundedCorners;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RectTransform loongTile;
    [SerializeField] private RectTransform dragonTile;
    [SerializeField] private RectTransform loongBar;
    [SerializeField] private RectTransform dragonBar;
    [SerializeField] private RectTransform progressBar;

    [SerializeField] private Image background;
    [SerializeField] private Image card;
    [SerializeField] private Image frame;

    [SerializeField] private AudioSource successSound;
    [SerializeField] private AudioSource failureSound;

    [SerializeField] private List<RectTransform> files;


    private List<RectTransform> dragonTiles;
    private List<RectTransform> loongTiles;

    private int halfFrameSize = 360;
    private int correctCount = 1;
    private int winningCount = 1;
    private int firstCorrectCount = 1;
    private int barLength = 0;

    private float duration = 0.1f;

    private bool isFileOpen = false;

    Vector2 originalFilePositon;

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
        SetLayout(2);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();

            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }

    public void OnLoongClicked(RectTransform currentTile)
    {
        successSound.Play();
        Vector2 originalSize = currentTile.sizeDelta;
        Vector2 scaleSize = new Vector2(currentTile.sizeDelta.x - 10, currentTile.sizeDelta.y - 10);

        firstCorrectCount++;
        correctCount ++;
        barLength = correctCount * 5 * winningCount;

        if (firstCorrectCount < 8)
        {
            currentTile.DOSizeDelta(scaleSize, duration)
                .OnComplete(() =>
                {
                    currentTile.DOSizeDelta(originalSize, duration)
                        .OnComplete(() => { SetLayout(correctCount / 2 + 2); });
                });
        }
        else
        {
            currentTile.DOSizeDelta(scaleSize, duration)
                .OnComplete(() =>
                {
                    currentTile.DOSizeDelta(originalSize, duration)
                        .OnComplete(() => { SetLayout(6); });
                });
        }



        if (barLength >= 500)
        {
            correctCount = 1;
            winningCount = winningCount + 1 > 20 ? 20 : winningCount + 1;
            barLength = correctCount * 5 * winningCount;

            int index = winningCount - 2;

            if (index < 18)
            {
                files[index].DOSizeDelta(new Vector2(files[index].sizeDelta.x + 30, files[index].sizeDelta.y + 30), duration)
                .OnComplete(() =>
                {
                    files[index].DORotate(new Vector3(0, 0, 30), duration)
                        .OnComplete(() =>
                        {
                            files[index].DORotate(new Vector3(0, 0, -30), duration * 2)
                                .OnComplete(() =>
                                {
                                    files[index].DORotate(Vector3.zero, duration)
                                        .OnComplete(() =>
                                        {
                                            files[index].DOSizeDelta(new Vector2(files[index].sizeDelta.x - 30, files[index].sizeDelta.y - 30), duration)
                                                .OnComplete(() =>
                                                {
                                                    files[index].Find("Button").gameObject.SetActive(true);
                                                    files[index].Find("LineMiddle").gameObject.SetActive(true);

                                                    (files[index].Find("LineUp") as RectTransform)
                                                        .DOAnchorPos(new Vector2(0, 35), duration * 2)
                                                            .OnComplete(() =>
                                                            {
                                                                (files[index].Find("LineUp") as RectTransform)
                                                                    .DORotate(Vector3.zero, duration * 2);
                                                            });

                                                    (files[index].Find("LineDown") as RectTransform)
                                                        .DOAnchorPos(new Vector2(0, -35), duration * 2)
                                                            .OnComplete(() =>
                                                            {
                                                                (files[index].Find("LineDown") as RectTransform)
                                                                    .DORotate(Vector3.zero, duration * 2);
                                                            });
                                                });
                                        });
                                });
                        });
                });
            }
            
        }

        loongBar.DOSizeDelta(new Vector2(loongBar.sizeDelta.x + 20, loongBar.sizeDelta.y + 20), duration * 2)
            .OnComplete(() =>
            {
                loongBar.DOSizeDelta(new Vector2(loongBar.sizeDelta.x - 20, loongBar.sizeDelta.y - 20), duration * 2);
            }
        );

        progressBar.DOSizeDelta(new Vector2(barLength, progressBar.sizeDelta.y), duration * 2);
    }

    public void OnDragonClicked(RectTransform currentTile)
    {
        //Debug.Log(currentTile.name);
        failureSound.Play();
        Vector2 originalSize = currentTile.sizeDelta;
        Vector2 scaleSize = new Vector2(currentTile.sizeDelta.x - 10, currentTile.sizeDelta.y - 10);
        float duration = 0.1f;
        correctCount = correctCount - 1 < 1 ? 1 : correctCount - 1;

        currentTile.DOSizeDelta(scaleSize, duration)
                .OnComplete(() =>
                {
                    currentTile.DOSizeDelta(originalSize, duration);
                }
            );

        dragonBar.DOSizeDelta(new Vector2(dragonBar.sizeDelta.x + 20, dragonBar.sizeDelta.y + 20), duration * 2)
            .OnComplete(() =>
            {
                dragonBar.DOSizeDelta(new Vector2(dragonBar.sizeDelta.x - 20, dragonBar.sizeDelta.y - 20), duration * 2);
            }
        );

        progressBar.DOSizeDelta(new Vector2(correctCount * 5 * winningCount, progressBar.sizeDelta.y), duration * 2);
    }

    private void SetLayout(int level)
    {
        int colorGroupIndex = Random.Range(0, Colors.colors.Length);

        background.DOColor(Colors.colors[colorGroupIndex][2], duration * 2);
        card.DOColor(Colors.colors[colorGroupIndex][1], duration * 2);
        frame.DOColor(Colors.colors[colorGroupIndex][3], duration * 2);

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

        for (int i = level * level - 1; i > 0; i--)
        {
            int k = Random.Range(0, i);
            Vector2 value = positions[k];
            positions[k] = positions[i];
            positions[i] = value;
        }

        for (int i = 0; i < level * level; i++)
        {
            if (i < winningCount && i < 18)
            {
                loongTiles[i].GetComponent<ImageWithRoundedCorners>().radius = (halfTileSize * 2 - 10) / 10f;
                loongTiles[i].gameObject.SetActive(true);
                loongTiles[i].DOAnchorPos(positions[i], duration);
                loongTiles[i].GetComponent<Image>().DOColor(Colors.colors[colorGroupIndex][0], duration * 2);
                loongTiles[i].sizeDelta = new Vector2(halfTileSize * 2 - 10, halfTileSize * 2 - 10);

                dragonTiles[i].gameObject.SetActive(false);
            }
            else
            {
                dragonTiles[i].GetComponent<ImageWithRoundedCorners>().radius = (halfTileSize * 2 - 10) / 10f;
                dragonTiles[i].gameObject.SetActive(true);
                dragonTiles[i].DOAnchorPos(positions[i], duration);
                dragonTiles[i].GetComponent<Image>().DOColor(Colors.colors[colorGroupIndex][0], duration * 2);
                dragonTiles[i].sizeDelta = new Vector2(halfTileSize * 2 - 10, halfTileSize * 2 - 10);

                loongTiles[i].gameObject.SetActive(false);
            }
            
        }

        for (int i = 0; i < files.Count; i++)
        {
            if (i < winningCount - 1)
            {
                files[i].gameObject.GetComponent<Image>().DOColor(Colors.colors[colorGroupIndex][1], duration * 2);
                files[i].Find("LineUp").GetComponent<Image>().DOColor(Colors.colors[colorGroupIndex][3], duration * 2);
                files[i].Find("LineMiddle").GetComponent<Image>().DOColor(Colors.colors[colorGroupIndex][3], duration * 2);
                files[i].Find("LineDown").GetComponent<Image>().DOColor(Colors.colors[colorGroupIndex][3], duration * 2);
            }
            else
            {
                files[i].gameObject.GetComponent<Image>().DOColor(Colors.colors[colorGroupIndex][3], duration * 2);
                files[i].Find("LineUp").GetComponent<Image>().DOColor(Colors.colors[colorGroupIndex][0], duration * 2);
                files[i].Find("LineMiddle").GetComponent<Image>().DOColor(Colors.colors[colorGroupIndex][0], duration * 2);
                files[i].Find("LineDown").GetComponent<Image>().DOColor(Colors.colors[colorGroupIndex][0], duration * 2);
            }
            files[i].Find("Text (TMP)").GetComponent<TextMeshProUGUI>().DOColor(Colors.colors[colorGroupIndex][3], duration * 2);
        }
    }

    public void OnClickFile(int fileIndex)
    {

        if (!isFileOpen)
        {
            for (int i = 0; i < 18; i++)
            {
                if (fileIndex == i)
                {
                    originalFilePositon = files[fileIndex].anchoredPosition;

                    files[fileIndex].DOAnchorPos(Vector2.zero, duration * 2)
                        .OnComplete(() =>
                        {
                            files[fileIndex].Find("LineUp").gameObject.SetActive(false);
                            files[fileIndex].Find("LineMiddle").gameObject.SetActive(false);
                            files[fileIndex].Find("LineDown").gameObject.SetActive(false);
                            files[fileIndex].GetComponent<ImageWithRoundedCorners>().radius = 15;
                            float x = (files[fileIndex].parent as RectTransform).sizeDelta.x;
                            float y = (files[fileIndex].parent.parent as RectTransform).rect.height - 50;
                            files[fileIndex].DOSizeDelta(new Vector2(x, y), duration * 2)
                                .OnComplete(() =>
                                {
                                    files[fileIndex].Find("Text (TMP)").gameObject.SetActive(true);
                                });
                        });
                }
                else
                {
                    files[i].gameObject.SetActive(false);
                }
            }
            isFileOpen = true;
        }
        else
        {
            files[fileIndex].Find("Text (TMP)").gameObject.SetActive(false);
            files[fileIndex].DOSizeDelta(new Vector2(150, 200), duration * 2)
                .OnComplete(() =>
                {
                    files[fileIndex].Find("LineUp").gameObject.SetActive(true);
                    files[fileIndex].Find("LineMiddle").gameObject.SetActive(true);
                    files[fileIndex].Find("LineDown").gameObject.SetActive(true);
                    files[fileIndex].GetComponent<ImageWithRoundedCorners>().radius = 10;
                    files[fileIndex].DOAnchorPos(originalFilePositon, duration * 2)
                        .OnComplete(() =>
                        {
                            for (int i = 0; i < 18; i++)
                            {
                                if (fileIndex != i)
                                {
                                    files[i].gameObject.SetActive(true);
                                }
                            }
                        });
                });
            isFileOpen = false;
        }
    }
}
