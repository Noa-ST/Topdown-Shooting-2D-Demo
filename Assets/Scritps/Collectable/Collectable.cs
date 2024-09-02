using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private int m_minBonus;
    [SerializeField] private int m_maxBonus;
    [SerializeField] private int m_lifeTime;
    [SerializeField] private int m_spawnForce;
    private int m_lifeTimeCounting;

    private Rigidbody2D m_rb;
    private FlashVfx m_flashVfx;

    protected int m_bonus;
    protected Player m_player;

    private void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_flashVfx = GetComponent<FlashVfx>();
    }

    private void Start()
    {
        m_lifeTimeCounting = m_lifeTime;
        m_player = GameManager.Ins.Player;
        m_bonus = Random.Range(m_minBonus, m_maxBonus) * m_player.PlayerStats.level;

        Init();
        Explode();
        FlashVfxCompleted();
        StartCoroutine(DisappearNoticeCountDown());
    }

    private IEnumerator DisappearNoticeCountDown()
    {
        while(m_lifeTimeCounting > 0)
        {
            float timeLifeleftRate = Mathf.Round((float)m_lifeTimeCounting / m_lifeTime);
            yield return new WaitForSeconds(1f);
            m_lifeTimeCounting--;
            if(timeLifeleftRate <= 0.3f && m_flashVfx != null)
            {
                m_flashVfx.Flash(m_lifeTimeCounting);
            }
        }
    }

    private void FlashVfxCompleted()
    {
        if (m_flashVfx == null) return;

        m_flashVfx.OnCompleted.RemoveAllListeners();
        m_flashVfx.OnCompleted.AddListener(OnDestroyCollectable);
    }

    private void OnDestroyCollectable()
    {
        Destroy(gameObject);
    }


    private void Explode()
    {
        if (m_rb == null) return;

        float randomForceX = Random.Range(-m_spawnForce, m_spawnForce);
        float randomForceY = Random.Range(-m_spawnForce, m_spawnForce);
        m_rb.velocity = new Vector2(randomForceX, randomForceY) * Time.deltaTime;
        StartCoroutine(StopMoving());

    }

    private IEnumerator StopMoving()
    {
        yield return new WaitForSeconds(0.8f);
        if(m_rb != null)
        {
            m_rb.velocity = Vector2.zero;
        }
    }

    public virtual void Init()
    {

    }

    public virtual void Trigger()
    {

    }


}
