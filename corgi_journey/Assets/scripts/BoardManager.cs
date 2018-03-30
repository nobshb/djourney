using UnityEngine;
using System;
using System.Collections.Generic; 		
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

namespace Completed
{
    public class BoardManager : MonoBehaviour
    {
        [Serializable] //embed a class with sub properties in the inspector
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

        public int columns = 6;                                         
        public int rows = 6;                                            
        public Count wallCount = new Count(3, 7);                       
        public Count ffCount = new Count(1, 3);                       
        public GameObject exit;                                    
        public GameObject[] floorTiles;                                
        public GameObject[] wallTiles;                                  
        public GameObject[] ffTiles;                                  
        public GameObject[] enemyTiles;                                
        public GameObject[] outerWallTiles;                            

        private Transform boardHolder; //clean hierarchy
        private List<Vector3> gridPositions = new List<Vector3>();  //list of possible locations to place tiles.

        void InitialiseList()
        {
            gridPositions.Clear();
            for (int x = 1; x < columns - 1; x++) //loop columns
            {
                for (int y = 1; y < rows - 1; y++) //loop rows
                {
                    gridPositions.Add(new Vector3(x, y, 0f));
                }
            }
        }
        
        void BoardSetup() //placing outer walls and floor tiles
        {
            boardHolder = new GameObject("Board").transform;
            for (int x = -1; x < columns + 1; x++)
            {
                for (int y = -1; y < rows + 1; y++)
                {
                    GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                    //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                    if (x == -1 || x == columns || y == -1 || y == rows)
                        toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                    //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                    GameObject instance =
                        Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                    instance.transform.SetParent(boardHolder);
                }
            }
        }

        //RandomPosition returns a random position from our list gridPositions.
        Vector3 RandomPosition()
        {
            //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
            int randomIndex = Random.Range(0, gridPositions.Count);
            //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
            Vector3 randomPosition = gridPositions[randomIndex];
            //Remove the entry at randomIndex from the list so that it can't be re-used.
            gridPositions.RemoveAt(randomIndex);
            //Return the randomly selected Vector3 position.
            return randomPosition;
        }

        //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
        void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
        {
            //Choose a random number of objects to instantiate within the minimum and maximum limits
            int objectCount = Random.Range(minimum, maximum + 1);
            //Instantiate objects until the randomly chosen limit objectCount is reached
            for (int i = 0; i < objectCount; i++)
            {
                //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
                Vector3 randomPosition = RandomPosition();
                //Choose a random tile from tileArray and assign it to tileChoice
                GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
                Instantiate(tileChoice, randomPosition, Quaternion.identity);
            }
        }

        public void SetupScene(int level)
        {
            BoardSetup(); //outer walls and floor.
            InitialiseList(); //reset  list of gridpositions
            LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
            LayoutObjectAtRandom(ffTiles, ffCount.minimum, ffCount.maximum);
            int enemyCount = (int)Mathf.Log(level, 3f); //number of enemies
            LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
            Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity); //set the exit tile
        }
    }
}
