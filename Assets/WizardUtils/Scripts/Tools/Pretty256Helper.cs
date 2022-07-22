using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.Tools
{
    public static class Pretty256Helper
    {
        public static string Encode(byte[] inArray, char[] Atlas = null)
        {
            if (Atlas == null) Atlas = AtlasFleschutz;

            string final = "";

            foreach (byte b in inArray)
            {
                try
                {
                    final += Atlas[b];
                }
                catch (IndexOutOfRangeException e)
                {
                    Debug.LogError($"Failed to index byte {b}\n" + e.ToString());
                    final += '?';
                }
            }

            return final;
        }

        public static byte[] Decode(string payload, char[] Atlas = null)
        {
            if (Atlas == null) Atlas = AtlasFleschutz;

            byte[] output = new byte[payload.Length];

            for (int payloadIndex = 0; payloadIndex < payload.Length; payloadIndex++)
            {
                char c = payload[payloadIndex];
                for (int atlasIndex = 0; atlasIndex < 256; atlasIndex++)
                {
                    if (Atlas[atlasIndex] == c)
                    {
                        output[payloadIndex] = (byte)atlasIndex;
                        break;
                    }    
                }
            }

            return output;
        }


        // ported from https://github.com/fleschutz/base256unicode
        public static char[] AtlasFleschutz => new char[256]
        {
'0','1','2','3','4','5','6','7','8','9',								 // ASCII digits
'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z', // ASCII upper
'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z', // ASCII lower
(char)192,(char)193,(char)194,(char)195,(char)196,(char)197,(char)198,(char)199,(char)200,(char)201,(char)202,(char)203,(char)204,(char)205,(char)206,(char)207,					 // Latin one
(char)208,(char)209,(char)210,(char)211,(char)212,(char)213,(char)214,(char)215,(char)216,(char)217,(char)218,(char)219,(char)220,(char)221,(char)222,(char)223,					 // Umlauts ...
(char)224,(char)225,(char)226,(char)227,(char)228,(char)229,(char)230,(char)231,(char)232,(char)233,(char)234,(char)235,(char)236,(char)237,(char)238,(char)239,
(char)240,(char)241,(char)242,(char)243,(char)244,(char)245,(char)246,(char)247,(char)248,(char)249,(char)250,(char)251,(char)252,(char)253,(char)254,(char)255,
(char)256,(char)257,(char)258,(char)259,(char)260,(char)261,(char)262,(char)263,(char)264,(char)265,(char)266,(char)267,(char)268,(char)269,(char)270,(char)271,
(char)272,(char)273,(char)274,(char)275,(char)276,(char)277,(char)278,(char)279,(char)280,(char)281,(char)282,(char)283,(char)284,(char)285,(char)286,(char)287,
(char)288,(char)289,(char)290,(char)291,(char)292,(char)293,(char)294,(char)295,(char)296,(char)297,(char)298,(char)299,(char)300,(char)301,(char)302,(char)303,
(char)304,(char)305,(char)306,(char)307,(char)308,(char)309,(char)310,(char)311,(char)312,(char)313,(char)314,(char)315,(char)316,(char)317,(char)318,(char)319,
(char)320,(char)321,(char)322,(char)323,(char)324,(char)325,(char)326,(char)327,(char)328,(char)329,(char)330,(char)331,(char)332,(char)333,(char)334,(char)335,
(char)336,(char)337,(char)338,(char)339,(char)340,(char)341,(char)342,(char)343,(char)344,(char)345,(char)346,(char)347,(char)348,(char)349,(char)350,(char)351,
(char)352,(char)353,(char)354,(char)355,(char)356,(char)357,(char)358,(char)359,(char)360,(char)361,(char)362,(char)363,(char)364,(char)365,(char)366,(char)367,
(char)368,(char)369,(char)370,(char)371,(char)372,(char)373,(char)374,(char)375,(char)376,(char)377,(char)378,(char)379,(char)380,(char)381,(char)382,(char)383,
(char)384,(char)385
        };
    }
}