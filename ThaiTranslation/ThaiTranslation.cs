using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using OWML.Utils;
using Steamworks;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using ThaiTranslation.Patches;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static TextTranslation;

namespace ThaiTranslation
{
    public class ThaiTranslation : ModBehaviour
    {
        public static ThaiTranslation Instance;

        public Font KmitlFont;
        public Font SilpakornFont;      // static
        public Font ChakraFont;
        public Font RsuFont;

        public Sprite EoTElogo;
        public Sprite OWlogo;

        public bool custom_dlc_logo;
        public bool custom_game_logo;


        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            LoadAssets();

            // Starting here, you'll have access to OWML's mod helper.
            var api = ModHelper.Interaction.TryGetModApi<ILocalizationAPI>("xen.LocalizationUtility");
            api.RegisterLanguage(this, "ไทย", "assets/Translation.xml");


            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());


            StartCoroutine(AttemptChangeLogo());
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                 if (loadScene == OWScene.SolarSystem)
                {
                    StartCoroutine(FixOnSolarSceneLoaded());
                }
                else if (loadScene == OWScene.TitleScreen)
                {
                    StartCoroutine(AttemptChangeLogo());
                }

            };

            custom_game_logo = ModHelper.Config.GetSettingsValue<bool>("owLogo");
            custom_dlc_logo = ModHelper.Config.GetSettingsValue<bool>("dlcLogo");
        }

        public override void Configure(IModConfig config)
        {
            custom_game_logo = config.GetSettingsValue<bool>("owLogo");
            custom_dlc_logo = config.GetSettingsValue<bool>("dlcLogo");
        }

        private void Update()
        {

        }

        private void LoadAssets()
        {
            var bundlePath = Path.Combine(ModHelper.Manifest.ModFolderPath, "assets", "thai_assets");

            if (!File.Exists(bundlePath))
            {
                ModHelper.Console.WriteLine($"AssetBundle is missing from {bundlePath}", MessageType.Error);
                return;
            }

            var ab = AssetBundle.LoadFromFile(bundlePath);
            if (ab == null)
            {
                ModHelper.Console.WriteLine("AssetBundle cannot be loaded!", MessageType.Error);
                return;
            }

            KmitlFont = ab.LoadAsset<Font>("Assets/Fonts/KMITLGO.ttf");
            SilpakornFont = ab.LoadAsset<Font>("Assets/Fonts/SILPAKORN_static.ttf");
            ChakraFont = ab.LoadAsset<Font>("Assets/Fonts/ChakraPetchLegacy.ttf");
            RsuFont = ab.LoadAsset<Font>("Assets/Fonts/RSU_BOLD.ttf");
            EoTElogo = ab.LoadAsset<Sprite>("Assets/Images/LogoEOTE.png");
            OWlogo = ab.LoadAsset<Sprite>("Assets/Images/LogoBase.png");

            ab.Unload(false);
        }

        private IEnumerator FixOnSolarSceneLoaded()
        {
            //yield return new WaitForSeconds(2);
            float timeout = 10f;

            // fix ship screen text
            GameObject shipScreenObj = null;
            while (shipScreenObj == null && timeout > 0)
            {
                shipScreenObj = GameObject.Find("Ship_Body/Module_Cockpit/Systems_Cockpit/ShipCockpitUI/CockpitCanvases/ShipWorldSpaceUI/ConsoleDisplay/Mask/LayoutGroup");
                timeout -= 1f;
                yield return new WaitForSeconds(1);
            }
                
            foreach (Transform child in shipScreenObj.transform)
            {
                if (child.name.StartsWith("TextTemplate(Clone)"))
                {
                    Text consoleText = child.GetComponent<Text>();
                    if (consoleText != null)
                    {
                        consoleText.fontSize = 54;
                        consoleText.font = SilpakornFont;
                        consoleText.lineSpacing = 1;
                    }

                    TextStyleApplier consoleTextStyle = child.GetComponent<TextStyleApplier>();
                    if (consoleTextStyle != null)
                    {
                        consoleTextStyle.spacing = 0;
                        consoleTextStyle.font = SilpakornFont;
                    }
                }
            }
                
            

            // fix pause menu
            GameObject pauseMenuObj = null;
            while (pauseMenuObj == null && timeout > 0)
            {
                pauseMenuObj = GameObject.Find("PauseMenu/PauseMenuCanvas/PauseMenuBlock/PauseMenuItems/PauseMenuItemsLayout");
                timeout -= 1f;
                yield return new WaitForSeconds(1);
            }
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

            // fix settings
            GameObject optionMenu = GameObject.Find("PauseMenu/OptionsCanvas/OptionsMenu-Panel");
            if (optionMenu != null)
            {
                Text[] optionTxtLists = optionMenu.GetComponentsInChildren<Text>(true);
                foreach (Text optionTxt in optionTxtLists)
                {
                    optionTxt.fontSize = 52;
                    optionTxt.font = ThaiTranslation.Instance.KmitlFont;
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
                
            

            // signalscope fixes
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
            if (distanceReticle != null) { distanceReticle.GetComponent<Text>().fontSize = 48; }


        }

        private IEnumerator AttemptChangeLogo()
        {
            if (SceneManager.GetActiveScene().name != "TitleScreen") { yield break; }

            // base replacement is in hook to game animation controller

            GameObject baseLogo = null;
            GameObject dlcLogo = null;
            int timeout = 10;

            while ((baseLogo == null || dlcLogo == null) && timeout > 0)
            {
                if (baseLogo == null) { baseLogo = GameObject.Find("TitleMenu/TitleCanvas/TitleLayoutGroup/Logo");  }
                if (dlcLogo == null) { dlcLogo = GameObject.Find("TitleMenu/TitleCanvas/TitleLayoutGroup/Logo_EchoesOfTheEye"); }
                yield return new WaitForEndOfFrame();
                timeout--;
            }

            if (baseLogo == null) { ModHelper.Console.WriteLine("Cannot find base logo", MessageType.Error); }
            if (dlcLogo == null) { ModHelper.Console.WriteLine("Cannot find dlc logo", MessageType.Error); }

            // base replacement
            if (this.custom_game_logo && baseLogo != null)
            {
                GameObject.Find("TitleCanvasHack/TitleLayoutGroup/OW_Logo_Anim/OW_Logo_Anim/OUTER").transform.localScale = Vector3.zero;
                GameObject.Find("TitleCanvasHack/TitleLayoutGroup/OW_Logo_Anim/OW_Logo_Anim/WILDS").transform.localScale = Vector3.zero;
                var baseImg = baseLogo.GetComponent<Image>();
                baseImg.sprite = this.OWlogo;
                baseImg.transform.position = new Vector3(375f, 975f, 0f);
                baseImg.transform.localScale = new Vector3(1.1f, 1.1f, 0f);
                baseImg.color = Color.white;
            }

            // dlc replacement
            if (this.custom_dlc_logo && dlcLogo != null)
            {
                dlcLogo.GetComponent<Image>().sprite = this.EoTElogo;
            }

        }

    }
}