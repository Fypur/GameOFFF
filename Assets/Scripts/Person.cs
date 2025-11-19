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
    public GameObject Canvas;

    public GameObject waveUIPrefab;

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
        StartCoroutine(Utils.SlideObject(gameObject, spawnPos, data.slideTime, data.easeType));
    }

    private IEnumerator PopOutCoroutine()
    {
        yield return Utils.SlideObject(gameObject, data.interactionPos, data.slideTime, data.easeType);

        switch (data.interaction)
        {
            case Interactions.Wave:
                yield return Wave();
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private IEnumerator Wave()
    {
        WaveUI waveUI = Instantiate(waveUIPrefab, Canvas.transform).GetComponent<WaveUI>();
        waveUI.parentPerson = this;
        yield return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(data.interactionPos, 0.3f);
    }
}
