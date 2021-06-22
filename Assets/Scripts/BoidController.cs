using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    public int SwarmIndex { get; set; }
    public float NoClumpingRadius { get; set; }
    public float LocalAreaRadius { get; set; }
    public float Speed { get; set; }
    public float SteeringSpeed { get; set; }

    public void SimulateMovement(List<BoidController> other, float time)
    {
        //default vars
        var steering = Vector3.zero;

        // separation
        var separationDirection = Vector3.zero;
        var separationCount = 0;
        var alignmentDirection = Vector3.zero;
        var alignmentCount = 0;
        var cohesionDirection = Vector3.zero;
        var cohesionCount = 0;

        var leaderBoid = other[0];
        var leaderAngle = 180f;

        foreach (BoidController boid in other)
        {
            // skip self
            if (boid == this)
                continue;

            var distance = Vector3.Distance(boid.transform.position, this.transform.position);

            // identify local neighbour
            if (distance < NoClumpingRadius)
            {
                separationDirection += boid.transform.position - transform.position;
                separationCount++;
            }

            if (distance < LocalAreaRadius && boid.SwarmIndex == this.SwarmIndex)
            {
                // alignment
                alignmentDirection += boid.transform.forward;
                alignmentCount++;

                // cohension
                cohesionDirection += boid.transform.position - transform.position;
                cohesionCount++;

                var angle = Vector3.Angle(boid.transform.position - transform.position, transform.forward);
                if (angle < leaderAngle && angle < 90f)
                {
                    leaderBoid = boid;
                    leaderAngle = angle;
                }
            }

        }

        // calculate average
        if (separationCount > 0)
        {
            separationDirection /= separationCount;
        }

        if (alignmentCount > 0)
        {
            alignmentDirection /= alignmentCount;
        }

        if (cohesionCount > 0)
        {
            cohesionDirection /= cohesionCount;
        }

        // flip
        separationDirection = -separationDirection;

        //get direction to center of mass
        cohesionDirection -= transform.position;


        // apply to steering
        steering += separationDirection.normalized;
        steering += alignmentDirection.normalized;
        steering += cohesionDirection.normalized;

        // local leader
        if (leaderBoid != null)
        {
            steering += (leaderBoid.transform.position - transform.position).normalized;
        }

        //obstacle avoidance
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, LocalAreaRadius, LayerMask.GetMask("Default")))
        {
            steering = ((hitInfo.point + hitInfo.normal) - transform.position).normalized;
        }

        //apply steering
        if (steering != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(steering), SteeringSpeed * time);
        }

        //move 
        transform.position += transform.TransformDirection(new Vector3(0, 0, Speed)) * time;
    }
}