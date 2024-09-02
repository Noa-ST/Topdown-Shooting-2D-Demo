using System;
using System.Collections;
using System.Collections.Generic;
using UDEV;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Actor : MonoBehaviour
{
    [Header("Common")]
    public ActionStats statData; // Dữ liệu thông số hành động của đối tượng (chứa các thông số như thời gian bất khả xâm phạm, thời gian knockback, v.v.)

    [LayerList]
    [SerializeField] private int m_invincibleLayer; // Layer cho trạng thái bất khả xâm phạm
    [LayerList]
    [SerializeField] private int m_normalLayer; // Layer cho trạng thái bình thường

    public Weapon weapon; // Vũ khí hiện tại của đối tượng

    protected bool m_isKnockback; // Trạng thái đối tượng có đang bị knockback hay không
    protected bool m_isInvincible; // Trạng thái đối tượng có đang bất khả xâm phạm hay không
    private bool m_isDead; // Trạng thái đối tượng đã chết hay chưa
    private float m_curHp; // Số lượng máu hiện tại của đối tượng

    protected Rigidbody2D m_rb; // Thành phần Rigidbody2D của đối tượng
    protected Animator m_amin; // Thành phần Animator của đối tượng
    protected Coroutine m_stopKnockbackCo;
    protected Coroutine m_invincibleCo;


    [Header("Event:")]
    public UnityEvent OnInit; // Sự kiện khi đối tượng được khởi tạo
    public UnityEvent OnTakeDamage; // Sự kiện khi đối tượng bị nhận sát thương
    public UnityEvent OnDead; // Sự kiện khi đối tượng chết

    // Thuộc tính kiểm tra và thiết lập trạng thái chết của đối tượng
    public bool IsDead { get => m_isDead; set => m_isDead = value; }
    // Thuộc tính kiểm tra và thiết lập lượng máu hiện tại của đối tượng
    public float CurHp
    {
        get => m_curHp;
        set => m_curHp = value;
    }

    // Phương thức Awake: Khởi tạo các thành phần của đối tượng
    protected virtual void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>(); // Gán thành phần Rigidbody2D của đối tượng
        m_amin = GetComponentInChildren<Animator>(); // Gán thành phần Animator từ con của đối tượng
    }

    // Phương thức Start: Khởi tạo đối tượng và kích hoạt sự kiện OnInit
    protected virtual void Start()
    {
        Init(); // Gọi phương thức Init để khởi tạo các giá trị ban đầu
        OnInit?.Invoke(); // Kích hoạt sự kiện OnInit nếu có người đăng ký
    }

    // Phương thức Init: Được sử dụng để khởi tạo hoặc tái khởi tạo đối tượng (có thể được ghi đè)
    public virtual void Init()
    {
        // Mặc định không làm gì, các lớp con có thể ghi đè để bổ sung logic khởi tạo
    }

    // Phương thức Takedamage: Xử lý khi đối tượng nhận sát thương
    public virtual void Takedamage(float damage)
    {
        if (damage < 0 || m_isInvincible) return; // Không xử lý nếu sát thương là số âm hoặc đối tượng đang ở trạng thái bất khả xâm phạm

        m_curHp -= damage; // Trừ sát thương khỏi máu hiện tại
        Knockback(); // Gọi phương thức Knockback để xử lý hiệu ứng knockback
        if (m_curHp <= 0) // Nếu máu <= 0, đối tượng chết
        {
            m_curHp = 0;
            Die(); // Gọi phương thức Die để xử lý khi đối tượng chết
        }

        OnTakeDamage?.Invoke(); // Kích hoạt sự kiện OnTakeDamage nếu có người đăng ký
    }

    // Phương thức Die: Xử lý khi đối tượng chết
    protected virtual void Die()
    {
        m_isDead = true; // Đánh dấu đối tượng đã chết
        m_rb.velocity = Vector3.zero; // Dừng mọi chuyển động của đối tượng

        OnDead?.Invoke(); // Kích hoạt sự kiện OnDead nếu có người đăng ký

        Destroy(gameObject, 0.5f); // Phá hủy đối tượng sau 0.5 giây
    }

    // Phương thức Knockback: Xử lý hiệu ứng knockback của đối tượng
    protected void Knockback()
    {
        if (m_isInvincible || m_isKnockback || m_isDead) return; // Không xử lý nếu đối tượng bất khả xâm phạm, đang bị knockback hoặc đã chết

        m_isKnockback = true; // Đánh dấu trạng thái đối tượng đang bị knockback

        m_stopKnockbackCo = StartCoroutine(StopKnockback()); // Bắt đầu coroutine để dừng hiệu ứng knockback sau một khoảng thời gian
    }

    protected void Invincible(float invincibleTime)
    {
        m_isKnockback = false;
        m_isInvincible = true;
        gameObject.layer = m_invincibleLayer;

        m_invincibleCo = StartCoroutine(StopInviciable(invincibleTime));
    }

    // Coroutine StopKnockback: Dừng hiệu ứng knockback sau thời gian định sẵn
    private IEnumerator StopKnockback()
    {
        yield return new WaitForSeconds(statData.knockbackTime); // Đợi trong thời gian knockback

        Invincible(statData.invincibleTime);
    }

    // Coroutine StopInviciable: Dừng trạng thái bất khả xâm phạm sau thời gian định sẵn
    private IEnumerator StopInviciable(float invincibleTime)
    {
        yield return new WaitForSeconds(invincibleTime); // Đợi trong thời gian bất khả xâm phạm

        m_isInvincible = false; // Kết thúc trạng thái bất khả xâm phạm

        gameObject.layer = m_normalLayer; // Chuyển đối tượng trở lại layer bình thường
    }

    // Phương thức Move: Được sử dụng để xử lý chuyển động của đối tượng (có thể được ghi đè)
    protected virtual void Move()
    {
        // Mặc định không làm gì, các lớp con có thể ghi đè để bổ sung logic chuyển động
    }
}




/*thuộc tính[RequireComponent(typeof(Rigidbody2D))] trong Unity được sử dụng để đảm bảo rằng một thành phần Rigidbody2D được gắn vào cùng một GameObject với script đang sử dụng thuộc tính này. Khi bạn thêm một script có thuộc tính này vào một GameObject, Unity sẽ tự động thêm thành phần Rigidbody2D nếu nó chưa có*/