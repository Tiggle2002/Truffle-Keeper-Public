using System.Collections;
using UnityEngine;

public static class Utilities
{
    public static int LoopAroundArray(int currentIndex, int increment, int max)
    {
        int newIndex = currentIndex + increment;

        if (newIndex < 0)
        {
            newIndex = max - Mathf.Abs(increment);
        }
        else if (newIndex > max - 1)
        {
            newIndex = 0;
        }

        return newIndex;
    }

    public static int LoopAround(int currentIndex, int increment, int max, int min)
    {
        int newIndex = currentIndex + increment;

        if (newIndex < min)
        {
            newIndex = max - Mathf.Abs(increment);
        }
        else if (newIndex > max)
        {
            newIndex = min;
        }

        return newIndex;
    }

    public static bool LayerInLayerMask(int layer, LayerMask layerMask)
    {
        if (((1 << layer) & layerMask) != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void Unparent(this GameObject objectToOrphan)
    {
        objectToOrphan.transform.SetParent(null);
    }

    public static void RotateAroundOrigin(this Transform transform)
    {
        if (transform.position.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    public static float RandomInRange(this Vector2 vector2)
    {
        return Random.Range(vector2.x, vector2.y);
    }

    public static int RandomInRange(this Vector2Int vector2)
    {
        return Random.Range(vector2.x, vector2.y + 1);
    }

    public static IEnumerator DespawnCoroutine(this GameObject obj, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        obj.SetActive(false);
    }
}
