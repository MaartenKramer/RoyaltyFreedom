using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;
    private Vector3 moveInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.None;

        StartCoroutine(EnableInterpolationAfterDelay(0.5f));
    }

    //enables interpolation after delay
    private IEnumerator EnableInterpolationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(moveX, 0, moveZ).normalized * speed;
    }

    private void FixedUpdate()
    {
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