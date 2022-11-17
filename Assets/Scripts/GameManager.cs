using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Sprite buttonSprite;

    public Sprite[] imagesSprites;

    public List<Sprite> piecesImages = new List<Sprite>();

    public List<Button> buttons = new List<Button>();

    public TextMeshProUGUI attempts;
    public TextMeshProUGUI timerText;

    public GameObject gameOverPanel;
    public Image gameOverImage;

    private bool firstGuess, secondGuess;
    public bool isGameOver;

    private bool victory;
    private bool isTimerOn = true;

    private int guessCounter;
    private int correctGuessCounter;
    private int totalGuesses;
    private int firstGuessIndex, secondGuessIndex;

    [SerializeField] private float timeRemaining = 60.0f;

    private string firstGuessName, secondGuessName; 

    private void Start()
    {
        ButtonsList();
        AddFunction();
        AddPieces();
        RandomizePiecesOrder(piecesImages);

        totalGuesses = piecesImages.Count / 2;

        isGameOver = false;
    }
    
    private void Update()
    {
        if (isTimerOn)
        {
            timeRemaining -= Time.deltaTime;
        }

        if (timeRemaining <= 0.0f)
        {
            timeRemaining = 0;
            victory = false;
            isGameOver = true;
            GameOver();
        }

        Timer();

        attempts.text = "Attempts : " + guessCounter;
    }

    private void Timer()
    {
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);

        timerText.text = $"{minutes} : {seconds}";
    }

    public void ResetGame()
    {
        foreach (Button button in buttons)
        {
            button.image.sprite = buttonSprite;
            button.image.color = new Color(1, 1, 1, 1);
            button.interactable = true;
        }
        RandomizePiecesOrder(piecesImages);
        guessCounter = 0;
        correctGuessCounter = 0;
    }

    void ButtonsList()
    {
        GameObject[] instances = GameObject.FindGameObjectsWithTag("Piece");

        for (int i = 0; i < instances.Length; i++)
        {
            buttons.Add(instances[i].GetComponent<Button>());
            buttons[i].image.sprite = buttonSprite;
        }
    }

    void AddPieces()
    {
        int looper = buttons.Count;
        int index = 0;

        for(int i = 0; i < looper; i++)
        {
            if(index == looper / 2)
            {
                index = 0;
            }
            piecesImages.Add(imagesSprites[index]);

            index++;
        }
    }

    void AddFunction()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => ButtonSelected());
        }
    }

    public void ButtonSelected()
    {
        if (!firstGuess)
        {
            firstGuess = true;
            firstGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            firstGuessName = piecesImages[firstGuessIndex].name;

            buttons[firstGuessIndex].image.sprite = piecesImages[firstGuessIndex];
            buttons[firstGuessIndex].interactable = false;
        }
        else if (!secondGuess)
        {
            secondGuess = true;
            secondGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
            secondGuessName = piecesImages[secondGuessIndex].name;

            buttons[secondGuessIndex].image.sprite = piecesImages[secondGuessIndex];
            buttons[firstGuessIndex].interactable = true;

            guessCounter++;

            StartCoroutine(CheckIfNamesMatch());
        }
    }

    IEnumerator CheckIfNamesMatch()
    {
        yield return new WaitForSeconds(0.8f);

        if(firstGuessName == secondGuessName)
        {
            yield return new WaitForSeconds(0.2f);

            buttons[firstGuessIndex].interactable = false;
            buttons[secondGuessIndex].interactable = false;

            buttons[firstGuessIndex].image.color = new Color(0, 0, 0, 0);
            buttons[secondGuessIndex].image.color = new Color(0, 0, 0, 0);

            IsGameOver();
        }
        else
        {
            yield return new WaitForSeconds(0.2f);

            buttons[firstGuessIndex].image.sprite = buttonSprite;
            buttons[secondGuessIndex].image.sprite = buttonSprite;
        }
        firstGuess = false;
        secondGuess = false;
    }

    void RandomizePiecesOrder(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite current = list[i];

            int randomIndex = Random.Range(i, list.Count);

            list[i] = list[randomIndex];
            list[randomIndex] = current;
        }
    }



    void IsGameOver()
    {
        correctGuessCounter++;

        if(correctGuessCounter == totalGuesses)
        {
            print("gameover");
            isGameOver = true;
            victory = true;
            GameOver();
        }
    }

    void GameOver()
    {
        if (isGameOver)
        {
            gameOverPanel.SetActive(true);
            isTimerOn = false;

            if (victory)
            {
                gameOverImage.sprite = Resources.Load<Sprite>("Sprites/victory");
            }
            else
            {
                gameOverImage.sprite = Resources.Load<Sprite>("Sprites/defeat");
            }
        }
    }
}
