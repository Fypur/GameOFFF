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
    [SerializeField] private TimerSlider timerSlider;

    private void Awake()
    {
        waveButton.onClick.AddListener(WaveButtonClick);
    }

    private void Start()
    {
        wavesLeftText.text = "Waves left: " + waveAmount;
        timerSlider.StartTimer(waveTime, Fail);
    }

    private void WaveButtonClick()
    {
        waveAmount--;

        if (waveAmount > 0)
            wavesLeftText.text = "Waves left: " + waveAmount;
        else
            Success();
    }

    private void Success()
    {
        Debug.Log("Agressive Wave success");
        End();
    }

    private void Fail()
    {
        Debug.Log("Agressive Wave fail");
        End();
    }

    private void End()
    {
        parentPerson.Leave();
        Destroy(gameObject);
    }
}
