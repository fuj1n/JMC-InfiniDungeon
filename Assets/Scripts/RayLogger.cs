using UnityEngine;
using System.Linq;
using System.Collections;

public class RayLogger : MonoBehaviour
{
#if !UNITY_EDITOR
    private void Start()
    {
        Destroy(gameObject);
    }
#endif

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.DrawRay(ray.origin, ray.direction * Vector3.Distance(ray.origin, hit.point), Color.green, 5F, true);
                Debug.Log("--------------");
                Debug.Log("Name = " + hit.collider.name);
                Debug.Log("Tag = " + hit.collider.tag);
                Debug.Log("Hit Point = " + hit.point);
                Debug.Log("Object = " + hit.collider.gameObject.name + " (@" + hit.collider.transform.position + hit.collider.gameObject.GetComponents<Component>().Aggregate("", (working, next) => working + ", " + next.GetType().Name) + ")");
            }
        }
    }
}