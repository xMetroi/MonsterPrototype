using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private Vector2 direction;
    [SerializeField] float speed;
    [SerializeField] float liveTime;
    [SerializeField] private Transform spriteTransform;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Destroy(gameObject);
    }

    /// <summary>
    /// Initialize the projectile variables
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="rotation"></param>
    /// <param name="speed"></param>
    /// <param name="liveTime"></param>
    public void Initialize(Vector2 direction, Quaternion rotation, float speed, float liveTime)
    {
        this.direction = direction.normalized;
        this.spriteTransform.rotation = rotation;
        this.speed = speed;
        this.liveTime = liveTime;

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
