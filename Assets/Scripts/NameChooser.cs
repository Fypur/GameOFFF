using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class NameChooser : MonoBehaviour
{
    [SerializeField] private float healedAmount;
    [SerializeField] private float damage;
    [SerializeField] private float heal;

    [HideInInspector] public Person parentPerson;
    [HideInInspector] public string[] possibleNames;
    [HideInInspector] public int correctNameIndex;

    [SerializeField] private Button button0;
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private TimerSlider timerSlider;

    private void Awake()
    {
        button0.onClick.AddListener(() => ChooseName(0));
        button1.onClick.AddListener(() => ChooseName(1));
        button2.onClick.AddListener(() => ChooseName(2));
        button3.onClick.AddListener(() => ChooseName(3));
    }

    private void Start()
    {
        timerSlider.StartTimer(GameManager.instance.nameChooserTime, Fail);

        if(possibleNames != null)
        {
            button0.gameObject.GetComponentInChildren<TMPro.TMP_Text>().text = possibleNames[0];
            button1.gameObject.GetComponentInChildren<TMPro.TMP_Text>().text = possibleNames[1];
            button2.gameObject.GetComponentInChildren<TMPro.TMP_Text>().text = possibleNames[2];
            button3.gameObject.GetComponentInChildren<TMPro.TMP_Text>().text = possibleNames[3];
        }
    }

    public void OnButtonHover()
    {
        Utils.AudioPlay("event:/Menu UI/button_hover");
    }

    private void ChooseName(int buttonIndex)
    {
        Utils.AudioPlay("event:/Menu UI/button_click");
        if (buttonIndex == correctNameIndex)
            Success();
        else
            Fail();
    }

    private void Fail()
    {
        GameManager.instance.Damage(damage);
        End();
    }

    private void Success()
    {
        GameManager.instance.Heal(heal);
        End();
    }

    private void End()
    {
        parentPerson.Leave();
        Destroy(gameObject);
    }
}
