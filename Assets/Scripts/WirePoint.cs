using UnityEngine;
using UnityEngine.EventSystems;

public class WirePoint : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [Header("Wire Settings")]
    [SerializeField] private string wireColor;

    private WirePuzzleManager puzzleManager;

    private void Start()
    {
        puzzleManager = FindFirstObjectByType<WirePuzzleManager>();
    }

    public string GetWireColor()
    {
        return wireColor;
    }

    public void OnPointerDown(
        PointerEventData eventData)
    {
        if (puzzleManager != null)
        {
            puzzleManager.StartConnection(this,eventData);
        }
    }

    public void OnDrag(
        PointerEventData eventData)
    {
        if (puzzleManager != null)
        {
            puzzleManager.UpdateConnection(eventData);
        }
    }

    public void OnPointerUp(
        PointerEventData eventData)
    {
        if (puzzleManager != null)
        {
            puzzleManager.EndConnection(eventData);
        }
    }
}