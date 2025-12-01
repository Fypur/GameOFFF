using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Gameplay properties")]
    public float waveTime = 4f;
    public float agressiveWaveTime = 5f;
    public int agressiveWaveAmount = 5;
    public float nameChooserTime = 7f;

    [Header("Level Data")]
    public bool showTutorial;
    public bool showOsuTutorial;
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
    [SerializeField] private IDCards IDCards;
    [SerializeField] private SlidingDoors slidingDoorsPrefab;
    [SerializeField] private GameObject explosionParticle;

    [HideInInspector] public List<Person> persons;
    [HideInInspector] public int addedPersonsCount;

    public static EventInstance music;
    private bool levelFinished;
    private int nameChooserIndex;
    private Vector2 originalIdPos;

    [System.Serializable] private class PersonSpawnSettings
    {
        [Range(0f, 1f)] public float chanceToSpawn = 0.4f;
        public float trySpawnEveryXSeconds = 1.5f;
        public float waveChance = 0.7f;
        public float agressiveWave = 0.1f;
        public float osuSlider = 0.1f;
        public float nameChooser = 0.1f;

        public NameChooserSettings[] nameChooserSettings;
    }

    [System.Serializable] public class NameChooserSettings
    {
        public string name;
        public string[] fakeNames;
        public Accessories accessory;
    }
    
    private void Awake()
    {
        instance = this;
        if (!music.isValid())
            music = RuntimeManager.CreateInstance("event:/Music/level_music_loop");

        nameChooserIndex = UnityEngine.Random.Range(0, spawnSettings.nameChooserSettings.Length);

        if (SlidingDoors.instance != null)
            SlidingDoors.instance.OnBeginLoadSceneOpen.AddListener(StartLevel);
        else
            StartLevel();
    }

    public void StartLevel()
    {
        if(spawnSettings.nameChooserSettings.Length == 0)
        {
            if(SlidingDoors.instance != null)
                SlidingDoors.instance.Open();
            StartCoroutine(StartRandomLevel());
        }
        else
        {
            for (int i = spawnSettings.nameChooserSettings.Length; i < IDCards.ids.Length; i++)
                IDCards.ids[i].SetActive(false);
            for (int i = 0; i < spawnSettings.nameChooserSettings.Length; i++)
            {
                IDCards.ids[i].GetComponentInChildren<TMP_Text>().text = spawnSettings.nameChooserSettings[i].name;
                //SET ACCESSORY SPRITE HERE
                //IDCards.ids[i].GetComponentInChildren<Image>().sprite = accessorySprite;
            }


            originalIdPos = IDCards.GetComponent<RectTransform>().anchoredPosition;
            StartCoroutine(Utils.SlideUIObject(IDCards.GetComponent<RectTransform>(), Vector2.zero, 1f, Ease.EaseType.CubicOut));
            if(SlidingDoors.instance != null)
                IDCards.playButton.onClick.AddListener(SlidingDoors.instance.Open);
            
            IDCards.playButton.onClick.AddListener(StartLevelAfterID);
        }
    }

    public void StartLevelAfterID()
    {
        StartCoroutine(Utils.SlideUIObject(IDCards.GetComponent<RectTransform>(), originalIdPos, 1f, Ease.EaseType.CubicIn));
        StartCoroutine(StartRandomLevel());
    }

    private void StartLevel(List<Person.Data> levelData)
    {
        foreach (Person.Data personData in levelData)
            StartCoroutine(SpawnPersonWait(personData));
    }

    private IEnumerator StartRandomLevel()
    {
        //UNCOMMENT THIS TO PLAY MUSIC
        music.getPlaybackState(out PLAYBACK_STATE playbackState);
        if (playbackState != PLAYBACK_STATE.PLAYING)
            music.start();
        else
            StartCoroutine(TapeStop(0f, 1.5f));


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
                    {
                        data.interaction = Interactions.NameChoice;
                        data.nameChooserSettings = spawnSettings.nameChooserSettings[nameChooserIndex++];
                        if (nameChooserIndex >= spawnSettings.nameChooserSettings.Length)
                            nameChooserIndex = 0;
                    }
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

        Utils.AudioPlay("event:/Interactions/pop_fail");

        healthBar.Damage(damage);
        StartCoroutine(Utils.Shake(Camera.main.gameObject, 0.2f, 0.1f));

        music.setParameterByName("Pitch_Up", 1f, true);
        music.setParameterByName("Pitch_Up", 0f, false);
    }

    public void Heal(float amount)
    {
        if (levelFinished)
            return;

        Utils.AudioPlay("event:/Interactions/pop_succeed");

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Instantiate(explosionParticle, mousePos, Quaternion.identity);

        healthBar.Heal(amount);
    }

    public void OnEndLevel(bool win)
    {
        levelFinished = true;
        StopAllCoroutines();

        if (SlidingDoors.instance == null)
            Instantiate(slidingDoorsPrefab);

        StartCoroutine(WinPopPeople(win));
    }

    private IEnumerator TapeStop(float endValue, float time)
    {
        music.getParameterByName("Tape_Stop", out float startValue);
        float t = 0;
        while(t <= time)
        {
            music.setParameterByName("Tape_Stop", Mathf.Lerp(startValue, endValue, t / time), false);
            t += Time.deltaTime * Time.timeScale;
            yield return null;
        }

        music.setParameterByName("Tape_Stop", endValue, false);
    }

    private IEnumerator WinPopPeople(bool win)
    {
        Action callback;
        if (win)
        {
            callback = WinDeath.instance.OnWin;
            music.stop(STOP_MODE.ALLOWFADEOUT);
            music.release();

        }
        else
        {
            callback = WinDeath.instance.OnDeath;
            StartCoroutine(TapeStop(1f, 3f));
        }

        if (win)
        {
            for (int i = persons.Count - 1; i >= 0; i--)
            {
                if (persons[i] == null)
                    continue;

                Instantiate(explosionParticle, persons[i].transform.position, Quaternion.identity);
                Utils.AudioPlay("event:/Interactions/pop_appear");
                Destroy(persons[i].gameObject);
                yield return new WaitForSeconds(0.2f);
            }
        }

        yield return SlidingDoors.instance.FinishLevel(SceneManager.GetActiveScene().name, callback);
    }

    private void Update()
    {
        if (Keyboard.current.oKey.wasPressedThisFrame)
            LevelSuccess();
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
        music.stop(STOP_MODE.IMMEDIATE);
        music.release();
    }
}
