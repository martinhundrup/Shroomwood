using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSprite : MonoBehaviour
{
    private PlayerController player;
    private SpriteRenderer sr;
    private Vector2 defaultPos;
    private bool isAttacking = false;

    private void Awake()
    {
        defaultPos = transform.localPosition;
        player = GetComponentInParent<PlayerController>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void SpriteFlipX(bool flip)
    {
        if (isAttacking) return;
        if (flip)
        {
            transform.localPosition = defaultPos * new Vector2(-1, 1);
            sr.flipX = true;
        }
        else
        {
            transform.localPosition = defaultPos;
            sr.flipX = false;
        }
    }
    
    public void Attack(float _time)
    {
        StartCoroutine(AttackCoroutine(_time));
    }

    private IEnumerator AttackCoroutine(float _time)
    {
        var pos = transform.localPosition;
        transform.localPosition = pos * new Vector2(-1, 1);
        sr.flipY = true;
        sr.flipX = !sr.flipX;
        isAttacking = true;

        yield return new WaitForSeconds(_time);

        sr.flipY = false;
        sr.flipX = !sr.flipX;
        transform.localPosition = pos;
        isAttacking = false;
    }

}
