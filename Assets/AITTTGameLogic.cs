using System.Threading;
using UnityEngine;

public class AITTTGameLogic : TTTGameLogic {
    private AIScript aiScript;

    private Thread aiThread;
    private int[] aiPlacePos;

    public override void Start() {
        base.Start();

        aiScript = GetComponent<AIScript>();
        whoseTurn = AIScript.HumanType;
    }

    public override void NextPerson() {
        base.NextPerson();

        if (whoseTurn == AIScript.AIType) {
            AIsTurn();
        }
    }

    public override void NextTurn(int[] gridPos, out Cell.CellOcc won) {
        base.NextTurn(gridPos, out won);

        if (won != Cell.CellOcc.BLOCKED && whoseTurn == AIScript.AIType) {
            AIsTurn();
        }
    }

    void Update() {
        if (aiThread != null && !aiThread.IsAlive && aiPlacePos != null) {
            WantToPlaceAt(new Vector2(aiPlacePos[0], aiPlacePos[1]));
            aiPlacePos = null;
        }
    }

    private void AIsTurn() {
        aiThread = new Thread(new ThreadStart(() => {
            aiPlacePos = aiScript.StartEvaluation();
        }));
        aiThread.Start();
    }

    public override void StartNewGame(int[] gridPos) {
        if (!gameStarted) {
            aiScript.Reset();
        }

        base.StartNewGame(gridPos);
    }
}
