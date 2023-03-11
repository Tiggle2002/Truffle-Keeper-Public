using System.Collections;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public static class TextPresenter
{
    private static ObjectPool<PoolObject> textPool;

    static TextPresenter()
    {
        GameObject textPrefab = Resources.Load<GameObject>("Prefabs/UI/Floating Text");
        textPool = new(textPrefab, 3);
    }

    public static IEnumerator PresentTextMesh(this Vector3 pos, string text, float activeTime, Sprite sprite = null)
    {
        GameObject textObj = textPool.PullGameObject();
        TextMeshProUGUI textMesh = textObj.GetComponentInChildren<TextMeshProUGUI>(true);
        Image image = null;
        if (sprite)
        {
            image = textObj.GetComponentInChildren<Image>(true);
            image.sprite = sprite;
        }

        textMesh.CrossFadeAlpha(1, 0.01f, false);
        textMesh.text = "";

        textObj.transform.position = pos;

        foreach (char character in text)
        {
            textMesh.text += character;

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(activeTime);

        textMesh.CrossFadeAlpha(0, 1f, false);
        if (sprite)
        {
            image.CrossFadeAlpha(0, 1f, false);
            image.sprite = UIService.Instance.emptySprite;
        }

        yield return new WaitForSeconds(1f);
        textObj.gameObject.SetActive(false);
    }

    public static IEnumerator PresentTextMeshCoroutine(this Vector3 pos, string text, Func<bool> playUntil, Sprite sprite = null)
    {
        GameObject textObj = textPool.PullGameObject();
        TextMeshProUGUI textMesh = textObj.GetComponentInChildren<TextMeshProUGUI>(true);
        Image image = null;
        if (sprite)
        {
            image = textObj.GetComponentInChildren<Image>(true);
            image.sprite = sprite;
        }

        textMesh.ResetText();

        textObj.transform.position = pos;

        foreach (char character in text)
        {
            textMesh.text += character;

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitUntil(playUntil);

        textMesh.CrossFadeAlpha(0, 1f, false);
        if (sprite)
        {
            image.CrossFadeAlpha(0, 1f, false);
            image.sprite = UIService.Instance.emptySprite;
        }

        yield return new WaitForSeconds(1f);
        textObj.gameObject.SetActive(false);
    }

    private static void ResetText(this TextMeshProUGUI textMesh)
    {
        textMesh.CrossFadeAlpha(1, 0.01f, false);
        textMesh.text = "";
    }
}