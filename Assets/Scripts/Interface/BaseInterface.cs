using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseInterface : SerializedMonoBehaviour, TEventListener<HUDEvent>
{
    #region Protected Fields
    public bool Open { get { return interfaceOpen; } }

    [FoldoutGroup("Interface Properties")]
    [SerializeField] protected GraphicRaycaster canvasInteractibility;
    [FoldoutGroup("Interface Properties")]
    [SerializeField] protected Canvas canvas;

    [FoldoutGroup("Interface Properties")]
    [SerializeField] protected float fadeLength;
    [FoldoutGroup("Interface Properties")]
    [SerializeField] protected bool startOpen;
    [FoldoutGroup("Interface Properties")]
    [SerializeField] protected bool closeWhenOtherInterfaceOpened;


    protected Coroutine fadeCoroutine;
    protected CanvasGroup interfaceCanvas;
    protected bool interfaceOpen = false;
    #endregion

    #region Interface Feedbacks
    [FoldoutGroup("Interface Feedbacks")]
    [SerializeField] private MMF_Player interfaceOpened;
    [FoldoutGroup("Interface Feedbacks")]
    [SerializeField] private MMF_Player interfaceClosed;
    #endregion

    #region Unity Update Methods
    protected virtual void Awake()
    {
        interfaceCanvas = GetComponentInChildren<CanvasGroup>(true);
    }

    protected virtual void Start()
    {
        SetInitialState();
    }
    #endregion

    #region Virtual Methods
    protected virtual void SetInitialState()
    {
        if (!startOpen)
        {
            if (interfaceCanvas)
            {
                interfaceCanvas.alpha = 0f;
                if (canvas)
                {
                    canvas.enabled = false;
                }
            }
            CloseInterface();
            SetInteractionEnabled(interfaceOpen);
        }
    }

    public virtual void OpenInterface()
    {
        StartCoroutine(FadeInterface(1));
        SetInteractionEnabled(true);
        interfaceOpen = true;
        interfaceOpened?.PlayFeedbacks();
    }

    public virtual void CloseInterface()
    {
        StartCoroutine(FadeInterface(0));
        SetInteractionEnabled(false);
        interfaceOpen = false;
        interfaceClosed?.PlayFeedbacks();
    }

    protected virtual void SetInteractionEnabled(bool enabled)
    {
        if (canvasInteractibility)
        {
            canvasInteractibility.enabled = enabled;
        }
    }

    private IEnumerator FadeInterface(float alpha)
    {
        if (interfaceCanvas != null)
        {
            fadeCoroutine = StartCoroutine(UI.FadeCanvasGroup(interfaceCanvas, alpha, fadeLength));
            yield return fadeCoroutine;
            if (canvas != null)
            {
                if (alpha <= 0)
                {
                    canvas.enabled = false;
                }
                else
                {
                    canvas.enabled = true;
                }
            }
        }
    }
    #endregion

    #region Abstract Methods
    protected abstract void UpdateInterface();
    #endregion

    #region TEvent Methods
    public virtual void OnEvent(HUDEvent information)
    {
        if (!EventForInterface(information.eventInterface))
        {
            CloseIfOtherOpened(information.eventType);
            return;
        }

        switch (information.eventType)
        {
            case HUDEventType.InterfaceUpdated:
                UpdateInterface();
                break;
            case HUDEventType.InterfaceToggled:
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                UI.ToggleInterface(interfaceOpen, CloseInterface, OpenInterface);
                SetInteractionEnabled(interfaceOpen);
                break;
            case HUDEventType.InterfaceClosed:
                if (interfaceOpen)
                {
                    CloseInterface();
                }
                break;
        }
    }

    private bool EventForInterface(BaseInterface eventInterface)
    {
        if (eventInterface != this)
        {
            return false;
        }
        return true;
    }

    private void CloseIfOtherOpened(HUDEventType eventType)
    {
        if (eventType != HUDEventType.InterfaceToggled)
        {
            return;
        }

        if (closeWhenOtherInterfaceOpened)
        {
            if (interfaceOpen)
            {
                CloseInterface();
            }
        }
    }

    protected virtual void OnEnable()
    {
        this.Subscribe<HUDEvent>();
    }

    protected virtual void OnDisable()
    {
        this.Unsubscribe<HUDEvent>();
    }
    #endregion
}

public static class InterfaceMethods
{
    public static IEnumerator CloseCoroutine(this BaseInterface baseInterface, float delay)
    {
        yield return new WaitForSeconds(delay);
        baseInterface.CloseInterface();
    }

}
