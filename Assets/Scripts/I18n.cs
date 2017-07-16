using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class I18n
{
    private static I18n instance;

    public Dictionary<string, Language> languages = new Dictionary<string, Language>();

    public Language currentLanguage;

    private I18n() { }

    public static I18n Create(bool useLanguageDiscovery = true)
    {
        if (instance != null)
            return instance;

        I18n i18n = new I18n();

        if (useLanguageDiscovery)
            i18n.FindLanguages();

        return i18n;
    }

    public string Translate(string s)
    {
        if (currentLanguage == null)
            return s;

        if (currentLanguage.data == null)
            currentLanguage.Load();

        if (currentLanguage.data != null)
            foreach (KeyValuePair<string, string> kvp in currentLanguage.data)
                while (s.Contains(kvp.Key))
                    s = s.Replace(kvp.Key, kvp.Value);

        return s;

    }

    private void FindLanguages()
    {
        if (!Directory.Exists("./Lang/"))
            return;

        foreach (string s in Directory.GetFiles("./Lang/"))
        {
            if (!s.EndsWith(".lang"))
                continue;

            StreamReader sr = File.OpenText(s);
            if (!sr.EndOfStream)
            {
                string meta = sr.ReadLine();

                if (meta.Trim().Equals("$fuj1n.jmc.infinidungeon.lang") && !sr.EndOfStream)
                {
                    string name = sr.ReadLine().Substring(1);

                    Language lang = new Language();
                    lang.id = Path.GetFileNameWithoutExtension(s);

                    if (lang.id == "en_GB")
                        currentLanguage = lang;

                    lang.name = name;
                    lang.src = s;

                    Register(lang);
                }
            }

            sr.Close();
        }
    }

    public void Register(Language lang)
    {
        languages[lang.id] = lang;
    }

    public class Language
    {
        public string id;
        public string name;

        public Dictionary<string, string> data;

        public string src;

        public void Load()
        {
            data = new Dictionary<string, string>();

            string[] lines = File.ReadAllLines(src);

            foreach (string line in lines)
            {
                string ln = line.Trim();
                if (ln.StartsWith("$") || ln.StartsWith("#") || string.IsNullOrEmpty(ln))
                    continue;

                if (!ln.Contains("="))
                {
                    Debug.LogWarning("Failed to find equal sign on a line of language " + id);
                    continue;
                }

                data[ln.Substring(0, ln.IndexOf("="))] = ln.Substring(ln.IndexOf("=") + 1);
            }
        }
    }
}
