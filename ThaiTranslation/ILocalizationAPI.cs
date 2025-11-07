using OWML.ModHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThaiTranslation
{
    internal interface ILocalizationAPI
    {
        void RegisterLanguage(ModBehaviour mod, string name, string translationPath);
        void AddLanguageFont(ModBehaviour mod, string name, string assetBundlePath, string fontPath);
        void AddLanguageFixer(string name, Func<string, string> fixer);
    }
}
