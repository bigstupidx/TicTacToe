using UnityEngine;

public class Cell : MonoBehaviour {

    private CellOcc cellOcc;
    public CellOcc cellType {
        get {
            return cellOcc;
        }
        set {
            cellOcc = value;
        }
    }
    
    private Animator animator;

    void Awake() {
        cellOcc = CellOcc.BLOCKED;
        
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Triggers idle for this cell
    /// </summary>
    public void TriggerIdle() {
        if (animator != null)
            animator.SetTrigger("Idle" + cellOcc.ToString());
    }

    /// <summary>
    /// Triggers draw in animator
    /// </summary>
    public void TriggerDraw() {
        if (animator != null)
            animator.SetTrigger("Draw" + cellOcc.ToString());
    }

    /// <summary>
    /// Resets animator
    /// </summary>
    public void ResetAnimator() {
        animator.SetTrigger("Reset");
    }

    // Returns this cell's template
    public CellTemplate GetCellTemplate() {
        CellTemplate template = new CellTemplate();
        template.cellOcc = cellType;

        // If it is blocked there is no gameobject
        if (cellType != CellOcc.BLOCKED)
            template.cellPosition = transform.position;

        return template;
    }

    // Updates attributes in cell template
    public void UpdateAttributes(CellTemplate cellTemplate) {
        cellType = cellTemplate.cellOcc;

        // If it is blocked there is no gameobject
        if (cellType != CellOcc.BLOCKED)
            transform.position = cellTemplate.cellPosition;
    }

    public enum CellOcc {
        X,
        O,
        BLOCKED,
        NONE = 0
    }
}