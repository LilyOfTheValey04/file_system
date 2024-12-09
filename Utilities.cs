using MyFileSustem.CusLinkedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSustem
{
    public static class Utilities
    {
        
        public static bool IsItNullorWhiteSpace(string input)
        {
            if (input == null) 
            {
                return true;
            }
            foreach (char c in input)
            {
                if (!IsItWhiteSpace(c))// Ако намерим символ, който не е празен, връщаме false
                {
                    return false;
                }
            }
            return true;// Ако всички символи са празни (или низът е празен), връщаме true
        }

        public static bool IsItWhiteSpace(char c)
        {
            return c == ' ' || c == '\f' || c == '\v' || c == '\t' || c == '\n' || c == '\r';
        }

        public static bool EndWith (string input,string suffix)
        {
            if (input == null || suffix == null)
            {
                return false;
            }
            if (input.Length<suffix.Length)
            {
                return false;
            }
            for (int i = 0; i < suffix.Length; i++)
            {
            //check if +/- i
                if (input[input.Length - suffix.Length - i] != suffix[i])
                {
                    return false;
                } 
            }
            return true;
        }

        public static string MyJoin(string seperator, IEnumerable<string> values)
        {
            if (values==null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            if (seperator==null)
            {
                seperator = string.Empty;
            }

            StringBuilder result = new StringBuilder();
            bool isfirst = true;

            foreach (string value in values) 
            {
                if (!isfirst)
                {
                    result.Append(seperator);
                }
                result.Append(value);
                isfirst = false;
            }
            return result.ToString();

        }

        public static string CustomTrim(string input)
        {
            if (input==null)
            {
                return null;
            }

            int start = 0;
            int end = input.Length - 1;

            // Намерете първия не-празен символ отляво
            while (start <= end && IsItWhiteSpace(input[start]))
            {
                start++;
            }

            // Намерете първия не-празен символ отдясно
            while (start <= end && IsItWhiteSpace(input[end]))
            {
                end--;
            }

            // Върнете подниза между start и end (включително)
            string result = "";
            for(int i =start; i<=end;i++)
            {
                result += input[i]; 
            }
            return result;
        }

         public static string CustomToLower(string input)
         {
             if(input==null)
             {
                 return null;
             }

             char[] result = new char[input.Length];
             for (int i = 0; i < result.Length; i++)
             {
                 char c = input[i];
                 if (c>='A'&& c<='Z')
                 {
                     result[i] = (char)(c+('a' - 'A'));
                 }
                 else
                 {
                     // Ако не е главна буква, запазваме го без промяна
                     result[i] = c;
                 }
             }
             return new string (result);
         }

        public static string[] CustomSplit(string input, char delimiter)
        {
            if (input==null)
            {
                return null;
            }

            MyLinkedList<string> result = new MyLinkedList<string>();
            string currentSegment = "";
            for (int i = 0; i < input.Length; i++) {
                if (input[i] ==delimiter)
                {
                    result.AddLast(currentSegment);
                    currentSegment = "";
                }
                else
                {
                    currentSegment+=(input[i]);
                }
            }
            // Добавяме последния сегмент, ако има такъв
            if (currentSegment!="")
            {
                result.AddLast(currentSegment);
            }

            return result.ToArray();
        }

        public static string[] CustomSplitAndRemoveEmpty(string input, char delimiter)
        {
            if (input == null)
            {
                return new string[0]; // Ако входът е null, връщаме празен масив
            }

            // Разделяме входа на базата на разделителя
            string[] segments= CustomSplit(input, delimiter);
            MyLinkedList<string> result = new MyLinkedList<string>();
            foreach (var segment in segments)
            {
                if (!IsItNullorWhiteSpace(segment))
                {
                  result.AddLast(segment);
                }
            }

            return result.ToArray();
        }

        



        // PadLeft
        //PadRight



    }
}
