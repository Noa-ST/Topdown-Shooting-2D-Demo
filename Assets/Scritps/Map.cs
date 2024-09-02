using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Transform playerSpawnPoint;
    [SerializeField] private Transform[] m_aiSpawnPoints;

    public Transform RandomAISpawnPoint
    {
        get
        {
            if (m_aiSpawnPoints == null || m_aiSpawnPoints.Length <= 0) return null;

            int randomIdx = Random.Range(0, m_aiSpawnPoints.Length);
            return m_aiSpawnPoints[randomIdx];
        }
    }
}
