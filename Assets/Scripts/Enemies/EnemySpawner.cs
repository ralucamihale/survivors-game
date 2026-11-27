using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelSpawnConfig
{
    public int minLevel = 1;
    public EnemyDataSO[] enemies;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyFactory factory;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private List<LevelSpawnConfig> levelConfigs;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private int maxActiveEnemies = 25;
    [SerializeField] private float spawnOffsetOutsideCamera = 2f;

    [Header("Difficulty Scaling per Level")]
    [SerializeField] private float hpScalePerLevel = 0.15f;
    [SerializeField] private float damageScalePerLevel = 0.10f;
    [SerializeField] private float speedScalePerLevel = 0.05f;
    [SerializeField] private float maxSpeedMultiplier = 1.6f;

    private Camera mainCam;
    private float spawnTimer;
    private int currentLevel = 1;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void OnEnable()
    {
        EventBus.Instance.Subscribe<PlayerLevelUpEvent>(OnLevelUp);
    }

    private void OnDisable()
    {
        EventBus.Instance.Unsubscribe<PlayerLevelUpEvent>(OnLevelUp);
    }

    private void OnLevelUp(PlayerLevelUpEvent e)
    {
        currentLevel = e.NewLevel;
    }

    private void Update()
    {
        if (playerTransform == null || levelConfigs.Count == 0) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f && factory.ActiveEnemyCount < maxActiveEnemies)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        EnemyDataSO[] poolForLevel = GetEnemiesForCurrentLevel();
        if (poolForLevel == null || poolForLevel.Length == 0) return;

        EnemyDataSO chosen = poolForLevel[UnityEngine.Random.Range(0, poolForLevel.Length)];
        Vector3 spawnPos = GetRandomPositionOutsideCamera();

        int bonusLevels = Mathf.Max(0, currentLevel - 1);
        float hpMult = 1f + bonusLevels * hpScalePerLevel;
        float dmgMult = 1f + bonusLevels * damageScalePerLevel;
        float speedMult = Mathf.Min(maxSpeedMultiplier, 1f + bonusLevels * speedScalePerLevel);

        factory.Create(chosen, spawnPos, playerTransform, hpMult, dmgMult, speedMult);
    }

    private EnemyDataSO[] GetEnemiesForCurrentLevel()
    {
        EnemyDataSO[] result = levelConfigs[0].enemies;
        foreach (var cfg in levelConfigs)
        {
            if (currentLevel >= cfg.minLevel)
                result = cfg.enemies;
        }
        return result;
    }

    private Vector3 GetRandomPositionOutsideCamera()
    {
        float camHeight = mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;
        Vector3 camPos = mainCam.transform.position;

        int side = UnityEngine.Random.Range(0, 4);
        float x = 0f, y = 0f;

        switch (side)
        {
            case 0: x = UnityEngine.Random.Range(-camWidth, camWidth); y = camHeight + spawnOffsetOutsideCamera; break;
            case 1: x = UnityEngine.Random.Range(-camWidth, camWidth); y = -camHeight - spawnOffsetOutsideCamera; break;
            case 2: x = -camWidth - spawnOffsetOutsideCamera; y = UnityEngine.Random.Range(-camHeight, camHeight); break;
            case 3: x = camWidth + spawnOffsetOutsideCamera; y = UnityEngine.Random.Range(-camHeight, camHeight); break;
        }

        return new Vector3(camPos.x + x, camPos.y + y, 0f);
    }
}