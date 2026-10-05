using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class WirePuzzleManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject puzzlePanel;
    [SerializeField] private TMP_Text resultText;

    [Header("Wire")]
    [SerializeField] private GameObject wirePrefab;
    [SerializeField] private RectTransform wireArea;

    [Header("Points")]
    [SerializeField] private WirePoint[] leftPoints;
    [SerializeField] private WirePoint[] rightPoints;

    [Header("Attempts")]
    [SerializeField] private int maxAttempts = 2;

    private int wrongAttempts;

    private WirePoint currentPoint;
    private GameObject currentWire;

    private List<WirePoint> connectedPoints =
        new List<WirePoint>();

    private bool isConnecting;

    private Camera eventCamera;

    private System.Action<bool> onPuzzleFinished;

    private void Awake()
    {
        puzzlePanel.SetActive(false);

        eventCamera = null;
    }

    // --------------------------------------------------
    // START PUZZLE
    // --------------------------------------------------

    public void StartPuzzle(
        System.Action<bool> callback)
    {
        onPuzzleFinished = callback;

        puzzlePanel.SetActive(true);

        ResetPuzzle();

        ShuffleRightPoints();

        wrongAttempts = 0;

        resultText.text = "";
    }

    // --------------------------------------------------
    // SHUFFLE RIGHT POINTS
    // --------------------------------------------------

    private void ShuffleRightPoints()
    {
        if (rightPoints == null || rightPoints.Length < 2)
        {
            return;
        }

        Vector3[] positions = new Vector3[rightPoints.Length];

        for (int i = 0; i < rightPoints.Length; i++)
        {
            RectTransform rect =rightPoints[i].GetComponent<RectTransform>();

            positions[i] = rect.position;
        }

        for (int i = 0; i < positions.Length; i++)
        {
            int randomIndex = Random.Range(i,positions.Length);

            Vector3 temp =positions[i];

            positions[i] =positions[randomIndex];

            positions[randomIndex] =temp;
        }

        bool sameArrangement = true;

        for (int i = 0;i < positions.Length;i++)
        {
            RectTransform rect =rightPoints[i].GetComponent<RectTransform>();

            if (rect.position != positions[i])
            {
                sameArrangement = false;
                break;
            }
        }

        if (sameArrangement)
        {
            Vector3 temp =positions[0];

            positions[0] =positions[1];

            positions[1] =temp;
        }

        for (int i = 0;i < rightPoints.Length;i++)
        {
            RectTransform rect =rightPoints[i].GetComponent<RectTransform>();

            rect.position =positions[i];
        }
    }

    // --------------------------------------------------
    // START CONNECTION
    // --------------------------------------------------

    public void StartConnection(WirePoint point,PointerEventData eventData)
    {
        if (isConnecting)
            return;

        if (connectedPoints.Contains(point))
            return;

        currentPoint = point;

        isConnecting = true;

        currentWire = Instantiate(wirePrefab, wireArea);

        currentWire.SetActive(true);

        UpdateWire(eventData);
    }

    // --------------------------------------------------
    // UPDATE CONNECTION
    // --------------------------------------------------

    public void UpdateConnection(PointerEventData eventData)
    {
        if (!isConnecting)
            return;

        if (currentWire == null)
            return;

        UpdateWire(eventData);
    }

    private void UpdateWire(PointerEventData eventData)
    {
        if (currentPoint == null)
            return;

        RectTransform startRect =currentPoint.GetComponent<RectTransform>();

        Vector2 startScreenPosition =RectTransformUtility.WorldToScreenPoint(eventCamera,startRect.position);

        Vector2 startPosition;
        Vector2 mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(wireArea,startScreenPosition,eventCamera,out startPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(wireArea,eventData.position,eventCamera,out mousePosition);

        WireConnection wire =currentWire.GetComponent<WireConnection>();

        wire.SetWire(startPosition,mousePosition);
    }

    // --------------------------------------------------
    // END CONNECTION
    // --------------------------------------------------

    public void EndConnection(
        PointerEventData eventData)
    {
        if (!isConnecting)
            return;

        WirePoint targetPoint =
            GetPointUnderPointer(eventData);

        // ----------------------------------------------
        // CORRECT CONNECTION
        // ----------------------------------------------

        if (targetPoint != null &&targetPoint != currentPoint &&targetPoint.GetWireColor() ==currentPoint.GetWireColor())
        {
            ConnectCorrectly(targetPoint);

            currentPoint = null;
            currentWire = null;

            isConnecting = false;

            CheckComplete();

            return;
        }

        // ----------------------------------------------
        // WRONG CONNECTION
        // ----------------------------------------------

        if (currentWire != null)
        {
            Destroy(currentWire);
        }

        wrongAttempts++;

        currentPoint = null;
        currentWire = null;

        isConnecting = false;

        // ----------------------------------------------
        // FIRST WRONG ATTEMPT
        // ----------------------------------------------

        if (wrongAttempts < maxAttempts)
        {
            int remainingAttempts = maxAttempts - wrongAttempts;

            resultText.text =
                "Wrong Connection!\n" +
                "You have only " +
                remainingAttempts +
                " more chance!";

            return;
        }

        // ----------------------------------------------
        // SECOND WRONG ATTEMPT
        // ----------------------------------------------

        PuzzleFailed();
    }

    // --------------------------------------------------
    // FIND POINT
    // --------------------------------------------------

    private WirePoint GetPointUnderPointer(PointerEventData eventData)
    {
        GameObject hit =eventData.pointerCurrentRaycast.gameObject;

        if (hit == null)
            return null;

        WirePoint point = hit.GetComponent<WirePoint>();

        if (point == null)
        {
            point =hit.GetComponentInParent<WirePoint>();
        }

        return point;
    }

    // --------------------------------------------------
    // CORRECT CONNECTION
    // --------------------------------------------------

    private void ConnectCorrectly(
        WirePoint targetPoint)
    {
        connectedPoints.Add(currentPoint);
        connectedPoints.Add(targetPoint);

        RectTransform startRect =currentPoint.GetComponent<RectTransform>();

        RectTransform endRect =targetPoint.GetComponent<RectTransform>();

        Vector2 startScreenPosition =RectTransformUtility.WorldToScreenPoint(eventCamera,startRect.position);

        Vector2 endScreenPosition =RectTransformUtility.WorldToScreenPoint(eventCamera,endRect.position);

        Vector2 startPosition;
        Vector2 endPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(wireArea,startScreenPosition,eventCamera,out startPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(wireArea,endScreenPosition,eventCamera,out endPosition);

        WireConnection wire =currentWire.GetComponent<WireConnection>();

        wire.SetWire(startPosition,endPosition);

        resultText.text ="Correct!";
    }

    // --------------------------------------------------
    // CHECK COMPLETE
    // --------------------------------------------------

    private void CheckComplete()
    {
        int totalPoints =leftPoints.Length + rightPoints.Length;

        if (connectedPoints.Count >= totalPoints)
        {
            resultText.text ="SYSTEM UNLOCKED!";

            // Tell TrapSwitch that puzzle succeeded.
            onPuzzleFinished?.Invoke(true);

            onPuzzleFinished = null;

            // Close puzzle.
            puzzlePanel.SetActive(false);
        }
    }

    // --------------------------------------------------
    // PUZZLE FAILED
    // --------------------------------------------------

    private void PuzzleFailed()
    {
        resultText.text ="PUZZLE FAILED!";

        // Tell TrapSwitch that puzzle failed.
        onPuzzleFinished?.Invoke(false);

        onPuzzleFinished = null;

        // Close puzzle UI.
        puzzlePanel.SetActive(false);
    }

    // --------------------------------------------------
    // RESET
    // --------------------------------------------------

    public void ResetPuzzle()
    {
        connectedPoints.Clear();

        WireConnection[] wires =wireArea.GetComponentsInChildren<WireConnection>();

        foreach (WireConnection wire in wires)
        {
            Destroy(wire.gameObject);
        }

        currentPoint = null;
        currentWire = null;

        isConnecting = false;
    }
}