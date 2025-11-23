using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject PersonPrefab;

    public Vector2 leftInitPos;
    public Vector2 rightInitPos;
    public Vector2 upInitPos;
    public Vector2 downInitPos;

    public List<Person.Data> level1Data;

    public float chanceToSpawn = 0.3f;

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
}
