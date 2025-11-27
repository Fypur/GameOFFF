using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using UnityEngine.UI;

public class OsuSlider : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private float slideTime = 5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float healAmount = 10f;
    [SerializeField] private float distanceToLast = 0.4f;
    [SerializeField] private OsuBall ball;

    [HideInInspector] public Person parentPerson;
    private bool ballSelected;
    private bool succeeded;
    private SpriteShapeController spriteShape;
    private EdgeCollider2D edgeCollider;

    private void Awake()
    {
        spriteShape = GetComponent<SpriteShapeController>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        ball.OnPointerDownEvent += OnBallPointerDown;
        ball.OnPointerUpEvent += OnBallPointerUp;

        Vector2 splineInitPos = spriteShape.spline.GetPosition(0) + transform.position;
        ball.transform.position = new Vector3(splineInitPos.x, splineInitPos.y, ball.transform.position.z);
    }

    private void Start()
    {
        StartCoroutine(Utils.TimerThen(slideTime, () => { if (!succeeded) Fail(); }));
    }

    private void OnBallPointerDown()
    {
        ballSelected = true;
    }

    private void OnBallPointerUp()
    {
        ballSelected = false;
        if(!succeeded)
            Fail();
    }

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        if (ballSelected)
        {
            if (edgeCollider.OverlapPoint(mousePos))
            {
                float radius = edgeCollider.edgeRadius;
                edgeCollider.edgeRadius = 0;
                Vector2 closest = edgeCollider.ClosestPoint(mousePos);
                ball.transform.position = new Vector3(closest.x, closest.y, ball.transform.position.z);
                edgeCollider.edgeRadius = radius;

                if (Vector2.Distance(ball.transform.position, transform.position + spriteShape.spline.GetPosition(spriteShape.spline.GetPointCount() - 1)) <= distanceToLast)
                {
                    ballSelected = false;
                    Success();
                }
            }
            else if(!succeeded)
            {
                Fail();
            }
        }
    }

    private void Success()
    {
        GameManager.Instance.Heal(healAmount);
        succeeded = true;
        End();
    }

    private void Fail()
    {
        GameManager.Instance.Damage(damage);
        End();
    }

    private void End()
    {
        parentPerson.Leave();
        Destroy(gameObject);
    }

    //block clicks from going through
    public void OnPointerDown(PointerEventData eventData)
    { }
}
