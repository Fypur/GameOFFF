using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Gameplay properties")]
    public float waveTime = 4f;
    public float agressiveWaveTime = 5f;
    public int agressiveWaveAmount = 5;
    public float nameChooserTime = 7f;

    [Header("Level Data")]
    [SerializeField] private PersonSpawnSettings spawnSettings;
    [SerializeField] private List<Person.Data> level1Data;

    [Header("Prefabs")]
    [SerializeField] private GameObject PersonPrefab;

    [Header("Initial Person Positions")]
    [SerializeField] private Vector2 leftInitPos;
    [SerializeField] private Vector2 rightInitPos;
    [SerializeField] private Vector2 upInitPos;
    [SerializeField] private Vector2 downInitPos;

    [Header("Misc")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private SlidingDoors slidingDoorsPrefab;

    [HideInInspector] public List<Person> persons;
    [HideInInspector] public int addedPersonsCount;

    private bool levelFinished;

    [System.Serializable] private class PersonSpawnSettings
    {
        [Range(0f, 1f)] public float chanceToSpawn = 0.4f;
        public float trySpawnEveryXSeconds = 1.5f;
        public float waveChance = 0.7f;
        public float agressiveWave = 0.1f;
        public float osuSlider = 0.1f;
        public float nameChooser = 0.1f;
    }

    private void Awake()
    {
        RuntimeManager.GetBus("bus:/").setVolume(0);
        instance = this;

        if (SlidingDoors.instance != null)
            SlidingDoors.instance.OnBeginLoadSceneOpen.AddListener(StartLevel);
        else
            StartLevel();
    }

    private void Start()
    {
        //StartLevel(level1Data);
        /*MAKE SLIDING DOORS ON SCENE OPEN AND CLOSE
            -> LOSING AND WINNING SCREENS*/
        
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.oKey.wasPressedThisFrame)
            SlidingDoors.instance.ClosedLoadSceneOpen("Menu");
    }

    public void StartLevel()
    {
        StartCoroutine(StartRandomLevel());
        GetComponent<StudioEventEmitter>().Play();
    }

    private void StartLevel(List<Person.Data> levelData)
    {
        foreach (Person.Data personData in levelData)
            StartCoroutine(SpawnPersonWait(personData));
    }

    private IEnumerator StartRandomLevel()
    {
        float t = spawnSettings.trySpawnEveryXSeconds;
        while (true)
        {
            if(t <= 0)
            {
                t = spawnSettings.trySpawnEveryXSeconds;

                if (UnityEngine.Random.Range(0f, 1f) <= spawnSettings.chanceToSpawn)
                {
                    Person.Data data = new Person.Data();
                    data.slideTime = UnityEngine.Random.Range(1.5f, 4f);

                    float r = UnityEngine.Random.value;
                    if (r <= spawnSettings.waveChance)
                        data.interaction = Interactions.Wave;
                    else if (r <= spawnSettings.waveChance + spawnSettings.agressiveWave)
                        data.interaction = Interactions.AgressiveWave;
                    else if (r <= spawnSettings.waveChance + spawnSettings.agressiveWave + spawnSettings.nameChooser)
                        data.interaction = Interactions.NameChoice;
                    else if (r <= spawnSettings.waveChance + spawnSettings.agressiveWave + spawnSettings.nameChooser + spawnSettings.osuSlider)
                        data.interaction = Interactions.Osu;


                    data.easeType = Ease.EaseType.QuintInAndOut;
                    data.interactionPos = new Vector2(UnityEngine.Random.Range(-8f, -5f), 0);

                    data.canvasOffset = new Vector2(0, (UnityEngine.Random.value - 1) * 2f);

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

    public void Damage(float damage)
    {
        if (levelFinished)
            return;

        healthBar.Damage(damage);
        StartCoroutine(Utils.Shake(Camera.main.gameObject, 0.2f, 0.1f));
    }

    public void Heal(float amount)
    {
        if (levelFinished)
            return;

        healthBar.Heal(amount);
    }

    public void OnEndLevel(bool win)
    {
        levelFinished = true;
        StopAllCoroutines();

        Action callback;
        if (win)
            callback = WinDeath.instance.OnWin;
        else
            callback = WinDeath.instance.OnDeath;

        if (SlidingDoors.instance == null)
            Instantiate(slidingDoorsPrefab);

        StartCoroutine(SlidingDoors.instance.FinishLevel(SceneManager.GetActiveScene().name, callback));
    }

    public void LevelSuccess()
        => OnEndLevel(true);


    public void Death()
        => OnEndLevel(false);


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
        instance = null;
    }
}
