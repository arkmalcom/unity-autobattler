using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class UnitAI : MonoBehaviour
{

    public float nextWaypointDistance = 3f;
    Vector2 direction;
    Vector2 force;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    private AbstractUnit unit;
    private AbstractUnit target;
    private SpriteRenderer sprite;

    [SerializeField] private float detectionRange;



    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<AbstractUnit>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        SeekTarget();
        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.transform.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Find targets based on Physics2D colliders
    private void SeekTarget()
    {
        Collider2D[] colliderArray = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        float distance = Mathf.Infinity;
        foreach (Collider2D collider in colliderArray)
        {
            // Check colliders for non-friendly units
            if (collider.TryGetComponent<AbstractUnit>(out AbstractUnit unitInRange))
            {
                if (unit.faction != unitInRange.faction & unitInRange.isAlive)
                {
                    float currentDistance = Vector3.Distance(transform.position, unitInRange.transform.position);
                    if (currentDistance < distance)
                    {
                        distance = currentDistance;
                        SetTarget(unitInRange);
                    }
                }
            }
        }
        return;
    }

    // Sets target
    private void SetTarget(AbstractUnit enemy)
    {
        target = enemy;
        unit.inCombat = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (unit.isAlive)
        {
            if (!unit.inCombat)
                SeekTarget();
            else
            {
                float distanceToTarget = Vector2.Distance(target.transform.position, unit.transform.position);
                if (!target.isAlive)
                {
                    unit.inCombat = false;
                }
                if (distanceToTarget <= unit.attackRange)
                {
                    unit.AttackTarget(target);
                    unit.seekingTarget = false;
                }
                else
                    unit.seekingTarget = true;
                if (unit.transform.position.x < target.transform.position.x)
                    // transform.localScale = new Vector3(-1f, 1f, 1f);
                    sprite.flipX = true;
                else if (unit.transform.position.x > target.transform.position.x)
                    // transform.localScale = new Vector3(1f, 1f, 1f);
                    sprite.flipX = false;

            }
        }
        else
        {
            Destroy(seeker);
        }
    }

    void FixedUpdate()
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        // else move towards waypoint
        force = direction * unit.moveSpeed * Time.deltaTime;

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        rb.AddForce(force);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
}
