using RPG.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerUI_Hero : MonoBehaviour
{
    [SerializeField] HeroController heroController;
    [SerializeField] PlayerController playerController;

    public void Update()
    {
        GetComponent<Outline>().enabled = heroController && heroController.GetComponent<Selectable>().isSelected;
    }

    public void OnPointerEnter()
    {
        Debug.Log("OnPointerEnter");
    }

    public void OnPointerExit()
    {
        Debug.Log("OnPointerExit");
    }

    public void OnClick()
    {
        foreach (var hero in playerController.selectedHeroes)
        {
            hero.GetComponent<Selectable>().Deselect();
        }

        heroController.GetComponent<Selectable>().Select();
        playerController.selectedHeroes.Clear();
        playerController.selectedHeroes.Add(heroController);
    }

    public void OnSelect()
    {
        Debug.Log("OnSelect");
    }

    public void OnDeselect()
    {
        Debug.Log("OnDeselect");
    }
}
