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

        // Font Size Adjustment
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UiSizeSetterText), nameof(UiSizeSetterText.DoResizeAction))]
        public static bool UiSizeSetterText_DoResizeAction(UiSizeSetterText __instance, UITextSize textSizeSetting)
        {
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

            if (ui_textSize == UITextSize.LARGE)
            {
                if (__instance._enableFontSizeChange) { __instance._targetText.fontSize = (int) (__instance._fontSizes.largeVal * ThaiTranslation._textSizeMutliplier_genText); }
                if (__instance._enableLineSpacingChange) { __instance._targetText.lineSpacing = (float) 1.4; }
            }
            else
            {
                if (__instance._enableFontSizeChange) { __instance._targetText.fontSize = (int) (__instance._fontSizes.normalVal * ThaiTranslation._textSizeMutliplier_genText); }
                if (__instance._enableLineSpacingChange) { __instance._targetText.lineSpacing = (float)1.2; }
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UiSizeSetterLangDependentText), nameof(UiSizeSetterLangDependentText.DoResizeAction))]
        public static bool UiSizeLangDependentText_DoResizeAction(UiSizeSetterLangDependentText __instance, UITextSize textSizeSetting)
        {
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
            float lineSpacing;
            bool horizonOverflow;

            if (ui_textSize == UITextSize.LARGE)
            {
                fontSize = __instance._entryFontSizeDefault.largeVal;
                lineSpacing = (float)1.4;
                horizonOverflow = false;
            }
            else
            {
                fontSize = __instance._entryFontSizeDefault.normalVal;
                lineSpacing = (float)1.2;
                horizonOverflow = false;
            }

            __instance._textField.fontSize = (int)(fontSize * ThaiTranslation._textSizeMutliplier_uiLang);
            __instance._textField.lineSpacing = lineSpacing;
            __instance._textField.horizontalOverflow = (horizonOverflow ? HorizontalWrapMode.Overflow : HorizontalWrapMode.Wrap);

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UiSizeSetterDialogueOption), nameof(UiSizeSetterDialogueOption.DoResizeAction))]
        public static bool UiSizeSetterDialogueOption_DoResizeAction(UiSizeSetterDialogueOption __instance, UITextSize textSizeSetting) 
        {
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
            float lineSpacing;
            float yPosMarker;

            if (ui_textSize == UITextSize.LARGE)
            {
                fontSize = __instance._entryFontSizeDefault.largeVal;
                lineSpacing = (float) 1.4;
                yPosMarker = 7;
            } else
            {
                fontSize = __instance._entryFontSizeDefault.normalVal;
                lineSpacing = (float)1.2;
                yPosMarker = 3;
            }

            __instance._textField.fontSize = (int)(fontSize * ThaiTranslation._textSizeMultiplier_dialogue);
            __instance._textField.lineSpacing = lineSpacing;
            Vector2 anchoredPos = __instance._optionSelectionMarkerTransform.anchoredPosition;
            anchoredPos.y = yPosMarker;
            __instance._optionSelectionMarkerTransform.anchoredPosition = anchoredPos;

            return false;
        }

        // Adjust ship log text size
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
                fontSize = subEntry_flag ? (int)(__instance._subEntryFontSizeDefault.largeVal * 1.2) : __instance._entryFontSizeDefault.largeVal;
                minHeight = __instance._minRootObjLayoutHeightDefault.largeVal;
            }
            else
            {
                fontSize = subEntry_flag ? (int) (__instance._subEntryFontSizeDefault.normalVal * 1.2) : __instance._entryFontSizeDefault.normalVal;

                minHeight = __instance._minRootObjLayoutHeightDefault.normalVal;
            }

            __instance._textField.fontSize = (int)(fontSize * ThaiTranslation._textSizeMultiplier_shipEntry);
            __instance._listItemLayoutElement.minHeight = (float)(minHeight * 1.2);

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UiSizeSetterShipLogFact), nameof(UiSizeSetterShipLogFact.DoResizeAction))]
        public static bool UISizeSetterShipLogFact_DoResizeAction(UiSizeSetterShipLogFact __instance, UITextSize textSizeSetting)
        {
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

            if (ui_textSize == UITextSize.LARGE)
            {
                fontSize = __instance._fontSizeDefault.largeVal;
            }
            else
            {
                fontSize = __instance._fontSizeDefault.normalVal;
            }

            __instance._textField.fontSize = (int)(fontSize * ThaiTranslation._textSizeMultiplier_shipFact);
            __instance._textField.lineSpacing = 1.05f;
            __instance._bulletPointTransform.anchoredPosition = __instance._bulletPointPositionDefault.normalVal;
            return false;
        }

        // Dialogue Spacing
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DialogueBoxVer2), nameof(DialogueBoxVer2.SetUpOptions))]
        public static void DialogueBOxVer2_SetUpOptions(DialogueBoxVer2 __instance)
        {
            GameObject optionsLayoutObj = GameObject.Find("DialogueCanvas/MainAnchorPoint/DialogueRect/VerticalLayoutGroup/OptionsRoot");
            if (optionsLayoutObj != null)
            {
                optionsLayoutObj.GetComponent<VerticalLayoutGroup>().spacing = 7.5f;
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

        // Setting custom font for nomai translator
        [HarmonyPostfix]
        [HarmonyPatch(typeof(NomaiTranslatorProp), nameof(NomaiTranslatorProp.InitializeFont))]
        public static void NomaiTranslatorProp_InitializeFont(NomaiTranslatorProp __instance)
        {

            __instance._fontInUse = ThaiTranslation._chakraFont;
            __instance._dynamicFontInUse = ThaiTranslation._chakraFont;
            __instance._fontSpacingInUse = 1.1f;

            //__instance._textField.lineSpacing = __instance._fontSpacingInUse;
            //__instance._textField.font = ThaiTranslation._thaisans;

          
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(NomaiTranslatorProp), nameof(NomaiTranslatorProp.DisplayTextNode))]
        public static void NomaiTranslatorProp_DisplayTextNode(NomaiTranslatorProp __instance)
        {
            __instance._textField.fontSize = 78;
        }

        // text promt for stone ( In ENG it's xxx projection stone but in Thai it should be ฉายภาพ xxx ) 
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SharedStone), nameof(SharedStone.GetDisplayName))]
        public static bool SharedStone_GetDisplayName(SharedStone __instance, ref string __result)
        {
            __result = UITextLibrary.GetString(UITextType.ItemSharedStonePrompt) + "<color=orange>" + NomaiRemoteCameraPlatform.IDToPlanetString(__instance._connectedPlatform) + "</color> ";
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemTool), nameof(ItemTool.UpdateState))]
        public static void ItemTool_UpdateState(ItemTool.PromptState newState, string itemName, ItemTool __instance)
        {

            if (TextTranslation.Get().GetLanguage().ToString() != "ไทย") { return; }
            if (__instance._promptState == ItemTool.PromptState.CANNOT_HOLD_MORE)
            {
                __instance._messageOnlyPrompt.SetVisibility(true);
                __instance._messageOnlyPrompt.SetText("ไม่สามารถืออย่างอื่นได้เนื่องจากในมือมี " + itemName + " อยู่แล้ว"); // hard coded cuz idk what im doing

                __instance._cancelButtonPrompt.SetVisibility(false);
                __instance._interactButtonPrompt.SetVisibility(false);
            }
            else if (__instance._promptState == ItemTool.PromptState.UNSOCKET)
            {
                __instance._messageOnlyPrompt.SetVisibility(false);
                __instance._cancelButtonPrompt.SetVisibility(false);

                __instance._interactButtonPrompt.SetVisibility(true);
                __instance._interactButtonPrompt.SetText("นำ " + itemName + " ออก"); // another hard-coded
            }
            

        }

        // Ship log clue mode
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ShipLogDetectiveMode), nameof(ShipLogDetectiveMode.OnEnterComputer))]
        public static void ShipLogDetectiveMode_OnEnterComputer(ShipLogController __instance)
        {
            
        }

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
                    factLayout.spacing = 15f;
                }
            }

            GameObject clueObj = GameObject.Find("Ship_Body/Module_Cabin/Systems_Cabin/ShipLogPivot/ShipLog/ShipLogPivot/ShipLogCanvas/DetectiveMode/ScaleRoot/PanRoot");
            if (clueObj != null)
            {
                foreach (Transform t in clueObj.transform)
                {
                    Transform title = t.Find("EntryCardRoot/NameBackground/Name");
                    if (title != null)
                    {
                        Text txtComp = title.GetComponent<Text>();
                        if (txtComp != null)
                        {
                            txtComp.font = ThaiTranslation._rsuFont;
                            txtComp.fontSize = 16;
                            txtComp.fontStyle = FontStyle.Bold;
                        }
                    }
                }
            }

        }





            }
}
