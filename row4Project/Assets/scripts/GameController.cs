using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Player
{
    public Image panel;
    public Text text;
    public Button button;
}

[System.Serializable]
public class PlayerColor
{
    public Color panelColor;
    public Color textColor;
}

public class GameController : MonoBehaviour
{


    public GameObject gameOverPanel;
    public Text gameOverText;
    public GameObject restartButton;
    public GameObject startInfo;

    public Player playerX;
    public Player playerO;
    public PlayerColor activePlayerColor;
    public PlayerColor inactivePlayerColor;

    public AI ai;

    private string activePlayer;

    public Text[,] buttonList;
    public GameObject tablero4;
    public GameObject prefabSpace;
    public byte rows, columns;


    //void SetGameControllerReferenceOnButtons()
    //{
    //    byte numberOfButtons = (byte)buttonList.Length;
    //    for (byte i = 0; i < numberOfButtons; i++)
    //    {
    //        Text buttonText = buttonList[i];
    //        Space space = buttonText.GetComponentInParent<Space>();
    //        space.SetGameControllerReference(this);
    //    }
    //}


    void Awake()
    {
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        //activePlayer = "X";
        //SetPlayerColors(playerX, playerO);
        // SetGameControllerReferenceOnButtons();

        buttonList = new Text[rows, columns];

        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                GameObject gameObject = (GameObject)Instantiate(prefabSpace, new Vector3(0, 0, 0), Quaternion.identity);
                gameObject.transform.SetParent(tablero4.transform);
                buttonList[row, column] = (Text)gameObject.GetComponentInChildren(typeof(Text));
                Space space = buttonList[row, column].GetComponentInParent<Space>();
                space.SetGameControllerReference(this);
                space.SetRowColumn(row, column);
            }
        }

        ai.SetButtonList(buttonList);
        ai.SetGameController(this);
    }

    public void FillColumn(byte column)
    {
        for (int row = rows - 1; row >= 0; row--)
        {
            Text buttonText = buttonList[row, column];
            if (buttonText.text == "")
            {
                Button button = buttonText.GetComponentInParent<Button>();
                buttonText.text = GetActivePlayer();
                button.interactable = false;
                break;
            }
        }
    }

    public string GetActivePlayer()
    {
        return activePlayer;
    }

    public void EndTurn()
    {
        if (IsWinState())
        {
            gameOverText.text = "¡Gana " + activePlayer + "!";
            GameOver();
        }
        else if (IsBoardFull())
        {
            gameOverText.text = "¡Empate!";
            SetPlayerColorsInactive();
            GameOver();
        }
        else
        {
            ChangeSides();
            if (activePlayer == "O")
            {
                ai.Play("O");
            }
            /*
            else if (activePlayer == "X")
            {
                ai.Play("X");
            }
            */
        }
    }

    bool IsBoardFull()
    {
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                Text buttonText = buttonList[row, column];
                if (buttonText.text == "") return false;
            }
        }
        return true;
    }

    bool IsWinState()
    {
        Board checkBoard = new Board(rows, columns);
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                Text spaceText = buttonList[row, column];
                checkBoard.spaces[row, column] = spaceText.text;
            }
        }
        checkBoard.activePlayer = this.activePlayer;
        if (checkBoard.IsWinningPosition(activePlayer)) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void ChangeSides()
    {
        if (activePlayer == "X")
        {
            activePlayer = "O";
            SetPlayerColors(playerO, playerX);
        }
        else
        {
            activePlayer = "X";
            SetPlayerColors(playerX, playerO);
        }
    }

    void GameOver()
    {
        DeactivateSpaces();
        gameOverPanel.SetActive(true);
        restartButton.SetActive(true);
    }

    void DeactivateSpaces()
    {
        SetSpacesInteractable(false);
    }

    void RestartSpaces()
    {
        EmptySpaces();
        SetSpacesInteractable(true);
    }

    void EmptySpaces()
    {
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                Text buttonText = buttonList[row, column];
                buttonText.text = "";
            }
        }
    }

    void SetSpacesInteractable(bool toggle)
    {
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                Text buttonText = buttonList[row, column];
                Button button = buttonText.GetComponentInParent<Button>();
                button.interactable = toggle;
            }
        }
    }

    void SetPlayerColors(Player newPlayer, Player oldPlayer)
    {
        newPlayer.panel.color = activePlayerColor.panelColor;
        newPlayer.text.color = activePlayerColor.textColor;
        oldPlayer.panel.color = inactivePlayerColor.panelColor;
        oldPlayer.text.color = inactivePlayerColor.textColor;
    }

    public void SetStartingSide(string startingSide)
    {
        activePlayer = startingSide;
        if (activePlayer == "X")
        {
            SetPlayerColors(playerX, playerO);
        }
        else
        {
            SetPlayerColors(playerO, playerX);
        }
        StartGame();
    }

    void SetPlayerColorsInactive()
    {
        playerX.panel.color = inactivePlayerColor.panelColor;
        playerX.text.color = inactivePlayerColor.textColor;
        playerO.panel.color = inactivePlayerColor.panelColor;
        playerO.text.color = inactivePlayerColor.textColor;
    }

    public void RestartGame()
    {
        gameOverPanel.SetActive(false);
        //playerSide = "X";
        //SetPlayerColors(playerX, playerO);
        restartButton.SetActive(false);
        SetPlayerButtons(true);
        SetPlayerColorsInactive();
        EmptySpaces();
        startInfo.SetActive(true);
    }

    void StartGame()
    {
        RestartSpaces();
        SetPlayerButtons(false);
        startInfo.SetActive(false);
        if (activePlayer == "O")
        {
            ai.Play("O");
        }
    }

    void SetPlayerButtons(bool toggle)
    {
        playerX.button.interactable = toggle;
        playerO.button.interactable = toggle;
    }

}
