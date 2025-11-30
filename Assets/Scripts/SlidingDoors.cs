using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SlidingDoors : MonoBehaviour
{
    [SerializeField] private string emptySceneName;
    public static SlidingDoors instance;
    public UnityEvent OnBeginLoadSceneOpen;
    public UnityEvent OnEndLoadSceneOpen;

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
        {
            Destroy(instance);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        openPosLeft = leftSlidingDoorImage.anchoredPosition;
        leftSlidingDoorImage.gameObject.SetActive(false);
        rightSlidingDoorImage.gameObject.SetActive(false);
    }

    public void ClosedLoadSceneOpen(string sceneName)
    {
        StartCoroutine(CloseLoadSceneOpenRoutine(sceneName));
    }

    private IEnumerator CloseLoadSceneOpenRoutine(string sceneName)
    {
        leftSlidingDoorImage.gameObject.SetActive(true);
        rightSlidingDoorImage.gameObject.SetActive(true);

        yield return SlideUI(closedPos.anchoredPosition - new Vector2(leftSlidingDoorImage.rect.width / 2, 0), closedPos.anchoredPosition + new Vector2(rightSlidingDoorImage.rect.width / 2, 0), closeTime, closeEaseType);

        yield return LoadSceneOpenRoutine(sceneName);
    }

    public IEnumerator FinishLevel(string unloadedScene, Action callback)
    {
        leftSlidingDoorImage.gameObject.SetActive(true);
        rightSlidingDoorImage.gameObject.SetActive(true);

        yield return SlideUI(closedPos.anchoredPosition - new Vector2(leftSlidingDoorImage.rect.width / 2, 0), closedPos.anchoredPosition + new Vector2(rightSlidingDoorImage.rect.width / 2, 0), closeTime, closeEaseType);

        SceneManager.LoadScene(emptySceneName);

        callback();
    }

    public IEnumerator LoadSceneOpenRoutine(string sceneName)
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);

        // 3. WAIT until the scene is fully loaded
        while (!asyncLoad.isDone)
            yield return null;

        OnBeginLoadSceneOpen.Invoke();

        yield return SlideUI(openPosLeft - new Vector2(leftSlidingDoorImage.rect.width / 2, 0), 2 * closedPos.anchoredPosition - openPosLeft + new Vector2(rightSlidingDoorImage.rect.width / 2, 0), closeTime, openEaseType);

        leftSlidingDoorImage.gameObject.SetActive(false);
        rightSlidingDoorImage.gameObject.SetActive(false);

        OnEndLoadSceneOpen.Invoke();
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
