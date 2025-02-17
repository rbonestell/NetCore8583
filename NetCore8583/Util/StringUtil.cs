﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NetCore8583.Util
{
    /// <summary>
    /// </summary>
    public static class StringUtil
    {
        private const string SAFE = ":~!@#$%^&*()-_+=/\\,.[]{}|?<>";
        private const string PRINTABLE = ": ~`!@#$%^&*()-_+=/\\,.[]{}|?<>\"'";

        private static readonly Dictionary<string, string> IpPatterns = new Dictionary<string, string>(0);

        /// <summary>
        ///     Check whether the given string is alphanumeric or not. True when it is alphanumeric and false on the contrary
        /// </summary>
        /// <param name="string0">THe string to check</param>
        /// <returns>bool</returns>
        public static bool IsAlphanumeric(this string string0)
        {
            var pattern = "^[a-zA-Z0-9-\\s+]*$";
            return Regex.IsMatch(string0,
                pattern);
        }

        /// <summary>
        ///     Check whether the given string contains only alphabetic characters. True when it contains only alphabetic
        ///     characters and false on the contrary
        /// </summary>
        /// <param name="string0">The string to check</param>
        /// <returns>bool</returns>
        public static bool IsWord(this string string0)
        {
            var pattern = "^[a-zA-Z]$";
            return Regex.IsMatch(string0,
                pattern);
        }

        /// <summary>
        ///     This function sanitze string against SQL Injections
        /// </summary>
        /// <param name="string0">The String to sanitize</param>
        /// <returns></returns>
        public static string Sanitize(this string string0)
        {
            if (string0.IsEmpty()) return null;
            if (string0.IsAlphanumeric()) return string0.Trim();
            if (string0.IsWord()) return string0.Trim();
            return string0.RegexReplace("-{2,}",
                    "-")
                // transforms multiple --- in - use to comment in sql scripts
                .RegexReplace(@"[*/]+",
                    string.Empty)
                // removes / and * used also to comment in sql scripts
                .RegexReplace(
                    @"(;|\s)(exec|execute|select|insert|update|delete|create|alter|drop|rename|truncate|backup|restore)\s",
                    string.Empty,
                    RegexOptions.IgnoreCase);
        }

        /// <summary>
        ///     This function checks the validity of an url. it returns true when valid and false on the contrary
        /// </summary>
        /// <param name="url">Url to check</param>
        /// <returns>true when valid and false on the contrary</returns>
        public static bool IsValidUrl(this string url)
        {
            var pattern =
                "^(?:(?:(ht|f)tps?|file|news|gopher)://)?(([\\w!~*'()\\.&=+$%-]+: )?[\\w!~*'()\\.&=+$%-]+@)?(([0-9]{1,3}\\.){3}[0-9]{1,3}|([\\w!~*'()-]+\\.)*([\\w^-][\\w-]{0,61})?[\\w]\\.[a-z]{2,6})(:[0-9]{1,4})?((/*)|(/+[\\w!~*'()\\.;?:@&=+$,%#-]+)+/*)$";
            return Regex.IsMatch(url,
                pattern);
        }

        /// <summary>
        ///     This function checks the validity of an string url. it returns true when valid and false on the contrary
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsUrl(this string url)
        {
            return url.StartsWith("http://") || url.StartsWith("https://");
        }

        /// <summary>
        ///     Thos function check whether a filename is a valid filename.
        /// </summary>
        /// <param name="filename">The file name</param>
        /// <param name="platformIndependent"></param>
        /// <returns></returns>
        public static bool IsValidFileName(this string filename,
            bool platformIndependent)
        {
            var sPattern = @"^(?!^(PRN|AUX|CLOCK\$|NUL|CON|COM\d|LPT\d|\..*)(\..+)?$)[^\x00-\x1f\\?*:\"";|/]+$";
            if (platformIndependent)
                sPattern =
                    @"^(([a-zA-Z]:|\\)\\)?(((\.)|(\.\.)|([^\\/:\*\?""\|<>\. ](([^\\/:\*\?""\|<>\. ])|([^\\/:\*\?""\|<>]*[^\\/:\*\?""\|<>\. ]))?))\\)*[^\\/:\*\?""\|<>\. ](([^\\/:\*\?""\|<>\. ])|([^\\/:\*\?""\|<>]*[^\\/:\*\?""\|<>\. ]))?$";
            return Regex.IsMatch(filename,
                sPattern,
                RegexOptions.CultureInvariant);
        }

        /// <summary>
        ///     This function will strip off the leading and trailing character in the string. For example, "1045" will just become
        ///     1045
        ///     <param name="string0">The string to strip off</param>
        /// </summary>
        /// <returns>string</returns>
        public static string StripQuotes(this string string0)
        {
            // if the string is not null nor empty
            if (!string0.IsEmpty() && string0.Length == 0) return string0;

            // if the first and the last characters are quoted, just do 1 substring
            var stringToArray = string0.ToCharArray();
            if (string0.Length > 1 && stringToArray[0].Equals('"') && stringToArray[string0.Length - 1].Equals('"'))
                return string0.Substring(1,
                    string0.Length - 1);
            if (stringToArray[0].Equals('"')) string0 = string0.Substring(1);
            else if (stringToArray[string0.Length - 1].Equals('"'))
                string0 = string0.Substring(0,
                    string0.Length - 1);
            return string0;
        }

        /// <summary>
        ///     Check whether a character is safe
        /// </summary>
        /// <param name="ch">Character to check</param>
        /// <returns>bool. True when it is safe and false on the contrary</returns>
        public static bool IsSafeChar(this char ch)
        {
            if (ch.CompareTo('a') >= 0 && ch.CompareTo('z') <= 0) return true;
            if (ch.CompareTo('A') >= 0 && ch.CompareTo('Z') <= 0) return true;
            if (ch.CompareTo('0') >= 0 && ch.CompareTo('9') <= 0) return true;

            // loop through the SAFE string
            // When the character matches one of the character in that SAFE string we exit
            var safeArray = SAFE.ToCharArray();
            return safeArray.Any(t => ch.CompareTo(t) == 0);
        }

        /// <summary>
        ///     Returns true if the String is considered a "safe" string where only specific
        ///     characters are allowed to be used.  Useful for checking passwords or other
        ///     information you don't want a user to be able to type just anything in.
        ///     This method does not allow any whitespace characters, newlines, carriage returns.
        ///     Primarily allows [a-z] [A-Z] [0-9] and a few other useful ASCII characters
        ///     such as ":~!@#$%^*()-_+=/\\,.[]{}|?<>" (but not the quote chars)
        /// </summary>
        /// <param name="string0">It is used to identify the string to check</param>
        /// <returns>bool. true when it is safe or false</returns>
        public static bool IsSafeString(this string string0)
        {
            // convert the string into characters array and loop through the array
            var stringArray = string0.ToCharArray();
            for (var i = 0; i < stringArray.Length; i++) if (!stringArray[i].IsSafeChar()) return false;
            return true;
        }

        /// <summary>
        ///     Check emptiness.True when it is empty and false on the contrary
        /// </summary>
        /// <param name="string0">String to check</param>
        /// <returns>bool. True when it is empty and false on the contrary</returns>
        public static bool IsEmpty(this string string0)
        {
            return string.IsNullOrEmpty(string0);
        }

        /// <summary>
        ///     Checks numeric. True when the string is numeric string and false on the contrary
        /// </summary>
        /// <param name="string0">String to check</param>
        /// <returns>bool. True when the string is numeric string and false on the contrary</returns>
        public static bool IsNumeric(this string string0)
        {
            // When the string is empty it is not a valid numeric
            if (string0.IsEmpty()) return false;
            // regex
            var regex = "^[-+]?[0-9]+$";
            return Regex.IsMatch(string0.Trim(),
                regex);
        }

        /// <summary>
        ///     Retrieve the substring from a given string starting from the last character to the defined length.
        /// </summary>
        /// <param name="str">String to truncate</param>
        /// <param name="length">the length to truncate</param>
        /// <returns></returns>
        public static string Right(this string str,
            int length)
        {
            return str.Substring(str.Length - length,
                length);
        }

        /// <summary>
        ///     Checks money
        /// </summary>
        /// <param name="string0">String to check</param>
        /// <param name="currencyPosition">The currency position in the string</param>
        /// <returns>bool. True when the string is in a valid money format and false on the contrary</returns>
        public static bool IsMoney(this string string0,
            string currencyPosition)
        {
            // When the string is null return false
            if (string0.IsEmpty()) return false;
            const string moneyRegex =
                "(?!0,?\\d)(?:\\d{1,3}(?:([,\\.])\\d{3})?(?:\\1\\d{3})*|(?:\\d+))((?!\\1)[,\\.]\\d{2})?";
            var regex = "^(?!\\x{00a2})\\p{Sc}?" + moneyRegex + "$";
            if (IsEmpty(currencyPosition) && currencyPosition.Equals("right"))
                regex = "^" + moneyRegex + "(?<!\\x{00a2})\\p{Sc}?$";
            return Regex.IsMatch(string0.Trim(),
                regex);
        }

        /// <summary>
        ///     Check valid email.True when it is a valid email address and false on the contrary
        /// </summary>
        /// <param name="string0">String to check</param>
        /// <returns>bool. True when it is avlid email address and false on the contrary</returns>
        public static bool IsValidEmail(this string string0)
        {
            var pattern =
                "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,}|com|org|net|edu|gov|mil|biz|info|mobi|name|aero|asia|jobs|museum)$";
            return Regex.IsMatch(string0,
                pattern);
        }

        /// <summary>
        ///     Checks IPv4
        /// </summary>
        /// <param name="string0">string to check</param>
        /// <returns>bool. True when it is a valid IP v4 and false on the contrary</returns>
        public static bool IsValidIPv4(this string string0)
        {
            // populate the pattern dictionary
            PopulateIpPatterns();

            // Get the IPv4 pattern regex
            string pattern;
            if (IpPatterns.TryGetValue("IPv4",
                out pattern))
                return Regex.IsMatch(string0,
                    pattern);
            return false;
        }

        /// <summary>
        ///     Checks IPv6
        /// </summary>
        /// <param name="string0">string to check</param>
        /// <returns>bool. True when it is a valid IP v6 and false on the contrary</returns>
        public static bool IsValidIPv6(this string string0)
        {
            // populate the pattern dictionary
            PopulateIpPatterns();

            // Get the IPv4 pattern regex
            string pattern;
            if (IpPatterns.TryGetValue("IPv6",
                out pattern))
                return Regex.IsMatch(string0,
                    pattern);
            return false;
        }

        /// <summary>
        ///     Checks IP
        /// </summary>
        /// <param name="string0">String to check</param>
        /// <returns>bool. True when it is a valid IP and false on the contrary</returns>
        public static bool IsValidIp(this string string0)
        {
            return string0.IsValidIPv4() || string0.IsValidIPv6();
        }

        /// <summary>
        ///     Check whether the given string has a minimum length
        /// </summary>
        /// <param name="string0">string to check</param>
        /// <param name="miniLength">The minimum length to check</param>
        /// <returns>bool. True when it has the given minimum length and false on the contrary</returns>
        public static bool HasMiniLength(this string string0,
            int miniLength)
        {
            if (string0.IsEmpty() && miniLength != 0) return false;
            if (!string0.IsEmpty()) return string0.Trim().Length >= miniLength;

            return false;
        }

        /// <summary>
        ///     Check whether the given string has a maximum length
        /// </summary>
        /// <param name="string0">string to check</param>
        /// <param name="maxLength">The maximum length to check</param>
        /// <returns>bool. True when it has the given maximum length and false on the contrary</returns>
        public static bool HasMaxLength(this string string0,
            int maxLength)
        {
            if (string0.IsEmpty() && maxLength != 0 && maxLength > 0) return true;
            if (string0.IsEmpty() && maxLength != 0 && maxLength < 0) return false;
            if (!string0.IsEmpty()) return string0.Trim().Length <= maxLength;

            return false;
        }

        /// <summary>
        ///     Time validation, determines if the string passed is a valid time.
        ///     Validates times as 12hr or 24hr (HH:MM) or am/pm ([H]H:MM[a|p]m)
        ///     Matches times separated by either : or . will match a 24 hour time, or a 12 hour time with AM or PM specified.
        ///     Allows 0-59 minutes, and 0-59 seconds. Seconds are not required.
        /// </summary>
        /// <param name="string0">String to validate</param>
        /// <returns>bool</returns>
        public static bool IsValidTime(this string string0)
        {
            var pattern =
                "^((([0]?[1-9]|1[0-2])(:|\\.)[0-5][0-9]((:|\\.)[0-5][0-9])?( )?(AM|am|aM|Am|PM|pm|pM|Pm))|(([0]?[0-9]|1[0-9]|2[0-3])(:|\\.)[0-5][0-9]((:|\\.)[0-5][0-9])?))$";
            return Regex.IsMatch(string0,
                pattern);
        }

        /// <summary>
        ///     Checks that a value is a valid phone number
        ///     The format used is either:
        ///     - ITU-T E.164 standard format which is for instance : +233247063817. According to ITU-T E.164 phone number are not
        ///     mor than 15 characters.
        ///     or - EPP format which is :  +CCC.NNNNNNNNNNxEEEE where C is the 1-3 digit country code, N is up to 14 digits, and E
        ///     is the (optional) extension.
        /// </summary>
        /// <param name="string0">The string to check</param>
        /// <param name="standard">The possible values are ITU and EPP . The default value is ITU</param>
        /// <returns>bool</returns>
        public static bool IsPhoneNumber(this string string0,
            string standard)
        {
            const string standardFormat = "^\\+(?:[0-9] ?){6,14}[0-9]$";
            //ITU-T E.164 standard
            if (!IsEmpty(standard) && standard.Equals("EPP"))
            {
                var regex = "^\\+[0-9]{1,3}\\.[0-9]{4,14}(?:x\\.+)?$";
                return Regex.IsMatch(string0,
                    regex);
            }

            return Regex.IsMatch(string0,
                standardFormat);
        }

        /// <summary>
        ///     validates a phone number
        /// </summary>
        /// <param name="string0">the phone number</param>
        /// <returns></returns>
        public static bool IsPhoneNumber(this string string0)
        {
            const string standardFormat = "^\\+(?:[0-9] ?){6,14}[0-9]$";
            return Regex.IsMatch(string0,
                standardFormat);
        }

        /// <summary>
        ///     checks whether a phone number is local
        /// </summary>
        /// <param name="string0">the phone number</param>
        /// <returns></returns>
        public static bool IsLocalPhoneNumber(this string string0)
        {
            const string regex = "^(?:[0-9] ?){6,14}[0-9]$";
            return new Regex(regex).IsMatch(string0);
        }

        /// <summary>
        ///     Mask a credit number by revealing the last four digits and append a prefix to it.
        /// </summary>
        /// <param name="ccnumber">the credit card number</param>
        /// <param name="prefix">the prefix</param>
        /// <example>
        ///     ccnumber = 4007000000027
        ///     prefix = x_card_num
        ///     masked = x_card_numXXXXXXXXX0027
        /// </example>
        /// <returns></returns>
        public static string MaskCreditCardNumber(this string ccnumber,
            string prefix = "")
        {
            var len = ccnumber.Length - 4;
            var sb = new StringBuilder();
            if (string.IsNullOrEmpty(prefix)) for (var i = 0; i < len; i++) sb.Append("*");
            else for (var i = 0; i < len - prefix.Length; i++) sb.Append("*");

            var last4Digit = ccnumber.Substring(ccnumber.Length - 4,
                4);
            return string.IsNullOrEmpty(prefix) ? $"{sb}{last4Digit}" : $"{prefix}{sb}{last4Digit}";
        }

        /// <summary>
        ///     Gets the card BIN
        /// </summary>
        /// <param name="cardNumber">the card number</param>
        /// <returns></returns>
        public static string CardBin(this string cardNumber)
        {
            return string.Join("",
                cardNumber.Take(6));
        }

        /// <summary>
        ///     Validates a datetime value.
        ///     All values matching the "date" core validation rule, and the "time" one will be valid
        ///     format. The possible formats to pass to the function are :
        ///     dmy 05-12-2011 or 05-12-11 separators can be a space, period, dash, forward slash
        ///     mdy 12-05-2011 or 12-05-11 separators can be a space, period, dash, forward slash
        ///     ymd 2011-12-05 or 11-12-05 separators can be a space, period, dash, forward slash
        ///     dMy 05 December 2011 or 05 Dec 2011
        ///     Mdy December 05, 2011 or Dec 05, 2011 comma is optional
        ///     My December 2011 or Dec 2011
        ///     my 12/2011 separators can be a space, period, dash, forward slash
        /// </summary>
        /// <param name="string0">The string to check</param>
        /// <param name="format">The format</param>
        /// <returns></returns>
        public static bool IsValidDate(this string string0,
            string format)
        {
            // When the format is not defined
            if (format.IsEmpty())
            {
                // When the string to check is not null
                if (!string0.IsEmpty())
                    return Regex.IsMatch(string0.Trim(),
                        "^(?:(?:(?:(?:(?:1[6-9]|[2-9]\\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00)))(\\/|-|\\.|\\x20)(?:0?2\\1(?:29)))|(?:(?:(?:1[6-9]|[2-9]\\d)?\\d{2})(\\/|-|\\.|\\x20)(?:(?:(?:0?[13578]|1[02])\\2(?:31))|(?:(?:0?[1,3-9]|1[0-2])\\2(29|30))|(?:(?:0?[1-9])|(?:1[0-2]))\\2(?:0?[1-9]|1\\d|2[0-8]))))$");
            }
            else
            {
                if (!string0.IsEmpty())
                {
                    // Based upon the format given the check will be done accordingly
                    var regex = "";
                    if (string.Equals(format.Trim(),
                        "dmy"))
                        regex =
                            "^(?:(?:31(\\/|-|\\.|\\x20)(?:0?[13578]|1[02]))\\1|(?:(?:29|30)(\\/|-|\\.|\\x20)(?:0?[1,3-9]|1[0-2])\\2))(?:(?:1[6-9]|[2-9]\\d)?\\d{2})$|^(?:29(\\/|-|\\.|\\x20)0?2\\3(?:(?:(?:1[6-9]|[2-9]\\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\\d|2[0-8])(\\/|-|\\.|\\x20)(?:(?:0?[1-9])|(?:1[0-2]))\\4(?:(?:1[6-9]|[2-9]\\d)?\\d{2})$";
                    if (string.Equals(format.Trim(),
                        "mdy"))
                        regex =
                            "^(?:(?:(?:0?[13578]|1[02])(\\/|-|\\.|\\x20)31)\\1|(?:(?:0?[13-9]|1[0-2])(\\/|-|\\.|\\x20)(?:29|30)\\2))(?:(?:1[6-9]|[2-9]\\d)?\\d{2})$|^(?:0?2(\\/|-|\\.|\\x20)29\\3(?:(?:(?:1[6-9]|[2-9]\\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:(?:0?[1-9])|(?:1[0-2]))(\\/|-|\\.|\\x20)(?:0?[1-9]|1\\d|2[0-8])\\4(?:(?:1[6-9]|[2-9]\\d)?\\d{2})$";
                    if (string.Equals(format.Trim(),
                        "ymd"))
                        regex =
                            "^(?:(?:(?:(?:(?:1[6-9]|[2-9]\\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00)))(\\/|-|\\.|\\x20)(?:0?2\\1(?:29)))|(?:(?:(?:1[6-9]|[2-9]\\d)?\\d{2})(\\/|-|\\.|\\x20)(?:(?:(?:0?[13578]|1[02])\\2(?:31))|(?:(?:0?[1,3-9]|1[0-2])\\2(29|30))|(?:(?:0?[1-9])|(?:1[0-2]))\\2(?:0?[1-9]|1\\d|2[0-8]))))$";
                    if (string.Equals(format.Trim(),
                        "dMy"))
                        regex =
                            "^((31(?!\\ (Feb(ruary)?|Apr(il)?|June?|(Sep(?=\\b|t)t?|Nov)(ember)?)))|((30|29)(?!\\ Feb(ruary)?))|(29(?=\\ Feb(ruary)?\\ (((1[6-9]|[2-9]\\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)))))|(0?[1-9])|1\\d|2[0-8])\\ (Jan(uary)?|Feb(ruary)?|Ma(r(ch)?|y)|Apr(il)?|Ju((ly?)|(ne?))|Aug(ust)?|Oct(ober)?|(Sep(?=\\b|t)t?|Nov|Dec)(ember)?)\\ ((1[6-9]|[2-9]\\d)\\d{2})$";
                    if (string.Equals(format.Trim(),
                        "Mdy"))
                        regex =
                            "^(?:(((Jan(uary)?|Ma(r(ch)?|y)|Jul(y)?|Aug(ust)?|Oct(ober)?|Dec(ember)?)\\ 31)|((Jan(uary)?|Ma(r(ch)?|y)|Apr(il)?|Ju((ly?)|(ne?))|Aug(ust)?|Oct(ober)?|(Sep)(tember)?|(Nov|Dec)(ember)?)\\ (0?[1-9]|([12]\\d)|30))|(Feb(ruary)?\\ (0?[1-9]|1\\d|2[0-8]|(29(?=,?\\ ((1[6-9]|[2-9]\\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)))))))\\,?\\ ((1[6-9]|[2-9]\\d)\\d{2}))$";
                    if (string.Equals(format.Trim(),
                        "My"))
                        regex =
                            "^(Jan(uary)?|Feb(ruary)?|Ma(r(ch)?|y)|Apr(il)?|Ju((ly?)|(ne?))|Aug(ust)?|Oct(ober)?|(Sep(?=\\b|t)t?|Nov|Dec)(ember)?)[ /]((1[6-9]|[2-9]\\d)\\d{2})$";
                    if (string.Equals(format.Trim(),
                        "my"))
                        regex = "^(((0[123456789]|10|11|12)([- /\\.])(([1][9][0-9][0-9])|([2][0-9][0-9][0-9]))))$";

                    // Perform the check and return the output
                    return Regex.IsMatch(string0.Trim(),
                        regex);
                }
            }
            return false;
        }

        /// <summary>
        ///     Time validation, determines if the string passed is a valid time.
        ///     Validates time as 24hr (HH:MM) or am/pm ([H]H:MM[a|p]m)
        ///     Does not allow/validate seconds.
        /// </summary>
        /// <param name="string0">The string to check</param>
        /// <returns>bool</returns>
        public static bool IsValidTimeAs24H(this string string0)
        {
            return Regex.IsMatch(string0,
                "^((0?[1-9]|1[012])(:[0-5]\\d){0,2} ?([AP]M|[ap]m))$|^([01]\\d|2[0-3])(:[0-5]\\d){0,2}$");
        }

        /// <summary>
        ///     Validates a datetime value.
        ///     All values matching the "date" core validation rule, and the "time" one will be valid
        ///     format. The possible formats to pass to the function are :
        ///     dmy 05-12-2011 or 05-12-11 separators can be a space, period, dash, forward slash
        ///     mdy 12-05-2011 or 12-05-11 separators can be a space, period, dash, forward slash
        ///     ymd 2011-12-05 or 11-12-05 separators can be a space, period, dash, forward slash
        ///     dMy 05 December 2011 or 05 Dec 2011
        ///     Mdy December 05, 2011 or Dec 05, 2011 comma is optional
        ///     My December 2011 or Dec 2011
        ///     my 12/2011 separators can be a space, period, dash, forward slash
        /// </summary>
        /// <param name="string0">The string to check</param>
        /// <param name="format">The format</param>
        /// <returns>bool</returns>
        public static bool IsValidDateTime(this string string0,
            string format)
        {
            // variable holding the end result
            var valid = false;

            // Get the various parts of the string0
            var parts = string0.Split(' ');

            // check the parts
            if (parts.Length > 1)
            {
                // Get the time part
                var time = parts[parts.Length - 1];

                // Reconstruct the date part
                var date = parts[0];
                for (var i = 1; i < parts.Length - 2; i++) date += " " + parts[i];

                // Perform the validation check
                valid = date.IsValidDate(format) && time.IsValidTime();
            }
            return valid;
        }

        /// <summary>
        ///     Checks that a value is a Ghana-based valid mobile number
        /// </summary>
        /// <param name="string0">The string to check</param>
        /// <param name="networkPrefix"></param>
        /// <returns>bool</returns>
        public static bool IsValidGhMsisdn(this string string0,
            string networkPrefix)
        {
            return Regex.IsMatch(string0,
                "^[0]?" + networkPrefix + "[0-9]{6,14}$");
        }

        /// <summary>
        ///     Checks if both strings are equal to each other.  Safely handles the case
        ///     where either string may be null.  The strings are evaluated as equal if
        ///     they are both null or if they actually equal each other.  One string
        ///     that is null while the other one isn't (even if its an empty string) will
        ///     be considered as NOT equal.  Case sensitive comparisons are optional.
        /// </summary>
        /// <param name="string0">The string to compare</param>
        /// <param name="string1">The string compared to</param>
        /// <param name="caseSensitive">Case sensitivity</param>
        /// <returns>bool</returns>
        public static bool IsEqual(this string string0,
            string string1,
            bool caseSensitive)
        {
            if (string0 == null && string1 == null) return true;
            if (string0 == null && !string.IsNullOrEmpty(string1)) return false;
            if (!string.IsNullOrEmpty(string0) && string1 == null) return false;

            if (caseSensitive)
                return string.Equals(string0,
                    string1);
            return false;
        }

        /// <summary>
        ///     Checks if both strings are equal to each other.  Safely handles the case
        ///     where either string may be null.  The strings are evaluated as equal if
        ///     they are both null or if they actually equal each other.  One string
        ///     that is null while the other one isn't (even if its an empty string) will
        ///     be considered as NOT equal.  Case sensitive comparisons are optional.
        /// </summary>
        /// <param name="string0">The string to compare</param>
        /// <param name="string1">The string compared to</param>
        /// <returns>bool</returns>
        public static bool IsEqual(this string string0,
            string string1)
        {
            return string0.IsEqual(string1,
                true);
        }

        /// <summary>
        ///     Get 7-bit ASCII character array from input String. The lower 7 bits of each character in the input string is
        ///     assumed to be the ASCII character value.
        /// </summary>
        /// <param name="string0"></param>
        /// <returns>byte[]</returns>
        public static byte[] GetAsciiBytes(this string string0)
        {
            return Encoding.ASCII.GetBytes(string0);
        }

        /// <summary>
        ///     Get string value of an array byte
        /// </summary>
        /// <param name="input">Byte array whose string value is needed</param>
        /// <returns>string</returns>
        public static string GetAsciiString(this byte[] input)
        {
            return Encoding.ASCII.GetString(input);
        }

        public static string GetString(this byte[] input)
        {
            return Encoding.UTF8.GetString(input);
        }

        /// <summary>
        ///     Returns the value from calling "toString()" on the object, but is a safe version that gracefully handles NULL
        ///     objects by returning a String of "".
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <returns>string</returns>
        public static string ToStringWithNullAsEmpty(this object obj)
        {
            return obj == null ? "" : obj.ToString();
        }

        /// <summary>
        ///     Safely capitalizes a string by converting the first character to upper
        ///     case. Handles null, empty, and strings of length of 1 or greater.  For example, this will convert "joe" to "Joe".
        ///     If the string is null, this
        ///     will return null.  If the string is empty such as "", then it'll just return an empty string such as "".
        /// </summary>
        /// <param name="string0">String whose first letter needs to be capitalized</param>
        /// <returns>string</returns>
        public static string Capitalize(this string string0)
        {
            // When the string is not null
            if (!string0.IsEmpty())
            {
                if (string0.Length == 1) return string0.ToUpper();
                var sb = new StringBuilder(string0.Length);
                sb.Append(string0.Substring(0,
                    1).ToUpper());
                sb.Append(string0.Substring(1));
                return sb.ToString();
            }
            return string0;
        }

        /// <summary>
        ///     Capitalizes the first letter of a string
        /// </summary>
        /// <param name="string0"></param>
        /// <returns></returns>
        public static string UppercaseFirst(this string string0)
        {
            return char.ToUpper(string0.First()) + string0.Substring(1).ToLower();
        }

        /// <summary>
        ///     Checks whether an object is a boolean or not
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public static bool IsBoolean(this object check)
        {
            var list = new List<object>
            {
                "1",
                "0",
                "true",
                "false",
                1,
                0
            };
            return list.Contains(check);
        }

        /// <summary>
        ///     Cheks whether a string is a valid UUID or GUID
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public static bool IsUuid(this string check)
        {
            return Regex.IsMatch(check,
                "/^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[0-5][a-fA-F0-9]{3}-[089aAbB][a-fA-F0-9]{3}-[a-fA-F0-9]{12}$/");
        }

        /// <summary>
        ///     Cheks whether a string is a valid UUID or GUID
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public static bool IsGuid(this string check)
        {
            //Guid guid;
            //return Guid.TryParse(check, out guid);
            try
            {
                var guid = new Guid(check);
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public static byte[] GetBytes(this string check)
        {
            return Encoding.UTF8.GetBytes(check);
        }

        public static sbyte[] GetSignedbytes(this string check,
            Encoding encoding = null)
        {
            var bytes = encoding == null ? Encoding.Default.GetBytes(check) : encoding.GetBytes(check);
            return Array.ConvertAll(bytes,
                b => unchecked((sbyte)b));
        }

        #region Private Routines

        private static string RegexReplace(this string stringValue,
            string matchPattern,
            string toReplaceWith)
        {
            return Regex.Replace(stringValue,
                matchPattern,
                toReplaceWith);
        }

        private static string RegexReplace(this string stringValue,
            string matchPattern,
            string toReplaceWith,
            RegexOptions regexOptions)
        {
            return Regex.Replace(stringValue,
                matchPattern,
                toReplaceWith,
                regexOptions);
        }

        // This property holds some characters that are not often allowed in most applications

        // populate the ipPatterns dictionary
        internal static void PopulateIpPatterns()
        {
            // variable holding the pattern
            var pattern = "";

            // check whether the dictionnary contains the key IPv6
            if (!IpPatterns.ContainsKey("IPv6"))
            {
                pattern = "((([0-9A-Fa-f]{1,4}:){7}(([0-9A-Fa-f]{1,4})|:))|(([0-9A-Fa-f]{1,4}:){6}";
                pattern += "(:|((25[0-5]|2[0-4]\\d|[01]?\\d{1,2})(\\.(25[0-5]|2[0-4]\\d|[01]?\\d{1,2})){3})";
                pattern += "|(:[0-9A-Fa-f]{1,4})))|(([0-9A-Fa-f]{1,4}:){5}((:((25[0-5]|2[0-4]\\d|[01]?\\d{1,2})";
                pattern +=
                    "(\\.(25[0-5]|2[0-4]\\d|[01]?\\d{1,2})){3})?)|((:[0-9A-Fa-f]{1,4}){1,2})))|(([0-9A-Fa-f]{1,4}:)";
                pattern +=
                    "{4}(:[0-9A-Fa-f]{1,4}){0,1}((:((25[0-5]|2[0-4]\\d|[01]?\\d{1,2})(\\.(25[0-5]|2[0-4]\\d|[01]?\\d{1,2}))";
                pattern += "{3})?)|((:[0-9A-Fa-f]{1,4}){1,2})))|(([0-9A-Fa-f]{1,4}:){3}(:[0-9A-Fa-f]{1,4}){0,2}";
                pattern += "((:((25[0-5]|2[0-4]\\d|[01]?\\d{1,2})(\\.(25[0-5]|2[0-4]\\d|[01]?\\d{1,2})){3})?)|";
                pattern += "((:[0-9A-Fa-f]{1,4}){1,2})))|(([0-9A-Fa-f]{1,4}:){2}(:[0-9A-Fa-f]{1,4}){0,3}";
                pattern += "((:((25[0-5]|2[0-4]\\d|[01]?\\d{1,2})(\\.(25[0-5]|2[0-4]\\d|[01]?\\d{1,2}))";
                pattern += "{3})?)|((:[0-9A-Fa-f]{1,4}){1,2})))|(([0-9A-Fa-f]{1,4}:)(:[0-9A-Fa-f]{1,4})";
                pattern += "{0,4}((:((25[0-5]|2[0-4]\\d|[01]?\\d{1,2})(\\.(25[0-5]|2[0-4]\\d|[01]?\\d{1,2})){3})?)";
                pattern += "|((:[0-9A-Fa-f]{1,4}){1,2})))|(:(:[0-9A-Fa-f]{1,4}){0,5}((:((25[0-5]|2[0-4]";
                pattern += "\\d|[01]?\\d{1,2})(\\.(25[0-5]|2[0-4]\\d|[01]?\\d{1,2})){3})?)|((:[0-9A-Fa-f]{1,4})";
                pattern +=
                    "{1,2})))|(((25[0-5]|2[0-4]\\d|[01]?\\d{1,2})(\\.(25[0-5]|2[0-4]\\d|[01]?\\d{1,2})){3})))(%.+)?";

                // add to the dictionary the IPv6 pattern
                IpPatterns.Add("IPv6",
                    pattern);
            }

            // check whether the dictionary contains the key IPv4
            if (!IpPatterns.ContainsKey("IPv4"))
            {
                pattern =
                    "(?:(?:25[0-5]|2[0-4][0-9]|(?:(?:1[0-9])?|[1-9]?)[0-9])\\.){3}(?:25[0-5]|2[0-4][0-9]|(?:(?:1[0-9])?|[1-9]?)[0-9])";
                IpPatterns.Add("IPv4",
                    pattern);
            }
        }

        #endregion Private Routines
    }
}