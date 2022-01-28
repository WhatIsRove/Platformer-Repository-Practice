using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float hp = 1f;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject respawnObject;

    [SerializeField] private float jumpVelocity = 40f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask lmGround;
    [Range(0f, 0.3f)][SerializeField] private float movementSmoothening = 0.3f;

    private float jumpPressedRemember = 0f;
    [SerializeField] private float jumpPressedRememberTime = 0.2f;
    [Range(0f, 1f)][SerializeField] private float jumpCutOff = 0.5f;
    [SerializeField] private float mGravityScale = 4f;
    private float wasGroundedRemember = 0f;
    [SerializeField] private float wasGroundedRememberTime = 0.2f;
    private bool bFalling = false;

    private float horizontalMove;
    private bool bFacingRight = true;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 velocity = Vector3.zero;

    private ParticleSystem trailPS;
    private ParticleSystem altTrailPS;
    private ParticleSystem impactPS;

    private bool bInvul = false;
    [SerializeField] private float invulDuration = 1.5f;
    [SerializeField] private float invulFlashes = 0.15f;

    private bool bDead = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); //when it wakes / loads grabs the rigidbody2d of the object the script is attached to.
        animator = GetComponent<Animator>();
        trailPS = GameObject.Find("Trail").GetComponent<ParticleSystem>();
        altTrailPS = GameObject.Find("AltTrail").GetComponent<ParticleSystem>();
        impactPS = GameObject.Find("Impact").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal"); // just gets when ever A or D is pressed and returns -1 to 1, unity has preset buttons like "Fire1" or "Horizontal" / "Vertical"

        Vector2 groundedCheckPosition = (Vector2)transform.position + new Vector2(0, -0.4f);
        Vector2 groundedCheckScale = (Vector2)transform.localScale + new Vector2(-0.2f, 0);
        bool bGrounded = Physics2D.OverlapBox(groundedCheckPosition, groundedCheckScale, 0, lmGround);

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        wasGroundedRemember -= Time.deltaTime;
        if (bGrounded)
        {
            wasGroundedRemember = wasGroundedRememberTime;
        }

        jumpPressedRemember -= Time.deltaTime;
        if (Input.GetButtonDown("Jump") && rb.velocity.y <= 0)
        {
            jumpPressedRemember = jumpPressedRememberTime;
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutOff);
        }

        //jumping
        if ((wasGroundedRemember > 0) && (jumpPressedRemember > 0))
        {
            animator.SetTrigger("Jumping");
            wasGroundedRemember = 0;
            jumpPressedRemember = 0;
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);

            Impact();
        }

        //falling
        if (rb.velocity.y < 0)
        {
            animator.SetBool("Falling", true);
            bFalling = true;
        }

        //landing
        if (rb.velocity.y == 0 && bFalling)
        {
            animator.SetTrigger("Landing");
            animator.SetBool("Falling", false);
            bFalling = false;

            Impact();
        }

        //flips direction you move to
        if (horizontalMove > 0 && !bFacingRight)
        {
            Flip();
        }

        if (horizontalMove < 0 && bFacingRight)
        {
            Flip();
        }

        //Fast fall
        if (rb.velocity.y < 0.2f) rb.gravityScale = mGravityScale * 2f;
        else rb.gravityScale = mGravityScale;
    }

    private void FixedUpdate()
    {
        //transform.position += new Vector3(horizontalMove, 0, 0) * moveSpeed * Time.fixedDeltaTime; <-- this is a basic move them toward that direction as a Vector3 (x, y, z)
        Vector3 targetVelocity = new Vector2(horizontalMove * moveSpeed * Time.fixedDeltaTime * 10f, rb.velocity.y); //same thing as above but separates it into velocity to look toward

        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothening); //and then slowly smoothening it toward that target velocity from your current

        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;

        trailPS.gameObject.transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
    }

    private void Flip()
    {
        bFacingRight = !bFacingRight;

        transform.Rotate(0f, 180f, 0f);
    }

    private void TrailOn()
    {
        trailPS.Play();
        altTrailPS.Play();
    }

    private void TrailOff()
    {
        trailPS.Stop();
        altTrailPS.Stop();
    }

    private void Impact()
    {
        impactPS.gameObject.SetActive(true);
        impactPS.Stop();
        impactPS.Play();
    }

    private IEnumerator BecomeInvul()
    {
        bInvul = true;

        for (float i = 0; i < invulDuration; i += invulFlashes)
        {
            yield return new WaitForSeconds(invulFlashes);
        }

        bInvul = false;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (!bDead && hp <= 0)
        {
            bDead = true;
            Die();
        }

        StartCoroutine(BecomeInvul());
    }

    private void Die()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Instantiate(respawnObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    } 
}
