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

    //  8 * 8 사이즈의 맵 크기
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

    //  많은 오브젝트를 생성하기 때문에 Hierarchy를 정리하기위해 모두 자식으로 넣을 것
    private Transform boardHolder;
    //  가능한 모든 다른 위치들을 추적하기 위해 사용, 오브젝트가 해당 장소에 있는지 없는지 추적하는데도 사용
    private List<Vector3> gridPositions = new List<Vector3>();

    //  List 내부의 Clear 함수를 써서 모든 리스트된 gridPosition을 초기화
    void InitializeList()
    {
        gridPositions.Clear();

        //  게임 상에서 벽이나, 적, 아이템들이 있을 수 있는 가능한 모든 위치를 만들어 줌
        //  열에 1을 뺀 이유는 floor 타일의 가장 자리를 남겨두기 위해
        //  (가장자리 까지 벽이 랜덤하게 생기면 탈출 불가능한 레벨이 생길 가능성이 있음)
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

                //  2D로 작업하기 때문에 Quaternion.identity 사용(회전 값 없이 인스턴스화)
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

    //  게임 보드가 만들어 질때 게임 매니저에 의해 호출되기 때문에 public 선언
    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        //  랜덤한 수의 적을 생성하는 대신 Mathf.Log 를 이용해 생성(로그 함수를 따라 어려워지도록 하기 위해)
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        //  출구는 언제나 같은 오브젝트를 사용하기 때문에 Instantiate를 호출하고 출구 프리팹을 넣는다.
        //  출구의 위치는 항상 같기 때문에 columns - 1, rows - 1
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
