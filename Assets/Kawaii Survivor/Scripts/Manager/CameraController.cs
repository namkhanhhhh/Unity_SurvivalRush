using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Elements")]
    [SerializeField]
    private Transform target;

    [Header("Settings")]
    [SerializeField]
    private Vector2 minMaxXY;
    private SpriteRenderer spriteRenderer;
    private void LateUpdate()
    {
        if (target == null) 
        {
            Debug.LogWarning("No target found");
            return;
        }
        Vector3 targetPosition = target.position;
        targetPosition.z = -10;

        targetPosition.x = Mathf.Clamp(targetPosition.x, -minMaxXY.x, minMaxXY.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -minMaxXY.y, minMaxXY.y);
        transform.position = targetPosition;
        ;
    }
}
