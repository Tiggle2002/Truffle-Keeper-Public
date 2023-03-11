using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseItemInterface : BaseInterface
{
    [FoldoutGroup("References"), SerializeField]
    protected ItemSlot[] itemSlots;

    public ItemSlot GetSlot(int index)
    {
        if (index > itemSlots.Length || index < 0)
        {
            return null;
        }
        return itemSlots[index];
    }
}



