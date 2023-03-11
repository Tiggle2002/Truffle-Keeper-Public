using UnityEngine;

public static class Interactable
{
    private static GameObject interactObject;

    public static GameObject InteractObject()
    {
        if (interactObject == null)
        {
            InstantiateInteractable();
        }
        return interactObject;
    }

    public static void InstantiateInteractable()
    {
        GameObject interactPrefab = Resources.Load<GameObject>("Prefabs/UI/Interact Pop-Up");
        interactObject = GameObject.Instantiate(interactPrefab);
    }

    public static void SetInteractable(this Transform transform)
    {
        InteractObject().gameObject.SetActive(true);
        SetInteractablePosition(transform.position);
    }

    private static void SetInteractablePosition(Vector3 position)
    {
        InteractObject().transform.position = position;
        InteractObject().transform.position += (Vector3.up * 2.5f);
    }

    public static void RemoveInteractable(this Transform transform)
    {
        InteractObject().gameObject.SetActive(false);
    }
}