using System.Collections;
using TMPro;
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
        restartButton.GetComponentInChildren<TMP_Text>().text = "Next Level";
        string nextLevelName = currentLevelName.Substring(0, currentLevelName.Length - 1) + (char.GetNumericValue(currentLevelName[currentLevelName.Length - 1]) + 1);
        restartButton.onClick.AddListener(() => { StopAllCoroutines(); StartCoroutine(MoveAndLoadScene(nextLevelName)); });
        StartCoroutine(WinDeathSlide());
    }

    public void OnDeath()
    {
        //choose death image
        restartButton.GetComponentInChildren<TMP_Text>().text = "Retry";
        restartButton.onClick.AddListener(() => { StopAllCoroutines(); StartCoroutine(MoveAndLoadScene(currentLevelName)); });
        StartCoroutine(WinDeathSlide());
    }

    private IEnumerator MoveAndLoadScene(string scene)
    {
        Utils.AudioPlay("event:/Menu UI/button_click");
        yield return StartCoroutine(Utils.SlideObject(gameObject, initPos, slideTime, Ease.EaseType.CubicIn));
        yield return SlidingDoors.instance.LoadSceneRoutine(scene);
    }

    public void BackToMainMenu()
    {
        Utils.AudioPlay("event:/Menu UI/button_click");
        StopAllCoroutines();
        StartCoroutine(MoveAndLoadScene("Menu"));
    }

    private IEnumerator WinDeathSlide()
    {
        yield return Utils.SlideObject(gameObject, Vector2.zero, slideTime, Ease.EaseType.CubicOut);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetStaticFields()
    {
        instance = null;
    }
}
