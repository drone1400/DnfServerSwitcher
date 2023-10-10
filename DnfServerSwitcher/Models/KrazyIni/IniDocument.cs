using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DnfServerSwitcher.Models.KrazyIni.Data;
using DnfServerSwitcher.Models.KrazyIni.Raw;
namespace DnfServerSwitcher.Models.KrazyIni {
    public class IniDocument {
        public Dictionary<string, IniSection> Sections { get; private set; } = new Dictionary<string, IniSection>();

        public IniSection this[string section] {
            get {
                if (!this.Sections.ContainsKey(section)) this.Sections[section] = new IniSection(section);
                return this.Sections[section];
            } 
            set {
                this.Sections[section] = value;
            }
        }

        public IniDocument() {
            
        }
        
        public IniDocument(RawIniDocument rawIni) {
            string currentActiveSection = "";
            List<string> commentBuffer = new List<string>();
            
            this.Sections.Add("", new IniSection(""));

            foreach (RawIniLine line in rawIni.Lines) {
                if (line is RawIniSection rawSection) {
                    currentActiveSection = rawSection.Section;
                    if (this.Sections.ContainsKey(currentActiveSection) == false) {
                        this.Sections.Add(currentActiveSection, new IniSection(currentActiveSection));
                    }

                    // if we had any comments before the section, add them!
                    if (commentBuffer.Count > 0) {
                        foreach (string comment in commentBuffer) {
                            this.Sections[currentActiveSection].Comments.Add(comment);
                        }
                        commentBuffer.Clear();
                    }
                } else if (line is RawIniComment rawComment) {
                    // add to comment buffer...
                    commentBuffer.Add(rawComment.Comment);
                } else if (line is RawIniKeyValue rawKey) {
                    if (this.Sections[currentActiveSection].ContainsKey(rawKey.Key) == false) {
                        IniKey newKeyEntry = new IniKey(rawKey.Key);
                        newKeyEntry.SetSimpleValue(rawKey.Value);
                        this.Sections[currentActiveSection][rawKey.Key] = newKeyEntry;
                    } else {
                        IniKey keyEntry = this.Sections[currentActiveSection][rawKey.Key];
                        switch (keyEntry.Value.Kind) {
                            case IniValueKind.Empty:
                                {
                                    keyEntry.SetSimpleValue(rawKey.Value);
                                    break;
                                }
                            case IniValueKind.Simple:
                                {
                                    // convert this key entry to a List of values
                                    List<string> newValues = new List<string>() {
                                        keyEntry.GetSimpleValue(),
                                        rawKey.Value,
                                    };
                                    keyEntry.SetListValue(newValues);
                                    break;
                                }
                            case IniValueKind.List:
                                {
                                    // add value to list!
                                    keyEntry.AccessListValue().Add(rawKey.Value);
                                    break;
                                }
                            case IniValueKind.Indexed:
                            case IniValueKind.IndexedList:
                                {
                                    throw new InvalidOperationException($"Error processing Key={keyEntry.Key}! An existing indexed key of the same name has already been found! Can not have both normal keys and indexed keys with the same name in the same ini! That is too krazy even for this parser... ");
                                }
                        }
                        
                        // if we had any comments before the key, add them!
                        if (commentBuffer.Count > 0) {
                            foreach (string comment in commentBuffer) {
                                this.Sections[currentActiveSection][rawKey.Key].Comments.Add(comment);
                            }
                            commentBuffer.Clear();
                        }
                    }
                } else if (line is RawIniKeyIndexValue rawIndexedKey) {
                    if (this.Sections[currentActiveSection].ContainsKey(rawIndexedKey.Key) == false) {
                        IniKey newKeyEntry = new IniKey(rawIndexedKey.Key);
                        newKeyEntry.SetIndexedValue(new Dictionary<int, string>() {
                            [rawIndexedKey.Index] = rawIndexedKey.Value,
                        });
                        this.Sections[currentActiveSection][rawIndexedKey.Key] = newKeyEntry;
                    } else {
                        IniKey keyEntry = this.Sections[currentActiveSection][rawIndexedKey.Key];
                        switch (keyEntry.Value.Kind) {
                            case IniValueKind.Empty:
                                {
                                    keyEntry.SetIndexedValue(new Dictionary<int, string>() {
                                        [rawIndexedKey.Index] = rawIndexedKey.Value,
                                    });
                                    break;
                                }
                            case IniValueKind.Simple:
                            case IniValueKind.List:
                                {
                                    throw new InvalidOperationException($"Error processing Key={rawIndexedKey.Key}! An existing non-indexed key of the same name has already been found! Can not have both normal keys and indexed keys with the same name in the same ini! That is too krazy even for this parser... ");
                                }
                            case IniValueKind.Indexed:
                                {
                                    Dictionary<int, string> dictionary = keyEntry.AccessIndexedValue();
                                    if (dictionary.ContainsKey(rawIndexedKey.Index) == false) {
                                        dictionary.Add(rawIndexedKey.Index, rawIndexedKey.Value);
                                    } else {
                                        // key already found... convert this into a IndexedList...
                                        Dictionary<int, List<string>> newDictionary = new Dictionary<int, List<string>>();
                                        foreach (KeyValuePair<int, string> kvp in dictionary) {
                                            newDictionary.Add(kvp.Key, new List<string>(){ kvp.Value });
                                        }
                                        keyEntry.SetIndexedValueList(newDictionary);
                                    }
                                    
                                    break;
                                }
                            case IniValueKind.IndexedList:
                                {
                                    Dictionary<int, List<string>> dictionary = keyEntry.AccessIndexedValueList();
                                    if (dictionary.ContainsKey(rawIndexedKey.Index) == false) {
                                        dictionary.Add(rawIndexedKey.Index, new List<string>(){rawIndexedKey.Value});
                                    } else {
                                        dictionary[rawIndexedKey.Index].Add(rawIndexedKey.Value);
                                    }
                                    break;
                                }
                            
                        }
                        
                    }
                    
                    // if we had any comments before the key, add them!
                    if (commentBuffer.Count > 0) {
                        foreach (string comment in commentBuffer) {
                            this.Sections[currentActiveSection][rawIndexedKey.Key].Comments.Add(comment);
                        }
                        commentBuffer.Clear();
                    }
                }
            }
        }

        public void WriteToFile(string fileName,  Encoding? encoding = null, string? lineTermination = null) {
            using FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            this.WriteToStream(fs, encoding, lineTermination);
        }

        public void WriteToStream(Stream stream, Encoding? encoding = null, string? lineTermination = null, int bufferSize = 1048576, bool leaveOpen = false) {
            using StreamWriter sw = new StreamWriter(stream, encoding ?? Encoding.Default, bufferSize, leaveOpen);
            string endLine = lineTermination ?? Environment.NewLine;
            
            foreach (KeyValuePair<string, IniSection> kvp in this.Sections) {
                if (kvp.Value.Comments.Count > 0 ||
                    kvp.Value.KeyDictionary.Count > 0) {
                    // do not write empty sections...
                    
                    // first write comments..
                    foreach (string s in kvp.Value.Comments) {
                        sw.Write(s);
                        sw.Write(endLine);
                    }
                    sw.Write(kvp.Value.ToString(endLine));
                    foreach (KeyValuePair<string, IniKey> kvp2 in kvp.Value.KeyDictionary) {
                        sw.Write(kvp2.Value.ToString(endLine));
                    }
                    sw.Write(endLine);
                }
            }
        }
        
        public void WriteToStringBuilder(StringBuilder sb, string? lineTermination = null) {
            string endLine = lineTermination ?? Environment.NewLine;
            
            foreach (KeyValuePair<string, IniSection> kvp in this.Sections) {
                if (kvp.Value.Comments.Count > 0 ||
                    kvp.Value.KeyDictionary.Count > 0) {
                    // do not write empty sections...
                    
                    // first write comments..
                    foreach (string s in kvp.Value.Comments) {
                        sb.Append(s);
                        sb.Append(endLine);
                    }
                    sb.Append(kvp.Value.ToString(endLine));
                    foreach (KeyValuePair<string, IniKey> kvp2 in kvp.Value.KeyDictionary) {
                        sb.Append(kvp2.Value.ToString(endLine));
                    }
                    sb.Append(endLine);
                }
            }
        }
    }
}
