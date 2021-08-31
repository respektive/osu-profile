using System;
using System.Collections.Generic;
using System.IO;

namespace osu_Profile.IO
{
    public class IniFile
    {
        #region Attributes
        private Dictionary<string, Dictionary<string, string>> sections;
        private string content;
        private string filename;
        private string separator;
        #endregion

        #region Constructors
        /// <summary>
        /// Create an undefined INI file, must precise the filename and separator when loading and exporting
        /// </summary>
        public IniFile() : this(null, null)
        { }

        /// <summary>
        /// Create an INI file
        /// </summary>
        /// <param name="filename">The name of the file</param>
        /// <param name="separator">The separator between the key and the value, generally, it will be "="</param>
        public IniFile(string filename, string separator)
        {
            this.filename = filename;
            this.separator = separator;
            sections = new Dictionary<string, Dictionary<string, string>>();
            content = "";
        }
        #endregion

        #region Methods
        /// <summary>
        /// Read the file and load its content (it will not work with an undefined INI file)
        /// </summary>
        public void Load() => Load(filename, separator);

        /// <summary>
        /// Read a file and load its content
        /// </summary>
        /// <param name="filename">The name of the file</param>
        /// <param name="separator">The separator between the key and the value, generally, it will be "="</param>
        public void Load(string filename, string separator)
        {
            if (filename == null)
                throw new ArgumentNullException("Can't call Load method with a null file name.");
            if (separator == null)
                throw new ArgumentNullException("Can't call Load method with a null separator.");
            sections = new Dictionary<string, Dictionary<string, string>>();
            content = File.ReadAllText(filename);
            List<string> lines = new List<string>(content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            string currentsection = "";
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("[") && lines[i].EndsWith("]"))
                {
                    currentsection = lines[i].Substring(1, lines[i].Length - 2);
                    if (!sections.ContainsKey(currentsection))
                        sections.Add(currentsection, new Dictionary<string, string>());
                    continue;
                }
                if (currentsection != "" && sections.ContainsKey(currentsection) && lines[i].Contains(separator))
                {
                    string[] parts = lines[i].Split(new string[] { separator }, 2, StringSplitOptions.None);
                    if (!sections[currentsection].ContainsKey(parts[0]))
                        sections[currentsection].Add(parts[0].Trim(), parts[1].Trim());
                    else
                        sections[currentsection][parts[0].Trim()] = parts[1].Trim();
                }
            }
        }

        /// <summary>
        /// Export the file (it will not work with an undefined INI file)
        /// </summary>
        public void Export() => Export(filename, separator);

        /// <summary>
        /// Export to a file specified with the file name
        /// </summary>
        /// <param name="filename">The name of the file</param>
        /// <param name="separator">The separator between the key and the value, generally, it will be "="</param>
        public void Export(string filename, string separator)
        {
            if (filename == null)
                throw new ArgumentNullException("Can't call Load method with a null file name.");
            if (separator == null)
                throw new ArgumentNullException("Can't call Load method with a null separator.");

            string tmpcontent = "";
            foreach (string section in sections.Keys)
            {
                tmpcontent += $"[{section}]{Environment.NewLine}";
                foreach (KeyValuePair<string, string> keyvalue in sections[section])
                    tmpcontent += keyvalue.Key + separator + keyvalue.Value + Environment.NewLine;
            }
            if (content == tmpcontent)
                return ;
            content = tmpcontent;
            if (File.Exists(filename))
            {
                if (File.ReadAllText(filename) != content)
                    File.Delete(filename);
                else
                    return ;
            }
            File.WriteAllText(filename, content);
        }

        /// <summary>
        /// Suppress all the content of the object
        /// </summary>
        public void Destroy()
        {
            content = null;
            foreach (KeyValuePair<string, Dictionary<string, string>> section in sections)
                section.Value.Clear();
            sections.Clear();
            sections = null;
        }

        /// <summary>
        /// Return the content of the loaded file
        /// </summary>
        /// <returns>The content of the loaded file</returns>
        public string GetFileContent() => content;

        /// <summary>
        /// Set the value of a key in the named section
        /// </summary>
        /// <param name="section">The name of the section</param>
        /// <param name="key">The name of the key</param>
        /// <param name="value">The value to set</param>
        public void SetValue(string section, string key, string value)
        {
            if (sections.ContainsKey(section))
                if (sections[section].ContainsKey(key))
                    sections[section][key] = value;
                else
                    sections[section].Add(key, value);
            else
            {
                sections.Add(section, new Dictionary<string, string>());
                sections[section].Add(key, value);
            }
        }

        /// <summary>
        /// Returns the value of the key in the named section
        /// </summary>
        /// <param name="section">The name of the section</param>
        /// <param name="key">The name of the key</param>
        /// <returns>The value of the key (null if the key not defined)</returns>
        public string GetValue(string section, string key) => GetValue(section, key, null);

        /// <summary>
        /// Returns the value of the key in the named section with a default value
        /// </summary>
        /// <param name="section">The name of the section</param>
        /// <param name="key">The name of the key</param>
        /// <param name="defaultvalue">The value to return if the key is not defined</param>
        /// <returns>The value of the key (of defaultvalue if the key not defined)</returns>
        public string GetValue(string section, string key, string defaultvalue)
        {
            if (sections.ContainsKey(section))
                if (sections[section].ContainsKey(key))
                    return (sections[section][key]);
            return (defaultvalue);
        }
        #endregion
    }
}