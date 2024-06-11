using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CursorFollower : MonoBehaviour
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        rectTransform.position = Input.mousePosition;
        canvasGroup.blocksRaycasts = false; // Make sure the image ignores raycasts      
    }

    void Update()
    {
        rectTransform.position = Input.mousePosition;
    }
}
