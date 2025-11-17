using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using OWML.Utils;
using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace ThaiTranslation
{
    public class ThaiTranslation : ModBehaviour
    {

        public static Font _kmitlFont;      // main font
        public static Font _silpakornFont;    // static font
        public static Font _chakraFont;
        public static Font _rsuFont;

        private bool _needShipscreenFix = false;

        public static ThaiTranslation Instance;

        public static float _textSizeMutliplier_genText = 1.15f;     // general text
        public static float _textSizeMutliplier_uiLang = 1.15f;      // uiLangDependant
        public static float _textSizeMultiplier_dialogue = 1.1f;    // DialogueOption
        public static float _textSizeMultiplier_shipFact = 1.1f;    // ShipFact
        public static float _textSizeMultiplier_shipEntry = 1.2f;   // ShipEntry


        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {

            loadFonts();
            // Starting here, you'll have access to OWML's mod helper.
            var api = ModHelper.Interaction.TryGetModApi<ILocalizationAPI>("xen.LocalizationUtility");
            api.RegisterLanguage(this, "ไทย", "assets/Translation.xml");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene != OWScene.SolarSystem) return;

                _needShipscreenFix = true;

            };
        }

        private void Update()
        {
            if (!_needShipscreenFix) return;


            // fix screen on ship by swapping font to static font
            GameObject shipScreen = GameObject.Find("Ship_Body/Module_Cockpit/Systems_Cockpit/ShipCockpitUI/CockpitCanvases/ShipWorldSpaceUI/ConsoleDisplay/Mask/LayoutGroup");

            if (shipScreen)
            {
                foreach (Transform child in shipScreen.transform)
                {
                    if (child.name.StartsWith("TextTemplate(Clone)"))
                    {
                        Text consoleText = child.GetComponent<Text>();
                        if (consoleText != null)
                        {
                            consoleText.fontSize = 54;
                            consoleText.font = _silpakornFont;
                            consoleText.lineSpacing = 1;
                        }

                        TextStyleApplier consoleTextStyle = child.GetComponent<TextStyleApplier>();
                        if (consoleTextStyle != null)
                        {
                            consoleTextStyle.spacing = 0;
                            consoleTextStyle.font = _silpakornFont;
                        }
                    }
                }
                _needShipscreenFix = false;
            }
        }

        public void loadFonts()
        {
            var ab_path = $"{ModHelper.OwmlConfig.ModsPath}/{ModHelper.Manifest.UniqueName}/assets/fontbundle";
            var ab = AssetBundle.LoadFromFile(ab_path);

            _kmitlFont = ab.LoadAsset<Font>("Assets/Fonts/KMITLGO.ttf");
            _silpakornFont = ab.LoadAsset<Font>("Assets/Fonts/SILPAKORN_static.ttf");
            _chakraFont = ab.LoadAsset<Font>("Assets/Fonts/ChakraPetchLegacy.ttf");
            _rsuFont = ab.LoadAsset<Font>("Assets/Fonts/RSU_BOLD.ttf");

            foreach (Font fontName in new Font[] { _kmitlFont, _silpakornFont, _chakraFont, _rsuFont })
            {
                if (fontName == null) { ModHelper.Console.WriteLine($"Cannot Load Font {nameof(fontName)}", MessageType.Error); }
            }

            ab.Unload(false);
        }

    }


}