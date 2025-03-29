using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    public KeyCode leftKey = KeyCode.LeftArrow;
    public KeyCode rightKey = KeyCode.RightArrow;
    public KeyCode jumpKey = KeyCode.UpArrow;
    public KeyCode interactKey = KeyCode.DownArrow;

    public Transform targetToInteract;
    public RulerController interactionRuler;

    public Rigidbody2D rb;
    public bool isGrounded;

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Update()
    {
        float move = 0f;

        if (Input.GetKey(leftKey))
            move = -1f;
        else if (Input.GetKey(rightKey))
            move = 1f;

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (Input.GetKeyDown(interactKey) && interactionRuler != null)
        {
            float dist = Vector2.Distance(transform.position, targetToInteract.position);
            if (dist < 1f)
                interactionRuler.enabled = !interactionRuler.enabled;
        }

        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
        isGrounded = false;
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
