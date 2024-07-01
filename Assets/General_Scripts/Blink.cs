using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    private Shader whiteShader;
    private Shader defaultShader;
    protected SpriteRenderer sr;
    private bool blinking = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        whiteShader = Shader.Find("GUI/Text Shader");
        defaultShader = sr.material.shader;
    }

    public void StartBlinking()
    {
        if (blinking) return;

        blinking = true;
        StartCoroutine(BlinkCoroutine());
    }

    public void StopBlinking()
    {
        blinking = false;
    }

    public void BlinkAmount(int _amt)
    {
        if (blinking) return;
        StartCoroutine(BlinkCoroutine(_amt));
    }

    protected IEnumerator BlinkCoroutine()
    {
        while (blinking)
        {
            sr.material.shader = whiteShader;
            yield return new WaitForSeconds(0.1f);
            sr.material.shader = defaultShader;
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    protected IEnumerator BlinkCoroutine(int blinkCount)
    {
        while (blinkCount > 0)
        {
            sr.material.shader = whiteShader;
            yield return new WaitForSeconds(0.1f);
            sr.material.shader = defaultShader;
            yield return new WaitForSeconds(0.1f);
            blinkCount--;
        }
        yield return null;
    }
}
