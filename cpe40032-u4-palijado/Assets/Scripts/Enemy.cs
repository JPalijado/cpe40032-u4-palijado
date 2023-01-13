using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3.0f;
    public AudioClip hitSound;
    public AudioClip spawnSound;

    private PlayerController playerControllerScript;
    private AudioSource playerAudio;
    private Rigidbody enemyRb;
    private GameObject player;
    private float yBound = -10.0f;

    public bool isBoss = false;
    public float spawnInterval;
    private float nextSpawn;
    public int miniEnemySpawnCount;
    private SpawnManager spawnManager;

    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        playerAudio = player.GetComponent<AudioSource>();

        // Checks if the enemy is boss
        if (isBoss)
        {
            spawnManager = FindObjectOfType<SpawnManager>();
        }
    }

    void Update()
    {
        // The enemy will follow the player to knock it off
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        enemyRb.AddForce(lookDirection * speed);

        if (isBoss)
        {
            // The boss will spawn mini enemies if it's still alive 
            if (Time.time > nextSpawn)
            {
                nextSpawn = Time.time + spawnInterval;
                spawnManager.SpawnMiniEnemy(miniEnemySpawnCount);
                playerAudio.PlayOneShot(spawnSound, 3.0f);
            }
         }

        // Destroys the enemy if it was pushed off the platform
        if (transform.position.y < yBound)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Plays a sound effect if the enemies collided with other gameobjects
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Rocket"))
        {
            playerAudio.PlayOneShot(hitSound, 3.0f);
        }
    }
}
