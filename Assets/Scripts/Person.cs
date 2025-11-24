using System;
using System.Collections;
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
        public float slideTime;

        public Ease.EaseType easeType;
    }

    public Data data;
    [SerializeField] private GameObject Canvas;

    [SerializeField] private GameObject waveUIPrefab;
    [SerializeField] private GameObject nameChooserPrefab;
    [SerializeField] private GameObject agressiveWavePrefab;

    private Vector2 spawnPos;

    private void Start()
    {
        spawnPos = transform.position;

        if (data.flippedX)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            Canvas.transform.localPosition = new Vector3(-Canvas.transform.localPosition.x, Canvas.transform.localPosition.y, Canvas.transform.localPosition.z);
        }

        PopOut();
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
                WaveUI waveUI = Instantiate(waveUIPrefab, Canvas.transform).GetComponent<WaveUI>();
                waveUI.parentPerson = this;
                break;
            case Interactions.NameChoice:
                NameChooser nameChooser = Instantiate(nameChooserPrefab, Canvas.transform).GetComponent<NameChooser>();
                nameChooser.parentPerson = this;
                //TODO: Make this depend on the actual person name etc
                nameChooser.possibleNames = new string[] { "Adam", "Amad", "Axel", "Adem" };
                nameChooser.correctNameIndex = 3;
                break;
            case Interactions.AgressiveWave:
                AgressiveWave agressiveWave = Instantiate(agressiveWavePrefab, Canvas.transform).GetComponent<AgressiveWave>();
                agressiveWave.parentPerson = this;
                agressiveWave.waveAmount = GameManager.Instance.agressiveWaveAmount;
                agressiveWave.waveTime = GameManager.Instance.agressiveWaveTime;
                break;
            default:
                break;
                //throw new NotImplementedException();
        }
    }

    private IEnumerator LeaveCoroutine()
    {
        yield return Utils.SlideObject(gameObject, spawnPos, data.slideTime, data.easeType);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(data.interactionPos, 0.3f);
    }
}
