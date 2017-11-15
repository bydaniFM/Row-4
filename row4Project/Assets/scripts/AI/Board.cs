using System;

public class Board {

    public string[,] spaces;
    public string activePlayer;

    public byte rows, columns;

    private byte[,] evaluationMatrix;

    public Board(byte _rows, byte _columns)
    {
        rows = _rows;
        columns = _columns;
        spaces = new string[rows, columns];

        evaluationMatrix = new byte[,] {{ 3, 4, 5, 7, 5, 4, 3},
                                        { 4, 6, 8, 10, 8, 6, 4},
                                        { 5, 8, 11, 13, 11, 8, 5},
                                        { 5, 8, 11, 13, 11, 8, 5},
                                        { 4, 6, 8, 10, 8, 6, 4},
                                        { 3, 4, 5, 7, 5, 4, 3}
                                       };
    }

    public int Evaluate(string player)
    {
        if (IsWinningPosition(player))
        {
            return 200;
        }
        if (IsWinningPosition(Opponent(player)))
        {
            return -200;
        }
        if (IsBoardFull())
        {
            return 0;
        }
        int evaluationSum = 0;
        for (byte row = 0; row < rows - 3; row++)   //-3 para que no se salga del array al hacer la comparacion
        {
            for (byte column = 0; column < columns; column++)
            {
                if(spaces[row, column] == activePlayer)
                {
                    evaluationSum += evaluationMatrix[row, column];
                }else if(spaces[row, column] == Opponent(activePlayer))
                {
                    evaluationSum -= evaluationMatrix[row, column];
                }
            }
        }
        return evaluationSum;
    }

    public int[] PossibleMoves()
    {
        int[] moves;
        int count = 0;

        for (byte column = 0; column < columns; column++)
        {
            if (IsEmptySpace(0, column)) count++;
        }
                
        moves = new int[count];

        count = 0;
        for (byte column = 0; column < columns; column++)
        {
            if (IsEmptySpace(0, column))
            {
                moves[count] = column;
                count++;
            }
        }
        return moves;
    }

    string Opponent (string player)
    {
        if (player == "X")
        {
            return "O";
        }
        else
        {
            return "X";
        }
    }

    public bool IsEndOfGame()
    {
        if (IsWinningPosition("X"))
        {
            return true;
        }
        else if (IsWinningPosition("O"))
        {
            return true;
        }
        else if (IsBoardFull())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsWinningPosition(string player)
    {
        if (IsVerticalWinning(player)) return true;
        if (IsHorizontalWinning(player)) return true;
        if (IsAscendentWinning(player)) return true;
        if (IsDescendentWinning(player)) return true;
        return false;
    }

    protected bool IsVerticalWinning(string player)
    {
        for (byte row = 0; row < rows - 3; row++)   //-3 para que no se salga del array al hacer la comparacion
        {
            for (byte column = 0; column < columns; column++)
            {
                if((spaces[row, column] == player)  &&
                   (spaces[row + 1, column] == player) &&
                   (spaces[row + 2, column] == player) &&
                   (spaces[row + 3, column] == player))
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected bool IsHorizontalWinning(string player)
    {
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns - 3; column++)
            {
                if ((spaces[row, column] == player) &&
                    (spaces[row, column + 1] == player) &&
                    (spaces[row, column + 2] == player) &&
                    (spaces[row, column + 3] == player))
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected bool IsAscendentWinning(string player)
    {
        for (byte row = 3; row < rows; row++)
        {
            for (byte column = 0; column < columns - 3; column++)
            {
                if ((spaces[row, column] == player) &&
                    (spaces[row - 1, column + 1] == player) &&
                    (spaces[row - 2, column + 2] == player) &&
                    (spaces[row - 3, column + 3] == player))
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected bool IsDescendentWinning(string player)
    {
        for (byte row = 0; row < rows - 3; row++)
        {
            for (byte column = 0; column < columns - 3; column++)
            {
                if ((spaces[row, column] == player) &&
                    (spaces[row + 1, column + 1] == player) &&
                    (spaces[row + 2, column + 2] == player) &&
                    (spaces[row + 3, column + 3] == player))
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected bool IsBoardFull()
    {
        for (byte column = 0; column < columns; column++)
        {
            if (IsEmptySpace(0, column)) return false;
        }
        return true;
    }
    public Board GenerateNewBoardFromMove(int move)
    {
        Board newBoard = this.DuplicateBoard();
        newBoard.Move(move, activePlayer);
        newBoard.activePlayer = Opponent(newBoard.activePlayer);
        return newBoard;
    }

    public Board DuplicateBoard ()
    {
        Board newBoard = new Board(rows, columns);

        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                newBoard.spaces[row, column] = this.spaces[row, column];
            }
        }
        newBoard.activePlayer = this.activePlayer;
        return newBoard;
    }

    public bool IsEmptySpace(int row, int column)
    {
        if (spaces[row, column] == "") return true;
        else return false;
    }

    public void Move(int column, string player)
    {
        for (int row = rows - 1; row >= 0; row--)
        {
            if (IsEmptySpace(row, column))
            {
                spaces[row, column] = player;
                break;
            }
        }
    }
}
