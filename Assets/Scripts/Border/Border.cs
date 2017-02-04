using System.Collections.Generic;
using UnityEngine;
using System;

public class Border : MonoBehaviour {

    public static float BORDER_DRAW_DELAY = 0.5f; // How much the draw of border should be delayed
    public static string FILE_PATH;

    protected static GameObject borderPrefab;

    protected static GameObject bordersParent;
    protected static GameObject bordersPoolingParent;

    protected static List<BorderStorageLogic> bordersNotShown;

    protected static List<GameObject> bordersShown;
    protected static Stack<GameObject> notUsedBorders;

    public void Start() {
        // Inits
        bordersShown = new List<GameObject>();
        bordersNotShown = new List<BorderStorageLogic>();
        notUsedBorders = new Stack<GameObject>();

        FILE_PATH = Application.persistentDataPath + "/Borders.txt";

        LoadResources();

        bordersParent = new GameObject("Borders");
        bordersParent.transform.parent = transform;
        bordersPoolingParent = new GameObject("BordersCellPooling");
        bordersPoolingParent.transform.parent = bordersParent.transform;
    }

    void OnApplicationPause(bool paused) {
        if (!paused) {
            LoadResources();
        }
    }

    void LoadResources() {
        borderPrefab = Resources.Load("Prefabs/Wall") as GameObject;
    }

    /// <summary>
    /// Write to FILE_PATH the data of borders
    /// </summary>
    public static void WriteBordersToFile() {
        // Add currently visible borders
        foreach (GameObject obj in bordersShown) {
            BorderStorage bs = obj.GetComponent<BorderStorage>();

            bordersNotShown.Add(bs.bsl);
        }

        string[] lines = new string[bordersNotShown.Count * 2 + 1];
        lines[0] = bordersNotShown.Count.ToString();

        // Order of data
        // Line 1: Count of borders
        // Others 1: CountOfBorderPoints borderPoint1x borderPoint 1y borderPoint 2x ... colorOfBorderR colorOfBorderG colorOfBorderB
        // Others 2: winLineStartX winLineStartY winLineEndX winLineEndY
        // Other 1 and two repeats countOfBorders times 
        string s = "";
        for (int k = 0; k < bordersNotShown.Count; k++) {
            s = bordersNotShown[k].Points.GetLength(0) + " ";
            for (int i = 0; i < bordersNotShown[k].Points.GetLength(0); i++) {
                s += bordersNotShown[k].Points[i, 0] + " ";
                s += bordersNotShown[k].Points[i, 1] + (i == bordersNotShown[k].Points.Length - 1 ? "" : " ");
            }
            
            lines[1 + k * 2] = s + bordersNotShown[k].Color.r + " " + bordersNotShown[k].Color.g + " " + bordersNotShown[k].Color.b;
            lines[2 + k * 2] = bordersNotShown[k].WinLinePoints[0, 0] + " " + bordersNotShown[k].WinLinePoints[0, 1] + " " + bordersNotShown[k].WinLinePoints[1, 0] + " " + bordersNotShown[k].WinLinePoints[1, 1];
        }

        try {
            System.IO.File.WriteAllLines(FILE_PATH, lines);
        } catch (Exception e) {
            Debug.LogError(e.StackTrace);
        }

        // Remove the ones we added
        for (int i = 0; i < bordersShown.Count; i++) {
            bordersNotShown.RemoveAt(bordersNotShown.Count - 1);
        }
    }

    /// <summary>
    /// Read from FILE_PATH the stored data
    /// </summary>
    public static void ReadBordersFromFile() {
        // Order of data written in WriteBorder Method
        try {
            string[] lines = System.IO.File.ReadAllLines(FILE_PATH);
            if (lines.Length == 0) return;
            int count = int.Parse(lines[0]);

            string[] line;
            for (int i = 0; i < count; i++) {
                line = lines[1 + 2 * i].Split(' ');

                string s = "";
                int[,] points = new int[int.Parse(line[0]), 2];
                int at = 1;
                for (int k = 0; k < points.GetLength(0); k++) {
                    points[k, 0] = int.Parse(line[at]);
                    points[k, 1] = int.Parse(line[at + 1]);
                    s += line[at] + " " + line[at + 1] + " ";
                    at += 2;
                }

                Color c = new Color(
                    float.Parse(line[line.Length - 3]),
                    float.Parse(line[line.Length - 2]),
                    float.Parse(line[line.Length - 1])
                );

                line = lines[2 + 2 * i].Split(' ');
                float[,] winLinePoints = new float[2, 2];
                for (int k = 0; k < winLinePoints.GetLength(0); k++)
                    for (int j = 0; j < winLinePoints.GetLength(1); j++)
                        winLinePoints[k, j] = float.Parse(line[k * 2 + j]);
                
                bordersNotShown.Add(new BorderStorageLogic(points, winLinePoints, c));
            }
        } catch (Exception e) {
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// Called from Grid class every second or so because I wanted to keep it consistent and the partion
    /// shown stuff is called there as well so why not call this there too.. Maybe not the best idea but
    /// if I start searching for it I'd go there first
    /// </summary>
    /// <param name="leftBottomPos"></param>
    /// <param name="topRightPos"></param>
    public static void UpdateBordersShown(int[] leftBottomPos, int[] topRightPos) {
        // We go backwards because we remove some objects while looping
        // Move all of the ones we see to the shown list
        for (int i = bordersNotShown.Count - 1; i >= 0; i--) {
            // If we need to show it then move it to the shown list
            if (bordersNotShown[i].MiddlePos[0] >= leftBottomPos[0] && bordersNotShown[i].MiddlePos[0] <= topRightPos[0] &&
                bordersNotShown[i].MiddlePos[1] >= leftBottomPos[1] && bordersNotShown[i].MiddlePos[1] <= topRightPos[1]) {

                MoveToShown(i, false);
            }
        }

        // Move the ones we don't see to the not seen
        // Go backwards because we delete some items while looping
        for (int i = bordersShown.Count - 1; i >= 0; i--) {
            // If we cannot see the border anymore
            // we calculate it based on the gameobjects pos because we set it to middlepos when instatiating from pool
            if (bordersShown[i].transform.position.x < leftBottomPos[0] || bordersShown[i].transform.position.x > topRightPos[0] ||
                bordersShown[i].transform.position.y < leftBottomPos[1] || bordersShown[i].transform.position.y > topRightPos[1]) {

                MoveToNotShown(i);
            }
        }
    }

    public static void AddBorderPoints(int[,] points, float[,] winLinePoints, Color color) {
        BorderStorageLogic b = new BorderStorageLogic(points, winLinePoints, color);

        bordersNotShown.Add(b);
        MoveToShown(bordersNotShown.Count - 1);
    }

    /// <summary>
    /// Moves border gameobject from notshown list to shown list
    /// </summary>
    /// <param name="index"></param>
    protected static void MoveToNotShown(int index) {
        // Remove index from bordersshown
        GameObject go = bordersShown[index];
        bordersShown.RemoveAt(index);

        // Store data we need
        BorderStorageLogic bsl = go.GetComponent<BorderStorage>().bsl;
        bordersNotShown.Add(bsl);

        // Reset AnimatedLineRenderer
        AnimatedLineRenderer alr = go.GetComponent<AnimatedLineRenderer>();
        alr.Reset();

        // Reset linerenderer
        go.GetComponent<LineRenderer>().numPositions = 0;

        // Reset Win line as well
        alr = go.transform.GetChild(0).gameObject.GetComponent<AnimatedLineRenderer>();
        alr.Reset();

        // Set parent
        go.transform.parent = bordersPoolingParent.transform;

        // Use pooling
        go.SetActive(false);
        notUsedBorders.Push(go);
    }

    /// <summary>
    /// Move border from notshown to shown
    /// </summary>
    /// <param name="index"></param>
    /// <param name="drawOut">Whether to play the drawin animation</param>
    protected static void MoveToShown(int index, bool drawOut = true) {
        // Remove index from the notshown list
        BorderStorageLogic bsl = bordersNotShown[index];
        bordersNotShown.RemoveAt(index);

        // Use pooling
        GameObject borderObject;
        if (notUsedBorders.Count > 0) {
            borderObject = notUsedBorders.Pop();
        } else {
            borderObject = Instantiate(borderPrefab);
        }

        // Set the gameobject active
        borderObject.SetActive(true);
        // Set parent
        borderObject.transform.parent = bordersParent.transform;

        // Set pos to middlepos
        borderObject.transform.position = new Vector3(bsl.MiddlePos[0], bsl.MiddlePos[1], borderObject.transform.position.z);
        
        // Store Data in Gameobject so later we can read it
        BorderStorage bs = borderObject.GetComponent<BorderStorage>();
        bs.SetData(bsl);

        LineRenderer lr = borderObject.GetComponent<LineRenderer>();
        // Whether we want to draw out the border or not
        if (drawOut) { 
            // Draw line animation
            AnimatedLineRenderer lineRenderer = borderObject.GetComponent<AnimatedLineRenderer>();
            for (int i = 0; i < bsl.Points.GetLength(0); i++)
                lineRenderer.Enqueue(bsl.GetPosAt(i));
        } else {
            lr.numPositions = bsl.Points.GetLength(0);
            for (int i = 0; i < bsl.Points.GetLength(0); i++)
                lr.SetPosition(i, bsl.GetPosAt(i));
        }

        lr.material.SetColor("_EmissionColor", bsl.Color);

        // Set color
        lr.startColor = bsl.Color;
        lr.endColor = bsl.Color;

        // Win Line Animation
        AnimatedLineRenderer winLineRenderer = borderObject.transform.GetChild(0).gameObject.GetComponent<AnimatedLineRenderer>();
        winLineRenderer.Enqueue(new Vector3(bsl.WinLinePoints[0, 0], bsl.WinLinePoints[0, 1]));
        winLineRenderer.Enqueue(new Vector3(bsl.WinLinePoints[1, 0], bsl.WinLinePoints[1, 1]));


        // Move to list
        bordersShown.Add(borderObject);
    }

    public struct BorderStorageLogic {
        private Color color;
        public Color Color {
            get { return color; }
        }

        private int[,] points;
        public int[,] Points {
            set {
                points = value;
                CalculateMinMaxPos();
            }
            get {
                return points;
            }
        }

        private float[,] winLinePoints;
        public float[,] WinLinePoints {
            get { return winLinePoints; }
            set { winLinePoints = value; }
        }

        private int[] minPos;
        public int[] MinPos { get { return minPos; } }
        private int[] maxPos;
        public int[] MaxPos { get { return maxPos; } }
        private int[] middlePos;
        public int[] MiddlePos { get { return middlePos; } }

        public BorderStorageLogic(int[,] points, float[,] winLinePoints, Color color) {
            minPos = new int[2];
            minPos[0] = int.MaxValue; minPos[1] = int.MaxValue;
            maxPos = new int[2];
            maxPos[0] = int.MinValue; maxPos[1] = int.MinValue;
            middlePos = new int[2];

            this.color = color;

            this.winLinePoints = winLinePoints;

            this.points = points;
            CalculateMinMaxPos();
        }

        public Vector3 GetPosAt(int index) {
            return new Vector3(points[index, 0], points[index, 1], AnimatedLineRenderer.ZPositionStatic);
        }

        /// <summary>
        /// The name is misleading because it calculates middlePos as well ha
        /// </summary>
        private void CalculateMinMaxPos() {
            for (int i = 0; i < points.GetLength(0); i++) {
                if (points[i, 0] < minPos[0]) minPos[0] = points[i, 0];
                if (points[i, 0] > maxPos[0]) maxPos[0] = points[i, 0];

                if (points[i, 1] < minPos[1]) minPos[1] = points[i, 1];
                if (points[i, 1] > maxPos[1]) maxPos[1] = points[i, 1];
            }

            middlePos[0] = (minPos[0] + maxPos[0]) / 2;
            middlePos[1] = (minPos[1] + maxPos[1]) / 2;
        }

        /// <summary>
        /// Returns it ready for bluetooth use
        /// </summary>
        public override string ToString() {
            string s = points.GetLength(0).ToString() + "#" +
            winLinePoints[0, 0].ToString() + "#" + winLinePoints[0, 1].ToString() + "#" + winLinePoints[1, 0].ToString() + "#" + winLinePoints[1, 1].ToString() + "#";
            for (int i = 0; i < points.GetLength(0); i++)
                s += points[i, 0].ToString() + "#" + points[i, 1].ToString() + "#";
            s += color.r.ToString() + "#" + color.g.ToString() + "#" + color.b.ToString();

            return s;
        }
    }
}
