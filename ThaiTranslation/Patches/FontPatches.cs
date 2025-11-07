using HarmonyLib;
using OWML.ModHelper.Events;
using UnityEngine;
using UnityEngine.UI;


namespace ThaiTranslation
{
    [HarmonyPatch]
    internal class FontPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.GetFont))]
        public static bool TextTranslation_GetFont(bool dynamicFont, ref Font __result)
        {
            __result = ThaiTranslation._kmitlFont;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.IsLanguageLatin))]
        public static bool TextTranslation_IsLanguageLatin(TextTranslation __instance, ref bool __result)
        {

            __result = false;
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TranslatedSign), nameof(TranslatedSign.Start))]
        public static void TranslatedSign_Start(TranslatedSign __instance)
        {

            __instance._interactVolume.gameObject.SetActive(true);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.GetLanguageFont))]
        public static bool TextTranslation_GetLanguageFont(ref Font __result)
        {
            if (TextTranslation.s_theTable == null)
            {
                __result = null;
                return false;
            }

            __result = TextTranslation.GetFont(false);


            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStyleManager), nameof(UIStyleManager.GetMenuFont))]
        public static bool UIStyleManager_GetMenuFont(ref Font __result)
        {
            var savedLanguage = PlayerData.GetSavedLanguage();


            __result = TextTranslation.GetFont(false);

            return false;
        }

        

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.GetGameOverFont))]
        public static bool TextTranslation_GetGameOverFont(ref Font __result)
        {
            __result = ThaiTranslation._kmitlFont;

            return false;
        }
        



        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextTranslation), nameof(TextTranslation.GetDefaultFontSpacing))]
        public static bool TextTranslation_GetDefaultFontSpacing(ref float __result)
        {
            if (TextTranslation.s_theTable == null)
            {
                __result = 1;
                return false;
            }


            //if (UsingCustomFont()) __result = LocalizationUtility.Instance.GetLanguage().DefaultFontSpacing;
            // else __result = TextTranslation.s_theTable.m_defaultSpacing[(int)LocalizationUtility.LanguageToReplace];

            //return false;
            __result = 2;
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FontAndLanguageController), nameof(FontAndLanguageController.InitializeFont))]
        public static void FontAndLanguageController_InitializeFont(FontAndLanguageController __instance)
        {


            foreach (var container in __instance._textContainerList)
            {
                if (container.isLanguageFont)
                {
                    container.textElement.fontSize = (int)(TextTranslation.GetModifiedFontSize(container.originalFontSize));
                    container.textElement.rectTransform.localScale = container.originalScale;
                    container.textElement.lineSpacing = (float)1.5;
                }
            }

        }

        // Fix spacing on every text by changing in-game properties
        // Not ideal but i don't even know what i'm doing and it works so...

        // Dialogue
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DialogueBoxVer2), nameof(DialogueBoxVer2.SetVisible))]
        public static void DialogueBoxV2_SetVisible(DialogueBoxVer2 __instance)
        {
            GameObject dialogueTextObj = GameObject.Find("DialogueCanvas/MainAnchorPoint/DialogueRect/VerticalLayoutGroup/DialogueText");
            if (dialogueTextObj != null)
            {
                Text DialogueTextComp = dialogueTextObj.GetComponent<Text>();
                if (DialogueTextComp != null)
                {
                    DialogueTextComp.lineSpacing = (float)1.2;
                    //DialogueTextComp.transform.localScale = new Vector3(sizeModifier, sizeModifier, sizeModifier);
                }
            }
        }

        // Dialogue Choices
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DialogueBoxVer2), nameof(DialogueBoxVer2.SetUpOptions))]
        public static void DialogueBOxVer2_SetUpOptions(DialogueBoxVer2 __instance)
        {
            GameObject optionsLayoutObj = GameObject.Find("DialogueCanvas/MainAnchorPoint/DialogueRect/VerticalLayoutGroup/OptionsRoot");
            if (optionsLayoutObj != null)
            {
                VerticalLayoutGroup optionsLayout = optionsLayoutObj.GetComponent<VerticalLayoutGroup>();
                if (optionsLayout != null)
                {
                    optionsLayout.spacing = (float)5;
                }

                foreach (Transform optionChild in optionsLayoutObj.transform)
                {
                    if (optionChild.name == "OptionBoxTemplate(Clone)")
                    {
                        Text optionChildText = optionChild.GetComponent<Text>();
                        if (optionChildText != null)
                        {
                           // optionChildText.fontSize = 32;
                            optionChildText.lineSpacing = (float)1.5;
                        }
                    }
                }
            }
        }

        // Button Prompt -- Honestly this is not necessay but let's keep it just in case
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ScreenPromptList), nameof(ScreenPromptList.AddScreenPrompt))]
        public static void ScreenPromptList_AddScreenPrompt(ScreenPromptList __instance, ScreenPromptElement __0)
        {
            Transform promptTxtObj = __0.transform.Find("Text");
            if (promptTxtObj != null)
            {
                Text promptText = promptTxtObj.GetComponent<Text>();
                promptText.fontSize = 12;

            }
        }

        // Fixing main menu character spacing
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TitleScreenManager), nameof(TitleScreenManager.OnTitleMenuAnimationComplete))]
        public static void TitleScreenManager_OnTitleMenuAnimationComplete(TitleScreenManager __instance)
        {
            GameObject mainMenuObj = GameObject.Find("TitleMenu/TitleCanvas/TitleLayoutGroup/MainMenuBlock/MainMenuLayoutGroup");
            if (mainMenuObj != null)
            {
                foreach (Transform menuChild in mainMenuObj.transform)
                {
                    if (menuChild.name.StartsWith("Button-"))
                    {
                        Transform buttonTextObj = menuChild.Find("LayoutGroup/Text");
                        if (buttonTextObj != null)
                        {
                            TextStyleApplier buttonStyle = buttonTextObj.GetComponent<TextStyleApplier>();
                            if (buttonStyle != null) { buttonStyle.spacing = 0; }

                            Text buttonText = buttonTextObj.GetComponent<Text>();
                            if (buttonText != null) { buttonText.fontSize = 52; }
                        }
                    }
                }
                // Adjust spacing
                VerticalLayoutGroup buttonLayout = mainMenuObj.GetComponent<VerticalLayoutGroup>();
                if (buttonLayout != null) { buttonLayout.spacing = 20; }
            }

            // adjust spacing if dlc is owned and showed which make some button goes too far down
            if (EntitlementsManager.IsDlcOwned() == EntitlementsManager.AsyncOwnershipStatus.Owned)
            {
                GameObject spacer = GameObject.Find("TitleMenu/TitleCanvas/TitleLayoutGroup/MainMenuBlock/MainMenuLayoutGroup/Spacer");
                spacer?.SetActive(false);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TitleScreenManager), nameof(TitleScreenManager.OnLanguageChanged))]
        public static void TitleScreenManager_OnLanguageChanged(TitleScreenManager __instance)
        {
            ThaiTranslation.Instance.ModHelper.Console.WriteLine(TextTranslation.Get().GetLanguage().ToString());
        }

        // fix settings tooltip line space is too large
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TitleScreenManager), nameof(TitleScreenManager.OnMenuPush))]
        public static void TitleScreenManager_OnMenuPush(TitleScreenManager __instance)
        {

            GameObject settingTooltipObj = GameObject.Find("TitleMenu/OptionsCanvas/OptionsMenu-Panel/OptionsDisplayPanel/Tooltips/PanelTooltips/ToolTip-Text");
            if (settingTooltipObj != null)
            {
                Text settingTooltipText = settingTooltipObj.GetComponent<Text>();
                if (settingTooltipText != null)
                {
                    settingTooltipText.lineSpacing = 1;
                }
            }

            // fix settings
            GameObject optionMenu = GameObject.Find("TitleMenu/OptionsCanvas/OptionsMenu-Panel");
            GameObject profileManageMenu = GameObject.Find("TitleMenu/ProfileManagementCanvas/ProfileManagement-Panel");

            if (optionMenu != null)
            {
                Text[] optionTxtLists = optionMenu.GetComponentsInChildren<Text>(true);
                foreach (Text optionTxt in optionTxtLists)
                {
                    optionTxt.fontSize = 52;
                    optionTxt.font = ThaiTranslation._kmitlFont;
                }
            }

            if (profileManageMenu != null)
            {
                Text[] pManageTxtList = optionMenu.GetComponentsInChildren<Text>(true);
                foreach (Text pTxt in pManageTxtList)
                {
                    pTxt.fontSize = 52;
                    pTxt.font = ThaiTranslation._kmitlFont;
                }
            }

            GameObject languageSetting = GameObject.Find("TitleMenu/OptionsCanvas/OptionsMenu-Panel/OptionsDisplayPanel/TextAudioMenu/Content/UIElement-Language/HorizontalLayoutGroup/ControlBlock/HorizontalLayoutGroup/Panel-CenterText/CenterTextLabel");
            if (languageSetting != null) { languageSetting.transform.localScale = new Vector3(1, 1, 1); }
        }

        // Same thing but with pause menu
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PauseMenuManager), nameof(PauseMenuManager.OnActivateMenu))]
        public static void PauseMenuManager_onActivateMenu(PauseMenuManager __instance)
        {


            GameObject pauseMenuObj = GameObject.Find("PauseMenu/PauseMenuCanvas/PauseMenuBlock/PauseMenuItems/PauseMenuItemsLayout");
            if (pauseMenuObj != null)
            {
                foreach (Transform menuChild in pauseMenuObj.transform)
                {
                    if (menuChild.name.StartsWith("Button-"))
                    {
                        Transform buttonTextObj = menuChild.Find("HorizontalLayoutGroup/Text");
                        if (buttonTextObj != null)
                        {
                            TextStyleApplier buttonStyle = buttonTextObj.GetComponent<TextStyleApplier>();
                            if (buttonStyle != null) { buttonStyle.spacing = 0; }

                            Text buttonText = buttonTextObj.GetComponent<Text>();
                            if (buttonText != null) { buttonText.fontSize = 52; }
                        }
                    }

                    if (menuChild.name == "LabelPaused")
                    {
                        Transform labelTextObj = menuChild.Find("Text");
                        if (labelTextObj != null)
                        {
                            TextStyleApplier textStyle = labelTextObj.GetComponent<TextStyleApplier>();
                            if (textStyle != null) { textStyle.spacing = 0; }

                            Text labelText = labelTextObj.GetComponent<Text>();
                            if (labelText != null) { labelText.fontSize = 52; }
                        }
                    }
                }

            }

            // fix settings
            GameObject optionMenu = GameObject.Find("PauseMenu/OptionsCanvas/OptionsMenu-Panel");
            if (optionMenu != null)
            {
                Text[] optionTxtLists = optionMenu.GetComponentsInChildren<Text>(true);
                foreach (Text optionTxt in optionTxtLists)
                {
                    optionTxt.fontSize = 52;
                    optionTxt.font = ThaiTranslation._kmitlFont;
                }
            }

            Transform settingTooltipObj = optionMenu.transform.Find("OptionsDisplayPanel/Tooltips/PanelTooltips/ToolTip-Text");
            if (settingTooltipObj != null)
            {
                Text settingTooltipText = settingTooltipObj.GetComponent<Text>();
                if (settingTooltipText != null)
                {
                    settingTooltipText.lineSpacing = 1;
                }
            }

        }

        // fix popup
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PopupMenu), nameof(PopupMenu.SetUpPopup))]
        public static void PopupManu_SetUpPopup(PopupMenu __instance)
        {
            Transform popupElem = __instance.transform.Find("PopupBlock/PopupElements/Text");
            if (popupElem != null)
            {
                Text popupText = popupElem.GetComponent<Text>();
                if (popupText != null)
                {
                    popupText.lineSpacing = 1;
                    popupText.fontSize = 52;
                }
            }
        }

        // fix ship signalscope
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SignalscopeUI), nameof(SignalscopeUI.Activate))]
        public static void SignalscopeUI_Activate(SignalscopeUI __instance)
        {
            Font signalScopeFont = ThaiTranslation._kmitlFont;
            if (PlayerState._atFlightConsole)
            {
                // modify to use static font if player is on ship
                signalScopeFont = ThaiTranslation._silpakornFont;
            }

            __instance._signalscopeLabel.font = signalScopeFont;
            __instance._signalscopeLabel.fontSize = 42;

            __instance._distanceLabel.font = signalScopeFont;
            __instance._distanceLabel.fontSize = 52;

            // adjust text on helmet ui 
            GameObject frequencyLabel = GameObject.Find("PlayerHUD/HelmetOnUI/UICanvas/SigScopeDisplay/FrequencyLabel");
            if (frequencyLabel != null)
            {
                Text frequencyTxt = frequencyLabel.GetAddComponent<Text>();
                if (frequencyTxt != null)
                {

                    frequencyTxt.fontStyle = FontStyle.Bold;
                    frequencyTxt.lineSpacing = (float)1.2;
                }
            }
            // for helmet off ( why tf are they seperated?? )
            frequencyLabel = GameObject.Find("PlayerHUD/HelmetOffUI/SignalscopeCanvas/SigScopeDisplay/FrequencyLabel");
            if (frequencyLabel != null)
            {
                Text frequencyTxt = frequencyLabel.GetAddComponent<Text>();
                if (frequencyTxt != null)
                {

                    frequencyTxt.fontStyle = FontStyle.Bold;
                    frequencyTxt.lineSpacing = (float)1.2;
                }
            }

            // distance ui
            GameObject distanceReticle = GameObject.Find("PlayerHUD/HelmetOffUI/SignalscopeReticule/DistanceText");
            if(distanceReticle != null) { distanceReticle.GetComponent<Text>().fontSize = 48; }
        }

        // Ship log minor text adjustment for better reading
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ShipLogController), nameof(ShipLogController.LateInitialize))]
        public static void ShipLogController_LateInitialize(ShipLogController __instance)
        {
            GameObject factListObj = GameObject.Find("Ship_Body/Module_Cabin/Systems_Cabin/ShipLogPivot/ShipLog/ShipLogPivot/ShipLogCanvas/DescriptionField/FactListMask/FactList");
            if (factListObj != null)
            {
                VerticalLayoutGroup factLayout = factListObj.GetAddComponent<VerticalLayoutGroup>();
                if (factLayout != null)
                {
                    factLayout.spacing = (float)15;
                }
                foreach (Transform fact in factListObj.transform)
                {
                    Text factText = fact.GetComponent<Text>();
                    if (factText != null)
                    {
                        factText.fontSize = 25;
                    }
                }
            }

            //GameObject entryListObj = GameObject.Find("Ship_Body/Module_Cabin/Systems_Cabin/ShipLogPivot/ShipLog/ShipLogPivot/ShipLogCanvas/MapMode/EntryMenu/EntryListRoot/EntryList");
            GameObject entryListObj = null;
            if (entryListObj != null)
            {
                foreach (Transform entryChild in entryListObj.transform)
                {
                    Transform entry = entryChild.Find("AnimRoot/Name");

                    if (entry != null)
                    {
                        Text entryText = entry.GetComponent<Text>();
                        if (entryText != null)
                        {
                            entryText.fontSize = 20;
                        }
                    }
                }
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UiSizeSetterShipLogEntry), nameof(UiSizeSetterShipLogEntry.DoResizeAction))]
        public static bool UISizeSetterShipLogEntry_DoResizeAction(UITextSize textSizeSetting, UiSizeSetterShipLogEntry __instance)
        {

            bool subEntry_flag = __instance._shipLogEntryListItem.IsSubEntry();

            UITextSize ui_textSize = textSizeSetting;
            if (ui_textSize == UITextSize.AUTO)
            {
                if (PlayerData.IsUILargeTextSize())
                {
                    ui_textSize = UITextSize.LARGE;
                }
                else
                {
                    ui_textSize = UITextSize.SMALL;
                }
            }

            int fontSize;
            float minHeight;

            if (ui_textSize == UITextSize.LARGE)
            {
                fontSize = subEntry_flag ? 26 : 32;
                minHeight = 26;
            }
            else
            {
                fontSize = subEntry_flag ? 20 : 26;
                minHeight = 20;
            }

            __instance._textField.fontSize = fontSize;
            __instance._listItemLayoutElement.minHeight = minHeight;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UiSizeSetterShipLogFact), nameof(UiSizeSetterShipLogFact.DoResizeAction))]
        public static bool UISizeSetterShipLogFace_DoResizeAction(UiSizeSetterShipLogFact __instance, UITextSize textSizeSetting)
        {
            // placeholder
            return true;
        }

        // Setting custom font for nomai translator
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NomaiTranslatorProp), nameof(NomaiTranslatorProp.InitializeFont))]
        public static bool NomaiTranslatorProp_InitializeFont(NomaiTranslatorProp __instance)
        {
            __instance._fontInUse = ThaiTranslation._chakraFont;
            __instance._dynamicFontInUse = ThaiTranslation._chakraFont;
            __instance._fontSpacingInUse = (float)1.1;


            __instance._textField.font = __instance._fontInUse;
            __instance._textField.lineSpacing = __instance._fontSpacingInUse;
            return false;
        }

    }
}
