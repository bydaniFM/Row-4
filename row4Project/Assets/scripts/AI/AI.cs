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
    [SerializeField]
    private Text aspirationPathText;

    public Text moveText, scoreText, timeText;

    // PARA FUNCIONAMIENTO CON HASH
    public ZobristKeys zobristKeys;
    protected TranspositionTable transpositionTable;
    [SerializeField]
    private int hashTableLength = 90000000;
    private int maximumExploredDepth = 0;
    private int globalGuess = INFINITE;
    [SerializeField]
    private int MAX_ITERATIONS = 10;

    void Awake ()
    {
        zobristKeys = new ZobristKeys(42, 2);
        // zobristKeys.Print();
        transpositionTable = new TranspositionTable(hashTableLength);
    }

    public void SetButtonList(Text[,] bl)
    {
        buttonList = bl;
    }

    public void SetGameController(GameController gc)
    {
        gameController = gc;
    }

    public void ResetPreviousScore ()
    {
        previousScore = MINUS_INFINITE;
    }

    public void Play(string actPlayer)
    {
        ScoringMove move;
        activePlayer = actPlayer;
        ObserveBoard();

        DateTime DateBefore = DateTime.Now;
        move = Minimax (board, 0);
        DateTime DateAfter = DateTime.Now;
        moveText.text = "Move: " + move.move;
        scoreText.text = "Score: " + move.score;
        timeText.text = "Time: " + (DateAfter - DateBefore).TotalSeconds;

        DateBefore = DateTime.Now;
        move = Negamax (board, 0);
        DateAfter = DateTime.Now;
        moveText.text += " // " + move.move;
        scoreText.text += " // " + move.score;
        timeText.text += "\n // " + (DateAfter - DateBefore).TotalSeconds;

        DateBefore = DateTime.Now;
        move = NegamaxAB (board, 0, MINUS_INFINITE, INFINITE);
        DateAfter = DateTime.Now;
        moveText.text += " // " + move.move;
        scoreText.text += " // " + move.score;
        timeText.text += "\n // " + (DateAfter - DateBefore).TotalSeconds;

        DateBefore = DateTime.Now;
        move = AspirationSearch (board);
        DateAfter = DateTime.Now;
        moveText.text += " // " + move.move;
        scoreText.text += " // " + move.score;
        timeText.text += "\n // " + (DateAfter - DateBefore).TotalSeconds;

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
        board.zobristKeys = this.zobristKeys;
        board.CalculateHashValue();
    }

    void Move (ScoringMove scoringMove)
    {
        gameController.FillColumn((byte)scoringMove.move);
        gameController.EndTurn();
    }

    /*
    ScoringMove FakeMinimax(Board board, string activePlayer, byte depth)
    {
        ScoringMove bestMove = new ScoringMove(0, 0);
        int[] possibleMoves;
        possibleMoves = board.PossibleMoves();
        foreach (int move in possibleMoves)
        {
            if (board.IsEmptySpace(move))
            {
                bestMove = new ScoringMove(0, move);
            }
        }
        return bestMove;
    }
    */
    
    ScoringMove Minimax(Board board, byte depth)
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
                scoringMove = Minimax(newBoard, (byte) (depth + 1));

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

    ScoringMove Negamax (Board board, byte depth)
    {
        // Devuelve el score del tablero y la jugada con la que se llega a él.
        int bestMove = 0;
        int bestScore = 0;
        ScoringMove scoringMove; // score, movimiento
        Board newBoard;
        // Comprobar si hemos terminado de hacer recursión
        if (board.IsEndOfGame() || depth == MAX_DEPTH)
        {
            if (depth%2 == 0)
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

                // Recursividad
                scoringMove = Negamax(newBoard, (byte)(depth + 1));

                int invertedScore = -scoringMove.score;

                // Actualizar mejor score
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

    ScoringMove NegamaxAB (Board board, byte depth, int alfa, int beta)
    {
        // Devuelve el score del tablero y la jugada con la que se llega a él.
        int bestMove = 0;
        int bestScore = 0;

        ScoringMove scoringMove; // score, movimiento
        Board newBoard;
        // Comprobar si hemos terminado de hacer recursión
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

                // Recursividad
                scoringMove = NegamaxAB (newBoard, (byte)(depth + 1), -beta, -Math.Max(alfa, bestScore));

                int invertedScore = -scoringMove.score;

                // Actualizar mejor score
                if (invertedScore > bestScore)
                {
                    bestScore = invertedScore;
                    bestMove = move;
                }
                if (bestScore >= beta)
                {
                    scoringMove = new ScoringMove(bestScore, bestMove);
                    return scoringMove;
                }
            }
            scoringMove = new ScoringMove(bestScore, bestMove);
        }
        return scoringMove;
    }

    ScoringMove AspirationSearch (Board board)
    {
        int alfa, beta;
        ScoringMove move;
        string aspirationPath ="Aspiration Path: ";

        if (previousScore != MINUS_INFINITE)
        {
            alfa = previousScore - windowRange;
            beta = previousScore + windowRange;
            while (true)
            {
                move = NegamaxAB(board, 0, alfa, beta);
                if (move.score <= alfa)
                {
                    aspirationPath += "Fail soft alfa.";
                    alfa = MINUS_INFINITE;
                }
                else if (move.score >= beta)
                {
                    aspirationPath += "Fail soft beta.";
                    beta = INFINITE;
                }
                else
                {
                    aspirationPath += "Success";
                    break;
                }
            }
        }
        else
        {
            aspirationPath += "Normal Negamax";
            move = NegamaxAB(board, 0, MINUS_INFINITE, INFINITE);
        }
        previousScore = move.score;
        aspirationPathText.text = aspirationPath;
        return move;
    }

    ScoringMove Test (Board board, byte depth, int gamma)
    {
        // Devuelve el score del tablero y la jugada con la que se llega a él.
        int bestMove = 0;
        int bestScore = 0;

        ScoringMove scoringMove; // score, movimiento
        Board newBoard;
        Record record;

        if (depth > maximumExploredDepth)
        {
            maximumExploredDepth = depth;
        }

        record = transpositionTable.GetRecord(board.hashValue);

        if (record != null)
        {
            if (record.depth > MAX_DEPTH - depth)
            {
                if (record.minScore > gamma)
                {
                    scoringMove = new ScoringMove(record.minScore, record.bestMove);
                    return scoringMove;
                }
                if (record.maxScore < gamma)
                {
                    scoringMove = new ScoringMove(record.maxScore, record.bestMove);
                    return scoringMove;
                }
            }
        }
        else
        {
            record = new Record();
            record.hashValue = board.hashValue;
            record.depth = MAX_DEPTH - depth;
            record.minScore = MINUS_INFINITE;
            record.maxScore = INFINITE;
        }
        
        
        // Comprobar si hemos terminado de hacer recursión
        if (board.IsEndOfGame() || depth == MAX_DEPTH)
        {
            if (depth % 2 == 0)
            {
                record.maxScore = board.Evaluate(activePlayer);
            }
            else
            {
                record.maxScore = - board.Evaluate(activePlayer);
            }
            record.minScore = record.maxScore;
            transpositionTable.SaveRecord(record);
            scoringMove = new ScoringMove(record.maxScore, 0);
        }
        else
        {
            bestScore = MINUS_INFINITE;

            int[] possibleMoves;
            possibleMoves = board.PossibleMoves();

            foreach (int move in possibleMoves)
            {
                // newBoard = board.GenerateNewBoardFromMove(move);
                newBoard = board.HashGenerateNewBoardFromMove(move);

                // Recursividad
                scoringMove = Test(newBoard, (byte)(depth + 1), -gamma);

                int invertedScore = -scoringMove.score;

                // Actualizar mejor score
                if (invertedScore > bestScore)
                {
                    bestScore = invertedScore;
                    bestMove = move;
                    record.bestMove = move;
                }
                if (bestScore < gamma)
                {
                    record.maxScore = bestScore;
                }
                else
                {
                    record.minScore = bestScore;
                }
            }
            transpositionTable.SaveRecord(record);
            scoringMove = new ScoringMove(bestScore, bestMove);
        }
        return scoringMove;
    }


}