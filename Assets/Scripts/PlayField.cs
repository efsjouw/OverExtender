using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayField : MonoBehaviour
{
    [SerializeField] Transform topLeft;
    [SerializeField] Transform topRight;
    [SerializeField] Transform bottomLeft;
    [SerializeField] Transform bottomRight;

    float leftDistance;
    float rightDistance;

    float topDistance;
    float bottomDistance;

    public enum SpawnDirection
    {
        Left,
        Right,
        Top,
        Bottom
    }    

    private void Start()
    {
        leftDistance = Vector3.Distance(topLeft.position, bottomLeft.position);
        rightDistance = Vector3.Distance(topRight.position, bottomRight.position);

        topDistance = Vector3.Distance(topLeft.position, topRight.position);
        bottomDistance = Vector3.Distance(bottomLeft.position, bottomRight.position);
    }

    public Enemy spawnEnemy(SpawnDirection spawnDirection, Enemy enemyPrefab)
    {
        float xPosition = 0;
        float zPosition = 0;
        switch (spawnDirection)
        {
            case SpawnDirection.Left:
                xPosition = topLeft.position.x;
                zPosition = Random.Range(topLeft.position.y, bottomLeft.position.y);
                break;
            case SpawnDirection.Right:
                xPosition = topRight.position.x;
                zPosition = Random.Range(topRight.position.y, bottomRight.position.y);
                break;
            case SpawnDirection.Top:
                xPosition = Random.Range(topLeft.position.x, topRight.position.x);
                zPosition = topLeft.position.z;
                break;
            case SpawnDirection.Bottom:
                xPosition = Random.Range(bottomLeft.position.x, bottomRight.position.x);
                zPosition = bottomLeft.position.z;
                break;
        }

        GameObject gameObject = Instantiate(enemyPrefab.gameObject);
        gameObject.transform.position = new Vector3(xPosition, 0, zPosition);
        return gameObject.GetComponent<Enemy>();
    }

    /// <summary>
    /// Will return position for enemy to move towards
    /// Will skip parts of the playing field based on spawn direction
    /// Will look if near top/bottom or left/right in order to give a position that is the longest
    /// </summary>
    /// <param name="spawnDirection"></param>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public Vector3 getMovePosition(SpawnDirection spawnDirection, Enemy enemy)
    {
        float xPosition = 0;
        float zPosition = 0;

        bool includeLeft = true;
        bool includeRight = true;

        bool includeTop = true;
        bool includeBottom = true;

        switch (spawnDirection)
        {
            case SpawnDirection.Left:
                includeLeft = false; //Don't include left because already here
                float positionDistanceA = Vector3.Distance(enemy.transform.position, topLeft.position);
                if (positionDistanceA >= (leftDistance / 2)) includeTop = false; //Is the near to the topLeft, so don't return top because it's too close
                else includeBottom = false; //Is the near to the bottomLeft, so don't return bottom because it's too close
                break;
            case SpawnDirection.Right:
                includeRight = false;
                float positionDistanceB = Vector3.Distance(enemy.transform.position, topRight.position);
                if (positionDistanceB >= (leftDistance / 2)) includeTop = false;
                else includeBottom = false; 
                break;
            case SpawnDirection.Top:
                includeTop = false;
                float positionDistanceC = Vector3.Distance(enemy.transform.position, topLeft.position);
                if (positionDistanceC >= (leftDistance / 2)) includeTop = false;
                else includeBottom = false;
                break;
            case SpawnDirection.Bottom:
                includeBottom = false;
                float positionDistanceD = Vector3.Distance(enemy.transform.position, bottomLeft.position);
                if (positionDistanceD >= (leftDistance / 2)) includeTop = false;
                else includeBottom = false;
                break;
        }

        List<Vector3> positions = new List<Vector3>();

        if (includeLeft) positions.Add(getRandomPositionBetweenPoints(topLeft, bottomLeft));
        if (includeRight) positions.Add(getRandomPositionBetweenPoints(topRight, bottomRight));
        if (includeBottom) positions.Add(getRandomPositionBetweenPoints(bottomLeft, bottomRight));
        if (includeTop) positions.Add(getRandomPositionBetweenPoints(topLeft, topRight));

        return positions[Random.Range(0, positions.Count - 1)];
    }

    private Vector3 getRandomPositionBetweenPoints(Transform transformA, Transform transformB)
    {
        float xPosition = Random.Range(transformA.position.x, transformB.position.x);
        float zPosition = Random.Range(transformA.position.z, transformB.position.z);
        return new Vector3(xPosition, 0, zPosition);
    }
}
