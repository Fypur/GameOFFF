using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class SlidingDoors : MonoBehaviour
{
    public static SlidingDoors instance;
    public UnityEvent OnBeginOpen;
    public UnityEvent OnEndOpen;

    [SerializeField] private float closeTime;
    [SerializeField] private RectTransform leftSlidingDoorImage;
    [SerializeField] private RectTransform rightSlidingDoorImage;
    private Vector2 openPosLeft;
    [SerializeField] private RectTransform closedPos;
    [SerializeField] private Ease.EaseType closeEaseType;
    [SerializeField] private Ease.EaseType openEaseType;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance != null)
            Destroy(instance);

        instance = this;
    }

    private void Start()
    {
        openPosLeft = leftSlidingDoorImage.anchoredPosition;
    }

    public void CloseAndLoadScene(string sceneName)
    {
        StartCoroutine(CloseAndLoadSceneRoutine(sceneName));
    }

    private IEnumerator CloseAndLoadSceneRoutine(string sceneName)
    {
        leftSlidingDoorImage.gameObject.SetActive(true);
        rightSlidingDoorImage.gameObject.SetActive(true);

        yield return SlideUI(closedPos.anchoredPosition - new Vector2(leftSlidingDoorImage.rect.width / 2, 0), closedPos.anchoredPosition + new Vector2(rightSlidingDoorImage.rect.width / 2, 0), closeTime, closeEaseType);

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);

        OnBeginOpen.Invoke();

        yield return SlideUI(openPosLeft - new Vector2(leftSlidingDoorImage.rect.width / 2, 0), 2 * closedPos.anchoredPosition - openPosLeft + new Vector2(rightSlidingDoorImage.rect.width / 2, 0), closeTime, openEaseType);

        leftSlidingDoorImage.gameObject.SetActive(false);
        rightSlidingDoorImage.gameObject.SetActive(false);

        OnEndOpen.Invoke();
    }

    private IEnumerator SlideUI(Vector2 toPositionLeft, Vector2 toPositionRight, float time, Ease.EaseType easeType)
    {
        Vector2 fromPositionleft = leftSlidingDoorImage.anchoredPosition;
        Vector2 fromPositionRight = rightSlidingDoorImage.anchoredPosition;

        Func<float, float> easeFunc = easeType.ToFunc();
        float t = 0;
        while (t < time)
        {
            leftSlidingDoorImage.anchoredPosition = Vector2.LerpUnclamped(fromPositionleft, toPositionLeft, easeFunc(t / time));
            rightSlidingDoorImage.anchoredPosition = Vector2.LerpUnclamped(fromPositionRight, toPositionRight, easeFunc(t / time));
            t += Time.deltaTime * Time.timeScale;
            yield return null;
        }

        leftSlidingDoorImage.anchoredPosition = toPositionLeft;
        rightSlidingDoorImage.anchoredPosition = toPositionRight;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetStaticFields()
    {
        instance = null;
    }
}
