using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Animator))]
public class NPCFetcher : MonoBehaviour
{
    public enum State { Patrol, Seek, Carry, Dropoff }

    [SerializeField] private XRGrabInteractable target;
    [SerializeField] private WaypointPatrol patrol;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private float fetchSpeed = 1.4f;
    [SerializeField] private float pickupDistance = 0.6f;
    [SerializeField] private float dropoffDistance = 0.7f;
    [SerializeField] private float settleSpeedThreshold = 0.2f;
    [SerializeField] private float rotateSpeed = 8f;
    [SerializeField] private Vector3 handHoldOffset = new Vector3(0.05f, 0f, 0.05f);
    [SerializeField] private float carryDelayBeforeDropoff = 1.0f;

    private State _state = State.Patrol;
    private Animator _anim;
    private Rigidbody _targetRb;
    private Transform _rightHand;
    private float _carryTimer;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        if (_anim != null && _anim.isHuman)
            _rightHand = _anim.GetBoneTransform(HumanBodyBones.RightHand);

        if (target != null)
        {
            _targetRb = target.GetComponent<Rigidbody>();
            target.selectExited.AddListener(OnReleased);
        }
        if (patrol == null) patrol = GetComponent<WaypointPatrol>();
    }

    private void OnDestroy()
    {
        if (target != null) target.selectExited.RemoveListener(OnReleased);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (_state != State.Patrol) return;
        _state = State.Seek;
        if (patrol != null) patrol.enabled = false;
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Seek: UpdateSeek(); break;
            case State.Carry: UpdateCarry(); break;
            case State.Dropoff: UpdateDropoff(); break;
        }
    }

    private void UpdateSeek()
    {
        if (target == null) { ResumePatrol(); return; }

        // Wait for thrown object to settle
        if (_targetRb != null && _targetRb.linearVelocity.magnitude > settleSpeedThreshold)
        {
            _anim.SetFloat("Speed", 0f);
            return;
        }

        if (MoveTowards(target.transform.position, pickupDistance)) Pickup();
    }

    private void UpdateCarry()
    {
        _anim.SetFloat("Speed", 0f);
        _carryTimer += Time.deltaTime;
        if (_carryTimer >= carryDelayBeforeDropoff && dropPoint != null)
        {
            _state = State.Dropoff;
        }
    }

    private void UpdateDropoff()
    {
        if (dropPoint == null) { ResumePatrol(); return; }
        if (MoveTowards(dropPoint.position, dropoffDistance)) Drop();
    }

    // Returns true if NPC arrived at target (within stopDistance)
    private bool MoveTowards(Vector3 worldPos, float stopDistance)
    {
        var to = worldPos - transform.position;
        to.y = 0f;
        if (to.sqrMagnitude < stopDistance * stopDistance)
        {
            _anim.SetFloat("Speed", 0f);
            return true;
        }
        if (to.sqrMagnitude > 0.001f)
        {
            var look = Quaternion.LookRotation(to);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotateSpeed * Time.deltaTime);
        }
        var step = transform.forward * fetchSpeed * Time.deltaTime;
        transform.position += new Vector3(step.x, 0f, step.z);
        _anim.SetFloat("Speed", fetchSpeed);
        return false;
    }

    private void Pickup()
    {
        if (target == null || _rightHand == null) { ResumePatrol(); return; }
        if (_targetRb != null)
        {
            _targetRb.isKinematic = true;
            _targetRb.detectCollisions = false;
        }
        target.enabled = false;
        target.transform.SetParent(_rightHand, true);
        target.transform.localPosition = handHoldOffset;
        target.transform.localRotation = Quaternion.identity;
        _carryTimer = 0f;
        _state = State.Carry;
    }

    private void Drop()
    {
        if (target == null) { ResumePatrol(); return; }
        target.transform.SetParent(null, true);
        target.transform.position = dropPoint.position;
        target.transform.rotation = dropPoint.rotation;
        if (_targetRb != null)
        {
            _targetRb.isKinematic = false;
            _targetRb.detectCollisions = true;
            _targetRb.useGravity = true;
            _targetRb.linearVelocity = Vector3.zero;
        }
        target.enabled = true;
        ResumePatrol();
    }

    private void ResumePatrol()
    {
        _state = State.Patrol;
        if (patrol != null) patrol.enabled = true;
    }
}
