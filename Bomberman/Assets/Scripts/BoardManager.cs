using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count 
    {
        private int minimum;
        private int maximum;
        public Count (int min, int max)
        {
            this.minimum = min;
            this.maximum = max;
        }

        public int getMin() 
        {
            return this.minimum;
        }
        public int getMax()
        {
            return this.maximum;
        }
    }

    private int cols = 30;
    private int rows = 10;
    public Count breakableWallCount = new Count(40,60);
    public GameObject[] breakableWallTiles;
    public GameObject[] bonusWallTiles;
    public GameObject[] outerWallTiles;
    public GameObject[] enemyTiles;
    public GameObject[] exitWall;
    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();
    private int playerX = 1;
    private int playerY = 8;
    
    void InitializeList() 
    {
        gridPositions.Clear();
        for (int x = 1; x < cols ; x++) 
        {
            for (int y = 1; y < rows ; y++) {

                if (!(x%2 != 1 && y%2 != 1) && !(x == playerX && (y == playerY || y == playerY + 1)) 
                                            && !(x == playerX + 1 && y == playerY + 1))
                    gridPositions.Add(new Vector3(x + 0.5f, y + 0.5f, 0f));
            }
        }
    }

    void BoardSetup() 
    {
        boardHolder = new GameObject("Board").transform;
        for (int x = 0; x < cols + 1; x++) 
        {
            for (int y = 0; y < rows + 1; y++)
            {
                if (x == 0 || x == cols || y == 0 || y == rows)
                {
                    GameObject toInstantiate = outerWallTiles[Random.Range(0,outerWallTiles.Length)];
                    GameObject instance = Instantiate(toInstantiate, new Vector3(x + 0.5f, y + 0.5f, 0f),
                        Quaternion.identity) as GameObject;
                    instance.transform.SetParent(boardHolder);
                    instance.layer = 8;
                    instance.AddComponent<BoxCollider2D>();
                }
                else if (x % 2 == 0 && y % 2 == 0)
                {
                    GameObject toInstantiate = outerWallTiles[Random.Range(0,outerWallTiles.Length)];
                    GameObject instance = Instantiate(toInstantiate, new Vector3(x + 0.5f,y +0.5f ,0f),
                        Quaternion.identity);
                    instance.transform.SetParent(boardHolder);
                    instance.layer = 8;
                    instance.AddComponent<BoxCollider2D>();
                }
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randIndex = Random.Range(0, gridPositions.Count);
        Vector3 randPosition = gridPositions[randIndex];
        gridPositions.RemoveAt(randIndex);
        return randPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int min, int max) 
    {
        int objectCount = Random.Range(min, max + 1);
        for (int i = 0; i < objectCount; i++) 
        {
            Vector3 randomPosition = RandomPosition();
            GameObject  chosenTile = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(chosenTile, randomPosition, Quaternion.identity);
        }
    }

    void LayoutEnemiesAtRandom(GameObject[] tileArray, int min, int max, int top) 
    {
        int objectCount = Random.Range(min, max + 1);
        for (int i = 0; i < objectCount; i++) 
        {
            Vector3 randomPosition;
            do
            {
                randomPosition = RandomPosition();
            } while (CheckIfOnLineOfSight(randomPosition));

            GameObject chosenTile = tileArray[Random.Range(0, top)];
            Instantiate(chosenTile, randomPosition, Quaternion.identity);
        }
    }

    private bool CheckIfOnLineOfSight(Vector3 enemyPositionCandidate)
    {
        List<RaycastHit2D[]> hits = new List<RaycastHit2D[]>();
        hits.Add(Physics2D.LinecastAll(enemyPositionCandidate, enemyPositionCandidate + rows * Vector3.up));
        hits.Add(Physics2D.LinecastAll(enemyPositionCandidate, enemyPositionCandidate + rows * Vector3.down));
        hits.Add(Physics2D.LinecastAll(enemyPositionCandidate, enemyPositionCandidate + cols * Vector3.left));
        hits.Add(Physics2D.LinecastAll(enemyPositionCandidate, enemyPositionCandidate + cols * Vector3.right));
        for (int i = 0; i < hits.Count; i++)
        {
            for (int j = 0; j < hits[i].Length; j++)
            {
                if (hits[i][j].collider.gameObject.CompareTag("Player"))
                {
                    return true;
                }
                if (hits[i][j].collider.gameObject.CompareTag("Brick"))
                {
                    break;
                }
            }
        }
        return false;
    }
    
    public void SetupScene(int level, int enemyCount) 
    {
        InitializeList();
        LayoutObjectAtRandom(breakableWallTiles, breakableWallCount.getMin(), breakableWallCount.getMax());
        LayoutObjectAtRandom(exitWall, 1, 1);
        LayoutObjectAtRandom(bonusWallTiles, 1, 1);
        int top = level;
        if (level >= enemyTiles.Length )
            top = enemyTiles.Length;
        LayoutEnemiesAtRandom(enemyTiles, enemyCount, enemyCount, top);
    }
}
