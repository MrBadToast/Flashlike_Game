using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class Player_Platformer : StaticSerializedMonoBehaviour<Player_Platformer>
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float launchMult = 2f;

    [Header("점프 체크")]
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    public LayerMask groundLayer;
    public LayerMask cannonLayer;
    public LayerMask deathLayer;

    [Header("사운드")]
    public AudioClip jumpClip;
    public AudioClip deathClip;
    public AudioClip fallClip;

    [Header("기타")]
    public AnimationCurve deathPush;

    public Transform lastCheckPoint;
    public Transform falldownThreshold;

    public GameObject deathCam;
    public CinemachineCamera playerCam;

    public UnityEvent OnPlayerReset;

    [HideInInspector] public Vector2 cannonDirection;

    [HideInInspector] public Vector3 deathContact;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public bool isGrounded;

    private bool isActive = true;
    public Collider2D lastGroundCollider { get; private set; }

    private IPlayerState currentState;
    public IPlayerState CurrentState => currentState;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ChangeState(new Standby());
    }

    public void StartPlayer()
    {
        ChangeState(new MoveState());
    }

    void Update()
    {
        if (!isActive)
            return;

        // 바닥 체크 (OverlapBox)
        lastGroundCollider = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        isGrounded = lastGroundCollider != null;

        currentState?.Update(this);
    }

    public void ChangeState(IPlayerState newState)
    {
        if (currentState == newState)
            return;

        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }

    public void ResetPlayer()
    {
        OnPlayerReset?.Invoke();
        ChangeState(new MoveState());
        rb.linearVelocity = Vector2.zero;
        animator.Play("Idle");
    }

    public void SetCameraTarget(Transform target)
    {
        playerCam.Follow = target;
    }

    public void LaunchPlayer()
    {
        rb.linearVelocity = new Vector2(0, jumpForce * launchMult);
        animator.SetTrigger("Jump");
    }

    public void CannonPlayer(Vector2 direction)
    {
        if (currentState is CannonReady)
        {
            cannonDirection = direction.normalized;
            ChangeState(new CannonShoot());
        }
    }

    public void ActivatePlayer()
    {
        isActive = true;
        if (rb != null)
            rb.simulated = true;
    }

    public void DeactivatePlayer()
    {
        isActive = false;
        if (rb != null)
        {
            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DeadZone"))
        {
            deathContact = collision.GetContact(0).point;
            ChangeState(new DeadState());
        }

        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.SetParent(collision.transform);
        }

        if (collision.gameObject.CompareTag("Launcher"))
        {
            collision.gameObject.GetComponent<Animator>().Play("Launch");
            collision.gameObject.GetComponent<AudioSource>().Play();
            LaunchPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadZone"))
        {
            deathContact = collision.ClosestPoint(transform.position);
            ChangeState(new DeadState());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }
    }
}


public interface IPlayerState
{
    void Enter(Player_Platformer player);
    void Update(Player_Platformer player);
    void Exit(Player_Platformer player);
}

public class Standby : IPlayerState
{
    public void Enter(Player_Platformer player) 
    { 
        
    }
    public void Update(Player_Platformer player) 
    {
        if (player.isGrounded)
        {
            player.animator.SetBool("Grounded", true);
        }
        else
        {
            player.animator.SetBool("Grounded", false);
        }
    }
    public void Exit(Player_Platformer player) { }
}

public class MoveState : IPlayerState
{
    public void Enter(Player_Platformer player) 
    { 
        player.rb.simulated = true;
    }

    public void Update(Player_Platformer player)
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        player.rb.linearVelocity = new Vector2(Mathf.Lerp(player.rb.linearVelocityX,moveInput * player.moveSpeed,0.5f), player.rb.linearVelocity.y);
        player.animator.SetBool("HorInput", moveInput != 0);
        player.animator.SetFloat("Vertical", player.rb.linearVelocity.y);

        if(moveInput > 0)
        {
            player.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            player.transform.localScale = new Vector3(-1, 1, 1);
        }

        if (player.isGrounded)
        {
            player.animator.SetBool("Grounded", true);
        }
        else
        {
            player.animator.SetBool("Grounded", false);
        }

        if (Input.GetButtonDown("Jump") && player.isGrounded)
        {
            player.audioSource.PlayOneShot(player.jumpClip);
            player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, player.jumpForce);
            player.animator.SetTrigger("Jump");
        }


        if(player.transform.position.y < player.falldownThreshold.position.y)
        {
            player.ChangeState(new FallState());
        }

    }

    public void Exit(Player_Platformer player)
    {

    }
}

public class DeadState : IPlayerState
{
    Vector3 push;
    Vector3 origin;
    float timer = 0f;

    public void Enter(Player_Platformer player)
    {
        player.rb.linearVelocity = Vector2.zero;
        player.rb.simulated = false;
        player.animator.SetTrigger("Dead");
        player.audioSource.PlayOneShot(player.deathClip);
        player.deathCam.SetActive(true);

        push = player.transform.position + (player.transform.position - player.deathContact).normalized;
        origin = player.transform.position;
        timer = 0f;
    }
    public void Update(Player_Platformer player) 
    {
        timer += Time.deltaTime;

        player.transform.position = Vector3.Lerp(origin,push,player.deathPush.Evaluate(Mathf.Clamp(timer,0f,1f)));

        if (timer >= 3f)
        {
            player.transform.position = player.lastCheckPoint.position;
            player.ResetPlayer();
        }
    }
    public void Exit(Player_Platformer player) 
    {
        player.deathCam.SetActive(false);
    }
}   

public class FallState : IPlayerState
{
    public float time;

    public void Enter(Player_Platformer player) 
    {
        player.animator.SetTrigger("Fall");
        player.audioSource.PlayOneShot(player.fallClip);
        player.playerCam.gameObject.SetActive(false);
    }
    public void Update(Player_Platformer player)
    {
        time += Time.deltaTime;

        if (time >= 3f)
        {
            player.transform.position = player.lastCheckPoint.position;
            player.ResetPlayer();
        }
    }
    public void Exit(Player_Platformer player)
    {
        player.playerCam.gameObject.SetActive(true);
    }
}

public class CannonReady:IPlayerState
{
    public void Enter(Player_Platformer player)
    {
        player.rb.bodyType = RigidbodyType2D.Static;
        player.animator.SetTrigger("Jump");

    }
    public void Update(Player_Platformer player)
    {
        if (Physics2D.CircleCast(player.transform.position, 0.2f, player.cannonDirection, 0.1f, player.deathLayer))
        {
            player.deathContact = (player.transform.position + new Vector3(player.cannonDirection.x, player.cannonDirection.y) * 0.1f);
            player.ChangeState(new DeadState());
            return;
        }
    }
    public void Exit(Player_Platformer player)
    {
        player.rb.bodyType = RigidbodyType2D.Dynamic;
        player.transform.parent = null;
    }
}

public class CannonShoot : IPlayerState
{
    private float airDuation = 0.5f;
    private float speed = 15f;
    private float timer = 0f;

    public void Enter(Player_Platformer player)
    {
        player.rb.bodyType = RigidbodyType2D.Static;
        timer = 0f;
    }
    public void Update(Player_Platformer player)
    {
        timer += Time.deltaTime;

        if (timer < airDuation)
        {
            if(Physics2D.CircleCast(player.transform.position, 0.2f, player.cannonDirection, 0.1f, player.groundLayer))
            {
                player.ChangeState(new MoveState());
                return;
            }

            if(Physics2D.CircleCast(player.transform.position, 0.2f, player.cannonDirection, 0.1f, player.deathLayer))
            {
                player.deathContact = (player.transform.position + new Vector3(player.cannonDirection.x, player.cannonDirection.y) * 0.1f);
                player.ChangeState(new DeadState());
                return;
            }

            player.transform.Translate(player.cannonDirection * speed * Time.deltaTime);
        }

        if (timer >= airDuation)
        {
            player.ChangeState(new MoveState());
        }
    }
    public void Exit(Player_Platformer player)
    {
        player.rb.bodyType = RigidbodyType2D.Dynamic;
        player.rb.linearVelocity = player.cannonDirection * speed * Time.deltaTime;
    }
}