using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFollower : MonoBehaviour
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        rectTransform.position = Input.mousePosition;

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false; // Make sure the image ignores raycasts
        }
        else
        {
            Debug.LogWarning("CanvasGroup component missing on CursorFollowerImage.");
        }
    }

    void Update()
    {
        rectTransform.position = Input.mousePosition;
    }
}
