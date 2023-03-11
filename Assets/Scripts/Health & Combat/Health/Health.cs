using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Sirenix.Utilities;
using System.Linq;

public class Health : MonoBehaviour
{
    public DamageInstigator RecentDamager { get { return recentDamager; } } 
    [SerializeField, ReadOnly] protected int currentHealth;
    #region Health Properties
    [FoldoutGroup("Health Properties")]
    [Required("Entity Object Not Assigned!"), SerializeField] public EntityObject entityObject;
    #endregion

    #region Damage Response
    [FoldoutGroup("Damage Properties")]
    [SerializeField] private bool invulnerableOnHit = false;

    [FoldoutGroup("Damage Properties"), ShowIf("invulnerableOnHit", false)]
    [SerializeField] private float invulnerableDuration = 0.25f;

    [FoldoutGroup("Damage Properties")]
    [SerializeField] protected bool disableOnDeath = true;

    [FoldoutGroup("Damage Properties")]
    [SerializeField] protected bool destroyOnDeath = false;

    [FoldoutGroup("Damage Properties")]
    [SerializeField] protected bool triggerGameFailedOnDeath = false;
    #endregion

    #region Feedbacks
    [FoldoutGroup("Feedbacks")]
    [SerializeField] protected MMF_Player spawnFeedback;

    [FoldoutGroup("Feedbacks")]
    [SerializeField] protected MMF_Player healedFeedback;

    [FoldoutGroup("Feedbacks")]
    [SerializeField] protected MMF_Player damagedFeedback;

    [FoldoutGroup("Feedbacks")]
    [SerializeField] protected MMF_Player deathFeedback;
    #endregion

    #region References
    protected Rigidbody2D rb;
    protected Collider2D bc;
    protected DamageInstigator recentDamager;
    private bool invulnerable;
    private Action<int> onHealthChanged;
    private Action onDeathAction;
    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        InitialiseComponents();
    }

    protected virtual void InitialiseComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<Collider2D>();
    }

    public virtual void Start()
    {
        InitialiseHealth();
    }
    #endregion

    protected virtual void InitialiseHealth()
    {
        currentHealth = entityObject.InitialHealth();
        spawnFeedback?.PlayFeedbacks();
        HealthEvent.Trigger(HealthEventType.Spawn, this, CurrentHealthPercentage());
        onHealthChanged?.Invoke(CurrentHealthPercentage());
    }

    #region Event Methods
    public void AddListener(params Action<int>[] actionsToAdd)
    {
        for (int i = 0; i < actionsToAdd.Length; i++)
        {
            onHealthChanged += actionsToAdd[i];
        }
    }

    public void RemoveListener(params Action<int>[] actionsToRemove)
    {
        for (int i = 0; i < actionsToRemove.Length; i++)
        {
            onHealthChanged -= actionsToRemove[i];
        }
    }
    #endregion

    #region Damage Methods
    public virtual void Damage(int damage, DamageInstigator instigator)
    {
        recentDamager = instigator;
        TakeDamage(damage);
        if (invulnerableOnHit)
        {
            StartCoroutine(ApplyInvulnerability());
        }
    
    }

    [Button("Damage")]
    protected virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        PerformChecks();
    }

    private IEnumerator ApplyInvulnerability()
    {
        invulnerable = true;

        yield return new WaitForSeconds(invulnerableDuration);

        invulnerable = false;
    }

    protected virtual void PerformChecks()
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(Kill());
        }
        else
        {
            if (recentDamager && damagedFeedback)
            {
                damagedFeedback.transform.position = DamagedFeedbackPosition();
                damagedFeedback.transform.rotation = GetDamagedInstigatorDirection();
                damagedFeedback?.PlayFeedbacks();
            }
        }
        HealthEvent.Trigger(HealthEventType.Damaged, this, CurrentHealthPercentage());
        onHealthChanged?.Invoke(CurrentHealthPercentage());
    }

    private Quaternion GetDamagedInstigatorDirection()
    {
        if (transform.position.x - recentDamager.transform.position.x > 0)
        {
            return Quaternion.Euler(0f, 180f, 0f);
        }
        return Quaternion.Euler(0f, 0f, 0f);
    }

    protected Vector3 DamagedFeedbackPosition()
    {
        if (recentDamager == null)
        {
            return Vector3.zero;
        }

        float posX = Mathf.Clamp(recentDamager.transform.position.x, bc.bounds.min.x, bc.bounds.max.x);
        float posY = Mathf.Clamp(recentDamager.transform.position.y + 1.5f, bc.bounds.min.y, bc.bounds.max.y); //Lazy Solution - Damage Instigator's position is hardcoded

        return new Vector3(posX, posY, 0);
    }

    protected virtual IEnumerator Kill()
    {
        yield return deathFeedback?.PlayFeedbacksCoroutine(this.transform.position, 1f, false);

        TriggerDeathEvent();

        if (disableOnDeath)
        {
            gameObject.SetActive(false);
        }
        else if (destroyOnDeath)
        {
            Destroy(this.gameObject);
        }
    }

    protected virtual void TriggerDeathEvent()
    {
        HealthEvent.Trigger(HealthEventType.Death, this);

        if (entityObject.entityType == EntityType.Player)
        {
            GameEvent.Trigger(GameEventType.PlayerDeath);
        }
        if (triggerGameFailedOnDeath)
        {
            GameEvent.Trigger(GameEventType.LevelFailed);
        }
    }
    #endregion

    #region Healing Methods
    public virtual void HealByPercentage(int percentage)
    {
        int healthToAdd = Mathf.RoundToInt((float)entityObject.MaxHealth() * percentage.PercentageToDecimal());

        Heal(healthToAdd);
    }

    public virtual void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > entityObject.MaxHealth())
        {
            currentHealth = entityObject.MaxHealth();
        }

        healedFeedback?.PlayFeedbacks();

        HealthEvent.Trigger(HealthEventType.Changed, this, CurrentHealthPercentage());
        onHealthChanged?.Invoke(CurrentHealthPercentage());
    }

    [Button("Respawn")]
    public virtual void Respawn()
    {
        InitialiseHealth();
        invulnerable = false;
        HealthEvent.Trigger(HealthEventType.Respawn, this, CurrentHealthPercentage());
        onHealthChanged?.Invoke(CurrentHealthPercentage());
    }
        #endregion

    #region Data
    public int CurrentHealthPercentage() 
    { 
        return Mathf.RoundToInt(currentHealth / (float)entityObject.MaxHealth() * 100); 
    }

    public void SetHealthData(EntityObject healthData)
    {
        entityObject = healthData;
        InitialiseHealth();
    }

    public bool Alive()
    {
        return currentHealth > 0;
    }

    public virtual bool Damagable()
    {
        if (invulnerable || currentHealth <= 0)
        {
            return false;
        }

        return true;
    }

    public virtual bool ImmuneToInstigator(DamageInstigator instigator)
    {
        return false;
    }
    #endregion
}

public struct HealthEvent
{
    public HealthEventType eventType;
    public Health correspondingHealth;
    public int healthPercentage;

    public HealthEvent(HealthEventType eventType, Health correspondingHealth, int healthPercentage)
    {
        this.eventType = eventType;
        this.correspondingHealth = correspondingHealth;
        this.healthPercentage = healthPercentage;
    }

    static HealthEvent eventToCall;

    public static void Trigger(HealthEventType eventType, Health correspondingHealth)
    {
        eventToCall.eventType = eventType;
        eventToCall.correspondingHealth = correspondingHealth;

        EventBus.TriggerEvent(eventToCall);
    }


    public static void Trigger(HealthEventType eventType, Health correspondingHealth, int healthPercentage)
    {
        eventToCall.eventType = eventType;
        eventToCall.correspondingHealth = correspondingHealth;
        eventToCall.healthPercentage  = healthPercentage;

        EventBus.TriggerEvent(eventToCall);
    }
}

public enum HealthEventType { Changed, Damaged, Spawn, Respawn, Death }
