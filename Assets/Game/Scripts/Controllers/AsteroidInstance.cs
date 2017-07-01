﻿using UnityEngine;

/// <summary>
/// Instance of an asteroid which manages it's positon and destruction.
/// </summary>
public class AsteroidInstance : MonoBehaviour
{
    private float speed = 10;
    private int impactRadius;

    private Vector3 targetPosition;
    private GameObject impactPrefab;

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        TerrainUtilities.GenerateCircle(1, World.Current.WorldData, WorldController.Instance.WorldCoordiantesToGridSpace(transform.position), TileType.Empty);

        if (transform.position != targetPosition) return;
        Impact();
    }

    private void Impact()
    {
        float size = impactRadius / 2f;
        Vector2i impactPosition = WorldController.Instance.WorldCoordiantesToGridSpace(transform.position + new Vector3(0, size));
        TerrainUtilities.GenerateFuzzyCircle(impactRadius, impactRadius + 2, World.Current.WorldData, impactPosition, TileType.Empty);
        GameObject impactGameObject = Instantiate(impactPrefab, transform.position, Quaternion.identity);
        impactGameObject.transform.localScale = new Vector3(size, size, size);

        Destroy(impactGameObject, impactGameObject.GetComponent<ParticleSystem>().main.duration);
        Destroy(gameObject);
    }

    public static AsteroidInstance Create(int impactRadius, float speed, Vector3 targetPosition, GameObject prefab, GameObject impactPrefab)
    {
        float angle = Random.Range(40, -40)  * Mathf.Deg2Rad;
        int distance = Mathf.FloorToInt(Camera.main.orthographicSize * 3);

        Vector3 spawnPosition = targetPosition + new Vector3(Mathf.Sin(angle) * distance, Mathf.Cos(angle) * distance, -1);
        GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity);
        AsteroidInstance asteroidInstance = instance.AddComponent<AsteroidInstance>();

        asteroidInstance.impactRadius = impactRadius;
        asteroidInstance.speed = speed;
        asteroidInstance.targetPosition = targetPosition;
        asteroidInstance.impactPrefab = impactPrefab;

        return asteroidInstance;
    }
}
