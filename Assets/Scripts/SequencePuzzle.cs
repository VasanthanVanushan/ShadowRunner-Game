using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SequencePuzzle : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject puzzlePanel;
    [SerializeField] private Image[] sequenceLights;
    [SerializeField] private TMP_Text resultText;

    [Header("Buttons")]
    [SerializeField] private Button redButton;
    [SerializeField] private Button greenButton;
    [SerializeField] private Button yellowButton;

    [Header("Settings")]
    [SerializeField] private int sequenceLength = 3;
    [SerializeField] private float displayTime = 0.6f;

    private int[] sequence;
    private int playerIndex;

    private System.Action<bool> puzzleResult;

    private Color red = Color.red;
    private Color green = Color.green;
    private Color yellow = Color.yellow;

    private void Awake()
    {
        redButton.onClick.AddListener(() => SelectColor(0));
        greenButton.onClick.AddListener(() => SelectColor(1));
        yellowButton.onClick.AddListener(() => SelectColor(2));

        //puzzlePanel.SetActive(false);
    }

    public void StartPuzzle(System.Action<bool> resultCallback)
    {
        puzzleResult = resultCallback;

        puzzlePanel.SetActive(true);

        playerIndex = 0;
        resultText.text = "";

        GenerateSequence();

        StartCoroutine(ShowSequence());
    }

    private void GenerateSequence()
    {
        sequence = new int[sequenceLength];

        for (int i = 0; i < sequence.Length; i++)
        {
            sequence[i] = Random.Range(0, 3);
        }
    }

    private IEnumerator ShowSequence()
    {
        SetButtons(false);

        for (int i = 0; i < sequence.Length; i++)
        {
            sequenceLights[i].gameObject.SetActive(true);

            sequenceLights[i].color = GetColor(sequence[i]);

            yield return new WaitForSecondsRealtime(displayTime);

            sequenceLights[i].gameObject.SetActive(false);

            yield return new WaitForSecondsRealtime(0.2f);
        }

        SetButtons(true);
    }

    private void SelectColor(int selectedColor)
    {
        if (selectedColor != sequence[playerIndex])
        {
            PuzzleFailed();
            return;
        }

        playerIndex++;

        if (playerIndex >= sequence.Length)
        {
            PuzzleSolved();
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
        yield return new WaitForSecondsRealtime(0.8f);

        puzzlePanel.SetActive(false);

        puzzleResult?.Invoke(success);
    }

    private Color GetColor(int index)
    {
        if (index == 0)
            return red;

        if (index == 1)
            return green;

        return yellow;
    }

    private void SetButtons(bool value)
    {
        redButton.interactable = value;
        greenButton.interactable = value;
        yellowButton.interactable = value;
    }
}