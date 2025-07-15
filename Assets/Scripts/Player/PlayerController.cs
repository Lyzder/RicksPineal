using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    [Header("Sprite")]
    [SerializeField] private GameObject spriteObject;
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float initialJumpForce;
    [SerializeField] private float extraJumpForce;
    [SerializeField] private float bounceSpeed;
    [SerializeField] private LayerMask levelCollisionLayer;
    [SerializeField] private float maxJumpTime;
    private float jumpTimer;
    private short moveDirection;
    private Vector2 moveInput;
    [Header("Ground check")]
    [SerializeField] private Transform groundCheckLeft;
    [SerializeField] private Transform groundCheckCenter;
    [SerializeField] private Transform groundCheckRight;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float coyoteTime;
    private float coyoteTimer;
    private bool isGrounded;
    private bool jumpQueued;
    private bool jumped;
    [Header("Wall Slide")]
    [SerializeField] private float slowSlideSpeed;
    [SerializeField] private float fastSlideSpeed;
    [SerializeField] private float wallJumpSpeedHorizontal;
    [SerializeField] private float wallJumpSpeedVertical;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private float wallCheckPointOffset;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float wallRegrabCooldown;
    [SerializeField] private float wallJumpControlLockTime;
    private float wallJumpControlTimer = 0f;
    private bool isTouchingWall;
    private bool isWallSliding;
    private float regrabTimer;
    [Header("Sats")]
    [SerializeField] private short hp;
    [SerializeField] private short maxHp;
    public short ammo;
    [Header("Shooting")]
    [SerializeField] GameObject bulletPrefab;
    public Vector3 bulletSpawnOffset;
    [SerializeField] private float shotCooldown;
    private float shotCdTimer;
    [Header("Interactions")]
    [SerializeField] private float damageFrames;
    [SerializeField] private float iFrames;
    [SerializeField] private bool invincible;
    [SerializeField] private float damageRecoilHorizontal;
    [SerializeField] private float damageRecoilVertical;
    [SerializeField] private BoxCollider2D stompHitbox;
    [SerializeField] private float deadTime;
    private float deadTimer;
    private bool ignoreCollision;
    private float damageFramesTimer;
    [Header("Power ups")]
    [SerializeField] private bool hasWallSlide;
    [Header("Effects")]
    [SerializeField] GameObject jumpEffect;
    [SerializeField] GameObject shootEffect;
    [SerializeField] GameObject pickupEffect;
    public Vector3 jumpEffectOffset;
    public Vector3 shootEffectOffset;
    [Header("Sound Effects")]
    [SerializeField] private AudioClip jumpSfx;
    [SerializeField] private AudioClip stompSfx;
    [SerializeField] private AudioClip shootSfx;
    [SerializeField] private AudioClip hurtSfx;
    [SerializeField] private AudioClip noAmmoSfx;
    [SerializeField] private AudioClip reloadSfx;
    [SerializeField] private AudioClip deadSfx;
    // Input flags
    private bool isJump;
    // Components
    private Rigidbody2D rb;
    private BoxCollider2D collider2d;
    private InputSystem_Actions inputActions;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    // Player State
    public bool isAlive { get; private set; }
    public enum States : ushort
    {
        Default = 0,
        Damage = 1,
        Firing = 2,
        WallSlide = 3,
    }
    public States playerState { get; private set; }
    // Events
    public event Action<short> OnHealthChanged;
    public event Action<short> OnAmmoChanged;
    public event Action OnDeath;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        collider2d = GetComponent<BoxCollider2D>();
        moveDirection = 1;
        coyoteTimer = 0;
        jumpQueued = false;
        jumped = false;
        jumpTimer = 0;
        isWallSliding = false;
        isAlive = true;
        // Initialize Input Actions
        inputActions = InputManager.Instance.inputActions;
    }

    // Start is called before the first frame update
    void Start()
    {
        //GameManager.Instance.RegisterPlayer(this);
        OnHealthChanged?.Invoke(hp);
        OnAmmoChanged?.Invoke(ammo);
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.UI.Disable();
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Jump.performed += OnJumpPerformed;
        inputActions.Player.Jump.canceled += OnJumpCanceled;
        inputActions.Player.Attack.performed += OnAttackPerformed;
        //GameManager.Instance.OnWinning += DisableControls;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Jump.canceled -= OnJumpCanceled;
        inputActions.Player.Attack.performed -= OnAttackPerformed;
        //GameManager.Instance.OnWinning -= DisableControls;
    }

    // START UPDATE METHODS

    void Update()
    {
        //if (!isAlive)
        //    return;
        //if (!isGrounded)
        //    RunCoyoteTimer();
        //if (shotCdTimer > 0)
        //    AttackCooldown();
        if (regrabTimer > 0)
            WallRegrabTimer();
        if (wallJumpControlTimer > 0)
            WallJumpSpeedTimer();
        UpdateAnimator();
        UpdateDirection();
        AdjustSpritePosition();
    }

    private void FixedUpdate()
    {
        if (!isAlive)
            return;
        CheckWall();
        CheckGrounded();
        Move();
        ExtendJump();
        if (jumpQueued)
        {
            Jump();
        }
    }

    // END UPATE METHODS

    // START MOVEMENT METHODS

    private void Move()
    {
        Vector2 newVelocity = rb.velocity;

        // Damps player movement when jumping off a wall
        float controlPercent = 1f - Mathf.Pow(wallJumpControlTimer / wallJumpControlLockTime, 2f);

        float targetVelocityX = moveInput.x * moveSpeed;

        // During wall jump lockout, limit control
        if (wallJumpControlTimer > 0f)
        {
            newVelocity.x = Mathf.Lerp(wallJumpSpeedHorizontal * moveDirection, targetVelocityX, controlPercent);
        }
        else
        {
            // Full control
            newVelocity.x = targetVelocityX;
        }

        rb.velocity = newVelocity;

        wallCheckPoint.localPosition = new Vector3(0.4f * moveDirection + wallCheckPointOffset, 0);
    }

    private void Jump()
    {
        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(wallJumpSpeedHorizontal * moveDirection * -1, wallJumpSpeedVertical), ForceMode2D.Impulse);
            wallJumpControlTimer = wallJumpControlLockTime;
            isWallSliding = false;
            ChangeDirection();
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, initialJumpForce), ForceMode2D.Impulse);
            jumpTimer = maxJumpTime;
        }
        regrabTimer = wallRegrabCooldown;
        jumpQueued = false;
        jumped = true;
        //Instantiate(jumpEffect, groundCheckLeft.position + jumpEffectOffset, Quaternion.identity);
        //AudioManager.Instance.PlaySFX(jumpSfx);
    }

    private void ExtendJump()
    {
        if (isJump && jumpTimer > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, extraJumpForce);
            jumpTimer -= Time.fixedDeltaTime;
        }
        else
            jumpTimer = 0;
    }

    private void CheckGrounded()
    {
        bool hitLeft = Physics2D.Raycast(groundCheckLeft.position, Vector2.down, groundCheckDistance, levelCollisionLayer);
        bool hitCenter = Physics2D.Raycast(groundCheckCenter.position, Vector2.down, groundCheckDistance, levelCollisionLayer);
        bool hitRight = Physics2D.Raycast(groundCheckRight.position, Vector2.down, groundCheckDistance, levelCollisionLayer);
        isGrounded = hitLeft || hitRight || hitCenter;

        if (isGrounded && rb.velocity.y <= 0)
        {
            coyoteTimer = 0;
            jumped = false;
            jumpTimer = 0;
            //stompHitbox.enabled = false;
            //collider2d.size = new Vector2(0.8f, 0.89f);
        }
        else if (playerState != States.Damage)
        {
            //stompHitbox.enabled = true;
            //collider2d.size = new Vector2(0.8f, 0.65f);
        }
    }

    private void CheckWall()
    {
        if (regrabTimer > 0)
            return;

        Vector2 direction = moveDirection > 0 ? Vector2.right : Vector2.left;
        isTouchingWall = Physics2D.Raycast(wallCheckPoint.position, direction, wallCheckDistance, levelCollisionLayer);

        // Wall slide only when airborne, moving into wall, and touching wall
        // Player doesn't need to hold the direction to stay sliding
        if (!isWallSliding)
            isWallSliding = isTouchingWall && !isGrounded && moveInput.x != 0;
        else
            isWallSliding = isTouchingWall && !isGrounded;

        // Slow slide by default, fast if pressing down
        if (isWallSliding)
        {
            float slideSpeed = (moveInput.y < 0) ? fastSlideSpeed : slowSlideSpeed;
            rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
            jumpTimer = 0;
        }
    }

    // END MOVEMENT METHODS

    //private void Shoot()
    //{
    //    Bullet bullet;
    //    GameObject effect;
    //    // Spawn bullet
    //    Vector3 bulletOffset = new Vector3(bulletSpawnOffset.x * moveDirection, bulletSpawnOffset.y, bulletSpawnOffset.z);
    //    bullet = Instantiate(bulletPrefab, transform.position + bulletOffset, Quaternion.identity).GetComponent<Bullet>();
    //    bullet.SetDirection(moveDirection);
    //    shotCdTimer = shotCooldown;
    //    ammo -= 1;
    //    OnAmmoChanged?.Invoke(ammo);
    //    // Spawn Effect
    //    Vector3 effectOffset = new Vector3(shootEffectOffset.x * moveDirection, shootEffectOffset.y, shootEffectOffset.z);
    //    effect = Instantiate(shootEffect);
    //    effect.transform.SetParent(transform);
    //    effect.transform.localPosition = effectOffset;
    //    effect.transform.localRotation = Quaternion.identity;
    //    if (moveDirection > 0)
    //    {
    //        effect.GetComponent<ParticleSystem>().GetComponent<ParticleSystemRenderer>().flip = new Vector3(1f, 0);
    //    }
    //    else if (moveDirection < 0)
    //    {
    //        effect.GetComponent<ParticleSystem>().GetComponent<ParticleSystemRenderer>().flip = new Vector3(0f, 0);
    //    }
    //    AudioManager.Instance.PlaySFX(shootSfx);
    //}

    // START INPUT READING METHODS

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        if (playerState == States.Damage)
            return;
        moveInput = ctx.ReadValue<Vector2>();
        //if (moveInput.x > 0)
        //{
        //    moveDirection = 1;
        //    FlipSprite();
        //}
        //else if (moveInput.x < 0)
        //{
        //    moveDirection = -1;
        //    FlipSprite();
        //}
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector3.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        isJump = true;
        if (playerState == States.Damage)
            return;
        if (isGrounded || isWallSliding)
        { //|| (coyoteTimer < coyoteTime && !jumped) 
            jumpQueued = true;
        }
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        isJump = false;
    }

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        if (playerState == States.Damage)
            return;
        if (shotCdTimer > 0)
            return;
        if (ammo <= 0)
        {
            //AudioManager.Instance.PlaySFX(noAmmoSfx);
            return;
        }
        //Shoot();
    }

    // END INPUT READING METHODS

    void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.CompareTag("Solid"))
        //{
        //    ContactPoint2D contact = collision.GetContact(0);
        //    Vector2 normal = contact.normal;

        //    if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
        //    {
        //        ChangeDirection();
        //        TemporarilyIgnoreCollision(0.1f);
        //    }
        //}
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //TODO
            TakeDamage();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (ignoreCollision)
            return;
        //if (collision.gameObject.CompareTag("Solid"))
        //{
        //    ContactPoint2D[] contacts = collision.contacts;
        //    Vector2 normal;
        //    foreach (ContactPoint2D contactPoint in contacts)
        //    {
        //        normal = contactPoint.normal;

        //        if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
        //        {
        //            ChangeDirection();
        //            TemporarilyIgnoreCollision(0.1f);
        //            return;
        //        }
        //    }
        //}
    }

    public void TemporarilyIgnoreCollision(float duration)
    {
        StartCoroutine(IgnoreCollisionRoutine(duration));
    }

    private IEnumerator IgnoreCollisionRoutine(float duration)
    {
        ignoreCollision = true;
        yield return new WaitForSeconds(duration);
        ignoreCollision = false;
    }

    private void RunCoyoteTimer()
    {
        if (coyoteTimer < coyoteTime)
        {
            coyoteTimer += Time.deltaTime;
        }
    }

    // For debug visualization
    private void OnDrawGizmosSelected()
    {
        if (groundCheckLeft != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheckLeft.position, new(0.05f, groundCheckDistance));
        }
        if (groundCheckCenter != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheckCenter.position, new(0.05f, groundCheckDistance));
        }
        if (groundCheckRight != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheckRight.position, new(0.05f, groundCheckDistance));
        }
        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(wallCheckPoint.position, new(wallCheckDistance, 0.05f));
        }
    }

    public void ChangeDirection()
    {
        moveDirection *= -1;
        FlipSprite();
        wallCheckPoint.localPosition = new Vector3(0.4f * moveDirection + wallCheckPointOffset, 0);
    }

    private void FlipSprite()
    {
        if (moveDirection >= 0)
            spriteRenderer.flipX = false;
        else
            spriteRenderer.flipX = true;
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("HorizontalSpeed", math.abs(rb.velocity.x));
        animator.SetFloat("VerticalSpeed", rb.velocity.y);
        animator.SetBool("IsWallSliding", isWallSliding);
    }

    private void UpdateDirection()
    {
        if (wallJumpControlTimer > 0)
            return;
        if (moveInput.x > 0)
        {
            moveDirection = 1;
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < 0)
        {
            moveDirection = -1;
            spriteRenderer.flipX = true;
        }
    }

    public void TakeDamage()
    {
        if (invincible)
            return;
        hp -= 1;
        playerState = States.Damage;
        OnHealthChanged?.Invoke(hp);
        gameObject.layer = LayerMask.NameToLayer("PlayerInvincible");
        //AudioManager.Instance.PlaySFX(hurtSfx);
        if (hp <= 0)
        {
            isAlive = false;
            animator.SetBool("IsDead", true);
            moveInput = Vector3.zero;
            StartCoroutine(DeadSequence());
            DamageRecoil(2f);
            //AudioManager.Instance.PlaySFX(deadSfx);
            DisableControls();
        }
        else
        {
            // TODO jugador sigue vivo
            animator.ResetTrigger("IsHurt");
            animator.SetTrigger("IsHurt");
            invincible = true;
            StartCoroutine(InvulnerableTimer());
            StartCoroutine(DamageTimer());
            DamageRecoil(1.5f);
        }
    }

    private IEnumerator InvulnerableTimer()
    {
        float elapsed = 0f;
        int flickerInterval = 4;  // flicker every 2 frames
        int frameCounter = 0;

        while (elapsed < iFrames)
        {
            if (frameCounter % flickerInterval == 0)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }

            frameCounter++;
            elapsed += Time.deltaTime;
            yield return null; // wait for next rendered frame
        }

        spriteRenderer.enabled = true;
        invincible = false;
        gameObject.layer = LayerMask.NameToLayer("PlayerDefault");
    }

    private IEnumerator DamageTimer()
    {
        damageFramesTimer = 0;
        while (damageFramesTimer < damageFrames)
        {
            damageFramesTimer += Time.deltaTime;
            yield return null;
        }
        playerState = States.Default;
        animator.ResetTrigger("IsRecovered");
        animator.SetTrigger("IsRecovered");
    }

    private void DamageRecoil(float mult)
    {
        float horizontalRecoil = damageRecoilHorizontal * moveDirection * -1;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(horizontalRecoil * mult, damageRecoilVertical * mult), ForceMode2D.Impulse);
    }

    public void InstaKill()
    {
        //TODO
        GameManager.Instance.RespawnPlayer();
    }

    public void ResetPlayer()
    {
        rb.velocity = Vector2.zero;
        ResetAnimator();
    }

    private void ResetAnimator()
    {
        animator.Rebind();
        animator.Update(0f);
    }

    //private void AttackCooldown()
    //{
    //    shotCdTimer -= Time.deltaTime;
    //    if (shotCdTimer <= 0)
    //    {
    //        if (ammo > 0)
    //            AudioManager.Instance.PlaySFX(reloadSfx);
    //        else
    //            AudioManager.Instance.PlaySFX(noAmmoSfx);
    //    }
    //}

    private void StompBounce()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, bounceSpeed), ForceMode2D.Impulse);
        //AudioManager.Instance.PlaySFX(stompSfx);
    }

    public void OnStomp()
    {
        StompBounce();
    }

    public void Heal(short amount)
    {
        hp += amount;
        if (hp > maxHp)
            hp = maxHp;
        OnHealthChanged?.Invoke(hp);
    }

    public void Reload(short amount)
    {
        ammo += amount;
        OnAmmoChanged?.Invoke(ammo);
    }

    public void PlayPickup()
    {
        Instantiate(pickupEffect, transform);
    }

    private IEnumerator DeadSequence()
    {
        deadTimer = 0;
        while (deadTimer < deadTime)
        {
            deadTimer += Time.deltaTime;
            yield return null;
        }
        OnDeath?.Invoke();
    }

    public void SetHp(short hp)
    {
        this.hp = hp;
        OnHealthChanged?.Invoke(hp);
    }

    public void SetAmmo(short ammo)
    {
        this.ammo = ammo;
        OnAmmoChanged?.Invoke(ammo);
    }

    public short GetHp()
    {
        return hp;
    }

    public short GetAmmo()
    {
        return ammo;
    }

    private void DisableControls()
    {
        inputActions.Disable();
    }

    private void WallRegrabTimer()
    {
        if (regrabTimer > 0)
            regrabTimer -= Time.deltaTime;
    }

    private void WallJumpSpeedTimer()
    {
        if (wallJumpControlTimer > 0)
            wallJumpControlTimer -= Time.deltaTime;
        if (isGrounded)
            wallJumpControlTimer = 0;
    }

    private void AdjustSpritePosition()
    {
        if (isWallSliding)
        {
            spriteObject.transform.localPosition = new Vector3(0.15f * moveDirection, 0);
        }
        else
        {
            spriteObject.transform.localPosition = Vector3.zero;
        }
    }
}
