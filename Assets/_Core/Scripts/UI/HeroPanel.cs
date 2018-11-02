using RPG.Character;
using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroPanel : MonoBehaviour
{
    [SerializeField] HeroController heroController;
    [SerializeField] ExplorationMode playerController;
    [SerializeField] Color hoveredBorderColor;
    [SerializeField] Color selectedBorderColor;
    [SerializeField] Color normalBorderColor;
    [SerializeField] Image backgroundImage;

    Selectable selectable;

    void Awake()
    {
        if (heroController == null)
            return;

        selectable = heroController.GetComponent<Selectable>();
        backgroundImage = GetComponent<Image>();
    }

    void OnEnable()
    {
        if (heroController == null)
            return;

        selectable.onSelected += OnSelected;
        selectable.onDeselected += OnDeselected;
        selectable.onHighlight += OnHighlight;
        selectable.onDeHighlight += OnDehighlight;

        backgroundImage.color = normalBorderColor;
    }

    void OnDisable()
    {
         if (heroController == null)
            return;
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

    public void OnPointerEnter()
    {
        if (!selectable.isSelected)
        {
            backgroundImage.color = hoveredBorderColor;
        }
    }

    public void OnPointerExit()
    {
        if (!selectable.isSelected)
        {
            backgroundImage.color = normalBorderColor;
        }
    }

    public void OnClick(BaseEventData baseEventData)
    {
        PointerEventData pointerEventData = baseEventData as PointerEventData;

        if (heroController == null)
            return;

        if (pointerEventData.clickCount == 2)
        {
            var rtsCamera = Camera.main.GetComponent<RTS_Cam.RTS_Camera>();
            rtsCamera.SetTarget(heroController.transform);
        }

        playerController.selectedHero.GetComponent<Selectable>().Deselect();
        playerController.selectedHero = heroController;
        heroController.GetComponent<Selectable>().Select();
    }

    public void OnSelect()
    {

    }

    public void OnDeselect()
    {

    }
}
