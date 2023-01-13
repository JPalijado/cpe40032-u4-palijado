using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameObject focalPoint;
    public GameObject powerupIndicator;

    public float speed = 5.0f;
    public bool hasPowerup = false;
    public bool gameOver = false;

    private Rigidbody playerRb;
    private float powerUpStrength = 15.0f;

    public PowerUpType currentPowerUp = PowerUpType.None;
    public GameObject rocketPrefab;
    private GameObject tmpRocket;
    private Coroutine powerupCountdown;

    // Variables for smash powerup
    public float hangTime;
    public float smashSpeed;
    public float explosionForce;
    public float explosionRadius;
    bool smashing = false;
    float floorY;

    // Variables for sound effects
    public AudioClip powerupSound;
    public AudioClip unpowerupSound;
    public AudioClip gameOverSound;
    public AudioClip rocketSound;
    public AudioClip jumpSound;
    public AudioClip smashSound;
    private AudioSource playerAudio;

    void Start()
    {
        // Gets components and finds focal point gameobject
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
        playerAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Moves the player forward or backward depending on the rotation of the camera
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput);

        // Spawns the powerup indicator on the player position
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);

        // Tells the player that the game is over
        if (transform.position.y < -1)
        {
            gameOver = true;
            Debug.Log("Game Over!");
            playerAudio.PlayOneShot(gameOverSound, 1.0f);
        }

        // Launch the rockets by pressing F key
        if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F))
        {
            LaunchRockets();
            playerAudio.PlayOneShot(rocketSound, 1.0f);
        }

        // Smash nearby enemies by pressing spacebar
        if (currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing)
        {
            smashing = true;
            StartCoroutine(Smash());
            playerAudio.PlayOneShot(jumpSound, 1.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks if the player has picked up a powerup
        if (other.CompareTag("Powerup"))
        {
            // Activates the powerup for a short time
            hasPowerup = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            powerupIndicator.gameObject.SetActive(true);
            Destroy(other.gameObject);
            playerAudio.PlayOneShot(powerupSound, 3.0f);

            if (powerupCountdown != null)
            {
                StopCoroutine(powerupCountdown);
            }
            powerupCountdown = StartCoroutine(PowerupCountdownRoutine());
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        // Serves as the countdown of the powerup
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        // Returns the currentPowerUp to none
        currentPowerUp = PowerUpType.None;
        powerupIndicator.gameObject.SetActive(false);
        playerAudio.PlayOneShot(unpowerupSound, 8.0f);
    }
    
    // Smash powerup
    IEnumerator Smash()
    {
        var enemies = FindObjectsOfType<Enemy>();

        // Stores the y position before taking off
        floorY = transform.position.y;

        // Calculates the amount of time we will go up
        float jumpTime = Time.time + hangTime;

        while (Time.time < jumpTime)
        {
            // Move the player up while still keeping their x velocity.
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }

        // Now move the player down
        while (transform.position.y > floorY)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * 2);
            yield return null;
        }

        // Cycle through all enemies
        for (int i = 0; i < enemies.Length; i++)
        {
            // Apply an explosion force that originates from our position.
            if (enemies[i] != null)
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
                playerAudio.PlayOneShot(smashSound, 4.0f);
        }
        // We are no longer smashing, so set the boolean to false
        smashing = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Checks if the powerup is Pushback
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp == PowerUpType.Pushback)
        {
            // Suddenly pushes the enemy away from the player, if the player has the powerup
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);
            enemyRigidbody.AddForce(awayFromPlayer * powerUpStrength, ForceMode.Impulse);
        }
    }

    // Launches the missiles at each enemy
    void LaunchRockets()
    {
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            tmpRocket.GetComponent<RocketBehaviour>().Fire(enemy.transform);
        }
    }
}
