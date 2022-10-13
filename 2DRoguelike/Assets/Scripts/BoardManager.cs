using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    //  8 * 8 �������� �� ũ��
    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    //  ���� ������Ʈ�� �����ϱ� ������ Hierarchy�� �����ϱ����� ��� �ڽ����� ���� ��
    private Transform boardHolder;
    //  ������ ��� �ٸ� ��ġ���� �����ϱ� ���� ���, ������Ʈ�� �ش� ��ҿ� �ִ��� ������ �����ϴµ��� ���
    private List<Vector3> gridPositions = new List<Vector3>();

    //  List ������ Clear �Լ��� �Ἥ ��� ����Ʈ�� gridPosition�� �ʱ�ȭ
    void InitializeList()
    {
        gridPositions.Clear();

        //  ���� �󿡼� ���̳�, ��, �����۵��� ���� �� �ִ� ������ ��� ��ġ�� ����� ��
        //  ���� 1�� �� ������ floor Ÿ���� ���� �ڸ��� ���ܵα� ����
        //  (�����ڸ� ���� ���� �����ϰ� ����� Ż�� �Ұ����� ������ ���� ���ɼ��� ����)
        for(int x = 1; x < columns - 1; x++)
        {
            for(int y= 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for(int x = -1; x < columns + 1; x++)
        {
            for(int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                //  2D�� �۾��ϱ� ������ Quaternion.identity ���(ȸ�� �� ���� �ν��Ͻ�ȭ)
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for(int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    //  ���� ���尡 ����� ���� ���� �Ŵ����� ���� ȣ��Ǳ� ������ public ����
    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        //  ������ ���� ���� �����ϴ� ��� Mathf.Log �� �̿��� ����(�α� �Լ��� ���� ����������� �ϱ� ����)
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        //  �ⱸ�� ������ ���� ������Ʈ�� ����ϱ� ������ Instantiate�� ȣ���ϰ� �ⱸ �������� �ִ´�.
        //  �ⱸ�� ��ġ�� �׻� ���� ������ columns - 1, rows - 1
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
