using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class HangmanController : MonoBehaviour
{
    [SerializeField] GameObject wordContainer;
    [SerializeField] GameObject keyboardContainer;
    [SerializeField] GameObject letterContainer;
    [SerializeField] GameObject[] hangmanStages;
    [SerializeField] GameObject letterButton;

    private string word;
    private int incorrectGuesses;
    private int correctGuesses;

    private void Start()
    {
        InitializeButtons();
        StartCoroutine(FetchWordFromAPI());
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

    private IEnumerator FetchWordFromAPI()
    {
        string apiUrl = "https://random-word-api.herokuapp.com/word?number=1";

        using (UnityWebRequest request = CreateWebRequest(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                word = jsonResponse.Trim(new char[] { '[', ']', '"' }).ToUpper(); // JSON yanıtını temizle ve kelimeyi büyük harfe çevir
                InitializeGame();
            }
            else
            {
                Debug.LogError("Error fetching word: " + request.error);
            }
        }
    }





    private UnityWebRequest CreateWebRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new BypassCertificate(); // Sertifika kontrolünü atla
        return request;
    }

    private class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true; // Sertifika kontrolünü her zaman geçerli kıl
        }
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
            Invoke("StartNewGame", 3f); // 3 saniye sonra yeni oyuna başla
        }
        
        if(incorrectGuesses == hangmanStages.Length) // Lose condition
        {
            for(int i = 0; i < word.Length; i++)
            {
                wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].color = Color.red;
                wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].text = word[i].ToString();
            }
            Invoke("StartNewGame", 3f); // 3 saniye sonra yeni oyuna başla
        }
    }

    private void StartNewGame()
    {
        StartCoroutine(FetchWordFromAPI()); // Yeni kelimeyi çek
    }

}
