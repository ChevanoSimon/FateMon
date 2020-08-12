using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText = default;

    PartyMemberUI[] memberSlots;
    List<Servant> Servants;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Servant> servants)
    {
        this.Servants = servants;

        for( int i = 0; i < memberSlots.Length; i++)
        {
            if (i < servants.Count)
                memberSlots[i].SetData(servants[i]);
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Servant";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < Servants.Count; i++)
        {
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message = default;
    }
}
