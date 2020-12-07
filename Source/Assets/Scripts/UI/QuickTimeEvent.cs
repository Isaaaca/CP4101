using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickTimeEvent : MonoBehaviour
{
    public enum State
    {
        Neutral,
        PassedPassive,
        PassedAggro,
        Failed
    }

    public State state;
    public float duration;
    public float slowDownSpeed;
    private List<KeyCode> passivistKeys;
    private short nextPassiveKey;
    private KeyCode agressiveKey;
    public List<KeyCode> validKeys;
    private float countdownTimer;
    [SerializeField] private GameObject aggroButton = null;
    [SerializeField] private List<GameObject> PassiveButtons = new List<GameObject>();

    // Start is called before the first frame update
    void OnEnable()
    {
        countdownTimer = duration;
        state = State.Neutral;
        int i = Random.Range(0, validKeys.Count);
        agressiveKey = validKeys[i];
        passivistKeys = new List<KeyCode>(validKeys);
        passivistKeys.RemoveAt(i);
        nextPassiveKey = 0;
        aggroButton.GetComponentInChildren<Text>().text = agressiveKey.ToString();
        aggroButton.SetActive(true);
        for (int j = 0; j<passivistKeys.Count; j++)
        {
            PassiveButtons[j].GetComponentInChildren<Text>().text = passivistKeys[j].ToString();
            PassiveButtons[j].SetActive(true);
        }
    }

    private void OnDisable()
    {
        aggroButton.SetActive(false);
        for (int j = 0; j < PassiveButtons.Count; j++)
        {
            PassiveButtons[j].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKey(agressiveKey))
                state = State.PassedAggro;
            else if (Input.GetKey(passivistKeys[nextPassiveKey]))
            {
                
                nextPassiveKey++;
                if (nextPassiveKey>= passivistKeys.Count)
                {
                    state = State.PassedPassive;
                }
            }
        }
        countdownTimer -= Time.unscaledDeltaTime;
        if (countdownTimer <= 0f)
        {
            state = State.Failed;
        }
        if (state == State.Neutral)
            Time.timeScale = Mathf.Clamp(Time.timeScale - Time.unscaledDeltaTime * slowDownSpeed, 0, 1);
        else
            Time.timeScale = 1;
        
    }
}
