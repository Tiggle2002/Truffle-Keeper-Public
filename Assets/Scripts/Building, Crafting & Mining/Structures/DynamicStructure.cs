using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;


public class DynamicStructure : Structure
{
    #region Private Variables
    private StructureIntegrityLevel currentLevel;
    #endregion

    protected override void Initialise()
    {
        OrderStructureLevels();
    }

    public override bool Upgradeable()
    {
        return state == StructureState.Intact && structureData.upgradeable;
    }

    public override bool Repairable()
    {
        return state == StructureState.Destroyed || state == StructureState.Damaged;
    }

    public override void Upgrade()
    {
        base.Upgrade();
        OrderStructureLevels();
        GetComponentInChildren<Health>(true).SetHealthData(structureData.healthData);
    }

    private void OrderStructureLevels()
    {
        if (structureData.structureLevels.Count > 0)
        {
            OrderLevelsHPAscending();
            sr.sprite = structureData.structureLevels.Last().sprite;
        }
    }

    private void OrderLevelsHPAscending()
    {
        var structureHealthLevels = from level in structureData.structureLevels
                                                                  orderby level.HealthPercentage() ascending
                                                                  select level;

        structureData.structureLevels = structureHealthLevels.ToList<StructureIntegrityLevel>();
    }

    protected virtual void CheckLevelMatchesHealth(int healthPercentage)
    {
        for (int i = 0; i < structureData.structureLevels.Count; i++)
        {
            if (healthPercentage <= structureData.structureLevels[i].HealthPercentage())
            {
                ChangeLevel(structureData.structureLevels[i]);
                SetSpriteToCurrentLevel();
                break;
            }
        }
    }

    protected void ChangeLevel(StructureIntegrityLevel newLevel)
    {
        currentLevel = newLevel;
    }

    protected void SetSpriteToCurrentLevel()
    {
        if (sr.sprite != currentLevel.sprite)
        {
            sr.sprite = currentLevel.sprite;
        }
    }

    private void MatchStateToHealth(int healthPercentage)
    {
        if (healthPercentage < 100 && state != StructureState.Damaged)
        {
            SetState(StructureState.Damaged);
        }
        else if (healthPercentage == 0 && state != StructureState.Destroyed)
        {
            SetState(StructureState.Destroyed);
            if (disableCollider) bc.enabled = false;
        }
    }

    private void NotifyOfHealthChanged()
    {
        StructureEvent.Trigger(StructureEventType.HealthChanged, this);
    }

    public override void OnEnable()
    {
        GetComponentInChildren<Health>(true).AddListener(CheckLevelMatchesHealth, MatchStateToHealth);
        base.OnEnable();
    }

    public virtual void OnDisable()
    {
        GetComponentInChildren<Health>(true).RemoveListener(CheckLevelMatchesHealth, MatchStateToHealth);
    }
}

[System.Serializable]
public struct StructureIntegrityLevel
{
    [TableColumnWidth(200, Resizable = false), PreviewField(Alignment = ObjectFieldAlignment.Center)]
    public Sprite sprite;

    [SerializeField]
    private int healthPercentage;

    public int HealthPercentage()
    {
        return healthPercentage;
    }
}