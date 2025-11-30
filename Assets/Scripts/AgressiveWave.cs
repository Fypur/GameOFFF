using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AgressiveWave : MonoBehaviour
{
    [SerializeField] private float healedAmount = 5;
    [SerializeField] private float damage = 10;

    [HideInInspector] public Person parentPerson;
    [HideInInspector] public int waveAmount = 5;
    [HideInInspector] public float waveTime = 5f;

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

    public void OnButtonHover()
    {
        Utils.AudioPlay("event:/Menu UI/button_hover");
    }

    private void WaveButtonClick()
    {
        waveAmount--;
        Utils.AudioPlay("event:/Menu UI/button_click");

        if (waveAmount > 0)
            wavesLeftText.text = "Waves left: " + waveAmount;
        else
            Success();
    }

    private void Success()
    {
        GameManager.instance.Heal(healedAmount);
        End();
    }

    private void Fail()
    {
        GameManager.instance.Damage(damage);
        End();
    }

    private void End()
    {
        parentPerson.Leave();
        Destroy(gameObject);
    }
}
