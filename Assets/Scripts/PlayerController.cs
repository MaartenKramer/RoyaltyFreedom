using UnityEngine;
using System.Collections;
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;
    private Vector3 moveInput;
    private Animator anim;
    private bool isRunning = false;
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.None;
        StartCoroutine(EnableInterpolationAfterDelay(0.5f));
    }
    // enables interpolation after delay
    private IEnumerator EnableInterpolationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
    private void Update()
    {
        // stops player animations during dialogue
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive())
        {
            anim.SetBool("IsWalk", false);
            anim.SetBool("IsRun", false);
            return;
        }

        // checks if player is running
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 8;
            isRunning = true;
        }
        else
        {
            speed = 5;
            isRunning = false;
        }

        // calculates movement
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(moveX, 0, moveZ).normalized * speed;

        // animation and character direction logic
        anim.SetFloat("Vertical", moveZ);
        anim.SetFloat("Horizontal", moveX);

        if (moveX != 0 || moveZ != 0)
        {
            anim.SetBool("IsWalk", true);
            anim.SetBool("IsRun", isRunning);

            if (moveX < 0)
            {
                anim.transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            else if (moveX > 0)
            {
                anim.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }
        else
        {
            anim.SetBool("IsWalk", false);
            anim.SetBool("IsRun", false);
        }
    }
    private void FixedUpdate()
    {
        // freezes player movement during dialogue
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive())
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        if (moveInput.magnitude > 0.1f)
        {
            rb.linearVelocity = new Vector3(moveInput.x, rb.linearVelocity.y, moveInput.z);
        }
        else
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            Vector3 smoothVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, 0.2f);
            rb.linearVelocity = new Vector3(smoothVelocity.x, rb.linearVelocity.y, smoothVelocity.z);
        }
    }
}