﻿using RPG.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//TODO: no longer used?  Replaced with HeroPanel
public class PlayerUI_Hero : MonoBehaviour
{
    [SerializeField] HeroController heroController;
    [SerializeField] PlayerController playerController;
    [SerializeField] Color hoveredBorderColor;
    [SerializeField] Color selectedBorderColor;
    [SerializeField] Color normalBorderColor;

    Selectable selectable;

    [SerializeField] Image backgroundImage;

    void OnEnable()
    {
        selectable.onSelected += OnSelected;
        selectable.onDeselected += OnDeselected;
        selectable.onHighlight += OnHighlight;
        selectable.onDeHighlight += OnDehighlight;
        backgroundImage.color = normalBorderColor;
    }

    void OnDisable()
    {
        
    }

    void Awake()
    {
        selectable = heroController.GetComponent<Selectable>();
    }

    void Start()
    {
        
    }

    void OnSelected()
    {
        backgroundImage.color = selectedBorderColor;
    }

    void OnDeselected()
    {
        backgroundImage.color = normalBorderColor;
    }

    void OnHighlight()
    {

    }

    void OnDehighlight()
    {

    }
    public void Update()
    {
    }

    public void OnPointerEnter()
    {
        backgroundImage.color = hoveredBorderColor;
    }

    public void OnPointerExit()
    {
        backgroundImage.color = normalBorderColor;
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
