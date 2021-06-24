using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSceneController : MonoBehaviour
{
    public BoidController boidPrefab;

    public int spawnBoids = 100;
    public float boidSpeed = 10f;
    public float boidSteeringSpeed = 100f;
    public float boidNoClumpingArea = 10f;
    public float boidLocalArea = 10f;
    public float boidSimulationArea = 50f;
    public int numberOfSwarms = 3;

    private List<BoidController> boids;

    private void Start()
    {
        boids = new List<BoidController>();

        for (int i = 0; i < spawnBoids; i++)
        {
            SpawnBoid(boidPrefab.gameObject, 0);
        }
    }

    private void Update()
    {
        foreach (BoidController boid in boids)
        {
            boid.SimulateMovement(boids, Time.deltaTime);
        }
    }

    private void SpawnBoid(GameObject prefab, int swarmIndex)
    {
        var boidInstance = Instantiate(prefab);
        boidInstance.transform.localPosition += new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
        boidInstance.transform.localRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

        var boidController = boidInstance.GetComponent<BoidController>();

        var upperBoundedNumberOfSwarms = Random.Range(numberOfSwarms - 1, 9);
        var lowerBoundedNumberOfSwarms = Random.Range(0, upperBoundedNumberOfSwarms);

        boidController.SwarmIndex = lowerBoundedNumberOfSwarms;
        boidController.Speed = boidSpeed;
        boidController.SteeringSpeed = boidSteeringSpeed;
        boidController.LocalAreaRadius = boidLocalArea;
        boidController.NoClumpingRadius = boidNoClumpingArea;
        boids.Add(boidController);
    }
}