using UnityEngine;
using System.Collections.Generic;

public class Dodge_ButtonSpawner : StaticSerializedMonoBehaviour<Dodge_ButtonSpawner>
{
    public GameObject objectsToSpawn;
    public Vector2 spawnRange = new Vector2(5, 5);

    private void OnEnable()
    {
        SpawnAllObjects();
    }

    private void SpawnAllObjects()
    {
        if (DodgePatternManager.buttonSpawned) return;


        DodgePatternManager.buttonSpawned = Instantiate(objectsToSpawn, GetRandomPosition(), Quaternion.identity);
    }

    private Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(-spawnRange.x / 2, spawnRange.x / 2);
        float randomY = Random.Range(-spawnRange.y / 2, spawnRange.y / 2);
        return new Vector3(randomX, randomY, 0) + transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange.x, spawnRange.y, 0));
    }


}
