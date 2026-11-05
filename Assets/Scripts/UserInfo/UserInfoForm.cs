using TMPro;
using UnityEngine;

// Goes on the User Info screen (UserInfoCanvas). The screen's Continue/Start
// button calls OnContinuePressed(). The name and ID are stored in UserSession
// and the menu moves on to the next screen (Schematic Select).
//
// Text can come from the VR keyboard or a desktop keyboard: this script only
// reads the input fields' text, so it works with either.
public class UserInfoForm : MonoBehaviour
{
    [Header("Input fields")]
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_InputField idField;

    [Header("Screens")]
    [Tooltip("The screen to hide when the info is valid (this User Info canvas).")]
    [SerializeField] private GameObject currentScreen;
    [Tooltip("The screen to show when the info is valid (Schematic Select).")]
    [SerializeField] private GameObject nextScreen;

    [Header("Optional")]
    [Tooltip("Text that shows what is wrong. Leave empty to only log the error.")]
    [SerializeField] private TMP_Text errorText;

    private void OnEnable()
    {
        ShowError(string.Empty);
    }

    // Hook this to the Continue/Start button's On Click ().
    public void OnContinuePressed()
    {
        string userName = nameField != null ? nameField.text.Trim() : string.Empty;
        string studentId = idField != null ? idField.text.Trim() : string.Empty;

        string error = Validate(userName, studentId);
        if (error != null)
        {
            ShowError(error);
            return;
        }

        ShowError(string.Empty);
        UserSession.SetUser(userName, studentId);
        Debug.Log($"[UserInfoForm] Signed in {userName} ({studentId})");

        if (currentScreen != null) currentScreen.SetActive(false);
        if (nextScreen != null) nextScreen.SetActive(true);
    }

    // Input checking (empty name, 9-digit Bronco ID, error messages) belongs
    // to Haredth's User Info work (REQ-F-02). Until that is decided, this
    // accepts any input. Return a message here to block Continue.
    public static string Validate(string userName, string studentId)
    {
        return null;
    }

    private void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(!string.IsNullOrEmpty(message));
        }
        else if (!string.IsNullOrEmpty(message))
        {
            Debug.LogWarning("[UserInfoForm] " + message);
        }
    }
}
