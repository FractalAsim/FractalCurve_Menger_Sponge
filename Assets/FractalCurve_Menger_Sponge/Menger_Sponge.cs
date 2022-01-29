using System.Collections.Generic;
using UnityEngine;

public class Menger_Sponge : MonoBehaviour
{
    class Cube : MonoBehaviour
    {
        GameObject Prefab;
        Vector3 Position;
        float Size;

        public void SetPrefab(GameObject prefab)
        {
            Prefab = prefab;
        }
        public void SetPosSize(float posx, float posY, float posZ, float size)
        {
            Position = new Vector3(posx, posY, posZ);
            transform.position = Position;
            Size = size;
            transform.localScale = new Vector3(size, size, size);
        }
        //create 8 new cubes offset from the center
        public List<Cube> Generate(Transform parent)
        {
            List<Cube> boxes = new List<Cube>();
            for (var x = -1; x < 2; x++)
            {
                for (var y = -1; y < 2; y++)
                {
                    for (var z = -1; z < 2; z++)
                    {
                        int sum = Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z);
                        float newSize = Size / 3;

                        if (sum > 1)
                        {
                            GameObject newbox = Instantiate(Prefab, parent);
                            Cube b = newbox.AddComponent<Cube>();
                            b.SetPrefab(Prefab);
                            b.SetPosSize(Position.x + x * newSize, Position.y + y * newSize, Position.z + z * newSize, newSize);
                            boxes.Add(b);
                        }
                    }
                }
            }
            return boxes;
        }
    }

    public GameObject CubePrefab;
    public Vector3 StartPos = new Vector3(0, 0, 0);
    public float StartSize = 200;

    List<Cube> sponges = new List<Cube>();
    int currentstepcount = 0;
    int maxstepcount = 3;
    
    void Start()
    {
        GameObject newcube = Instantiate(CubePrefab,transform);
        Cube b = newcube.AddComponent<Cube>();
        b.SetPrefab(CubePrefab);
        b.SetPosSize(StartPos.x, StartPos.y, StartPos.z, StartSize);
        sponges.Add(b);
    }

    void Update()
    {
        if (currentstepcount < maxstepcount && Input.GetKeyDown(KeyCode.Space))
        {
            //For each existing cube, split off and generate 8 new ones
            List<Cube> nextSpongeList = new List<Cube>();
            foreach (Cube b in sponges)
            {
                nextSpongeList.AddRange(b.Generate(transform));
                Destroy(b.gameObject);
            }

            sponges = nextSpongeList;
            currentstepcount++;
            print("Sponge Count: " + sponges.Count);
        }
    }
}