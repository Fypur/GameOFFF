using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    [System.Serializable]
    public struct Data
    {
        public Interactions interaction;
        public float popOutTime;
        public Direction poppingOutFrom;
        public bool flippedX;

        public Vector2 interactionPos;
        public Vector2 canvasOffset;
        public float slideTime;

        public Ease.EaseType easeType;
    }

    public Data data;
    private static List<Person> persons;
    private static int addedPersonsCount;

    public Canvas canvas;

    [SerializeField] private GameObject waveUIPrefab;
    [SerializeField] private GameObject nameChooserPrefab;
    [SerializeField] private GameObject agressiveWavePrefab;
    [SerializeField] private OsuSlidersContainer osuSliderContainer;

    private Vector2 spawnPos;

    private void Start()
    {
        spawnPos = transform.position;
        canvas.transform.position += (Vector3)data.canvasOffset;

        if (data.flippedX)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            canvas.transform.localPosition = new Vector3(-canvas.transform.localPosition.x, canvas.transform.localPosition.y, canvas.transform.localPosition.z);
        }

        PopOut();

        canvas.worldCamera = Camera.main;

        if (persons == null)
            persons = new();
        persons.Add(this);
        addedPersonsCount++;
        if (addedPersonsCount % 10 == 0)
        {
            addedPersonsCount = 0;
            for (int i = 0; i < persons.Count; i++)
                persons[i].canvas.sortingOrder = i;
        }
        else
            canvas.sortingOrder = persons[persons.Count - 1].canvas.sortingOrder + 1;
    }

    public void PopOut()
    {
        StartCoroutine(PopOutCoroutine());
    }

    public void Leave()
    {
        StopAllCoroutines();

        StartCoroutine(LeaveCoroutine());
    }

    private IEnumerator PopOutCoroutine()
    {
        yield return Utils.SlideObject(gameObject, data.interactionPos, data.slideTime, data.easeType);

        switch (data.interaction)
        {
            case Interactions.Wave:
                WaveUI waveUI = Instantiate(waveUIPrefab, canvas.transform).GetComponent<WaveUI>();
                waveUI.parentPerson = this;
                break;
            case Interactions.NameChoice:
                NameChooser nameChooser = Instantiate(nameChooserPrefab, canvas.transform).GetComponent<NameChooser>();
                nameChooser.parentPerson = this;
                //TODO: Make this depend on the actual person name etc
                nameChooser.possibleNames = new string[] { "Adam", "Amad", "Axel", "Adem" };
                nameChooser.correctNameIndex = 3;
                break;
            case Interactions.AgressiveWave:
                AgressiveWave agressiveWave = Instantiate(agressiveWavePrefab, canvas.transform).GetComponent<AgressiveWave>();
                agressiveWave.parentPerson = this;
                agressiveWave.waveAmount = GameManager.Instance.agressiveWaveAmount;
                agressiveWave.waveTime = GameManager.Instance.agressiveWaveTime;
                break;
            case Interactions.Osu:
                OsuSlider osuSlider = Instantiate(osuSliderContainer.GetRandomOne(), canvas.transform).GetComponent<OsuSlider>();
                osuSlider.parentPerson = this;
                break;
            default:
                break;
                //throw new NotImplementedException();
        }
    }

    private IEnumerator LeaveCoroutine()
    {
        persons.Remove(this);
        yield return Utils.SlideObject(gameObject, spawnPos, data.slideTime, data.easeType);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(data.interactionPos, 0.3f);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetStaticFields()
    {
        persons = null;
        addedPersonsCount = 0;
    }
}
