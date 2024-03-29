﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VakilPors.Core.Contracts.Services;

namespace VakilPors.Core.Services
{
    public class AntiSpamService : IAntiSpam
    {
        public AntiSpamService()
        {

        }

        public async Task<String> IsSpam(string Text)
        {
            bool isSpam =await  SpamCheck(Text);
            if (isSpam)
            {
                return "This message is detected as a spam and can not be shown.";
            }
            else
            {
                return "ok";
            }
        }
        public async Task <bool> SpamCheck(string text)
        {
            string pattern = @"(\+98|0)?9(\s?\d){9}"; // Pattern for Iranian phone numbers
            pattern += @"|\@\S*"; // Pattern for IDs starting with "@"
            pattern += @"|\#\w+"; // pattern for hashtags
            pattern += @"|\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b"; // Pattern for email addresses
            pattern += @"|\b(?:https?://|www\.)\S+\b"; // Pattern for URLs
            pattern += @"|\b09\d{9}\b"; // Pattern for Iranian phone numbers starting with "09"
            pattern += @"|\bwww\.\S+\b"; // Pattern for URLs starting with "www."
            pattern += @"|\b021\d{8}\b"; // Pattern for Tehran static numbers starting with "021"

            MatchCollection matches = Regex.Matches(text, pattern);
            bool containsRepeatedSequences =await CheckForRepeatedSequences(text);
            return (matches.Count > 0) || containsRepeatedSequences;
        }
        public async Task<bool> CheckForRepeatedSequences(string text)
        {
            const int MaxSequenceLength = 10; // Maximum length of repeated sequence to consider as spam
      
            List<string> sequences = new List<string>();
            for (int i = 0; i < text.Length - MaxSequenceLength + 1; i++)
            {
                for (int j = MaxSequenceLength; j <= text.Length - i; j++)
                {
                    string sequence = text.Substring(i, j);
                    if (sequences.Contains(sequence))
                    {
                        return true;
                    }
                    sequences.Add(sequence);
                }
            }

            return false;
        }
    }
}
 