using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SurvivalElements
{
    public class PlayerManager : SingletonMonoBehaviour<PlayerManager>, TEventListener<ItemEvent>
    {
        #region Input Actions
        

        public InputAction useAction { get; private set; }
        public InputAction interactAction { get; private set; }
        #endregion

        #region Variables
        public ItemType currentItemType;

        private Vector3 currentMousePos;
        private Vector3 currentTrajectory;
        private int UILayer;
        #endregion

        #region References
        public PlayerInput PlayerInput;
        public Inventory inventory;
        private Camera playerCamera;
        private BoxCollider2D bc;
        #endregion

        public void Start()
        {
            playerCamera = Camera.main;
            PlayerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
            bc = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();
            useAction = PlayerInput.actions["Fire"];
            interactAction = PlayerInput.actions["Interact"];
            inventory = GetComponentInChildren<Inventory>();
            UILayer = LayerMask.NameToLayer("UI");
        }


        public void Update()
        {
            if (useAction != null && useAction.WasReleasedThisFrame() && !MouseOverUI())
            {
                ItemEvent.Trigger(ItemEventType.UseInputHalted);
            }

            if (useAction != null && useAction.IsPressed() && !MouseOverUI())
            {
                ItemEvent.Trigger(ItemEventType.ItemUseRequestHold);
            }

            if (useAction != null && useAction.WasPerformedThisFrame() && !MouseOverUI())
            {
                ItemEvent.Trigger(ItemEventType.ItemUseRequest);
            }

            if (useAction != null && useAction.WasPressedThisFrame() && !MouseOverUI())
            {
                ItemEvent.Trigger(ItemEventType.PlayerClick);
            }

            if (interactAction != null && interactAction.WasPressedThisFrame())
            {
                PlayerEvent.Trigger(PlayerEventType.PlayerInteracted);
            }
        }

        public Vector3 GetMousePosition()
        {
            if (GameManager.Instance.paused || MouseOverUI())
            {
                return currentMousePos;
            }
            currentMousePos = playerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            return currentMousePos;
        }

        public Vector2 GetProjectileTrajectory()
        {
            if (GameManager.Instance.paused)
            {
                return currentTrajectory;
            }

            currentTrajectory = (GetMousePosition() - bc.bounds.center).normalized;
            return currentTrajectory;
        }

        public Ray Ray()
        {
            return playerCamera.ScreenPointToRay(GetMousePosition());
        }

        public void OnEvent(ItemEvent information)
        {
            switch (information.eventType)
            {
                case ItemEventType.ItemSelected:
 
                    break;
            }
        }

        public void OnEnable()
        {
            this.Subscribe<ItemEvent>();
        }

        public void OnDisable()
        {
            this.Unsubscribe<ItemEvent>();
        }

        private bool MouseOverUI()
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults());
        }

        private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == UILayer && curRaysastResult.gameObject.activeInHierarchy)
                    return true;
            }
            return false;
        }

        private static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
    }
}