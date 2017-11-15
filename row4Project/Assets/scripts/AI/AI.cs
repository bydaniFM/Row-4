using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AI : MonoBehaviour
{
    public byte MAX_DEPTH = 15;

    public Text[,] buttonList;
    public GameController gameController;

    private int INFINITE = 10000;
    private int MINUS_INFINITE = -10000;

    private Board board;
    private string activePlayer;

    public Text moveText;
    public Text scoreText;
    public Text timeText;

    public void SetButtonList(Text[,] bl)
    {
        buttonList = bl;
    }

    public void SetGameController(GameController gc)
    {
        gameController = gc;
    }

    public void Play(string actPlayer)
    {
        ScoringMove move;
        activePlayer = actPlayer;
        ObserveBoard();

        DateTime timeBefore = DateTime.Now;
        move = Minimax(board, activePlayer, 0);
        DateTime timeAfter = DateTime.Now;
        moveText.text = "Move: " + move.move;
        scoreText.text = "Score: " + move.score;
        timeText.text = "Time: " + (timeAfter - timeBefore);
        Debug.Log("Jugador Activo:" + activePlayer + " Jugada Elegida:" + move.move + "/" + move.score);

        Move(move);
    }

    void ObserveBoard()
    {
        board = new Board(gameController.rows, gameController.columns);
        byte rows = gameController.rows;
        byte columns = gameController.columns;
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                Text spaceText = buttonList[row, column];
                board.spaces[row, column] = spaceText.text;
            }
        }
        board.activePlayer = this.activePlayer;
    }

    void Move(ScoringMove scoringMove)
    {
        gameController.FillColumn((byte)scoringMove.move);
        gameController.EndTurn();
    }
    
    //ScoringMove FakeMinimax(Board board, string activePlayer, byte depth)
    //{
    //    ScoringMove bestMove = new ScoringMove(0, 0);
    //    int[] possibleMoves;
    //    possibleMoves = board.PossibleMoves();
    //    foreach (int move in possibleMoves)
    //    {
    //        if (board.IsEmptySpace(move))
    //        {
    //            bestMove = new ScoringMove(0, move);
    //        }
    //    }
    //    return bestMove;
    //}
    
    ScoringMove Minimax(Board board, string activePlayer, byte depth)
    {
        // Devuelve el score del tablero y la jugada con la que se llega a él.
        int bestMove = 0;
        int bestScore = 0;
        ScoringMove scoringMove; // score, movimiento
        Board newBoard;
        // Comprobar si hemos terminado de hacer recursión
        if (board.IsEndOfGame() || depth == MAX_DEPTH)
        {
            scoringMove = new ScoringMove(board.Evaluate(activePlayer), 0);
        }
        else
        {
            if (board.activePlayer == activePlayer) bestScore = MINUS_INFINITE;
            else bestScore = INFINITE;

            int[] possibleMoves;
            possibleMoves = board.PossibleMoves();

            foreach (int move in possibleMoves)
            {
                newBoard = board.GenerateNewBoardFromMove(move);

                // Recursividad
                scoringMove = Minimax(newBoard, activePlayer, (byte) (depth + 1));

                // Actualizar mejor score
                if (board.activePlayer == activePlayer)
                {
                    if (scoringMove.score > bestScore)
                    {
                        bestScore = scoringMove.score;
                        bestMove = move;
                    }
                }
                else
                {
                    if (scoringMove.score < bestScore)
                    {
                        bestScore = scoringMove.score;
                        bestMove = move;
                    }
                }
            }
            scoringMove = new ScoringMove(bestScore, bestMove);
        }
        return scoringMove;
    }

}