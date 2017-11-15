using System;
using UnityEngine;

public class ZobristKeys
{

    protected int[,] keys; // 31 bits
    protected int boardPositions, numberOfPieces;

    public ZobristKeys(int _boardPositions, int _numberOfPieces)
    {
        System.Random rnd = new System.Random();
        boardPositions = _boardPositions;
        numberOfPieces = _numberOfPieces;

        keys = new int[boardPositions, numberOfPieces];

        for (int i = 0; i < boardPositions; i++)
        {
            for (int j = 0; j < numberOfPieces; j++)
            {
                keys[i, j] = rnd.Next(int.MaxValue);
            }
        }
    }

    public int GetKey(int position, int piece)
    {
        return keys[position, piece];
    }

    public void Print()
    {
        string output = "";
        output += "Claves Zobrist:\n";
        for (int i = 0; i < boardPositions; i++)
        {
            for (int j = 0; j < numberOfPieces; j++)
            {
                output += "Posición " + Convert.ToString(i).PadLeft(2, '0') + ", Pieza " + j + ": ";
                output += Convert.ToString(keys[i, j], 2).PadLeft(32, '0');
                output += "\n";
            }
        }
        Debug.Log(output);
    }
}
