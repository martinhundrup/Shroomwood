using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] private string hitboxTag; // immune to hitboxes of the same tag
    [SerializeField] private float health;

    protected SpriteRenderer sr;
    private Shader whiteShader;
    private Shader defaultShader;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        whiteShader = Shader.Find("GUI/Text Shader");
        defaultShader = sr.material.shader;
    }

    public float Health
    {
        get { return health; }
        private set 
        { 
            health = value;
            CheckHealth();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var hitbox = collision.gameObject.GetComponent<Hitbox>();

        if (hitbox != null && hitbox.HitboxTag != hitboxTag)
        {
            Health -= hitbox.Damage;
            StartCoroutine(Blink());
        }
    }


    private void CheckHealth()
    {
        if (health <= 0) { Destroy(this.gameObject); }
    }

    protected IEnumerator Blink()
    {
        int _count = 1;

        while (_count > 0)
        {
            sr.material.shader = whiteShader;
            yield return new WaitForSeconds(0.1f);
            sr.material.shader = defaultShader;
            yield return new WaitForSeconds(0.1f);
            _count--;
        }
        yield return null;
    }
}
