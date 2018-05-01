using System.Collections.Concurrent;
using UnityEngine;

public class PrefabPool 
{
    private BlockingCollection<GameObject> m_available = new BlockingCollection<GameObject>();
    private BlockingCollection<GameObject> m_unavailable = new BlockingCollection<GameObject>();
    private Object Prefab;

    public PrefabPool(Object prefab)
    {
        Prefab = prefab;
    }

    public GameObject Get(Vector3 position)
    {
        GameObject go;

        if(m_available.TryTake(out go))
        {
            go.transform.position = position;
            go.transform.rotation = Quaternion.identity;
            go.SetActive(true);
        }
        else
        {
            go = Object.Instantiate(Prefab, position, Quaternion.identity) as GameObject;
        }

        m_unavailable.Add(go);
        return go;
    }

    public void Release(GameObject go)
    {
        go.transform.SetParent(null);
        m_available.Add(go);
    }

    public void ReleaseAll()
    {
        while(m_unavailable.Count > 0)
        {
            m_available.Add(m_unavailable.Take());
        }
    }
}
