using System.Collections;
using UnityEngine;

public class CharacterConfiner : MonoBehaviour, TEventListener<WorldEvent>
{
    #region Fields
    private Bounds bounds;
    private Vector2 constrainedPos;
    private BoxCollider2D characterCollider;
    #endregion

    public void Awake()
    {
        characterCollider = GetComponentInParent<BoxCollider2D>();
    }

    public void Start()
    {
        InitialiseBounds();
    }

    private void InitialiseBounds()
    {
        PolygonCollider2D levelCollider = LevelBounder.Instance.levelBoundsCollider;

        bounds.min = levelCollider.bounds.min;
        bounds.max = levelCollider.bounds.max;
    }

    public void Update()
    {
        ConfineCharacterX();
        ConfineCharacterY();
    }

    #region Confinement Methods
    private void ConfineCharacterX()
    {
        if (characterCollider.bounds.min.x < bounds.min.x)
        {
            constrainedPos.x = bounds.min.x + (characterCollider.size.x / 2);
            constrainedPos.y = transform.position.y;

            ConstrainCharacter(constrainedPos);
        }

        if (characterCollider.bounds.max.x > bounds.max.x)
        {
            constrainedPos.x = bounds.max.x - (characterCollider.size.x / 2);
            constrainedPos.y = transform.position.y;

            ConstrainCharacter(constrainedPos);
        }
    }

    private void ConfineCharacterY()
    {
        if (characterCollider.bounds.max.y > bounds.max.y)
        {
            constrainedPos.x = transform.position.x;
            constrainedPos.y = bounds.max.y - (characterCollider.size.y / 2);

            ConstrainCharacter(constrainedPos);
        }

        if (characterCollider.bounds.min.y < bounds.min.y)
        {
            constrainedPos.x = transform.position.x;
            constrainedPos.y = bounds.min.y + (characterCollider.size.y / 2);

            ConstrainCharacter(constrainedPos);
        }
    }

    private void ConstrainCharacter(Vector2 constrainedPos)
    {
        transform.position = constrainedPos;
        Physics2D.SyncTransforms();
    }
    #endregion

    #region TEvent Methods
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
    #endregion
}