using Cinemachine;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using SurvivalElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialPlayerSpawner : MonoBehaviour
{
    [Required, SerializeField] private Transform playerSpawn;
    [Required, SerializeField] private GameObject playerPrefab;
    [Required, SerializeField] private MMF_Player spawnFeedback;

    [SerializeField] protected Sprite imageToDisplay;
    [Required, SerializeField] private bool cameraFollowPlayer = true;
    [Required, SerializeField] private int playerHealthThisLevel;
    [SerializeField] private List<ItemSlotData> itemsToGive;

    private CinemachineVirtualCamera playerCam;

    public void Awake()
    {
        if (cameraFollowPlayer)
        {
            playerCam = FindObjectOfType<CinemachineVirtualCamera>();
        }
        StartCoroutine(SpawnPlayer());
    }

    private IEnumerator SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, playerSpawn) as GameObject;

        InitialisePlayer(player);

        yield return new WaitForSeconds(1f);
        Vector3 tutorialTextPos = new(playerSpawn.position.x, playerSpawn.position.y + 10f);
        StartCoroutine(tutorialTextPos.PresentTextMesh("To Move", 3.5f, imageToDisplay));
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
