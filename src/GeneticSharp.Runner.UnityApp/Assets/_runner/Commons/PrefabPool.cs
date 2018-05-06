using System.Collections.Concurrent;
using UnityEngine;

public class PrefabPool 
{
    private BlockingCollection<GameObject> m_available = new BlockingCollection<GameObject>();
    private BlockingCollection<GameObject> m_unavailable = new BlockingCollection<GameObject>();
    private Object m_prefab;
    private GameObject m_poolContainer;

    public PrefabPool(Object prefab)
    {
        m_prefab = prefab;
        m_poolContainer = new GameObject($"{prefab.name}Pool");
    }

    public GameObject Get(Vector3 position)
    {
        GameObject go;

        if(m_available.TryTake(out go))
        {
            go.transform.position = position;
            go.transform.rotation = Quaternion.identity;
            go.SetActive(true);

            go.SendMessage("OnGetFromPool", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            go = Object.Instantiate(m_prefab, position, Quaternion.identity) as GameObject;
        }

        m_unavailable.Add(go);
        return go;
    }

    public void Release(GameObject go)
    {
        go.transform.SetParent(m_poolContainer.transform);
        m_available.Add(go);

        go.SendMessage("OnRelaseToPool", SendMessageOptions.DontRequireReceiver);
    }

    public void ReleaseAll()
    {
        while(m_unavailable.Count > 0)
        {
            m_available.Add(m_unavailable.Take());
        }
    }
}
