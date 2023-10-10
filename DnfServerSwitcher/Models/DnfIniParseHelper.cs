using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DnfServerSwitcher.Models.KrazyIni;
using DnfServerSwitcher.Models.KrazyIni.Raw;

namespace DnfServerSwitcher.Models {
    public static class DnfIniParseHelper {
        
        public static IniDocument ParseDnf2011SystemIni(string filePath) {
            // NOTE: DNF2011 uses UTF-16 encoding with no byte order mark for the system.ini file!!!...
            // Also important to note, the system.ini is a non-standard ini format!
            // ------------
            // it uses duplicate key values for defining array/list values!
            // example:
            //    something=FirstValue
            //    something=SecondValue
            // ------------
            // it also uses indexed key values for defining array/list values!
            // example:
            //    something[1]=FirstValue
            //    something[2]=SecondValue
            // ------------
            // also to note, from my experience with DNF2001, the system.ini file
            // can contain a combination of the two!
            // example:
            //    something[1]=FirstValue
            //    something[1]=ReplacementValueOrSomething
            //    something[2]=SecondValue
            // surprisingly, DNF2001 seems to parse such entries just fine!
            // operating here on assumption that DNF2011 would behave the same...
            // ------------
            // luckily, I don't think it ever uses the same name for both an indexed and a non indexed key...
            // so this example should not be valid, i think:
            //    something=FirstValue
            //    something[2]=SecondValue
            // operating here on assumption that keys are either non indexed lists, or indexed arrays (or perhaps maps/dictionaries?)
            // ------------
            // so in light of these weird quirks of DNF, i made a weird custom ini parser for it...
            RawIniDocument rawDoc = new RawIniDocument();
            rawDoc.ParseFile(filePath, Encoding.Unicode);
            IniDocument doc = new IniDocument(rawDoc);
            return doc;
        }

        public static void WriteDnf2011SystemIni(string filePath, IniDocument doc) {
            // NOTE: DNF2011 uses UTF-16 encoding with no byte order mark for the system.ini file!!!...
            // I know I'm repeating myself, but the file must be in this exact format or the game WILL crash!
            // if there is a byte order mark, the game will crash, even if the file is in UTF-16 encoding!
            // so we write the ini to a string builder, encode that to a byte buffer
            // and then write the bytes to the file without the BOM at the start!
            
            StringBuilder sb = new StringBuilder(1048576); // initialize with 1024 * 1024 capacity, should be enough to hold all the ini text!
            doc.WriteToStringBuilder(sb, "\r\n");

            byte[] rawUtf16Bytes = Encoding.Unicode.GetBytes(sb.ToString());
            
            using FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            fs.Write(rawUtf16Bytes, 0, rawUtf16Bytes.Length);
            
            // NOTE: DNF2011 uses a double null byte termination for the file...
            // from what I can determine, the game doesn't crash if they aren't there, but leave them there anyway
            fs.WriteByte(0x00);
            fs.WriteByte(0x00);
        }
    }
}
