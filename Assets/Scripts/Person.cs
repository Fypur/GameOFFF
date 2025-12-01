using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameManager;

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
        public GameManager.NameChooserSettings nameChooserSettings;
    }

    public Data data;

    public Canvas canvas;

    [SerializeField] private SpriteRenderer accessorySprite;

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

        if (GameManager.instance.persons == null)
            GameManager.instance.persons = new();
        GameManager.instance.persons.Add(this);
        GameManager.instance.addedPersonsCount++;
        if (GameManager.instance.addedPersonsCount % 10 == 0)
        {
            GameManager.instance.addedPersonsCount = 0;
            for (int i = 0; i < GameManager.instance.persons.Count; i++)
                GameManager.instance.persons[i].canvas.sortingOrder = i;
        }
        else
            canvas.sortingOrder = GameManager.instance.persons[GameManager.instance.persons.Count - 1].canvas.sortingOrder + 1;
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
                SetupNameChooser(nameChooser);
                break;
            case Interactions.AgressiveWave:
                AgressiveWave agressiveWave = Instantiate(agressiveWavePrefab, canvas.transform).GetComponent<AgressiveWave>();
                agressiveWave.parentPerson = this;
                agressiveWave.waveAmount = GameManager.instance.agressiveWaveAmount;
                agressiveWave.waveTime = GameManager.instance.agressiveWaveTime;
                break;
            case Interactions.Osu:
                OsuSlider osuSlider = Instantiate(osuSliderContainer.GetRandomOne(), canvas.transform).GetComponent<OsuSlider>();
                osuSlider.parentPerson = this;
                break;
            default:
                break;
                //throw new NotImplementedException();
        }

        Utils.AudioPlay("event:/Ambience/person_greeting");
        Utils.AudioPlay("event:/Interactions/pop_appear");
    }

    private IEnumerator LeaveCoroutine()
    {
        GameManager.instance.persons.Remove(this);
        yield return Utils.SlideObject(gameObject, spawnPos, data.slideTime, data.easeType);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(data.interactionPos, 0.3f);
    }

    private void SetupNameChooser(NameChooser nameChooser)
    {
        string[] possibleNames = new string[4];
        for (int i = 0; i < possibleNames.Length - 1; i++)
            possibleNames[i] = data.nameChooserSettings.fakeNames[i];
        possibleNames[possibleNames.Length - 1] = data.nameChooserSettings.name;

        Utils.Shuffle(possibleNames);

        int correctIndex = 0;
        for(int i = 1; i < possibleNames.Length; i++)
        {
            if (possibleNames[i] == data.nameChooserSettings.name)
            {
                correctIndex = i;
                break;
            }
        }

        nameChooser.parentPerson = this;
        nameChooser.possibleNames = possibleNames;
        nameChooser.correctNameIndex = correctIndex;

        //SET ACCESSORY SPRITE HERE
        //accessorySprite.gameObject.SetActive(true);
        /*switch (data.nameChooserSettings.accessory)
        {
            case Accessories.Hat:
                accessorySprite.sprite = 
        }*/
    }
}
