using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Confiner : MonoBehaviour, TEventListener<WorldEvent>
{
    [SerializeField, FoldoutGroup("Confinement Settings")] private bool confineX;
    [SerializeField, FoldoutGroup("Confinement Settings")] private bool confineY;

    protected Bounds boundsToConfine;
    private Bounds levelBounds;
    private Vector2 constrainedPos;

    private void Start()
    {
        InitialiseBounds();
        InitialiseComponents();
    }

    private void InitialiseBounds()
    {
        levelBounds.min = LevelBounder.Instance.levelBoundsCollider.bounds.min;
        levelBounds.max = LevelBounder.Instance.levelBoundsCollider.bounds.max;
    }

    protected abstract void InitialiseComponents();

    private void Update()
    {
        UpdateTargetBounds();
        Confine();
    }

    protected abstract void UpdateTargetBounds();

    private void Confine()
    {
        if (confineX) ConfineX();
        if (confineY) ConfineY();
    }

    private void ConfineX()
    {
        if (boundsToConfine.min.x < levelBounds.min.x)
        {
            constrainedPos.x = levelBounds.min.x + (boundsToConfine.size.x / 2);
            constrainedPos.y = transform.position.y;

            Confine(constrainedPos);
        }

        if (boundsToConfine.max.x > levelBounds.max.x)
        {
            constrainedPos.x = levelBounds.max.x - (boundsToConfine.size.x / 2);
            constrainedPos.y = transform.position.y;

            Confine(constrainedPos);
        }
    }

    private void ConfineY()
    {
        if (boundsToConfine.max.y > levelBounds.max.y)
        {
            constrainedPos.x = transform.position.x;
            constrainedPos.y = levelBounds.max.y - (boundsToConfine.size.y / 2);

            Confine(constrainedPos);
        }

        if (boundsToConfine.min.y < levelBounds.min.y)
        {
            constrainedPos.x = transform.position.x;
            constrainedPos.y = levelBounds.min.y + (boundsToConfine.size.y / 2);

            Confine(constrainedPos);
        }
    }

    private void Confine(Vector2 constrainedPos)
    {
        transform.position = new Vector3(constrainedPos.x, constrainedPos.y, transform.position.z);
        Physics2D.SyncTransforms();
    }

    public void OnEvent(WorldEvent eventData)
    {
        switch (eventData.eventType)
        {
            case WorldEventType.BoundsChanged:
                InitialiseBounds();
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