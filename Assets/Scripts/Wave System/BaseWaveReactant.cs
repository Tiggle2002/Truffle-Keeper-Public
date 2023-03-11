using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public abstract class BaseWaveReactant : MonoBehaviour, TEventListener<MonumentEvent>
{
    [SerializeField, FoldoutGroup("Feedback")] private MMF_Player feedback;

    public abstract void OnEvent(MonumentEvent eventData);

    public virtual void OnEnable()
    {
        this.Subscribe();
    }

    public virtual void OnDisable()
    {
        this.Unsubscribe();
    }
}
