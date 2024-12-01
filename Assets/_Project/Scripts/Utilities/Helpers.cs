using UnityEngine;

public class Helpers
{
    private static Camera cam = Camera.main;
    public static Vector3 GetMouseWorldPosition(Vector3 position)
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = cam.WorldToScreenPoint(position).z;
        return cam.ScreenToWorldPoint(mouseScreenPosition);
    }
}