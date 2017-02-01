using UnityEngine;
using System.Collections;

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
        animator.SetTrigger("Idle" + cellOcc.ToString());
    }

    /// <summary>
    /// Triggers draw in animator
    /// </summary>
    public void TriggerDraw() {
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
        template.cellPosition = transform.position;

        return template;
    }

    // Updates attributes in cell template
    public void UpdateAttributes(CellTemplate cellTemplate) {
        cellType = cellTemplate.cellOcc;

        transform.position = cellTemplate.cellPosition;
    }

    public enum CellOcc {
        X,
        O,
        BLOCKED
    }
}