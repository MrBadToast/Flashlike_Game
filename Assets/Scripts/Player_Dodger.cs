using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player_Dodger : StaticSerializedMonoBehaviour<Player_Dodger>
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    public Rigidbody2D Rb => rb;

    [Header("사운드")]
    public AudioClip deathClip;
    public AudioClip dashClip;

    [Header("기타")]
    public AnimationCurve deathPush;
    public GameObject deathCam;

    private Vector3 deathContact;

    [HideInInspector]public Vector2 moveInput;
    private Animator animator;

    private IPlayerDodgeState currentState;
    private AudioSource audioSource;

    private Vector3 initialPosition;
    private float dashTimer = 0f;
    private float dashCooldown = 0.5f;

    public IPlayerDodgeState CurrentState => currentState;
    public bool enableOnStart = true;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        initialPosition = transform.position;

        if (enableOnStart)
        {
            ChangeState(new DodgeMoveState());
        }
        else
        {
            ChangeState(new DodgeStandbyState());
        }
    }

    void Update()
    {
        currentState?.Update(this);
    }

    void FixedUpdate()
    {
        currentState?.FixedUpdate(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DeadZone") || collision.gameObject.CompareTag("CodeObjects"))
        {
            if (currentState is DodgeDashState) return;

            deathContact = collision.GetContact(0).point;
            ChangeState(new DodgeDeadState());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadZone") || collision.gameObject.CompareTag("CodeObjects"))
        {
            if(currentState is DodgeDashState) return;

            deathContact = collision.ClosestPoint(transform.position);
            ChangeState(new DodgeDeadState());
        }
    }

    public void ResetPlayer()
    {
        ChangeState(new DodgeMoveState());
        rb.linearVelocity = initialPosition;
        animator.SetBool("Hor", false);
        animator.SetBool("Ver", false);

        animator.SetLayerWeight(0, 1);
        animator.SetLayerWeight(1, 0);
        animator.Play("Idle", 0);

    }

    public void ChangeState(IPlayerDodgeState newState)
    {
        if (currentState == newState) return;

        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }

    public Vector2 GetMoveInput() => moveInput;

    public class DodgeStandbyState : IPlayerDodgeState
    {
        public void Enter(Player_Dodger player)
        {
        }
        public void Update(Player_Dodger player)
        {
        }
        public void FixedUpdate(Player_Dodger player)
        {
        }
        public void Exit(Player_Dodger player)
        {
        }
    }

    public class DodgeMoveState : IPlayerDodgeState
    {
        float dashCooldownTimer = 0f;

        public void Enter(Player_Dodger player)
        {

        }

        public void Update(Player_Dodger player)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            dashCooldownTimer += Time.deltaTime;

            player.moveInput = new Vector2(h, v).normalized;

            if (h > 0)
            {
                player.transform.localScale = new Vector3(1, 1, 1);
                player.animator.SetBool("Hor", true);
            }
            else if (h < 0)
            {
                player.transform.localScale = new Vector3(-1, 1, 1);
                player.animator.SetBool("Hor", true);
            }
            else
            {
                player.animator.SetBool("Hor", false);
            }

            if (v > 0)
            {
                player.animator.SetLayerWeight(0, 0);
                player.animator.SetLayerWeight(1, 1);
                player.animator.SetBool("Ver", true);
            }
            else if (v < 0)
            {
                player.animator.SetLayerWeight(0, 1);
                player.animator.SetLayerWeight(1, 0);
                player.animator.SetBool("Ver", true);
            }
            else
            {
                player.animator.SetBool("Ver", false);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if(dashCooldownTimer < player.dashCooldown) return;

                player.ChangeState(new DodgeDashState());

            }
        }
        public void FixedUpdate(Player_Dodger player)
        {
            Vector2 vel = player.moveInput * player.moveSpeed * Time.fixedDeltaTime;
           player.Rb.linearVelocity = Vector2.Lerp(player.Rb.linearVelocity, vel, 0.5f);

        }

        public void Exit(Player_Dodger player)
        {

        }
    }

    public class DodgeDashState : IPlayerDodgeState
    {
        float dashDuration  = 0.1f;
        float dashSpeed = 5f;
        Vector2 dir;
        public void Enter(Player_Dodger player)
        {
            dir = player.GetMoveInput();
            if (dir == Vector2.zero) dir = Vector2.right * player.transform.localScale.x;
            player.dashTimer = 0f;
            player.animator.SetBool("Dash",true);
            player.audioSource.PlayOneShot(player.dashClip);
        }
        public void Update(Player_Dodger player)
        {

        }
        public void FixedUpdate(Player_Dodger player)
        {
            player.dashTimer += Time.deltaTime;

            if (dashDuration > player.dashTimer)
            {
                player.Rb.linearVelocity = dir * player.moveSpeed * dashSpeed * Time.fixedDeltaTime;
            }
            else
            {
                player.ChangeState(new DodgeMoveState());
            }
        }
        public void Exit(Player_Dodger player)
        {
            player.animator.SetBool("Dash", false);
        }
    }

    public class DodgeDeadState : IPlayerDodgeState
    {
        Vector3 push;
        Vector3 origin;
        float timer = 0f;

        public void Enter(Player_Dodger player)
        {
            player.rb.linearVelocity = Vector2.zero;
            player.rb.simulated = false;

            player.animator.SetLayerWeight(0, 1);
            player.animator.SetLayerWeight(1, 0);
            player.animator.Play("Dead",0);

            player.audioSource.PlayOneShot(player.deathClip);
            player.deathCam.SetActive(true);

            push = player.transform.position + (player.transform.position - player.deathContact).normalized;
            origin = player.transform.position;
            timer = 0f;
        }
        public void Update(Player_Dodger player)
        {
            timer += Time.deltaTime;

            player.transform.position = Vector3.Lerp(origin, push, player.deathPush.Evaluate(Mathf.Clamp(timer, 0f, 1f)));


            if (timer >= 3f)
            {

                if (DodgePatternManager.Instance.isBonusStage)
                {
                    DodgePatternManager.Instance.OnPlayerDeadInBonusStage();
                    player.ChangeState(new DodgeStandbyState());
                    return;
                }

                player.transform.position = Vector3.zero;
                player.ResetPlayer();
            }
        }

        public void FixedUpdate(Player_Dodger player)
        {
        }

        public void Exit(Player_Dodger player)
        {
            if(DodgePatternManager.Instance.isBonusStage) return;

            DodgePatternManager.Instance.ResetPattern();
            player.deathCam.SetActive(false);
            player.rb.simulated = true;
        }
    }



}

public interface IPlayerDodgeState
{
    void Enter(Player_Dodger player);
    void Update(Player_Dodger player);
    void FixedUpdate(Player_Dodger player);
    void Exit(Player_Dodger player);
}


