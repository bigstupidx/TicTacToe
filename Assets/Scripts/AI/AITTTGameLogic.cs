using System.Threading;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class AITTTGameLogic : TTTGameLogic {
    public float aiMinThinkTime = 0.7f;
    private float aiTillThinkTime = 0f;

    private AIScript aiScript;

    private Thread aiThread;
    private int[] aiPlacePos;

    private ScoringScript scoring;
    private int humanPoint = 0;
    private int aiPoint = 0;

    public override void Start() {
        base.Start();

        aiScript = GetComponent<AIScript>();
        scoring = FindObjectOfType<ScoringScript>();
        whoseTurn = AIScript.HumanType;
    }

    public override void NextPerson() {
        base.NextPerson();
    }

    public override void NextTurn(int[] gridPos, out Cell.CellOcc won) {
        base.NextTurn(gridPos, out won);

        // If no one has won and it is AI's turn make AI place
        if (won == Cell.CellOcc.BLOCKED && whoseTurn == AIScript.AIType) {
            AIsTurn();
        } else if (whoseTurn == AIScript.AIType && won != Cell.CellOcc.BLOCKED) { // if player has won put a delay before AI puts
            Invoke("AiPlaceRandom", 3f);
        }

        // Set points
        if (won == AIScript.AIType) aiPoint++;
        else if (won == AIScript.HumanType) humanPoint++;

        if (AIScript.AIType == Cell.CellOcc.X) scoring.SetScore(aiPoint, humanPoint);
        else scoring.SetScore(humanPoint, aiPoint);
    }

    /// <summary>
    /// Makes AI place at a random pos that is nearby the first game
    /// </summary>
    private void AiPlaceRandom() {
        int[] pos = aiScript.PlaceDownRandom();
        if (WantToPlaceAt(new Vector2(pos[0], pos[1]))) {
            Camera.main.transform.DOMove(new Vector3(pos[0], pos[1], Camera.main.transform.position.z), 1f);
        }
    }

    void Update() {
        // If aithread is not working anymore but it has given a position place there
        if (aiThread != null && !aiThread.IsAlive && aiPlacePos != null) {
            if (Time.time < aiTillThinkTime) { // We want the ai to at least make it like it think for aiMinThinkTime so if it didn't think for that much time make it only execute it after a while
                // Also add a little randomness just so it's not rythmical
                StartCoroutine(ProcessAIData(aiTillThinkTime - Time.time + Random.Range(0f, aiMinThinkTime * 0.4f), new int[] { aiPlacePos[0], aiPlacePos[1] }));
            } else {
                ProcessAIData(new int[] { aiPlacePos[0], aiPlacePos[1] });
            }
            aiPlacePos = null;
        }
    }

    private void ProcessAIData(int[] pos) {
        WantToPlaceAt(new Vector2(pos[0], pos[1]));

        // If it's out of camera's bounds move camera there
        if (!grid.IsInCameraSight(pos)) {
            // Move camera there
            Camera.main.transform.DOMove(new Vector3(pos[0], pos[1], Camera.main.transform.position.z), 
                Vector2.Distance(new Vector2(pos[0], pos[1]), Camera.main.transform.position) * 0.3f);
        }
    }

    private IEnumerator ProcessAIData(float delayTime, int[] pos) {
        yield return new WaitForSeconds(delayTime);
        ProcessAIData(pos);
    }

    /// <summary>
    /// Starts a thread in which the ai works
    /// </summary>
    private void AIsTurn() {
        aiThread = new Thread(new ThreadStart(() => {
            aiPlacePos = aiScript.StartEvaluation();
        }));
        aiThread.Start();
        aiTillThinkTime = Time.time + aiMinThinkTime;
    }

    public override void StartNewGame(int[] gridPos) {
        if (!gameStarted) {
            aiScript.Reset();
        }

        base.StartNewGame(gridPos);
    }
}
