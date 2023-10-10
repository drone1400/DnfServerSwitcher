using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
namespace DnfServerSwitcher.Models.KrazyIni.Raw;

public class RawIniDocument {
    public static readonly Regex RegexWhitespace = new Regex(@"\A\s*\Z");
    public static readonly Regex RegexComment = new Regex(@"\A;(?<comment>.*)\Z");
    public static readonly Regex RegexSection = new Regex(@"\A\s*\[\s*(?<section>\w+(\.\w+)*)\s*\]\s*\Z");
    public static readonly Regex RegexKeyValue = new Regex(@"\A\s*(?<key>\w+)\s*=\s*(?<value>.*)\s*\Z");
    public static readonly Regex RegexKeyIndexValue = new Regex(@"\A\s*(?<key>\w+)\s*\[(?<index>\d+)\]=\s*(?<value>.*)\s*\Z");
    public static readonly Regex RegexHexnumber = new Regex(@"\A\s*[0x|0X](?<value>[a-fA-F0-9]+)\s*\Z");

    public List<RawIniLine> Lines { get; private set; } = new List<RawIniLine>();
    public List<int> ErrorLineIndex { get; private set; } = new List<int>();

    public void AddLine(RawIniLine line) {
        this.Lines.Add(line);
    }

    public bool ParseFile(string fileName, Encoding? encoding = null) {
        try {
            using FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            return this.ParseStream(fs, encoding);
        } catch {
            return false;
        }
    }
    
    public bool ParseStream(Stream stream, Encoding? encoding = null) {
        bool hasErrors = false;
        try {
            using StreamReader sr = new StreamReader(stream, encoding ?? Encoding.Default);

            this.Lines.Clear();
            this.ErrorLineIndex.Clear();

            int lineIndex = 0;

            while (!sr.EndOfStream) {
                string ? line = sr.ReadLine();
                if (line is string lineStr) {
                    if (!this.ProcessLine(lineStr)) {
                        hasErrors = true;
                        this.ErrorLineIndex.Add(lineIndex);
                    }
                }
                lineIndex++;
            }
            
            return !hasErrors;
        } catch {
            return false;
        }
    }

    private bool ProcessLine(string line) {
        Match matchWhiteespace = RawIniDocument.RegexWhitespace.Match(line);
        if (matchWhiteespace.Success) {
            this.Lines.Add(new RawIniWhitespace(line));
            return true;
        }

        Match matchComment = RawIniDocument.RegexComment.Match(line);
        if (matchComment.Success) {
            this.Lines.Add(new RawIniComment(line, matchComment.Groups["comment"].Value));
            return true;
        }

        Match matchKeyValue = RawIniDocument.RegexKeyValue.Match(line);
        if (matchKeyValue.Success) {
            this.Lines.Add(new RawIniKeyValue(line, matchKeyValue.Groups["key"].Value, matchKeyValue.Groups["value"].Value));
            return true;
        }
        
        Match matchKeyIndexValue = RawIniDocument.RegexKeyIndexValue.Match(line);
        if (matchKeyIndexValue.Success) {
            try {
                this.Lines.Add(new RawIniKeyIndexValue(line,
                    matchKeyIndexValue.Groups["key"].Value,
                    int.Parse(matchKeyIndexValue.Groups["index"].Value),
                    matchKeyIndexValue.Groups["value"].Value));
            } catch (Exception) {
                return false;
            }
            return true;
        }

        Match matchSection = RawIniDocument.RegexSection.Match(line);
        if (matchSection.Success) {
            this.Lines.Add(new RawIniSection(line, matchSection.Groups["section"].Value));
            return true;
        }
        
        

        this.Lines.Add(new RawIniBadLine(line));
        return false;
    }
}
