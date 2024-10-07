using UnityEngine;
using UnityEngine.EventSystems;

public class CameraZoom : MonoBehaviour, IDragHandler
{
    public Camera cam;
    public float zoomSpeed = 2f; 
    public float minZoom = 2f; 
    public float maxZoom = 10f; 
    public float dragSpeed = 0.5f;
    
    private void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    private void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        
        if (scrollInput != 0f)
        {
            cam.orthographicSize -= scrollInput * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        Vector3 dragInput = new Vector3(eventData.delta.x, eventData.delta.y, 0);
        
        Vector3 move = cam.ScreenToWorldPoint(dragInput) - cam.ScreenToWorldPoint(Vector3.zero);
        
        cam.transform.position -= move * dragSpeed;
    }
}