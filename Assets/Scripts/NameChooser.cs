using UnityEngine;
using UnityEngine.UI;

public class NameChooser : MonoBehaviour
{
    public Person parentPerson;
    public string[] possibleNames;
    public int correctNameIndex;

    [SerializeField] private Button button0;
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;

    private void Awake()
    {
        button0.onClick.AddListener(() => ChooseName(0));
        button1.onClick.AddListener(() => ChooseName(1));
        button2.onClick.AddListener(() => ChooseName(2));
        button3.onClick.AddListener(() => ChooseName(3));
    }

    private void ChooseName(int buttonIndex)
    {
        if (buttonIndex == correctNameIndex)
            Correct();
        else
            Mistake();

        parentPerson.Leave();
        Destroy(gameObject);
    }

    private void Mistake()
    {
        //TODO: Apply damage here
    }

    private void Correct()
    {
        
    }
}
