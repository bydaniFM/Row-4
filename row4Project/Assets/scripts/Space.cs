using UnityEngine;
using UnityEngine.UI;

public class Space : MonoBehaviour {
    public Button button;
    public Text buttonText;
    // public string playerSide;

    public byte row, column;

    private GameController gameController;

    public void SetGameControllerReference (GameController gc)
    {
        gameController = gc;
    }

    public void SetRowColumn(byte _row, byte _column)
    {
        row = _row;
        column = _column;
    }

    public void SetSpace() {
        //buttonText.text = gameController.GetActivePlayer();
        //button.interactable = false;
        gameController.FillColumn(column);
        gameController.EndTurn();
    }
}
