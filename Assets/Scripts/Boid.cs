using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private Vector2 _velocity;
    private BoidType _type;
    // private SpriteRenderer _renderer;
    private Vector2 _startVelocity;
    public void Init(BoidType type, Vector2 startVelocity, GameObject prefab)
    {
        _velocity = RandomStartVelocity(startVelocity);
        _type = type;
        // _renderer = GetComponentInChildren<SpriteRenderer>();
        _startVelocity = startVelocity;
        // var color = _type switch
        // {
        //     BoidType.Blue => Color.blue,
        //     BoidType.Red => Color.red,
        //     _ => Color.black
        // };
        // _renderer.color = color;
        Instantiate(prefab, transform);
    }

    private Vector2 RandomStartVelocity(Vector2 startVelocity)
    {
        var start = new Vector2(
            Random.Range(-startVelocity.x, startVelocity.x), 
            Random.Range(-startVelocity.y, startVelocity.y));
        if (start.x == 0 && start.y == 0)
        {
            start = new Vector2(
                Random.Range(-startVelocity.x, startVelocity.x), 
                Random.Range(-startVelocity.y, startVelocity.y));
        }

        return start;
    }
    
    public BoidType GetBoidType()
    {
        return _type;
    }

    public Vector2 GetVelocity()
    {
        return _velocity;
    }
    
    public void Move(List<Boid> neighbors, List<Effector> effectors)
    {
        Vector2 acceleration = Vector2.zero;
        acceleration += Separation(neighbors) * BoidProvider.SeparationWeight;
        acceleration += Alignment(neighbors) * BoidProvider.AlignmentWeight; 
        acceleration += Cohesion(neighbors) * BoidProvider.CohesionWeight;

        acceleration += Effectors(effectors);

        acceleration += CenteringForce();
        _velocity += acceleration;
        _velocity = Vector2.ClampMagnitude(_velocity, BoidProvider.MaxSpeed);

        var updated = (Vector3)_velocity * Time.deltaTime;
        if (float.IsNaN(updated.x) || float.IsNaN(updated.y) || float.IsNaN(updated.z))
        {
            _velocity = RandomStartVelocity(_startVelocity);
            updated = _velocity * Time.deltaTime;
        }
        
        transform.position += updated;
        transform.up = _velocity; 
    }
    
    private Vector2 Separation(List<Boid> neighbors)
    {
        var steer = Vector2.zero;
        foreach (var neighbor in neighbors)
        {
            var difference = (Vector2)transform.position - (Vector2)neighbor.transform.position;
            steer += difference.normalized / difference.magnitude;
        }

        if (!(steer.magnitude > 0)) return steer;
        steer = steer.normalized * BoidProvider.MaxSpeed - _velocity;
        steer = Vector2.ClampMagnitude(steer, BoidProvider.MaxForce);
        return steer;
    }
    
    private Vector2 Alignment(List<Boid> neighbors)
    {
        var avgVelocity = Vector2.zero;
        foreach (var neighbor in neighbors)
        {
            avgVelocity += neighbor._velocity;
        }

        if (neighbors.Count <= 0) return Vector2.zero;
        
        avgVelocity /= neighbors.Count;
        avgVelocity = avgVelocity.normalized * BoidProvider.MaxSpeed;
        var steer = avgVelocity - _velocity;
        return Vector2.ClampMagnitude(steer, BoidProvider.MaxForce);
    }
    
    private Vector2 Cohesion(List<Boid> neighbors)
    {
        var avgPosition = Vector2.zero;
        foreach (var neighbor in neighbors)
        {
            avgPosition += (Vector2)neighbor.transform.position;
        }

        if (neighbors.Count <= 0) return Vector2.zero;
        
        avgPosition /= neighbors.Count;
        var desired = (avgPosition - (Vector2)transform.position).normalized * BoidProvider.MaxSpeed;
        var steer = desired - _velocity;
        return Vector2.ClampMagnitude(steer, BoidProvider.MaxForce);
    }

    private Vector2 Effectors(List<Effector> effectors)
    {
        var steer = Vector2.zero;
        foreach (var effector in effectors)
        {
            steer = effector.ApplyEffects(this);
        }
        return steer;
    }
    
    private Vector2 CenteringForce()
    {
        Vector2 position = transform.position;
        
        float distanceFromCenter = Vector2.Distance(position, Vector2.zero);
        
        if (distanceFromCenter > BoidProvider.CenteringForce)
        {
            Vector2 directionToCenter = (Vector2.zero - position).normalized;
            float distanceFactor = distanceFromCenter - BoidProvider.CenteringForce;
            
            return directionToCenter * (distanceFactor * BoidProvider.MaxForce);
        }

        return Vector2.zero;
    }
}
