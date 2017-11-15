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

    private const int INFINITE = 10000;
    private const int MINUS_INFINITE = -10000;

    private Board board;
    private string activePlayer;

    private int previousScore = MINUS_INFINITE;
    [SerializeField]
    private int windowRange = 5;

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

    public void ResetPreviousScore()
    {
        previousScore = MINUS_INFINITE;
    }

    public void Play(string actPlayer)
    {
        ScoringMove move;
        activePlayer = actPlayer;
        ObserveBoard();

        DateTime timeBefore;

        timeBefore = DateTime.Now;
        move = Minimax(board, 0);
        DateTime timeAfter = DateTime.Now;
        moveText.text = "Move: " + move.move;
        scoreText.text = "Score: " + move.score;
        timeText.text = "Time: " + (timeAfter - timeBefore).TotalSeconds;

        timeBefore = DateTime.Now;
        move = Negamax(board, 0);
        timeAfter = DateTime.Now;
        moveText.text += "//: " + move.move;
        scoreText.text += "//: " + move.score;
        timeText.text += "//: " + (timeAfter - timeBefore).TotalSeconds;

        timeBefore = DateTime.Now;
        move = NegamaxAB(board, 0, MINUS_INFINITE, INFINITE);
        timeAfter = DateTime.Now;
        moveText.text += "//: " + move.move;
        scoreText.text += "//: " + move.score;
        timeText.text += "//: " + (timeAfter - timeBefore).TotalSeconds;

        timeBefore = DateTime.Now;
        move = AspirationSearch(board);
        timeAfter = DateTime.Now;
        moveText.text += "//: " + move.move;
        scoreText.text += "//: " + move.score;
        timeText.text += "//: " + (timeAfter - timeBefore).TotalSeconds;

        //Debug.Log("Jugador Activo:" + activePlayer + " Jugada Elegida:" + move.move + "/" + move.score);

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
    
    ScoringMove Minimax(Board board, byte depth)
    {
        // Returns the board score and the play with which it gets to the score
        int bestMove = 0;
        int bestScore = 0;
        ScoringMove scoringMove; // score, move
        Board newBoard;

        // Check if we are finished recursing
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

                // Recursivity
                scoringMove = Minimax(newBoard, (byte) (depth + 1));

                // Update best score
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

    ScoringMove Negamax(Board board, byte depth)
    {
        // Returns the board score and the play with which it gets to the score
        int bestMove = 0;
        int bestScore = 0;
        ScoringMove scoringMove; // score, move
        Board newBoard;

        // Check if we are finished recursing
        if (board.IsEndOfGame() || depth == MAX_DEPTH)
        {
            if(depth%2 == 0)
            {
                scoringMove = new ScoringMove(board.Evaluate(activePlayer), 0);
            }else
            {
                scoringMove = new ScoringMove(-board.Evaluate(activePlayer), 0);
            }
            
        }
        else
        {
            bestScore = MINUS_INFINITE;

            int[] possibleMoves;
            possibleMoves = board.PossibleMoves();

            foreach (int move in possibleMoves)
            {
                newBoard = board.GenerateNewBoardFromMove(move);

                // Recursivity
                scoringMove = Negamax(newBoard, (byte)(depth + 1));

                int invertedScore = -scoringMove.score;

                // Update best score
                
                if (invertedScore > bestScore)
                {
                    bestScore = invertedScore;
                    bestMove = move;
                }
                
            }
            scoringMove = new ScoringMove(bestScore, bestMove);
        }
        return scoringMove;
    }

    ScoringMove NegamaxAB(Board board, byte depth, int alpha, int beta)
    {
        int bestMove = 0;
        int bestScore = 0;
        ScoringMove scoringMove; // score, move
        Board newBoard;

        // Check if we are finished recursing
        if (board.IsEndOfGame() || depth == MAX_DEPTH)
        {
            if (depth % 2 == 0)
            {
                scoringMove = new ScoringMove(board.Evaluate(activePlayer), 0);
            }
            else
            {
                scoringMove = new ScoringMove(-board.Evaluate(activePlayer), 0);
            }

        }
        else
        {
            bestScore = MINUS_INFINITE;

            int[] possibleMoves;
            possibleMoves = board.PossibleMoves();

            foreach (int move in possibleMoves)
            {
                newBoard = board.GenerateNewBoardFromMove(move);

                // Recursivity
                scoringMove = NegamaxAB(newBoard, (byte)(depth + 1), -beta, -Math.Max(alpha, bestScore));

                int invertedScore = -scoringMove.score;

                // Update best score

                if (invertedScore > bestScore)
                {
                    bestScore = invertedScore;
                    bestMove = move;
                }
                if(bestScore >= beta)   //Prune
                {
                    scoringMove = new ScoringMove(bestScore, bestMove);
                    return scoringMove;
                }
            }
            scoringMove = new ScoringMove(bestScore, bestMove);
        }
        return scoringMove;
    }

    // Test with depth:6, window:8
    // More efficient when player makes good moves
    ScoringMove AspirationSearch(Board board)
    {
        int alpha, beta;
        ScoringMove move;

        if(previousScore != MINUS_INFINITE)
        {
            alpha = previousScore - windowRange;
            beta = previousScore + windowRange;
            while (true)
            {
                move = NegamaxAB(board, 0, alpha, beta);
                if (move.score <= alpha)        alpha = MINUS_INFINITE;
                else if (move.score >= beta)    beta = INFINITE;
                else                            break;
            }
        }
        else
        {
            move = NegamaxAB(board, 0, MINUS_INFINITE, INFINITE);
        }
        previousScore = move.score;
        return move;
    }

}