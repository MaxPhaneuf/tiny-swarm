using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Effect
{
    public BoidType type;
    public float force;
}

public enum EffectorType
{
    Food,
    Exit
}

public class Effector : MonoBehaviour
{
    public EffectorType type;
    public BoidType eatenBy;
    public float radius;
    public float triggerRadius;
    public float timeActive;
    public float timeDecayed = 10;
    public List<Effect> effects = new();

    private SpriteRenderer _renderer;
    private Animator _animator;

    private float _currentTime;
    private static readonly int Open = Animator.StringToHash("Open");

    private bool _opened;
    private float _timeOpened;
    private float _timeDecaying;
    private void Start()
    {
        if(type == EffectorType.Food)
            _renderer = GetComponent<SpriteRenderer>();
        if (type == EffectorType.Exit)
            _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (type == EffectorType.Exit && _opened)
        {
            _timeOpened += Time.deltaTime;
            if (_timeOpened >= timeActive)
            {
                _opened = false;
                _animator.SetBool(Open, false);
            }
        } else if (type == EffectorType.Food)
        {
            _timeDecaying += Time.deltaTime;
            if (_timeDecaying >= timeDecayed)
            {
                BoidProvider.Effectors.Remove(this);
                Destroy(gameObject);
            }
        }
    }

    public Vector2 ApplyEffects(Boid target)
    {
        Vector2 steer = Vector2.zero;
        if (type == EffectorType.Exit)
        {
            _animator.SetBool(Open, true);
            _opened = true;
            _timeOpened = 0;
        }

        foreach (var effect in effects)
        {
            if (effect.type == target.GetBoidType())
                steer += ApplyEffect(target) * effect.force;
        }

        Trigger(target);
        
        return steer;
    }
    
    private Vector2 ApplyEffect(Boid target)
    {
        var desired = ((Vector2)transform.position - (Vector2)target.transform.position).normalized * BoidProvider.MaxSpeed;
        var steer = desired - target.GetVelocity();
        return Vector2.ClampMagnitude(steer, BoidProvider.MaxForce);
    }

    private void Trigger(Boid target)
    {
        if (type == EffectorType.Food && eatenBy == target.GetBoidType())
        {
            if (_renderer && Vector2.Distance(transform.position, target.transform.position) <= triggerRadius)
            {
                _currentTime += Time.deltaTime;
                var color = _renderer.color;
                var targetColor = Color.Lerp(color, new Color(color.r, color.b, color.g, .1f), _currentTime / timeActive);
                _renderer.color = targetColor;
                if (_currentTime >= timeActive)
                {
                    BoidProvider.Effectors.Remove(this);
                    Destroy(gameObject);
                }
            }
        }
        else if (type == EffectorType.Exit)
        {
            if (Vector2.Distance(transform.position, target.transform.position) <= triggerRadius)
            {
                BoidProvider.ToRemove.Add(target);
                GameManager.Instance.Collect();
                Destroy(target.gameObject);
            }
        }
    }
}