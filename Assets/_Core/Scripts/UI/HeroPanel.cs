using RPG.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroPanel : MonoBehaviour
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
        backgroundImage = GetComponent<Image>();
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
