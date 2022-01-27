using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float jumpVelocity = 40f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask lmGround;

    [Range(0f, 0.3f)][SerializeField] private float movementSmoothening = 0.3f;

    private float horizontalMove;

    private Rigidbody2D rb;

    private Vector3 velocity = Vector3.zero;
    private Vector2 groundedCheck;

    private float jumpPressedRemember = 0f;
    [SerializeField] private float jumpPressedRememberTime = 0.2f;

    private float wasGroundedRemember = 0f;
    [SerializeField] private float wasGroundedRememberTime = 0.2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); //when it wakes / loads grabs the rigidbody2d of the object the script is attached to.
    }

    // Update is called once per frame
    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal"); // just gets when ever A or D is pressed and returns -1 to 1, unity has preset buttons like "Fire1" or "Horizontal" / "Vertical"

        groundedCheck = (Vector2)transform.position + new Vector2(0, -0.1f);
        bool bGrounded = Physics2D.OverlapBox(groundedCheck, transform.localScale, 0, lmGround);

        wasGroundedRemember -= Time.deltaTime;
        if(bGrounded)
        {
            wasGroundedRemember = wasGroundedRememberTime;
        }

        jumpPressedRemember -= Time.deltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressedRemember = jumpPressedRememberTime;
        }

        if ((wasGroundedRemember > 0) && (jumpPressedRemember > 0))
        {
            wasGroundedRemember = 0;
            jumpPressedRemember = 0;
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }
    }

    private void FixedUpdate()
    {
        //transform.position += new Vector3(horizontalMove, 0, 0) * moveSpeed * Time.fixedDeltaTime; <-- this is a basic move them toward that direction as a Vector3 (x, y, z)
        Vector3 targetVelocity = new Vector2(horizontalMove * moveSpeed * Time.fixedDeltaTime * 10f, rb.velocity.y); //same thing as above but separates it into velocity to look toward

        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothening); //and then slowly smoothening it toward that target velocity from your current

    }
}
