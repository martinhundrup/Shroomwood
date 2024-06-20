using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingEffect : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer sr;

    private bool isSwinging;

    [SerializeField] private float distanceFromCenter = 2.0f; // Set the distance from the center
    [SerializeField] private float orbitSpeed = 5.0f;         // Speed of orbiting
    [SerializeField] private GameObject hitbox;

    private void Awake()
    {
        //animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isSwinging)
        {
            OrbitAroundLocalZero();
            RotateOutwardsFromCenter();
        }
    }

    public void Swing(float _time)
    {
        StartCoroutine(SwingCoroutine(_time));
    }

    private IEnumerator SwingCoroutine(float _time)
    {
        //animator.Play("Swing");
        isSwinging = true;
        sr.enabled = true;
        var _hitbox = Instantiate(hitbox, this.transform);
        _hitbox.transform.localPosition = Vector2.zero;

        yield return new WaitForSeconds(_time);

        //animator.StopPlayback();
        sr.enabled = false;
        Destroy(_hitbox);
        isSwinging = false;
    }

    void OrbitAroundLocalZero()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector3 direction = (mousePosition - transform.parent.position).normalized;
        Vector3 targetPosition = direction * distanceFromCenter;

        float angle = orbitSpeed * Time.deltaTime;
        Vector3 orbitPosition = Quaternion.Euler(0, 0, angle) * targetPosition;
        transform.localPosition = orbitPosition;
    }

    void RotateOutwardsFromCenter()
    {
        Vector3 direction = transform.localPosition.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 180);
    }
}
