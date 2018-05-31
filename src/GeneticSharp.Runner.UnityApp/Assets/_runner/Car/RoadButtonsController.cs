using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class RoadButtonsController : MonoBehaviour
    {

        public Object RoadButtonPrefab;
        public Vector2 StartPosition = Vector2.zero;

        private void Start()
        {
            var roads = Resources.LoadAll<CarSampleConfig>("Roads");
            var position = StartPosition;

            foreach (var road in roads)
            {
                var go = Instantiate(RoadButtonPrefab, transform) as GameObject;
                go.name = road.name.Replace("Road", string.Empty);
                go.GetComponentInChildren<Text>().text = go.name;

                var r = go.GetComponent<RectTransform>();
                r.localPosition = position;
                position += new Vector2(0, -r.rect.height);

                var button = go.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    CarSampleController.SetConfig(road);
                    SceneManager.LoadScene("CarScene");
                });
            }
        }
    }
}
