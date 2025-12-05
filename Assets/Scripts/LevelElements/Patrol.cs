using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Patrol : MonoBehaviour
{
    public List<Transform> patrolPoints;
    public Transform currentPoint;
    [SerializeField] private int index;

    private Animator anim;

    [SerializeField] private float timer;
    public float waitTime;
    public float speed;

    public GameObject moveThis;

    public enum State
    {
        Patrolling, Idling
    }
    public State state = State.Idling;

    void Start()
    {
        anim = moveThis.GetComponent<Animator>();

        PickRandomPoint();
        moveThis.transform.position = currentPoint.position;
        timer = 0;
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
            case State.Idling: Idling(); break;
            case State.Patrolling: Patrolling(); break;
        }
    }

    private void PickRandomPoint()
    {
        index = Random.Range(0, patrolPoints.Count);
        currentPoint = patrolPoints[index];
    }

    private void PickNextPoint()
    {
        index ++;
        if (index > patrolPoints.Count-1)
        {
            index = 0;
            currentPoint = patrolPoints[index];
        }
        else
        {
            currentPoint = patrolPoints[index];
        }
    }

    private void Idling()
    {
        moveThis.transform.position = currentPoint.position;
        anim.Play("Idle");

        if (timer >= waitTime)
        {
            timer = 0;
            PickNextPoint();
            state = State.Patrolling;
        }
    }

    private void Patrolling()
    {
        moveThis.transform.position = Vector3.MoveTowards(moveThis.transform.position, currentPoint.position, speed);
        anim.Play("Walk");

        if (moveThis.transform.position == currentPoint.position)
        {
            timer = 0;
            state = State.Idling;
        }
    }
}
