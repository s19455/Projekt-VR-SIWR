using UnityEngine;

public class WaypointPatrol : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float speed = 1.2f;
    [SerializeField] private float arriveDistance = 0.3f;
    [SerializeField] private float rotateSpeed = 6f;
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParam = "Speed";

    private int _idx;

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        var target = waypoints[_idx];
        if (target == null) return;

        var delta = target.position - transform.position;
        delta.y = 0f;

        if (delta.sqrMagnitude < arriveDistance * arriveDistance)
        {
            _idx = (_idx + 1) % waypoints.Length;
            return;
        }

        // Rotate
        if (delta.sqrMagnitude > 0.001f)
        {
            var look = Quaternion.LookRotation(delta);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotateSpeed * Time.deltaTime);
        }

        // Move forward
        var step = transform.forward * speed * Time.deltaTime;
        transform.position += new Vector3(step.x, 0f, step.z);

        if (animator != null && !string.IsNullOrEmpty(speedParam))
            animator.SetFloat(speedParam, speed);
    }

    private void OnDrawGizmosSelected()
    {
        if (waypoints == null) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);
            var next = waypoints[(i + 1) % waypoints.Length];
            if (next != null) Gizmos.DrawLine(waypoints[i].position, next.position);
        }
    }
}
