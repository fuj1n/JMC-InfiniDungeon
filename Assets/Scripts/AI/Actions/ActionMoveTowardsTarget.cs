using UnityEngine;

public class ActionMoveTowardsTarget : AIAction
{
    private Rigidbody rigidbody;

    private float distance;
    private Vector3 forwardOffset;
    private Transform target;

    private float speed;

    private float stepTime;

    private AudioSource source;

    public ActionMoveTowardsTarget(float distance, Vector3 forwardOffset, Transform target, float speed, float stepTime = 0F, AudioSource source = null)
    {
        this.distance = distance;
        this.forwardOffset = forwardOffset;
        this.target = target;

        this.speed = speed;

        this.stepTime = stepTime;
        this.source = source;
    }

    public override void OnStart()
    {
        base.OnStart();

        rigidbody = owner.GetComponent<Rigidbody>();
    }

    public override bool IsDone()
    {
        return Vector3.Distance(owner.transform.position, target.transform.position) <= distance;
    }

    public override void OnPhysicsUpdate()
    {
        base.OnPhysicsUpdate();

        if (Vector3.Distance(owner.transform.position, target.transform.position) > distance)
        {
            rigidbody.AddForce((owner.transform.forward + forwardOffset) * speed, ForceMode.VelocityChange);

            if (source)
                source.Play();

            if (stepTime > 0F)
                Sleep(stepTime);
        }
    }
}
