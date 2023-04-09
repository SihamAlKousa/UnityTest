using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl instance;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpForce = 10f;


    [SerializeField] Rigidbody2D player;
    bool isGrounded;
    public Transform groundCheckPoint;
    //public Transform stickyCheckPoint;
    [SerializeField] LayerMask whatIsGround;
    //[SerializeField] LayerMask whatIsSticky;

    private Animator anim;
    private SpriteRenderer theSr;

    public bool canDoubleJump;
    public float KnockBackLength = 0.25f, KnockBackForce = 5;
    private float KnockBackCounter;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        theSr = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        if (KnockBackCounter <= 0)
        {
            if (isGrounded)
            {
                canDoubleJump = true;
            }
            isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, 0.2f, whatIsGround);

            float horizontalInput = Input.GetAxis("Horizontal");
      
            player.velocity = new Vector2(moveSpeed * horizontalInput, player.velocity.y);

            // Flip player sprite depending on direction
            if (player.velocity.x < 0)
            {
                theSr.flipX = true;
            }
            else if (player.velocity.x > 0)
            {
                theSr.flipX = false;
            }
            // Jumping
            if (Input.GetButtonDown("Jump"))
            {
                if (isGrounded || canDoubleJump)
                {
                    //float jumpForceToUse = jumpForce;
                    player.velocity = new Vector2(player.velocity.x, jumpForce);
                    player.velocity = new Vector2(player.velocity.x, Mathf.Clamp(player.velocity.y, 0f, jumpForce));
                    //isSticky = false;

                    if (!isGrounded && canDoubleJump)
                    {
                        canDoubleJump = false;
                    }
                }
            }
            // Play animations
        }
        else
        {
            KnockBackCounter -= Time.deltaTime;
            if (!theSr.flipX)
            {
                player.velocity = new Vector2(-KnockBackForce, player.velocity.y);
            }
            else
            {
                player.velocity = new Vector2(KnockBackForce, player.velocity.y);
            }
        }
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("moveSpeed", Mathf.Abs(player.velocity.x));
    }
    public void KnockBack()
    {
        KnockBackCounter = KnockBackLength;
        player.velocity = new Vector2(0f, KnockBackForce);
        anim.SetTrigger("hurt");
        AudioManager.instance.playSFX(1);
    }

}