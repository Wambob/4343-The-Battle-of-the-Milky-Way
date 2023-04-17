using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject projectilePrefab, powerUpPrefab, explosionPrefab;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Camera cam;
    [SerializeField] private int projectilePoolSize, powerUpPoolSize, explosionPoolSize, maxEnemies;
    [SerializeField] private float projectileLifetime, distanceFromCamera, explosionScale;

    public enum Team { Player, Enemy };
    public enum EnemyType { FighterShip = 0, BomberShip = 1, MotherShip = 2, RuntShip = 3 };

    public static GameplayManager instance;
    public GameObject player, parent;
    public Transform lockOn;
    public int currentEnemies;

    private GameObject[] spawnedProjectiles, spawnedPowerUps, bounds;
    private ParticleSystem[] spawnedExplosions;
    private Projectile[] projectileScripts;
    private Transform north, south, east, west;
    private int projectilePointer, powerUpPointer, explosionPointer;

    #endregion

    private void Awake()
    {
        //Get the player
        player = GameObject.FindGameObjectWithTag("Player");

        //Singleton
        instance = this;

        //Set bounds
        bounds = new GameObject[4];
        for (int i = 0; i < bounds.Length; i += 1)
        {
            bounds[i] = new GameObject("Bound" + i);
            bounds[i].transform.parent = parent.transform;
        }

        bounds[0].transform.position = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2, cam.pixelHeight, distanceFromCamera));
        north = bounds[0].transform;
        bounds[1].transform.position = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2, 0, distanceFromCamera));
        south = bounds[1].transform;
        bounds[2].transform.position = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight / 2, distanceFromCamera));
        east = bounds[2].transform;
        bounds[3].transform.position = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight / 2, distanceFromCamera));
        west = bounds[3].transform;
    }

    private void Start()
    {
        //Instantiate pools
        spawnedProjectiles = new GameObject[projectilePoolSize];
        projectileScripts = new Projectile[projectilePoolSize];
        spawnedPowerUps = new GameObject[powerUpPoolSize];
        spawnedExplosions = new ParticleSystem[explosionPoolSize];

        //Populate pools
        for (int i  = 0; i < spawnedProjectiles.Length; i += 1)
        {
            spawnedProjectiles[i] = Instantiate(projectilePrefab);
            spawnedProjectiles[i].transform.parent = parent.transform;
            spawnedProjectiles[i].SetActive(false);

            projectileScripts[i] = spawnedProjectiles[i].GetComponent<Projectile>();
        }

        for (int i = 0; i < spawnedPowerUps.Length; i += 1)
        {
            spawnedPowerUps[i] = Instantiate(powerUpPrefab);
            spawnedPowerUps[i].transform.parent = parent.transform;
            spawnedPowerUps[i].SetActive(false);
        }

        for (int i = 0; i < spawnedExplosions.Length; i += 1)
        {
            spawnedExplosions[i] = Instantiate(explosionPrefab).GetComponent<ParticleSystem>();
            spawnedExplosions[i].transform.parent = parent.transform;
            spawnedExplosions[i].gameObject.transform.localScale = new Vector3(explosionScale, explosionScale, explosionScale);
            spawnedExplosions[i].Stop();
            spawnedExplosions[i].Clear();

            explosionPointer = NextPointer(explosionPointer, spawnedExplosions.Length);
        }
    }

    #region Public Functions

    public void CallForProjectile(Transform[] origins, Vector3[] directions, float speed, float damage, Projectile.MoveType moveType, Projectile.Damagetype damageType, Team team, Projectile.ProjectileObject objectType)
    {
        //Repeat for each projectile required
        for (int i = 0; i < origins.Length; i += 1)
        {
            spawnedProjectiles[projectilePointer].SetActive(true);
            spawnedProjectiles[projectilePointer].transform.position = origins[i].position;

            projectileScripts[projectilePointer].PrepProjectile(directions[i], speed, damage, moveType, damageType, team, objectType, projectileLifetime);

            projectilePointer = NextPointer(projectilePointer, spawnedProjectiles.Length);
        }
    }

    public void CallForPowerUp(Vector3 origin, Vector3 forward)
    {
        //Spawn powerup from pool
        spawnedPowerUps[powerUpPointer].SetActive(true);
        spawnedPowerUps[powerUpPointer].transform.position = origin;
        spawnedPowerUps[powerUpPointer].transform.forward = forward;

        powerUpPointer = NextPointer(powerUpPointer, spawnedPowerUps.Length);
    }

    public void CallForEnemy(Vector3 origin, Vector3 rotation, EnemyType enemytype)
    {
        //Only spawn enemies when enemy count is below maximum
        if (currentEnemies < maxEnemies)
        {
            Instantiate(enemyPrefabs[((int)enemytype)], origin, Quaternion.Euler(rotation)).transform.parent = parent.transform;
            currentEnemies += 1;
        }
    }

    public void CallForExplosion(Vector3 origin)
    {
        //Clear particles before playing explosion
        spawnedExplosions[explosionPointer].Stop();
        spawnedExplosions[explosionPointer].Clear();
        spawnedExplosions[explosionPointer].transform.position = origin;
        spawnedExplosions[explosionPointer].Play();
    }

    public void LevelFinished(bool win)
    {
        UI.instance.DisableInputs();
        StartCoroutine(Conclusion(win));
    }

    public Vector3 GetValidPosition(Vector3 objectPosition)
    {
        //Returns a position within bounds
        //Bounds are set at the edges of the screen in awake
        return new Vector3(Mathf.Clamp(objectPosition.x, west.localPosition.x, east.localPosition.x), north.localPosition.y, Mathf.Clamp(objectPosition.z, south.localPosition.z, north.localPosition.z));
    }

    public Vector3 GetRandomPosition()
    {
        //Returns a random position within bounds in the top half of the screen
        return new Vector3(Random.Range(west.localPosition.x, east.localPosition.x), north.localPosition.y, Random.Range(0, north.localPosition.z));
    }

    public int NextPointer(int value, int max)
    {
        //Loops pointer around if out of limits
        if (value + 1 >= max)
        {
            return 0;
        }

        return value + 1;
    }

    #endregion

    private IEnumerator Conclusion(bool win)
    {
        //Slow timescale momentarily then call UI function
        Time.timeScale = 0.1f;
        yield return new WaitForSeconds(0.2f);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        if (win)
        {
            UI.instance.LoadMenu(2);
        }
        else
        {
            UI.instance.LoadMenu(3);
        }
    }
}