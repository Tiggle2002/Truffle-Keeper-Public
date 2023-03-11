using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class HomeBase : MonoBehaviour, TEventListener<MonumentEvent>
{
    [SerializeField] private SpriteRenderer spriteRenderer; 
    [SerializeField, TableList(NumberOfItemsPerPage = 1)] private BaseCampUpgrade[] upgradeRecipe;

    private CraftingMaterial[] currentRecipe;
    private int currentIndex = 0;

    public void Start()
    {
        ChangeHomeBase();
    }

    [Button("Upgrade Base")]
    private void ChangeHomeBase()
    {
        if (currentIndex < upgradeRecipe.Length)
        {
            SetUpgradeRecipe();
            ChangeSprite();
            InstantiateObjects();
        }

        PublishNewRecipe();

        currentIndex++;
    }

    private void InstantiateObjects()
    {
        foreach (var item in upgradeRecipe[currentIndex].objectsToSpawn)
        {
            Instantiate(item.prefab, item.position, item.prefab.transform.rotation, transform.parent);
        }
    }

    private void ChangeSprite()
    {
        spriteRenderer.sprite = upgradeRecipe[currentIndex].sprite;
    }

    private void SetUpgradeRecipe()
    {
        currentRecipe = upgradeRecipe[currentIndex].upgradeRecipe;
    }

    private void PublishNewRecipe()
    {
        MonumentEvent.Trigger(MonumentEventType.UpgradeRecipeChanged, currentRecipe, currentIndex);          
    }

    public void OnEvent(MonumentEvent eventData)
    {
        switch (eventData.eventType)
        {
            case MonumentEventType.Upgraded:
                ChangeHomeBase();
                break;
        }
    }

    public void OnEnable()
    {
        this.Subscribe();
    }

    public void OnDisable()
    {
        this.Unsubscribe();
    }
}

[System.Serializable]
public struct BaseCampUpgrade
{
    public Sprite sprite;
    public CraftingMaterial[] upgradeRecipe;
    public BaseCampObject[] objectsToSpawn;
 }

[System.Serializable]
public struct BaseCampObject
{
    public GameObject prefab;
    public Vector3 position;
}