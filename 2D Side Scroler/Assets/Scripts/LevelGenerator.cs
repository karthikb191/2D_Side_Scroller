using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    private readonly int TILE_DEACTIVATION_DISTANCE = 25;
    private readonly int MAX_BLOCK_LENGTH = 150;
    private readonly int BLOCK_CHAIN = 10;

    public static LevelGenerator Instance;

    [SerializeField]
    private int numberOfLevels = 3;
    
    private float xDistance;


    public List<Tile> platforms;
    public List<Tile> hazards;
    //object pool of type tiles
    private List<Tile> tiles;
    private List<Tile> activeTiles;
    private PlayerMovement player;

    private int currentLevel = 0;
    private int previousLevel;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        player = FindObjectOfType<PlayerMovement>();

        //platforms = new List<Tile>();

        tiles = new List<Tile>();
        activeTiles = new List<Tile>();

        previousLevel = currentLevel;

        //Spawn a set of 10 blocks at the start of the game
        InitialBlockSpawn();

        GameManager.Instance.ResetGameEvent += ResetLevel;

    }
	
	// Update is called once per frame
	void Update () {
        if(Mathf.Abs(player.transform.position.x - xDistance) < MAX_BLOCK_LENGTH)
            SpawnBlock();

        DeactivateTiles();
	}

    void InitialBlockSpawn()
    {
        int BLOCK_CHAIN = 7;
        for (int i = 0; i < BLOCK_CHAIN; i++)
        {
            Tile t = GetTile(0, true);

            t.transform.position = new Vector3(xDistance, currentLevel * 7, 0);
            t.transform.parent = this.transform;
            xDistance += t.GetLength();
            activeTiles.Insert(0, t);
            //Debug.Log("Tile is: " + t);
            t.Activate();

            //instantiate random hazard
            if (i != 0 && i != BLOCK_CHAIN - 1 && xDistance > 75)
            {
                if (Random.Range(0f, 1f) > 0.8f)
                {
                    Tile h = GetHazard(true);
                    h.transform.position = new Vector3(xDistance, currentLevel * 7, 0);
                    h.transform.parent = this.transform;
                    activeTiles.Insert(0, t);
                    //Debug.Log("Tile is: " + t);
                    t.Activate();
                }
            }
        }
    }

    void SpawnBlock()
    {
        //Get the block instantiation chain
        int chain = Random.Range(1, BLOCK_CHAIN);
        //Debug.Log("Chain: " + chain);
        for(int i = 0; i < chain; i++)
        {
            Tile t = GetTile(0, true);

            t.transform.position = new Vector3(xDistance, currentLevel * 7, 0);
            t.transform.parent = this.transform;
            xDistance += t.GetLength();
            activeTiles.Insert(0,t);
            //Debug.Log("Tile is: " + t);
            t.Activate();

            //instantiate random hazard
            if (i != 0 && i != chain-1 && xDistance > 75)
            {
                if (Random.Range(0f, 1f) > 0.8f)
                {
                    Tile h = GetHazard(true);
                    h.transform.position = new Vector3(xDistance, currentLevel * 7, 0);
                    h.transform.parent = this.transform;
                    activeTiles.Insert(0, h);
                    Debug.Log("Hazard index is: " + h.Id);
                    h.Activate();
                }
            }
        }

        //Generate a gap after a series of blocks
        float randomGapVariable = Random.Range(0f, 1f);

        if(randomGapVariable < 0.3f)
        {
            //Generate a gap of a random length
            int gapLength = Random.Range(5, 12);
            xDistance += gapLength;
        }

        //Change the path level with a certain random value
        if(Random.Range(0f, 1f) < 0.15f)
        {
            if(currentLevel == 0)
            {
                currentLevel++;
            }
            else if(currentLevel == numberOfLevels - 1)
            {
                currentLevel--;
            }
            else
            {
                Debug.Log("changing current level");
                currentLevel = Random.Range(0f, 1f) > 0.5f ? currentLevel+1 : currentLevel-1;
            }

        }
            
    }

    Tile GetTile(int id, bool platform)
    {
        //Debug.Log("Inside the get tile");
        Tile t = null;
        t = tiles.Find(x => x.Id == id && x.Platform == platform && !x.gameObject.activeSelf);

        if(t == null)
        {
            GameObject g = Instantiate(platforms[id].gameObject);

            t = g.GetComponent<Tile>();
            t.Platform = platform;
            t.Id = id;
            tiles.Add(t);
            tiles.Insert(0, t);
            //Debug.Log("Tile is.......: " + t);
        }
        return t;
    }
    Tile GetHazard(bool hazard)
    {
        Tile t = null;
        int id = Random.Range(0, hazards.Count);
        t = tiles.Find(x => x.Hazard == hazard && x.Id == id && !x.gameObject.activeSelf);
        if(t == null)
        {
            
            GameObject g = Instantiate(hazards[id].gameObject);

            t = g.GetComponent<Tile>();
            t.Hazard = hazard;
            t.Id = id;
            tiles.Add(t);
            tiles.Insert(0, t);
            //Debug.Log("Tile is.......: " + t);
        }
        return t;
    }

    void DeactivateTiles()
    {
        for(int i = 0; i < activeTiles.Count; i++)
        {
            if (activeTiles[i].transform.position.x - player.transform.position.x < -TILE_DEACTIVATION_DISTANCE)
            {
                activeTiles[i].gameObject.SetActive(false);
                activeTiles.RemoveAt(i);
            }

        }
    }

    void ResetLevel()
    {
        xDistance = 0;
        currentLevel = 0;

        //deactivate all the spawned tiles
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].gameObject.SetActive(false);
        }
        //clear the active tiles
        activeTiles.Clear();


        //Spawn a set of 10 blocks at the start of the game
        InitialBlockSpawn();
    }

}
