using System.Collections;
using System.Collections.Generic;
using GeneticSharp.Extensions.Tsp;
using UnityEngine;

public class CityController : MonoBehaviour {

    private Vector3 screenPoint;
    private Vector3 offset;

    public TspCity Data { get; set;}

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

        transform.localScale *= 2f;
    }

    void OnMouseUp()
    {
        transform.localScale /= 2f;
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;

        Data.X = transform.position.x;
        Data.Y = transform.position.y;
    }

    void Update()
    {
        transform.position = new Vector2(Data.X, Data.Y);
    }
}
