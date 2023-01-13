using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject[] powerupPrefabs;
    public int enemyCount;
    public int waveNumber = 1;

    private float spawnRange = 9;
    private PlayerController playerControllerScript;
    
    private AudioSource playerAudio;
    public AudioClip levelUpSound;

    public GameObject bossPrefab;
    public GameObject[] miniEnemyPrefabs;
    public int bossRound;

    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        playerAudio = GameObject.Find("Player").GetComponent<AudioSource>();

        // Spawns the first wave
        SpawnEnemyWave(waveNumber);
        Debug.Log("Level: " + waveNumber);

        // Spawns the first power up
        SpawnRandomPowerUp();
    }

    void Update()
    {
        // Counts the enemies on the platform
        enemyCount = FindObjectsOfType<Enemy>().Length;

        if (enemyCount == 0 && playerControllerScript.gameOver == false)
        {
            // Increments the number of enemies each wave by 1
            waveNumber++;
            // Checks if its boss round
            if (waveNumber % bossRound == 0)
            {
                SpawnBossWave(waveNumber);
                Debug.Log("Level: " + waveNumber + " - Boss Wave");
            }
            else
            {
                SpawnEnemyWave(waveNumber);
                Debug.Log("Level: " + waveNumber);
            }

            // Spawns a single power up
            SpawnRandomPowerUp();

            // Plays the level up sound effect
            playerAudio.PlayOneShot(levelUpSound, 10.0f);
        }
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        // Spawns enemies as long as it is less than the number of enemies needed to spawn
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int randomEnemy = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[randomEnemy], GenerateSpawnPosition(), enemyPrefabs[randomEnemy].transform.rotation);
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        // Generates random positions
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);

        Vector3 randomPos = new Vector3(spawnPosX, 0, spawnPosZ);
        return randomPos;
    }

    // Spawns a random powerup to the platform
    void SpawnRandomPowerUp()
    {
        int randomPowerup = Random.Range(0, powerupPrefabs.Length);
        Instantiate(powerupPrefabs[randomPowerup], GenerateSpawnPosition(), powerupPrefabs[randomPowerup].transform.rotation);
    }

    // Spawns the boss wave
    void SpawnBossWave(int currentRound)
    {
        int miniEnemysToSpawn;
        // Check if it is boss round
        if (bossRound != 0)
        {
            // Set the amount of mini enemies to spawn
            miniEnemysToSpawn = currentRound / bossRound;
        }
        else
        {
            miniEnemysToSpawn = 1;
        }
        var boss = Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation);
        boss.GetComponent<Enemy>().miniEnemySpawnCount = miniEnemysToSpawn;
    }

    public void SpawnMiniEnemy(int amount)
    {
        // Spawns mini enemies
        for (int i = 0; i < amount; i++)
        {
            int randomMini = Random.Range(0, miniEnemyPrefabs.Length);
            Instantiate(miniEnemyPrefabs[randomMini], GenerateSpawnPosition(), miniEnemyPrefabs[randomMini].transform.rotation);
        }
    }

}
