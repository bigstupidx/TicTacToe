using UnityEngine;

public class MenuButtonPanelUnlocks : MonoBehaviour {

    public RectTransform onlineButton;
    public RectTransform localButton;

    public RectTransform localAIButton;
    public RectTransform localMultiButton;

    public RectTransform bluetoothButton;
    public RectTransform gpButton;

    /// <summary>
    /// Set based on the localButton
    /// </summary>
    private float topButtonPosY;
    /// <summary>
    /// Set based on the onlineButton
    /// </summary>
    private float bottomButtonPosY;
    
	void Awake() {
        topButtonPosY = localButton.anchoredPosition.y;
        bottomButtonPosY = onlineButton.anchoredPosition.y;

		// Set button layout based on the unlocks

        // If local multi is not unlocked then the others are for sure not, because in order they are
        // Local multi -> Bluetooth -> GP
        if (!PreferencesScript.Instance.IsLocalMultiUnlocked()) {
            // Disable online button so none of the online features are available
            onlineButton.gameObject.SetActive(false);
            // Also disable local button because we can't choose between online and local yet
            localButton.gameObject.SetActive(false);

            // only the localAiButton is available so set it to centre position and enable it
            localAIButton.anchoredPosition = new Vector2(0, 0);
            CanvasGroup cg = localAIButton.GetComponent<CanvasGroup>();
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
            return;
        }

        // We have local multi unlocked now but not the others
        // We know that we have locamulti unlocked because if we didn't we would have returned by know
        if (!PreferencesScript.Instance.IsBluetoothUnlocked()) {
            // Disable online button so none of the online features are available
            onlineButton.gameObject.SetActive(false);
            // Also disable local button because we can't choose between online and local yet
            localButton.gameObject.SetActive(false);
            
            // LocalAiButton and local multi button is enabled so set the to be so
            CanvasGroup cg = localAIButton.GetComponent<CanvasGroup>();
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;

            cg = localMultiButton.GetComponent<CanvasGroup>();
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;

            return;
        }

        // We have bluetooth unlocked and gpmulti not unlocked
        if (PreferencesScript.Instance.IsBluetoothUnlocked() && !PreferencesScript.Instance.IsGPMultiUnlocked()) {
            // Disable online button because we only have one online feature and that is the bluetooth
            onlineButton.gameObject.SetActive(false);
            // So set bluetooth button to the pos of online button
            bluetoothButton.anchoredPosition = new Vector2(0, bottomButtonPosY);

            // Also set it to enabled
            CanvasGroup cg = bluetoothButton.GetComponent<CanvasGroup>();
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;

            // here we set the menubuttonpanelscipt's firstpanel buttons to not be the online button but rather be the bluetooth button
            // so the animation plays correctly
            MenuButtonPanelScript[] array = GetComponents<MenuButtonPanelScript>();
            // We have to check whoch of the scripts we want to set it in because there are more
            for (int i = 0; i < array.Length; i++) {
                if (array[i].mainButton.rectTransform == localButton) {
                    // we also have to set the firstPanel button
                    array[i].firstPanelButtons[1] = new MenuButton(bluetoothButton, bluetoothButton.GetComponent<CanvasGroup>());
                }
            }

            return;
        }

        // So here we know that everything is unlocked but jsut for good measures we check whether gpmulti is unlocked
        if (PreferencesScript.Instance.IsGPMultiUnlocked()) {
            // For now we don't want to do anything but if later on we want to do anything here ill just leave this
            return;
        }
    }
}
