using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Slider))]
public class SetTextToValueOfSlider : MonoBehaviour {

    [Tooltip("What text to set the value to.")]
    public TextMeshProUGUI text;

    private Slider slider;
    
	void Start() {
        slider = GetComponent<Slider>();
        text.text = slider.value.ToString();

        slider.onValueChanged.AddListener((float value) => {
            text.text = slider.value.ToString();
        });
	}
}
