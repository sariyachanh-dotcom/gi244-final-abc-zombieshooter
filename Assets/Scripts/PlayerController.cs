using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public int health = 5;
    public float walkSpeed = 5.0f;
    public float bulletSpeed = 10f;
    public float fireRate = 1f;
    public TextMeshProUGUI healthText;
    public GameObject bulletPrefab;
    public GameObject chargebulletPrefab;
    public float chargeTime = 2f;
    public float chargeBulletSpeed = 20f;

    private float currentCharge;
    private bool isCharging;

    public Camera mainCamera;

    private float immunityFrame = 0.5f;
    private float nextFireTime;
    private float nextDamageTime;
    private float defaultSpeed;
    private Rigidbody rb;
    private Coroutine speedCoroutine;
    private Coroutine slowCoroutine;
    private InputAction moveAction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
        
        defaultSpeed = walkSpeed;

        healthText.text = "Health " + health;
    }

    void Update()
    {
        var moveInput = moveAction.ReadValue<Vector2>();
        var moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        if (rb.linearVelocity.magnitude < walkSpeed)
        {
            rb.AddForce(moveDirection * walkSpeed);
        }

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();

            nextFireTime = Time.time + fireRate;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            isCharging = true;
            currentCharge = 0f;
        }

        if (Input.GetButton("Fire2"))
        {
            currentCharge += Time.deltaTime;
        }

        if (Input.GetButtonUp("Fire2"))
        {
            isCharging = false;

            if (currentCharge >= chargeTime)
            {
                ShootChargeBullet();
            }
        }


    }

    private void Shoot()
    {
        Vector3 mousePos = Input.mousePosition;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

        Vector3 dir = (mousePos - screenPos).normalized;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = new Vector3(dir.x, 0, dir.y) * bulletSpeed;
    }
    private void ShootChargeBullet()
    {
        Vector3 mousePos = Input.mousePosition;

        Vector3 sreenPos = mainCamera.WorldToScreenPoint(transform.position);

        Vector3 dir = (mousePos -  sreenPos).normalized;

        GameObject bullet = Instantiate(
            chargebulletPrefab,transform.position, Quaternion.identity);

        Rigidbody rb = bullet.GetComponent <Rigidbody>();

        rb.linearVelocity = new Vector3(dir.x, 0, dir.y) * chargeBulletSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            if (speedCoroutine != null)
            {
                StopCoroutine(speedCoroutine);
            }
            speedCoroutine = StartCoroutine(SpeedBuff());

            Destroy(other.gameObject);
        }

        if (other.CompareTag("EnemyDebuff"))
        {
            if (slowCoroutine != null)
            {
                StopCoroutine(slowCoroutine);
            }
            slowCoroutine = StartCoroutine(EnemySlowDown());

            Destroy(other.gameObject);
        }

        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    private IEnumerator SpeedBuff()
    {
        walkSpeed = defaultSpeed * 1.5f;

        yield return new WaitForSeconds(5f);

        walkSpeed = defaultSpeed;
    }

    private IEnumerator EnemySlowDown()
    {
        Enemy.isSlowDown = true;

        yield return new WaitForSeconds(7.5f);

        Enemy.isSlowDown = false;
    }

    private void TakeDamage(int damage)
    {
        if (Time.time >= nextDamageTime)
        {
            if (health > 0)
            {
                health -= damage;
                healthText.text = "Health " + health;

                nextDamageTime = Time.time + immunityFrame;
            }
        }
    }
}