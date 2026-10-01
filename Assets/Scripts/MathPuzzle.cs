using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;


public class MathPuzzle : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject puzzlePanel;
    [SerializeField] private TMP_Text questionText;

    [Header("Answer Buttons")]
    [SerializeField] private Button answerButton1;
    [SerializeField] private Button answerButton2;
    [SerializeField] private Button answerButton3;

    [Header("Result")]
    [SerializeField] private TMP_Text resultText;

    private int correctAnswer;
    private Action<bool> puzzleResult;

    private void Awake()
    {
        answerButton1.onClick.AddListener(() => CheckAnswer(answerButton1));
        answerButton2.onClick.AddListener(() => CheckAnswer(answerButton2));
        answerButton3.onClick.AddListener(() => CheckAnswer(answerButton3));
    }

    public void StartPuzzle(Action<bool> resultCallback)
    {
        puzzleResult = resultCallback;

        puzzlePanel.SetActive(true);
        resultText.text = "";

        GenerateQuestion();
    }

    private void GenerateQuestion()
    {
        // Generate two random numbers
        int number1 = UnityEngine.Random.Range(2, 20);
        int number2 = UnityEngine.Random.Range(2, 20);

        // Randomly select addition or subtraction
        bool isAddition = UnityEngine.Random.Range(0, 2) == 0;

        if (isAddition)
        {
            // Addition
            correctAnswer = number1 + number2;

            questionText.text = number1 + " + " + number2 + " = ?";
        }
        else
        {
            // Make sure the answer is not negative
            if (number1 < number2)
            {
                int temp = number1;
                number1 = number2;
                number2 = temp;
            }

            // Subtraction
            correctAnswer = number1 - number2;

            questionText.text = number1 + " - " + number2 + " = ?";
        }

        // Randomly select which button contains the correct answer
        int correctButton = UnityEngine.Random.Range(0, 3);

        int[] answers = new int[3];

        // Put correct answer into random button
        answers[correctButton] = correctAnswer;

        // Generate two different wrong answers
        for (int i = 0; i < 3; i++)
        {
            if (i == correctButton)
                continue;

            int wrongAnswer;

            do
            {
                wrongAnswer = correctAnswer + UnityEngine.Random.Range(-5, 6);
            }
            while (wrongAnswer == correctAnswer || Array.IndexOf(answers, wrongAnswer) != -1);
            answers[i] = wrongAnswer;
        }

        // Set button text
        answerButton1.GetComponentInChildren<TMP_Text>().text =answers[0].ToString();

        answerButton2.GetComponentInChildren<TMP_Text>().text =answers[1].ToString();

        answerButton3.GetComponentInChildren<TMP_Text>().text =answers[2].ToString();

        // Enable buttons
        SetButtons(true);
    }

    private void CheckAnswer(Button selectedButton)
    {
        int selectedAnswer = int.Parse(selectedButton.GetComponentInChildren<TMP_Text>().text);

        if (selectedAnswer == correctAnswer)
        {
            PuzzleSolved();
        }
        else
        {
            PuzzleFailed();
        }
    }

    private void PuzzleSolved()
    {
        resultText.text = "TRAP DISABLED!";

        SetButtons(false);

        StartCoroutine(FinishPuzzle(true));
    }

    private void PuzzleFailed()
    {
        resultText.text = "WRONG!";

        SetButtons(false);

        StartCoroutine(FinishPuzzle(false));
    }

    private IEnumerator FinishPuzzle(bool success)
    {
        // Realtime wait works even when Time.timeScale = 0
        yield return new WaitForSecondsRealtime(0.8f);

        puzzlePanel.SetActive(false);

        puzzleResult?.Invoke(success);
    }

    private void SetButtons(bool value)
    {
        answerButton1.interactable = value;
        answerButton2.interactable = value;
        answerButton3.interactable = value;
    }
}