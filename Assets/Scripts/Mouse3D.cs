using UnityEngine;

public class Mouse3D : MonoBehaviour
{
    public static Mouse3D instance;
    public LayerMask layer;

    void Awake()
    {
        instance = this;
    }
    public bool GetClickPos(Vector3 clickPos, out Vector3 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(clickPos);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 5000, layer))
        {
            pos = hitInfo.point;
            return true;
        }
        pos = Vector3.zero;
        return false;
    }
}
