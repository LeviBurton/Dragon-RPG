using RPG.Character;
using RPG.Characters;
using SubjectNerd.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// We use this for creating a new Hero when the game starts.
// This just controls the UI and populates a n
public class UI_Character_Creation : MonoBehaviour {

    [Reorderable]
    public RectTransform[] contentPages;

    public GameObject toggleIconPrefab;

    public HorizontalLayoutGroup genderOptions;
    public GenderConfig selectedGender = null;

    public HorizontalLayoutGroup raceOptions;
    public RaceConfig selectedRace = null;

    public HorizontalLayoutGroup classOptions;
    public ClassConfig selectedClass = null;

    public Button nextButton;
    public Button prevButton;
    public TextMeshProUGUI contentTitleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI descriptionTitleText;

    private int currentContentIndex;
    public string HeroName;

    public GameController gameController;
    public HeroController heroController;
    public WeaponSystem weaponSystem;
    public HealthSystem healthSystem;
    public AbilitySystem abilityScoreSystem;
    
    public CharacterSystem characterController;

    public string contentPageTitleOne = "Race/Gender/Class";
    public string contentPageTitleTwo = "Abilities";
    public string contentPageTitleThree = "Skills";
    public string contentPageTitleFour = "Spells";

    private HeroData newHeroData = new HeroData();

    void Start()
    {
        heroController = FindObjectOfType<HeroController>();
        characterController = heroController.GetComponent<CharacterSystem>();
        weaponSystem = characterController.GetComponent<WeaponSystem>();
        healthSystem = characterController.GetComponent<HealthSystem>();
        abilityScoreSystem = characterController.GetComponent<AbilitySystem>();

        contentTitleText.text = contentPageTitleOne;

        descriptionText.text = string.Empty;
        currentContentIndex = 0;

        for (int i = 0; i < contentPages.Length; i++)
        {
            contentPages[i].gameObject.SetActive(false);
        }

        contentPages[currentContentIndex].gameObject.SetActive(true);

        ClearOptions();

        nextButton.interactable = false;
        prevButton.interactable = false;

        // Testing out loading of configs.
        //if (gameController.gameData.hero != null)
        //{
        //    selectedClass = gameController.classConfigs[gameController.gameData.hero.classAssetName];
        //    selectedRace = gameController.raceConfigs[gameController.gameData.hero.raceAssetName];
        //    selectedGender = gameController.genderConfigs[gameController.gameData.hero.genderAssetName];
        //}

        foreach (var config in gameController.genderConfigs.OrderBy(x => x.Value.SortOrder))
        {
            var go = Instantiate(toggleIconPrefab, genderOptions.transform);
            var uiIcon = go.GetComponent<UI_Icon>();
            uiIcon.iconImage.sprite = config.Value.spriteIcon;
            var toggle = go.GetComponentInChildren<Toggle>();
            var toggleGroup = go.transform.parent.GetComponent<ToggleGroup>();
            toggle.group = toggleGroup;

            if (selectedGender == config.Value)
            {
                toggle.isOn = true;
                OnGenderConfigChanged(config.Value, toggle);
            }

            toggle.onValueChanged.AddListener(delegate {
                OnGenderConfigChanged(config.Value, toggle);
            });
        }

        foreach (var config in gameController.raceConfigs.OrderBy(x => x.Value.SortOrder))
        {
            var go = Instantiate(toggleIconPrefab, raceOptions.transform);
            var uiIcon = go.GetComponent<UI_Icon>();
            uiIcon.iconImage.sprite = config.Value.spriteIcon;
            var toggle = go.GetComponentInChildren<Toggle>();
            var toggleGroup = go.transform.parent.GetComponent<ToggleGroup>();
            toggle.group = toggleGroup;

            if (selectedRace == config.Value)
            {
                toggle.isOn = true;
                OnRaceConfigChanged(config.Value, toggle);
            }

            toggle.onValueChanged.AddListener(delegate {
                OnRaceConfigChanged(config.Value, toggle);
            });
        }

        foreach (var config in gameController.classConfigs.OrderBy(x => x.Value.SortOrder))
        {
            var go = Instantiate(toggleIconPrefab, classOptions.transform);

            var uiIcon = go.GetComponent<UI_Icon>();

            uiIcon.iconImage.sprite = config.Value.spriteIcon;
            var toggle = go.GetComponentInChildren<Toggle>();
            var toggleGroup = go.transform.parent.GetComponent<ToggleGroup>();

            toggle.group = toggleGroup;

            if (selectedClass == config.Value)
            {
                toggle.isOn = true;
                OnClassConfigChanged(config.Value, toggle);
            }

            toggle.onValueChanged.AddListener(delegate {
                OnClassConfigChanged(config.Value, toggle);
            });
        }
    }

    public bool IsValid_StepOne()
    {
        return selectedGender != null && selectedRace != null && selectedClass != null;
    }

    void OnGenderConfigChanged(GenderConfig config, Toggle toggle)
    {
        selectedGender = toggle.isOn ? config : null;
        nextButton.interactable = IsValid_StepOne();
        descriptionText.text = config.Description;
        descriptionTitleText.text = config.Name;
    }

    void OnRaceConfigChanged(RaceConfig config, Toggle toggle)
    {
        selectedRace = toggle.isOn ? config : null;
        nextButton.interactable = IsValid_StepOne();
        descriptionText.text = config.Description;
        descriptionTitleText.text = config.Name;
    }

    void OnClassConfigChanged(ClassConfig config, Toggle toggle)
    {
        WeaponConfig weaponConfig = null;
        bool useOtherHand = false;
        descriptionText.text = config.Description;
        descriptionTitleText.text = config.Name;
        selectedClass = toggle.isOn ? config : null;

        // Just some testing stuff...
        if (config.Name == "Fighter" && toggle.isOn)
        {
            weaponConfig = gameController.weaponConfigs.SingleOrDefault(x => x.Value.itemName == "Two Hand Sword").Value;
        }
    
        if (config.Name == "Rogue" && toggle.isOn)
        {
            weaponConfig = gameController.weaponConfigs.SingleOrDefault(x => x.Value.itemName == "Sword").Value;
        }

        if (config.Name == "Ranger" && toggle.isOn)
        {
            weaponConfig = gameController.weaponConfigs.SingleOrDefault(x => x.Value.itemName == "Bow").Value;
            useOtherHand = true;
        }

        if (config.Name == "Mage" && toggle.isOn)
        {
            weaponConfig = gameController.weaponConfigs.SingleOrDefault(x => x.Value.itemName == "Staff").Value;
        }

        if (config.Name == "Medic" && toggle.isOn)
        {
            weaponConfig = gameController.weaponConfigs.SingleOrDefault(x => x.Value.itemName == "Unarmed").Value;
        }

        if (weaponConfig != null)
        {
            weaponSystem.EquipWeapon(weaponConfig);
            weaponSystem.PutWeaponInHand(weaponConfig, useOtherHand);
        }
        else
        {
            weaponSystem.SetRelaxed();
            weaponSystem.RemoveWeaponFromHand();
        }

        nextButton.interactable = IsValid_StepOne();
    }

    private void ClearOptions()
    {
        foreach (Transform child in genderOptions.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in raceOptions.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in classOptions.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void OnNameChanged(string value)
    {
        HeroName = value;
    }

    public void UpdateContentPageTitle()
    {
        // This is a total hack due to how I organized the UI
        if (currentContentIndex == 0)
        {
            contentTitleText.text = contentPageTitleOne;
        }
        else if (currentContentIndex == 1)
        {
            contentTitleText.text = contentPageTitleTwo;
        }
        else if (currentContentIndex == 2)
        {
            contentTitleText.text = contentPageTitleThree;
        }
        else if (currentContentIndex == 3)
        {
            contentTitleText.text = contentPageTitleFour;
        }
    }

    public void SaveButtonClicked()
    {
        newHeroData.name = HeroName;
        newHeroData.characterType = ECharacterType.Player;
        newHeroData.classAssetName = gameController.classConfigs.SingleOrDefault(x => x.Value == selectedClass).Key;
        newHeroData.genderAssetName = gameController.genderConfigs.SingleOrDefault(x => x.Value == selectedGender).Key;
        newHeroData.raceAssetName = gameController.raceConfigs.SingleOrDefault(x => x.Value == selectedRace).Key;
        newHeroData.abilities = new List<AbilityData>();

        foreach (var ability in gameController.abilityConfigs.OrderBy(x => x.Value.SortOrder))
        {
            var data = new AbilityData(ability.Value.Type, ability.Value.Name, ability.Value, 8);

            newHeroData.abilities.Add(data);
        }

        gameController.SetHeroData(newHeroData);

        gameController.SaveGameData();
    }

    public void NextButtonClicked()
    {
        var previousContentIndex = currentContentIndex;
        contentPages[previousContentIndex].gameObject.SetActive(false);

        currentContentIndex++;
       
        if (currentContentIndex >= contentPages.Length)
        {
            currentContentIndex = contentPages.Length - 1;
            nextButton.interactable = false;
            prevButton.interactable = true;

            SaveButtonClicked();

        }
        else
        {
            nextButton.interactable = true;
            prevButton.interactable = true;
        }

        UpdateContentPageTitle();

        contentPages[currentContentIndex].gameObject.SetActive(true);
    }

    public void PrevButtonClicked()
    {
        var previousContentIndex = currentContentIndex;
        contentPages[previousContentIndex].gameObject.SetActive(false);

        currentContentIndex--;

        if (currentContentIndex <= 0)
        {
            currentContentIndex = 0;
      
            prevButton.interactable = false;
        }
        else
        {
            nextButton.interactable = true;
            prevButton.interactable = true;
        }

        UpdateContentPageTitle();

        contentPages[currentContentIndex].gameObject.SetActive(true);
    }

}
