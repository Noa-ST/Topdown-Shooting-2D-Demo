using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum GameState
{
    STARTING,
    PLAYING,
    PAUSED,
    GAMEOVER
}

public class GameManager : Singleton<GameManager>
{
    public static GameState state;

    [SerializeField] private Map m_mapPb;
    [SerializeField] private Player m_playerPb;
    [SerializeField] private Enemy[] m_enemyPb;
    [SerializeField] private GameObject m_aiSpawnVfx;

    [SerializeField] private float m_enemySpawnTime;
    [SerializeField] private int m_playerMaxLife;
    [SerializeField] private int m_playerStartingLife;

    private Map m_map;
    private Player m_player;
    private int m_curLife;
    private PlayerStats m_playerStats;

    public Player Player { get => m_player; private set => m_player = value; }
    public int CurLife
    {
        get => m_curLife;
        set
        {
            m_curLife = value;
            m_curLife = Mathf.Clamp(m_curLife, 0, m_playerMaxLife);
        }
    }

    protected override void Awake()
    {
        MakeSingleton(false);
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        state = GameState.STARTING;
        m_curLife = m_playerStartingLife;
        SpawnMap_Player();

        GUIManager.Ins.ShowGameGui(false);
    }

    private void SpawnMap_Player()
    {
        if (m_mapPb == null || m_playerPb == null) return;

        m_map = Instantiate(m_mapPb, Vector3.zero, Quaternion.identity);
        m_player = Instantiate(m_playerPb, m_map.playerSpawnPoint.position, Quaternion.identity);
    }

    public void PlayGame()
    {
        state = GameState.PLAYING;
        m_playerStats = m_player.PlayerStats;

        SpawnEnemy();

        if (m_player == null || m_playerStats == null) return;

        GUIManager.Ins.ShowGameGui(true);
        GUIManager.Ins.UpdateLifeInfo(m_curLife);
        GUIManager.Ins.UpdateCoinsCounting(Pref.coins);
        GUIManager.Ins.UpdateHpInfo(m_player.CurHp, m_playerStats.hp);
        GUIManager.Ins.UpdateLevelInfo(m_playerStats.level, m_playerStats.xp, m_playerStats.levelUpXpRequied);
    }

    private void SpawnEnemy()
    {
        var randomEnemy = GetRandomEnemy();
        if (randomEnemy == null || m_map == null) return;
        StartCoroutine(SpawnEnemy_coroutine(randomEnemy));
    }

    private Enemy GetRandomEnemy()
    {
        if (m_enemyPb == null || m_enemyPb.Length <= 0) return null;
        int randomIdx = UnityEngine.Random.Range(0, m_enemyPb.Length);
        return m_enemyPb[randomIdx];
    }

    private IEnumerator SpawnEnemy_coroutine(Enemy randomEnemy)
    {
        yield return new WaitForSeconds(3f);

        while (state == GameState.PLAYING)
        {
            if (m_map.RandomAISpawnPoint == null) break;

            Vector3 spawnPoint = m_map.RandomAISpawnPoint.position;
            if (m_aiSpawnVfx)
                Instantiate(m_aiSpawnVfx, spawnPoint, Quaternion.identity);

            yield return new WaitForSeconds(0.2f);
            Instantiate(randomEnemy, spawnPoint, Quaternion.identity);
            yield return new WaitForSeconds(m_enemySpawnTime);
        }
        yield return null;
    }

    public void GameoverChecking(Action OnLostLife = null, Action OnDead = null)
    {
        if (m_curLife <= 0) return;

        m_curLife--;
        OnLostLife?.Invoke();

        if (m_curLife <= 0)
        {
            state = GameState.GAMEOVER;
            OnDead?.Invoke();
        }
    }
}