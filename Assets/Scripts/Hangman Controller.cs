using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HangmanController : MonoBehaviour
{
    [SerializeField] GameObject wordContainer;
    [SerializeField] GameObject keyboardContainer;
    [SerializeField] GameObject letterContainer;
    [SerializeField] GameObject[] hangmanStages;
    [SerializeField] GameObject letterButton;
    [SerializeField] TextAsset possibleWord;

    private string word;
    private int incorrectGuesses;
    private int correctGuesses;

    private void Start()
    {
        InitializeButtons();
        InitializeGame();
    }

    private void InitializeButtons()
    {
        for(int i = 65; i <= 90; i++)
        {
            CreateButton(i);
        }
    }

    private void InitializeGame()
    {
        incorrectGuesses = 0;
        correctGuesses = 0;

        foreach(Button child in keyboardContainer.GetComponentsInChildren<Button>())
        {
            child.interactable = true;
        }

        foreach(Transform child in wordContainer.GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);
        }
        foreach(GameObject stage in hangmanStages)
        {
            stage.SetActive(false);
        }

        // Generate a new word
        word = GenerateWord().ToUpper();

        foreach(char letter in word)
        {
            var temp = Instantiate(letterContainer, wordContainer.transform);
        }
    }

    private void CreateButton(int i)
    {
        GameObject temp = Instantiate(letterButton, keyboardContainer.transform);
        temp.GetComponentInChildren<TextMeshProUGUI>().text = ((char)i).ToString();
        temp.GetComponent<Button>().onClick.AddListener(() => CheckLetter(((char)i).ToString()));
    }

    private string GenerateWord()
    {
        string[] wordList = possibleWord.text.Split('\n');
        string line = wordList[Random.Range(0, wordList.Length)];
        return line.Trim();
    }

    private void CheckLetter(string inputLetter)
    {
        bool letterInWord = false;
        inputLetter = inputLetter.ToUpper(); // Ensure the input letter is uppercase

        for(int i = 0; i < word.Length; i++)
        {
            if(inputLetter == word[i].ToString().ToUpper()) // Compare in uppercase
            {
                letterInWord = true;
                correctGuesses++;
                wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].text = inputLetter;   
            }
        }
        
        if(!letterInWord)
        {
            incorrectGuesses++;
            hangmanStages[incorrectGuesses - 1].SetActive(true);
        }

        CheckOutcome();
    }


    private void CheckOutcome()
    {
        if(correctGuesses == word.Length) // Win condition
        {
            for(int i = 0; i < word.Length; i++)
            {
                wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].color = Color.green;
            }
            Invoke("InitializeGame", 3f);
        }
        
        if(incorrectGuesses == hangmanStages.Length) // Lose condition
        {
            for(int i = 0; i < word.Length; i++)
            {
                wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].color = Color.red;
                wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].text = word[i].ToString();
            }
            Invoke("InitializeGame", 3f);
        }
    }
}
