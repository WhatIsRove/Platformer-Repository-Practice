using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float jumpVelocity = 40f;
    [SerializeField] private float moveSpeed = 5f;

    [Range(0f, 0.3f)][SerializeField] private float movementSmoothening = 0.3f;

    private float horizontalMove;
    private bool jump = false;

    private Rigidbody2D rb;

    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); //when it wakes / loads grabs the rigidbody2d of the object the script is attached to.
    }

    // Update is called once per frame
    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal"); // just gets when ever A or D is pressed and returns -1 to 1, unity has preset buttons like "Fire1" or "Horizontal" / "Vertical"
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    private void FixedUpdate()
    {
        //transform.position += new Vector3(horizontalMove, 0, 0) * moveSpeed * Time.fixedDeltaTime; <-- this is a basic move them toward that direction as a Vector3 (x, y, z)
        
        Vector3 targetVelocity = new Vector2(horizontalMove * moveSpeed * Time.fixedDeltaTime * 10f, rb.velocity.y); //same thing as above but separates it into velocity to look toward

        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothening); //and then slowly smoothening it toward that target velocity from your current

        if (jump)
        {
            jump = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }
    }
}
