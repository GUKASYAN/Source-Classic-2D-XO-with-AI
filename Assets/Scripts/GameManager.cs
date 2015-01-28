using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int X = 5;
    public int Y = 5;
    private int i, j, k;
    public GameObject Item;
    public Transform Lights;
    private bool _moveLightsTumbler;
    public bool turn;
    public int[] Score;
    public int[,] GameMatrix;
    public int MovsCount;
    public Dictionary<int[], Transform> Items;
    public static GameManager Instance;
    public bool GameStarted;
    public bool AImode;

    private void Start()
    {
        X = 5;
        Y = 5;
        Instance = this;
        GameStarted = false;
        AImode = true;
        _moveLightsTumbler = true;
        Vector3 itemPosition = Vector3.zero;
        itemPosition.z = -2;
        turn = true;
        Score = new int[2];
        //UIController.instance.UpdateScore();
        // UIController.instance.UpdateTurn(); 
        const int stepY = 3;
        const int stepX = 4;
        MovsCount = 0;
        int offsetY = Math.Abs(Y/2)*stepY;
        int offsetX = -(Math.Abs(X/2)*stepX);
        Items = new Dictionary<int[], Transform>();
        GameMatrix = new int[X, Y]; // игровая матрица: 0-empty 1-X 2-O 
        Debug.Log(X);
        for (i = 0; i < Y; i++)
        {
            for (j = 0; j < X; j++)
            {
                GameMatrix[i, j] = 0;
                itemPosition.x = offsetX;
                itemPosition.y = offsetY;
                var go = (GameObject) Instantiate(Item, itemPosition, Quaternion.identity);
                go.name = "Item[" + i + "]" + "[" + j + "]";
                Items.Add(new[] {i, j}, go.transform);
                offsetX += stepX;
            }
            offsetX = -(Math.Abs(X/2)*stepX);
            offsetY -= stepY;
        }
    }

    private void Update()
    {
        if (GameStarted)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, 10))
            {
                if (hit.collider.gameObject.GetComponent<MeshRenderer>().material.color == Color.white)
                {
                    _moveLightsTumbler = !_moveLightsTumbler;
                    SoundManager.Instance.PlayChecked();
                    SelectItem(hit.collider);
                    turn = !turn;
                    if (AImode)
                    {
                        if (MovsCount < X*Y)
                        {
                            int[] ans = AI.Instance.MakeAIMove(GameMatrix);
                            if (ans != null)
                                SelectItem(
                                    Items.FirstOrDefault(xx => xx.Value.name == ("Item[" + ans[1] + "][" + ans[2] + "]"))
                                        .Value.collider);
                            turn = !turn;
                        }
                    }
                    UIController.Instance.UpdateTurn();
                }
                else SoundManager.Instance.PlayFalse();
            }
            moveLights();
        }
    }
    private void moveLights()
    {
        Vector3 target = Lights.localPosition;
        if (_moveLightsTumbler)
            target.x = 5;
        else
            target.x = -5;
        Lights.localPosition = Vector3.Lerp(Lights.transform.localPosition, target, 0.01f);
    }

    public void SelectItem(Collider col)
    {
        col.gameObject.animation.Play("Item_Y_rotate");
        if (turn)
        {
            col.transform.FindChild("Label-XO").GetComponent<TextMesh>().text = "X";
            col.gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
            GameMatrix[
                Items.FirstOrDefault(xx => xx.Value == col.transform).Key[0],
                Items.FirstOrDefault(xx => xx.Value == col.transform).Key[1]] = 1;
            CheckWin();
        }
        else
        {
            col.transform.FindChild("Label-XO").GetComponent<TextMesh>().text = "O";
            col.gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
            GameMatrix[
                Items.FirstOrDefault(xx => xx.Value == col.transform).Key[0],
                Items.FirstOrDefault(xx => xx.Value == col.transform).Key[1]] = 2;
            CheckWin();
        }
        MovsCount++;
        if (MovsCount >= X*Y) ScoreCount(3);
    }

    private void CheckWin()
    {
        int winAmount = Y;
        var winLine = new int[winAmount, 2];

        int count = 0;
        for (i = 0; i < Y; i++) // check horizontal
        {
            j = 0;
            while (GameMatrix[i, j] == GameMatrix[i, j + 1] && GameMatrix[i, j] != 0 && GameMatrix[i, j + 1] != 0)
            {
                winLine[count, 0] = i;
                winLine[count, 1] = j;
                count++;
                if (count == winAmount - 1)
                {
                    ScoreCount(GameMatrix[i, j]);

                    winLine[count, 0] = i;
                    winLine[count, 1] = j + 1;
                    for (k = 0; k < winAmount; k++)
                        foreach (Transform item in Items.Values)
                        {
                            if (item.name.Contains("[" + winLine[k, 0] + "]" + "[" + winLine[k, 1] + "]"))
                            {
                                item.GetComponent<MeshRenderer>().material.color = Color.green;
                            }
                        }
                }
                j++;
                if (j == Y - 1) break;
            }
            count = 0;
        }
        count = 0;
        for (i = 0; i < X; i++) // check verical
        {
            j = 0;
            while (GameMatrix[j, i] == GameMatrix[j + 1, i] && GameMatrix[j + 1, i] != 0 && GameMatrix[j + 1, i] != 0)
            {
                winLine[count, 0] = j;
                winLine[count, 1] = i;
                count++;
                if (count == winAmount - 1)
                {
                    ScoreCount(GameMatrix[j, i]);
                    winLine[count, 0] = j + 1;
                    winLine[count, 1] = i;
                    for (k = 0; k < winAmount; k++)
                        foreach (Transform item in Items.Values)
                        {
                            if (item.name.Contains("[" + winLine[k, 0] + "]" + "[" + winLine[k, 1] + "]"))
                            {
                                item.GetComponent<MeshRenderer>().material.color = Color.green;
                            }
                        }
                    break;
                }
                j++;
                if (j == Y - 1) break;
            }
            count = 0;
        }
        count = 0;
        for (i = 0; i < Y - 1; i++) // check up-diagonal
        {
            if (GameMatrix[i, i] == GameMatrix[i + 1, i + 1] && GameMatrix[i, i] != 0 && GameMatrix[i + 1, i + 1] != 0)
            {
                winLine[count, 0] = i;
                winLine[count, 1] = i;
                count++;
                if (count == winAmount - 1)
                {
                    ScoreCount(GameMatrix[i, i]);
                    winLine[count, 0] = i + 1;
                    winLine[count, 1] = i + 1;
                    for (k = 0; k < winAmount; k++)
                        foreach (Transform item in Items.Values)
                        {
                            if (item.name.Contains("[" + winLine[k, 0] + "]" + "[" + winLine[k, 1] + "]"))
                            {
                                item.GetComponent<MeshRenderer>().material.color = Color.green;
                            }
                        }
                    break;
                }
                j++;
                if (j == Y - 1) break;
            }
            else
            {
                count = 0;
            }
        }
        count = 0;
        j = 0;
        for (i = Y - 1; i > 0; i--) // check down-diagonal
        {
            if (GameMatrix[j, i] == GameMatrix[j + 1, i - 1] && GameMatrix[j, i] != 0 && GameMatrix[j + 1, i - 1] != 0)
            {
                winLine[count, 0] = j;
                winLine[count, 1] = i;
                count++;
                if (count == winAmount - 1)
                {
                    ScoreCount(GameMatrix[j, i]);
                    winLine[count, 0] = j + 1;
                    winLine[count, 1] = i - 1;
                    for (k = 0; k < winAmount; k++)
                        foreach (Transform item in Items.Values)
                        {
                            if (item.name.Contains("[" + winLine[k, 0] + "]" + "[" + winLine[k, 1] + "]"))
                            {
                                item.GetComponent<MeshRenderer>().material.color = Color.green;
                            }
                        }
                    break;
                }
                j++;
                if (j == Y - 1) break;
            }
            else
            {
                count = 0;
            }
        }
    }

    private void ScoreCount(int type)
    {
        GameStarted = false;
        turn = true;
        if (type == 1)
        {
            Debug.Log("X-WIN");
            Score[0]++;
            UIController.Instance.WinPanel(type);
        }
        if (type == 2)
        {
            Debug.Log("O-WIN");
            Score[1]++;
            UIController.Instance.WinPanel(type);
        }
        if (type == 3)
        {
            Debug.Log("Draw");
            Score[0]++;
            Score[1]++;
            UIController.Instance.WinPanel(type);
        }
    }
}