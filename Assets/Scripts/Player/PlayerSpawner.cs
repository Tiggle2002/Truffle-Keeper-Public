using Cinemachine;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Required, SerializeField, FoldoutGroup("Spawn References")] private GameObject playerPrefab;
    [Required, SerializeField] private Transform playerSpawn;
    [Required, SerializeField] private MMF_Player spawnFeedback;
    [Required, SerializeField] private bool cameraFollowPlayer = true;
    [Required, SerializeField] private int playerHealthThisLevel;
    [SerializeField] private bool spawnWithItems;
    [SerializeField] private List<ItemSlotData> itemsToGive;

    private CinemachineVirtualCamera playerCam;

    public void Awake()
    {
        if (cameraFollowPlayer)
        {
            playerCam = FindObjectOfType<CinemachineVirtualCamera>();
        }
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, playerSpawn) as GameObject;

        InitialisePlayer(player);
    }

    private void InitialisePlayer(GameObject player)
    {
        CharacterHealth playerHealth = player.GetComponent<CharacterHealth>();
        playerHealth.entityObject.SetMaxHealth(playerHealthThisLevel, false);
        playerHealth.Respawn();
        spawnFeedback?.PlayFeedbacks();
        if (cameraFollowPlayer)
        {
            playerCam.GetComponent<CinemachineConfiner2D>().enabled = true;
            player.transform.position = playerSpawn.position;
            playerCam.Follow = player.transform;
        }
        if (spawnWithItems)
        {
            GivePlayerItems();
        }
    }

    [Button("Give Items")]
    private void GivePlayerItems()
    {
        for (int i = 0; i < itemsToGive.Count; i++)
        {
            InventoryEvent.Trigger(InventoryEventType.ItemAdded, new(itemsToGive[i].item, itemsToGive[i].quantity));
        }
    }
}
