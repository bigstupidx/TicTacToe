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
    private AudioSource audioSource;

    void Awake() {
        cellOcc = CellOcc.NONE;
        
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PreferencesScript.Instance.GetSoundVolume();
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
        if (animator != null) {
            animator.SetTrigger("Draw" + cellOcc.ToString());
            audioSource.clip = SignResourceStorage.Instance.GetRandomSoundFor(cellType);
            audioSource.Play();
        }
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

        return template;
    }

    // Updates attributes in cell template
    public void UpdateAttributes(CellTemplate cellTemplate) {
        cellType = cellTemplate.cellOcc;
    }

    public enum CellOcc {
        X = 1,
        O = 2,
        BLOCKED = 3,
        NONE = 0
    }
}