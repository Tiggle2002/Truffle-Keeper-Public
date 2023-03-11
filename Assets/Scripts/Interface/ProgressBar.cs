using MoreMountains.Tools;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : BaseInterface
{
    #region Variables
    [FoldoutGroup("Bar Properties")]
    [SerializeField] protected bool singleBar;
    [FoldoutGroup("Bar Properties")]
    [SerializeField] protected bool hideIfZero;
    [FoldoutGroup("Bar Properties")]
    [SerializeField] protected bool lerpFill;

    [FoldoutGroup("Bar Properties")]
    [ShowIf("lerpFill")]
    [Range(0f, 1f)]
    [SerializeField] private float lerpValue;

    [FoldoutGroup("Bar Properties")]
    [ShowIf("singleBar")]
    [SerializeField] protected Image bar;

    [FoldoutGroup("Bar Properties")]
    [HideIf("singleBar")]
    [SerializeField] protected Image[] bars;

    [FoldoutGroup("Bar Properties")]
    [SerializeField] protected Image background;
    #endregion

    #region Methods
    protected virtual void ResetBar(float initialFill = 1)
    {
        if (singleBar)
        {
            bar.fillAmount = initialFill;
            bar.enabled = true;
            bar.gameObject.SetActive(true);
            bar.color = new(bar.color.a, bar.color.g, bar.color.b, 1);
        }
        else
        {
            for (int i = 0; i < bars.Length; i++)
            {
                bars[i].fillAmount = initialFill;
            }
        }
    }
    
    protected virtual void UpdateBar(float value)
    {   
        if (singleBar)
        {
            StartCoroutine(SetBar(value));
        }
        else
        {
            SetBars(value);
        }
    }

    protected virtual IEnumerator SetBar(float newPercentage)
    {
        if (lerpFill)
        {
            yield return StartCoroutine(UI.LerpFilledImage(bar, newPercentage, lerpValue));
        }
        else
        {
            bar.fillAmount = newPercentage;
        }
    }

    private void SetBars(float newPercentage)
    {
        for (int i = 0; i < bars.Length; i++)
        {
            if (lerpFill)
            {
                StartCoroutine(UI.LerpFilledImage(bars[i], newPercentage, lerpValue));
            }
            else
            {
                bars[i].fillAmount = newPercentage;
            }
        }
    }
    #endregion

    protected void Show(bool shouldShow)
    {
        if (bars.Length > 0)
        {
            for (int i = 0; i < bars.Length; i++)
            {
                bars[i].enabled = shouldShow;
            }
        }
        else
        {
            bar.enabled = shouldShow;
        }

        if (background)
        {
            background.enabled = shouldShow;
        }
    }

    protected override void UpdateInterface()
    {
        throw new System.NotImplementedException();
    }
}


