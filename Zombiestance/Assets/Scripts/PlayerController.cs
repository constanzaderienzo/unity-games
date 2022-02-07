using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public CinemachineVirtualCamera cmCamera;
    public CinemachineVirtualCamera drone;
    public PlayableDirector playable;

    private Animator _animator;
    private CharacterController _controller;
    public PlayerShoot playerShoot;
    public GameObject weaponHolster;
    public WeaponSwitching weaponSwitch;
    public HealthBar healthBar;
    public DamageVisualizer damageVisualizer;
    public AudioClip footstepOnSandClip;
    private AudioSource audioSource;
    
    private float _speed;
    public float health;
    public float mouseSensitivity = 100f;
    public bool isDead;
    private bool shooting;
    private Quaternion _cameraRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        playerShoot = GetComponent<PlayerShoot>();
        weaponSwitch = weaponHolster.GetComponent<WeaponSwitching>();
        _speed = 15f;
        health = 100f;
        healthBar.SetMaxHealth(health);
        isDead = false;
        _animator.SetBool("Death_b", isDead);
        audioSource = GetComponent<AudioSource>();
        shooting = false;
        if (cmCamera != null)
        {
            _cameraRotation = cmCamera.transform.rotation;
        }
    }
    
    void FixedUpdate()
    {
        if(!GameManager.instance.playersTurn)
        {
            return;
        }
        if (!isDead)
        {
            MovePlayer();
        }
    }

    private void Update()
    {
        if(!GameManager.instance.playersTurn)
        {
            return;
        }
        if (!isDead)
        {
            Shoot();
            CameraFollow();
            if(Input.GetKeyDown(KeyCode.P))
            {
                Cursor.lockState = CursorLockMode.None;
                GameManager.instance.Pause();
            }
        }
    }
    
    private void Shoot()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            _animator.SetBool("Shoot_b", true);
            // Debug.Log("Shoot");
            playerShoot.Shoot(weaponSwitch.GetCurrentWeaponMuzzleFlash());
            if (!shooting)
            {
                shooting = true;
                StartCoroutine(PlayShootSound());            
            }
        }
        else
        {
            _animator.SetBool("Shoot_b", false);

        }
    }

    public IEnumerator PlayShootSound()
    {
        audioSource.PlayOneShot(weaponSwitch.GetCurrentWeaponShootingClip());
        yield return new WaitForSeconds(0.05f);
        shooting = false;
    }

    private void MovePlayer()
    {
        Vector3 direction; 
        _animator.SetFloat("Speed_f", 0f);
        bool move = false;
        if (Input.GetKey(KeyCode.W))
        {
            direction = transform.forward;
            _controller.Move(direction * (_speed * Time.fixedDeltaTime));
            _animator.SetFloat("Speed_f", _speed);
            move = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction = -transform.forward;
            _controller.Move(direction * (_speed * Time.fixedDeltaTime)); 
            _animator.SetFloat("Speed_f", _speed);
            move = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction = transform.right;
            _controller.Move(direction * (_speed * Time.fixedDeltaTime)); 
            _animator.SetFloat("Speed_f", _speed);
            move = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction = -transform.right;
            _controller.Move(direction * (_speed * Time.fixedDeltaTime)); 
            _animator.SetFloat("Speed_f", _speed);
            move = true;

        }
        if(move && !audioSource.isPlaying)
        {
            audioSource.Play();
            audioSource.SetScheduledEndTime(AudioSettings.dspTime + 1.5f);
            audioSource.clip = footstepOnSandClip;
        }
    }

    private void CameraFollow()
    {
        float _yRot = Input.GetAxis("Mouse X");
         
        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * mouseSensitivity;

        transform.Rotate(_rotation);

        float _xRot = Input.GetAxis("Mouse Y") * mouseSensitivity;
        _xRot = -_xRot;

        _cameraRotation *= Quaternion.Euler(_xRot, 0f, 0f);
        _cameraRotation = LockCameraMovement(_cameraRotation);

        if (cmCamera != null)
        {
            cmCamera.transform.localRotation = _cameraRotation;
            //camera.transform.Rotate(_cameraRotation);
        }
    }

    private Quaternion LockCameraMovement(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;
 
        var angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
 
        angleX = Mathf.Clamp(angleX, -90f, 90f);
 
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
 
        return q;
    }
    
    public void TakeDamage(float damage)
    {

        if (!isDead)
        {
            health -= damage;
            healthBar.SetHealth(health);

            if (health <= 0f)
                Die();
        }
    }

    public void TakeDamage(float damage, Vector3 from)
    {
        if (!isDead)
        {
            health -= damage;
            healthBar.SetHealth(health);
        
            float angle = Vector3.SignedAngle(transform.forward, transform.position - from, Vector3.up);
            if (angle > 135f || angle < -135f)
            {
                damageVisualizer.DamageFront();
            }
            else if (angle > 45f)
            {
                damageVisualizer.DamageLeft();
            }
            else if (angle < -45f)
            {
                damageVisualizer.DamageRight();
            }
            else
            {
                damageVisualizer.DamageBack();
            }
        
            if (health <= 0f)
                Die();
        }
    }

    public void AddLife(float life)
    {
        float postLife = health + life;
        health = postLife > 100f ? 100f : postLife;
        healthBar.SetHealth(health);
    }

    private void Die()
    {
        if (!isDead)
        {
            StartCoroutine(DestroyPlayer());
        }
    }

    public IEnumerator DestroyPlayer() 
    {
        isDead = true;
        drone.Priority = 15;
        playable.Play();
        _animator.SetBool("Death_b", isDead);
        yield return new WaitForSeconds(3.0f);
        Cursor.lockState = CursorLockMode.None;
        GameManager.instance.GameOver();
        drone.Priority = 9;


    }

    private void WaveWon()
    {
        playerShoot.weapon.damage += 10f;
    }
}
