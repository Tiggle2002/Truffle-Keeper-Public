using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ICategorised
{
    void SetCategory(int category);
}


public enum CategoryType { Structure, Item, Monument }
public class CategoryButton : SelectableButton
{
    #region Fields
    [FoldoutGroup("Category Settings"), SerializeField]
    private CategoryType categoryType;

    [FoldoutGroup("Category Settings"), SerializeField, ShowIf("categoryType", CategoryType.Structure)]
    private StructureCategory structureCategory;

    [FoldoutGroup("Category Settings"), SerializeField, ShowIf("categoryType", CategoryType.Item)]
    private ItemType itemCategory;
    #endregion

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        SelectPage();
    }

    public void SelectPage()
    {
        ICategorised categorisedInterface = correspondingInterface.GetComponent<ICategorised>();
        categorisedInterface.SetCategory(GetPageNumber());
    }

    private int GetPageNumber()
    {
        if (categoryType == CategoryType.Structure)
        {
            return (int)structureCategory;
        }
        if (categoryType == CategoryType.Item)
        {
            return (int)itemCategory;
        }

        return 0;
    }
}