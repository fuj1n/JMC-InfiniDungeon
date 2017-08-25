using UnityEngine;

public class Projectile : MonoBehaviour {
    public Targetable source;
    public Targetable target;
    public float speed;

    public float damage;

    private void Update()
    {
        if(!target)
        {
            Destroy(gameObject);
            return;
        }

        transform.LookAt(target.transform);

        Vector3 direction = target.transform.position - transform.position;
        direction.Normalize();

        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(target && other.gameObject == target.gameObject)
        {
            target.Damage(source, damage);
            Destroy(gameObject);
        }
    }
}
