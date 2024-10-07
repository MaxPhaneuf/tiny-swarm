using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Clicker : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent<Vector3> onClick = new();
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Camera.main == null || eventData.button != PointerEventData.InputButton.Left) return;
        var position = Camera.main.ScreenToWorldPoint(eventData.position);
        var world = new Vector3(position.x, position.y, 0);
        onClick.Invoke(world);
    }
}