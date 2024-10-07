using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public enum BoidType
{
    Black,
    Yellow,
    Red
}

public class BoidProvider : MonoBehaviour
{
    public Boid boidPrefab;

    public Vector2 startingVelocity = new(0.1f, 0.1f);
    public float maxSpeed = 5f;
    public float minForce = 0.01f;
    public float maxForce = 0.5f;
    public float separationWeight = 1.5f;
    public float cohesionWeight = 1;
    public float alignmentWeight = 1;
    public float centeringForce = 15;
    public float detectRadius = 2f;
    
    public static float MaxSpeed = 5f;
    public static float MinForce = 0.01f;
    public static float MaxForce = 0.5f;
    public static float SeparationWeight = 1.5f;
    public static float CohesionWeight = 1f;
    public static float AlignmentWeight = 1f;
    public static float CenteringForce = 15;
    
    private static readonly List<Boid> RegisteredBoid = new();
    public List<Spawner> spawners = new();
    public static readonly List<Effector> Effectors = new();
    public List<Effector> registeredEffectors = new();

    public static readonly List<Boid> ToRemove = new();

    public List<GameObject> blackBoid = new();
    public List<GameObject> redBoid = new();
    public List<GameObject> yellowBoid = new();
    private void Awake()
    {
        Refresh();
    }

    public void Refresh()
    {
        MaxSpeed = maxSpeed;
        MinForce = minForce;
        MaxForce = maxForce;
        SeparationWeight = separationWeight;
        CohesionWeight = cohesionWeight;
        AlignmentWeight = alignmentWeight;
        CenteringForce = centeringForce;
        Effectors.Clear();
        foreach (var effector in registeredEffectors)
        {
            Effectors.Add(effector);
        }
    }

    public void RemoveBoid()
    {
        foreach (var boid in RegisteredBoid)
        {
            Destroy(boid.gameObject);
        }
        RegisteredBoid.Clear();
    }
    
    private void Update()
    {
        // if (RegisteredBoid.Count <= 0)
        // {
        //     GameManager.Instance.FinishedLevel();
        //     return;
        // }
        foreach (var boid in RegisteredBoid)
        {
            var neighbors = GetNeighbors(boid);
            var effects = GetEffectors(boid);
            boid.Move(neighbors, effects);
        }
    }

    private void LateUpdate()
    {
        foreach (var boid in ToRemove)
        {
            RegisteredBoid.Remove(boid);
        }
        ToRemove.Clear();
    }

    public void CreateBoids()
    {
        foreach (var spawner in spawners)
        {
            for (int i = 0; i < spawner.amount; i++)
            {
                var boid = Instantiate(boidPrefab, transform);
                boid.transform.position = spawner.transform.position;
                GameObject prefab = blackBoid[0];
                if (spawner.type == BoidType.Black)
                {
                    prefab = blackBoid[Random.Range(0, blackBoid.Count - 1)];
                }
                else if (spawner.type == BoidType.Red)
                {
                    prefab = redBoid[Random.Range(0, redBoid.Count - 1)];
                }
                else if (spawner.type == BoidType.Yellow)
                {
                    prefab = yellowBoid[Random.Range(0, yellowBoid.Count - 1)];
                }
                boid.Init(spawner.type, startingVelocity, prefab);
                RegisteredBoid.Add(boid);
            }
        }
        
    }
    
    private List<Boid> GetNeighbors(Boid target)
    {
        var neighbors = new List<Boid>();
        foreach (var boid in RegisteredBoid)
        {
            var distance = Vector2.Distance(target.transform.position, boid.transform.position);
            if (boid != target && distance < detectRadius && target.GetBoidType() == boid.GetBoidType())
            {
                neighbors.Add(boid);
            }
        }
        return neighbors;
    }

    private List<Effector> GetEffectors(Boid target)
    {
        var effects = new List<Effector>();
        foreach (var effector in Effectors)
        {
            if (Vector2.Distance(target.transform.position, effector.transform.position) < effector.radius)
            {
                effects.Add(effector);
            }
        }
        return effects;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BoidProvider))]
public class BoidProviderEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var provider = (BoidProvider)target;
        if (GUILayout.Button("Refresh"))
        {
            provider.Refresh();
        }
    }
}
#endif
