using UnityEngine;

public class Patrol : MonoBehaviour
{
    public GameObject Point1;
    public GameObject Point2;

    private Animator anim;

    [SerializeField] private float timer;
    public float waitTime;
    public float speed;

    private SpriteRenderer sprite;

    public enum State
    {
        P1, P1ToP2, P2, P2ToP1
    }
    public State state = State.P1;

    void Start()
    {
        anim = this.gameObject.GetComponent<Animator>();
        sprite = this.gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        CheckState();

        timer += Time.deltaTime;
    }

    private void CheckState()
    {
        switch (state)
        {
            case State.P1: P1(); break;
            case State.P1ToP2: P1ToP2(); break;
            case State.P2: P2(); break;
            case State.P2ToP1: P2ToP1(); break;
        }
    }

    private void P1()
    {
        this.gameObject.transform.position = Point1.transform.position;
        anim.Play("IdleFront");
        sprite.flipX = false;

        if (timer >= waitTime)
        {
            timer = 0;
            state = State.P1ToP2;
        }
    }

    private void P1ToP2()
    {
        this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, Point2.transform.position, speed);
        anim.Play("Walk");

        if (this.gameObject.transform.position == Point2.transform.position)
        {
            timer = 0;
            state = State.P2;
        }
    }

    private void P2()
    {
        this.gameObject.transform.position = Point2.transform.position;
        anim.Play("IdleBack");
        sprite.flipX = true;

        if (timer >= waitTime)
        {
            timer = 0;
            state = State.P2ToP1;
        }
    }

    private void P2ToP1()
    {
        this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, Point1.transform.position, speed);
        anim.Play("Walk");

        if (this.gameObject.transform.position == Point1.transform.position)
        {
            timer = 0;
            state = State.P1;
        }
    }
}
