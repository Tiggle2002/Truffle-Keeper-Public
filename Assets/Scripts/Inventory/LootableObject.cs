using System;
using UnityEngine;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using SurvivalElements;
using System.Collections;
using Random = UnityEngine.Random;

public class LootableObject : MonoBehaviour, TEventListener<PlayerEvent>
{
    #region Serialized Fields
    [FoldoutGroup("Item Fields"), SerializeField]
    private ItemObject item;
    [FoldoutGroup("Item Fields"), SerializeField]
    private int quantity;
    [FoldoutGroup("Item Fields"), SerializeField]
    private bool interactToCollect = false;
    [FoldoutGroup("Feedback"), SerializeField]
    private MMF_Player spawnFeedback;
    [FoldoutGroup("Feedback"), SerializeField]
    private MMF_Player collectionFeedback;
    #endregion

    #region Private Variables
    private Inventory inventoryForItem;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private BoxCollider2D bc;

    private bool canCollect = false;
    #endregion

    #region Update Methods
    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        if (collectionFeedback == null)
        {
            collectionFeedback = GameObject.Find("Collection Feedback").GetComponent<MMF_Player>();
        }
    }
    #endregion

    #region Unity Collision Events
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("Block"))
        //{
        //    rb.gravityScale = 0;
        //    rb.velocity = Vector2.zero;
        //}
        CheckColliderForInventory(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Block"))
        {
            rb.gravityScale = 1;
        }
        SetCollectionEnabled(false);
    }
    #endregion

    #region Item Interaction
    private IEnumerator SpawnLoot()
    {
        bc.enabled = false;
        yield return new WaitForSeconds(Random.Range(0, 0.225f));
        spawnFeedback?.PlayFeedbacks();
        yield return new WaitForSeconds(0.05f);
        bc.enabled = true;
    }

    private void SetCollectionEnabled(bool enabled)
    {
        canCollect = enabled;
        if (enabled)
        {
            transform.SetInteractable();
        }
        else
        {
            transform.RemoveInteractable();
        }
    }
    #endregion

    #region Item Collection
    public void Set(ItemObject item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;

        sr.sprite = item == null ? null : item.sprite;
    }

    private void TryCollect()
    {
        if (inventoryForItem == null)
        {
            return;
        }

        if (!interactToCollect)
        {
            CollectItem();
        }
        else
        {
            SetCollectionEnabled(true);
        }
    }

    private void CheckColliderForInventory(Collider2D collider)
    {
        if (item == null || quantity == 0 || !collider.CompareTag("Player"))
        {
            return;
        }

        inventoryForItem = collider.GetComponentInChildren<Inventory>(true);
        TryCollect();
    }

    private void CollectItem()
    {
         if (inventoryForItem.CanContainItem(item.type))
        {
            AddItemToInventory();
        }
    }

    private void AddItemToInventory()
    {
        int spaceAvailable = CalculateInventorySpace();

        if (spaceAvailable != 0)
        {
            StartCoroutine(AddQuantityOfItemToInventory(spaceAvailable));
        }
    }

    private int CalculateInventorySpace()
    {
        int spaceAvailable = inventoryForItem.AvailableSpaceForItem(item);

        return QuantityToBeAdded(spaceAvailable);
    }

    private int QuantityToBeAdded(int spaceAvailable)
    {
        if (quantity < spaceAvailable)
        {
            spaceAvailable = quantity;
            quantity = 0;
        }
        else if (quantity > spaceAvailable)
        {
            quantity -= spaceAvailable;
        }
        else
        {
            quantity = 0;
        }

        return spaceAvailable;
    }

    private IEnumerator AddQuantityOfItemToInventory(int quantityToAdd)
    {
        PlayCollectionAnimation();
        yield return collectionFeedback.PlayFeedbacksCoroutine(transform.position);

        EventItem eventItem = new EventItem(item, quantityToAdd);

        InventoryEvent.Trigger(InventoryEventType.ItemAdded, eventItem);

        DisableIfDepleted();
    }

    private void PlayCollectionAnimation()
    {
        collectionFeedback.GetFeedbackOfType<MMF_DestinationTransform>().TargetTransform = this.transform; 
        collectionFeedback.GetFeedbackOfType<MMF_DestinationTransform>().Destination = PlayerManager.Instance.transform.Find("ItemCollectionPoint").transform;
    }

    private void DisableIfDepleted()
    {
        if (quantity <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region Pool Object Methods
    public void OnEvent(PlayerEvent eventData)
    {
        if (!canCollect)
        {
            return;
        }

        switch (eventData.eventType)
        {
            case PlayerEventType.PlayerInteracted:
                if (inventoryForItem && canCollect)
                {
                    CollectItem();
                }
                break;
        }
    }

    public void OnEnable()
    {
        this.Subscribe();
        StartCoroutine(SpawnLoot());

    }

    public void OnDisable()
    {
        this.Unsubscribe();
    }
    #endregion
}
