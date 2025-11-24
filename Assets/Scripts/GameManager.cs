using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Gameplay properties")]
    public float waveTime = 4f;
    public float agressiveWaveTime = 5f;
    public int agressiveWaveAmount = 5;

    [Header("Spawning People params")]
    [SerializeField, Range(0f, 1f)] public float chanceToSpawn = 0.3f;

    [Header("Level Data")]
    [SerializeField] private List<Person.Data> level1Data;

    [Header("Prefabs")]
    [SerializeField] private GameObject PersonPrefab;

    [Header("Initial Person Positions")]
    [SerializeField] private Vector2 leftInitPos;
    [SerializeField] private Vector2 rightInitPos;
    [SerializeField] private Vector2 upInitPos;
    [SerializeField] private Vector2 downInitPos;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //StartLevel(level1Data);
        //StartCoroutine(StartRandomLevel());
    }

    private void StartLevel(List<Person.Data> levelData)
    {
        foreach (Person.Data personData in levelData)
            StartCoroutine(SpawnPersonWait(personData));
    }

    private IEnumerator StartRandomLevel()
    {
        float t = 1;
        while (true)
        {
            if(t <= 0)
            {
                t = 1;

                if(UnityEngine.Random.Range(0f, 1f) <= chanceToSpawn)
                {
                    Person.Data data = new Person.Data();
                    data.slideTime = UnityEngine.Random.Range(1.5f, 4f);
                    data.interaction = Interactions.Wave;
                    data.easeType = Ease.EaseType.QuintInAndOut;
                    data.interactionPos = new Vector2(UnityEngine.Random.Range(-7f, -5f), 0);

                    if(UnityEngine.Random.Range(0f, 1f) < 0.5f)
                    {
                        data.poppingOutFrom = Direction.Left;
                    }
                    else
                    {
                        data.poppingOutFrom = Direction.Right;
                        data.interactionPos.x *= -1;
                        data.flippedX = true;
                    }



                    SpawnPerson(data);
                }
            }

            t -= Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SpawnPersonWait(Person.Data personData)
    {
        yield return new WaitForSeconds(personData.popOutTime);
        SpawnPerson(personData);
    }

    private void SpawnPerson(Person.Data personData)
    {
        Vector2 pos;
        switch (personData.poppingOutFrom)
        {
            case Direction.Right:
                pos = rightInitPos;
                break;
            case Direction.Up:
                pos = upInitPos;
                break;
            case Direction.Down:
                pos = downInitPos;
                break;
            default:
                pos = leftInitPos;
                break;
        }

        Person person = Instantiate(PersonPrefab, (Vector3)pos, Quaternion.identity).GetComponent<Person>();
        person.data = personData;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(leftInitPos, 0.3f);
        Gizmos.DrawWireSphere(rightInitPos, 0.3f);
        Gizmos.DrawWireSphere(downInitPos, 0.3f);
        Gizmos.DrawWireSphere(upInitPos, 0.3f);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetStaticFields()
    {
        Instance = null;
    }
}
