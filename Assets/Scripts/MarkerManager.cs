using UnityEngine;

public class MarkerManager : MonoBehaviour
{
    [System.Serializable]
    public class MarkerTarget
    {
        public Transform target;
        public Sprite icon;
    }

    public GameObject markerPrefab;
    public Transform canvas;
    public MarkerTarget[] targets;

    void Start()
    {
        foreach (MarkerTarget t in targets)
        {
            GameObject marker = Instantiate(markerPrefab, canvas);

            MarkerUI ui = marker.GetComponent<MarkerUI>();
            ui.target = t.target;
            ui.icon.sprite = t.icon;
        }
    }
}