using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinDeath : MonoBehaviour
{
    public static WinDeath instance;

    [SerializeField] private float slideTime = 2f;
    [SerializeField] private Ease.EaseType easeType;
    [SerializeField] private Button restartButton;

    private Vector2 initPos;
    private string currentLevelName;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        initPos = transform.position;
        currentLevelName = SceneManager.GetActiveScene().name;
    }

    public void OnWin()
    {
        //choose win image
        StartCoroutine(WinDeathSlide());
    }

    public void OnDeath()
    {
        //choose death image
        StartCoroutine(WinDeathSlide());
    }

    private IEnumerator MoveAndLoadScene(string scene)
    {
        yield return StartCoroutine(Utils.SlideObject(gameObject, initPos, slideTime, Ease.EaseType.CubicOut));
        yield return SlidingDoors.instance.LoadSceneRoutine(scene);
        yield return SlidingDoors.instance.OpenRoutine();
    }

    public void BackToMainMenu()
    {
        StopAllCoroutines();
        StartCoroutine(MoveAndLoadScene("Menu"));
    }

    private IEnumerator WinDeathSlide()
    {
        yield return Utils.SlideObject(gameObject, Vector2.zero, slideTime, Ease.EaseType.CubicOut);
        restartButton.onClick.AddListener(() => { StopAllCoroutines(); StartCoroutine(MoveAndLoadScene(currentLevelName)); });
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetStaticFields()
    {
        instance = null;
    }
}
