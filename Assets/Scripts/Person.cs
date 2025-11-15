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

        public Vector2 interactionPos;
        public float slideTime;

        public Ease.EaseType easeType;
    }

    public Data data;
    public GameObject Canvas;

    public GameObject waveUIPrefab;

    private void Start()
    {
        PopOut();
    }

    public void PopOut()
    {
        StartCoroutine(PopOutCoroutine());
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
        Instantiate(waveUIPrefab, Canvas.transform);
        yield return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(data.interactionPos, 0.3f);
    }
}
