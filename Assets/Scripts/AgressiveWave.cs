using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AgressiveWave : MonoBehaviour
{
    public Person parentPerson;
    public int waveAmount = 5;
    public float waveTime = 5f;

    [SerializeField] private TMPro.TMP_Text wavesLeftText;
    [SerializeField] private Button waveButton;

    private void Awake()
    {
        waveButton.onClick.AddListener(WaveButtonClick);
    }

    private void Start()
    {
        StartCoroutine(TimerCoroutine());
        wavesLeftText.text = "Waves left: " + waveAmount;
    }

    private void WaveButtonClick()
    {
        waveAmount--;

        if (waveAmount > 0)
            wavesLeftText.text = "Waves left: " + waveAmount;
        else
            Success();
    }

    private IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(waveTime);
        Fail();
    }

    private void Success()
    {
        End();
    }

    private void Fail()
    {
        End();
    }

    private void End()
    {
        parentPerson.Leave();
        Destroy(gameObject);
    }
}
