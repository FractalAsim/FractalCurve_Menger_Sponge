using System.Collections.Generic;
using UnityEngine;

//TODO add chunks
public class Menger_Sponge_CombineMesh : MonoBehaviour
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

    MeshFilter meshFilter;
    GameObject spongeParent;
    List<Cube> sponges = new List<Cube>();
    int currentstepcount = 0;
    int maxstepcount = 3;

    void Awake()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        if(!meshFilter) meshFilter = gameObject.AddComponent<MeshFilter>();
        if(!gameObject.GetComponent<MeshRenderer>()) gameObject.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
    }
    void Start()
    {
        spongeParent = new GameObject("Sponges");
        spongeParent.transform.parent = transform;

        GameObject newcube = Instantiate(CubePrefab, spongeParent.transform);
        Cube b = newcube.AddComponent<Cube>();
        b.SetPrefab(CubePrefab);
        b.SetPosSize(StartPos.x, StartPos.y, StartPos.z, StartSize);
        sponges.Add(b);

        meshFilter.mesh = CombineMesh(spongeParent);
    }

    void Update()
    {
        if (currentstepcount < maxstepcount && Input.GetKeyDown(KeyCode.Space))
        {
            //For each existing cube, split off and generate 8 new ones
            List<Cube> nextSpongeList = new List<Cube>();
            foreach (Cube b in sponges)
            {
                nextSpongeList.AddRange(b.Generate(spongeParent.transform));
                Destroy(b.gameObject);
            }

            sponges = nextSpongeList;
            meshFilter.mesh = CombineMesh(spongeParent);
            currentstepcount++;
            print("Sponge Count: " + sponges.Count);
        }
    }
    /// <summary>
    /// Combine the mesh of the gameobjects and all of its child meshes and returns the result
    /// </summary>
    Mesh CombineMesh(GameObject containerMesh)
    {
        MeshFilter[] meshFilters = containerMesh.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }

        Mesh CombinedMesh = new Mesh();
        CombinedMesh.CombineMeshes(combine);
        return CombinedMesh;
    }
}