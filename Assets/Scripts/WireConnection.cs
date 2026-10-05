using UnityEngine;

public class WireConnection : MonoBehaviour
{
    private RectTransform wireRect;

    private void Awake()
    {
        wireRect = GetComponent<RectTransform>();
    }

    public void SetWire(Vector2 start,Vector2 end)
    {
        Vector2 direction = end - start;

        float distance = direction.magnitude;

        float angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;

        wireRect.anchoredPosition = Vector2.Lerp(start,end,0.5f);

        wireRect.sizeDelta = new Vector2(distance,12f);

        wireRect.localRotation = Quaternion.Euler(0f,0f,angle);
    }
}