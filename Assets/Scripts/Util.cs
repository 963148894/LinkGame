using UnityEngine;

public class Util : MonoBehaviour
{

    public static void SafeDestroy(Transform obj)
    {
        if (obj != null)
        {
            Destroy(obj.gameObject);
        }
    }

    public static void SafeDestroy(GameObject obj)
    {
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    public static void SafeDestroy(ObjectShape obj)
    {
        if (obj != null)
        {
            Destroy(obj.gameObject);
        }
    }
}
