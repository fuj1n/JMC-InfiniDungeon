using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public Targetable source;
    public Targetable target;
    public float speed;

    public float damage;

    private void Update()
    {
        Vector3 direction = target.transform.position - transform.position;
        direction.Normalize();

        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == target.gameObject)
        {
            target.Damage(source, damage);
            Destroy(gameObject);
        }
    }
}
