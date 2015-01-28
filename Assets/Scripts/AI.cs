using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using System.Collections;
public class AI : MonoBehaviour
{
    public enum CellValue { Blank, X, O };

    private CellValue[,] _board = new CellValue[3, 3];
    public static AI Instance;

    private void Start()
    {
        Instance = this;
    }
    
    public CellValue ValueAt(int row, int col)
    {
        return _board[row, col];
    }

    public bool MakeMove(int position)
    {
        int row, col;
        ConvertPositionToRowCol(position, out row, out col);

        if (ValueAt(row, col) != CellValue.Blank)
        {
            return false;
        }

        _board[row, col] = CellValue.X;

        return true;

    }

    public void ConvertPositionToRowCol(int position, out int row, out int col)
    {
        switch (position)
        {
            case 1:
                row = 0;
                col = 0;
                break;
            case 2:
                row = 0;
                col = 1;
                break;
            case 3:
                row = 0;
                col = 2;
                break;
            case 4:
                row = 1;
                col = 0;
                break;
            case 5:
                row = 1;
                col = 1;
                break;
            case 6:
                row = 1;
                col = 2;
                break;
            case 7:
                row = 2;
                col = 0;
                break;
            case 8:
                row = 2;
                col = 1;
                break;
            case 9:
                row = 2;
                col = 2;
                break;
            default:
                throw new ArgumentException("Invalid Argument to ConvertPosition");
        }
    }

    public CellValue CheckForWinner()
    {
        /*
         * Check Horizontal
         * */
        for (int row = 0; row < 3; row++)
        {
            if (_board[row, 0] == _board[row, 1] &&
                _board[row, 1] == _board[row, 2] &&
                _board[row, 0] != CellValue.Blank)
            {
                return _board[row, 0];
            }
        }

        /*
         * Check Vertical
         * */
        for (int col = 0; col < 3; col++)
        {
            if (_board[0, col] == _board[1, col] &&
                _board[1, col] == _board[2, col] &&
                _board[0, col] != CellValue.Blank)
            {
                return _board[0, col];
            }
        }

        /*
         * Check Diagonals
         * */
        if (_board[0, 0] == _board[1, 1] &&
            _board[1, 1] == _board[2, 2] &&
            _board[0, 0] != CellValue.Blank)
        {
            return _board[0, 0];
        }

        if (_board[2, 0] == _board[1, 1] &&
            _board[1, 1] == _board[0, 2] &&
            _board[1, 1] != CellValue.Blank)
        {
            return _board[1, 1];
        }

        return CellValue.Blank;
    }

    private CellValue[,] convertBoard(int[,] sourceBoard)
    { 
        int sizex = 3;
        int sizey = 3;
        CellValue[,] newBoard = new CellValue[sizex, sizey];
        for (int i = 0; i < sizex; i++)
        {
            for (int j = 0; j < sizey; j++)
            {
                if (sourceBoard[i, j] == 1)
                {
                    newBoard[i, j] = CellValue.X;
                }
                else if (sourceBoard[i, j] == 2)
                {
                    newBoard[i, j] = CellValue.O;
                }
                else if (sourceBoard[i, j] == 0)
                {
                    newBoard[i, j] = CellValue.Blank;
                }
                else Debug.Log("Bad cell" + i + " " + j);
            }
        }
        return newBoard;
    }

    public int[] MakeAIMove(int[,] sourceBoard)
    {
        _board = convertBoard(sourceBoard);
        int[] bestMove = MiniMax(2, CellValue.O, int.MinValue, int.MaxValue);

        if (bestMove[1] < 0 || bestMove[2] < 0)
        {
            return null;
        }

        _board[bestMove[1], bestMove[2]] = CellValue.O;
        return bestMove;
    }

    private int[] MiniMax(int depth, CellValue player, int alpha, int beta)
    {
        var nextMoves = GenerateAvailableMoves();

        int score;
        int bestRow = -1;
        int bestCol = -1;

        if (nextMoves.Count == 0 || depth == 0)
        {
            //Game over or depth reached.
            score = Evaluate();
            return new int[] { score, bestRow, bestCol };
        }

        foreach (int move in nextMoves)
        {
            int row, col;
            ConvertPositionToRowCol(move, out row, out col);

            //try this move for current "player"
            _board[row, col] = player;

            if (player == CellValue.O) //Maximizing player
            {
                score = MiniMax(depth - 1, CellValue.X, alpha, beta)[0];
                if (score > alpha)
                {
                    alpha = score;
                    bestRow = row;
                    bestCol = col;
                }
            }
            else //Minimizing Player
            {
                score = MiniMax(depth - 1, CellValue.O, alpha, beta)[0];
                if (score < beta)
                {
                    beta = score;
                    bestRow = row;
                    bestCol = col;
                }
            }

            //undo move
            _board[row, col] = CellValue.Blank;

            //cut-off
            if (alpha >= beta) break;
        }

        return new int[] { player == CellValue.O ? alpha : beta, bestRow, bestCol };
    }

    public List<int> GenerateAvailableMoves()
    {
        var nextMoves = new List<int>();

        if (CheckForWinner() != CellValue.Blank) return nextMoves;

        for (int position = 1; position <= 9; position++)
        {
            int row, col;
            ConvertPositionToRowCol(position, out row, out col);

            if (_board[row, col] == CellValue.Blank) nextMoves.Add(position);
        }

        return nextMoves;
    }

    private int Evaluate()
    {
        int score = 0;
        score += EvaluateLine(0, 0, 0, 1, 0, 2);  // row 0
        score += EvaluateLine(1, 0, 1, 1, 1, 2);  // row 1
        score += EvaluateLine(2, 0, 2, 1, 2, 2);  // row 2
        score += EvaluateLine(0, 0, 1, 0, 2, 0);  // col 0
        score += EvaluateLine(0, 1, 1, 1, 2, 1);  // col 1
        score += EvaluateLine(0, 2, 1, 2, 2, 2);  // col 2
        score += EvaluateLine(0, 0, 1, 1, 2, 2);  // diagonal
        score += EvaluateLine(0, 2, 1, 1, 2, 0);  // alternate diagonal
        return score;

    }

    private int EvaluateLine(int row1, int col1, int row2, int col2, int row3, int col3)
    {
        int score = 0;

        //First Cell
        if (_board[row1, col1] == CellValue.O)
        {
            score = 1;
        }
        else if (_board[row1, col1] == CellValue.X)
        {
            score = -1;
        }

        //Second Cell
        if (_board[row2, col2] == CellValue.O)
        {
            if (score == 1) score = 10;
            else if (score == -1) return 0;
            else score = 1;
        }
        else if (_board[row2, col2] == CellValue.X)
        {
            if (score == -1) score = -10;
            else if (score == 1) return 0;
            else score = -1;
        }

        //Third Cell
        if (_board[row3, col3] == CellValue.O)
        {
            if (score > 0) score *= 10;
            else if (score < 0) return 0;
            else score = 1;
        }
        else if (_board[row3, col3] == CellValue.X)
        {
            if (score < 0) score *= 10;
            else if (score > 1) return 0;
            else score = -1;
        }

        return score;
    }
}




