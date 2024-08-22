using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private Vector2 direction;
    [SerializeField] private float damage;
    [SerializeField] float speed;
    [SerializeField] float liveTime;
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private Collider2D ignoreCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            if (collision != ignoreCollider)
            {
                IDamageable damageable = collision.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.Damage(damage);
                    Destroy(gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Initialize the projectile variables
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="rotation"></param>
    /// <param name="speed"></param>
    /// <param name="liveTime"></param>
    public void Initialize(Vector2 direction, Quaternion rotation, float damage, float speed, float liveTime, Collider2D ignoreCollider)
    {
        this.direction = direction.normalized;
        this.spriteTransform.rotation = rotation;
        this.damage = damage;
        this.speed = speed;
        this.liveTime = liveTime;
        this.ignoreCollider = ignoreCollider;

        StartCoroutine(DestroyCooldown());
    }

    private IEnumerator DestroyCooldown()
    {
        yield return new WaitForSeconds(liveTime);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
