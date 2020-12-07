using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class ParticipantInputField : MonoBehaviour, ISelectHandler // required interface when using the OnSelect method.
{

    [SerializeField] private AudioClip selectAudioClip = null;
    [SerializeField] private AudioClip pressAudioClip = null;
    [SerializeField] private AudioClip errorAudioClip = null;

    [SerializeField] private GameObject backButton = null;
    [SerializeField] private GameObject enterButton = null;
    [SerializeField] private Animator errorTextAnimator = null;

    [SerializeField] private TMPro.TMP_InputField inputField = null;

    public UnityEvent OnValidationSuccess;
    private bool errorTrigger = false;
    private void Start()
    {
        if (OnValidationSuccess == null)
            OnValidationSuccess = new UnityEvent();
        string querystring = "";
        string currUrl = Application.absoluteURL;
        int iqs = currUrl.IndexOf('?');
        // If query string variables exist, put them in
        // a string.
        if (iqs >= 0)
        {
            querystring = (iqs < currUrl.Length - 1) ? currUrl.Substring(iqs + 1) : string.Empty;
        }

        Regex rgx = new Regex(@"&?(.+?)=([^\s&]+)");
        MatchCollection matches = rgx.Matches(querystring);
        if (matches.Count > 0)
        {
            foreach (Match match in matches)
            {
                if (match.Groups[1].Value == "ParticipantID")
                {
                    inputField.text = match.Groups[2].Value;
                }
            }
        }

    }

    private void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == gameObject)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow)) EventSystem.current.SetSelectedGameObject(backButton);
            if (Input.GetKeyDown(KeyCode.Return)) EventSystem.current.SetSelectedGameObject(enterButton);
            
        }


        if (errorTrigger)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    //Do this when the selectable UI object is selected.
    public void OnSelect(BaseEventData eventData)
    {
        if (errorTrigger)
        {
            errorTrigger = false;
        }
        else if (selectAudioClip != null) AudioManager.PlayClip(selectAudioClip);
    }

    public void Validate()
    {
        Regex rgx = new Regex(@"^[1-9][0-9]{2}[AB][1-9][0-9]{2}$");
        MatchCollection matches = rgx.Matches(inputField.text);
        if (matches.Count > 0)
        {
            if (pressAudioClip != null) AudioManager.PlayClip(pressAudioClip);
            SaveManager.participantID = inputField.text;
            OnValidationSuccess.Invoke();
        }
        else
        {
            errorTrigger = true;
            if (errorAudioClip != null) AudioManager.PlayClip(errorAudioClip);
            errorTextAnimator.SetTrigger("Reveal");

        }
    }

}
