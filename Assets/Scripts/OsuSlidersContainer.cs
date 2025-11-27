using UnityEngine;

[CreateAssetMenu(fileName = "OsuSlidersContainer", menuName = "Scriptable Objects/OsuSlidersContainer")]
public class OsuSlidersContainer : ScriptableObject
{
    public GameObject[] osuSliders;

    public GameObject GetRandomOne()
        => osuSliders[UnityEngine.Random.Range(0, osuSliders.Length)];
}
