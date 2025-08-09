using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NumberToWord.Pages
{
    public class IndexModel : PageModel
    {
        // Whatever the user types in the input field ends up here
        [BindProperty]
        public string? WhatTheyTyped { get; set; }

        // This is what we'll show back to the user in words
        public string? VerbalResult { get; set; }

        // This runs when the form is submitted (POST)
        public void OnPost()
        {
            // Let’s make sure the user actually typed a number
            if (int.TryParse(WhatTheyTyped, out var actualNumber))
            {
                // Turn that number into English words
                VerbalResult = SayItOutLoud(actualNumber);
            }
            else
            {
                // Nope, not a number. Friendly warning.
                VerbalResult = "Umm, that doesn't look like a number.";
            }
        }

        // Converts an integer into English words — works up to millions
        private static string SayItOutLoud(int number)
        {
            if (number == 0) return "Zero. Plain and simple.";

            if (number < 0)
                return "Negative " + SayItOutLoud(Math.Abs(number)); // Recursively handle negative numbers

            var result = new List<string>(); // Final word list we'll stitch together

            // Handle large number chunks first (millions, thousands, hundreds)
            var chunks = new[]
            {
                (1_000_000, "million"),
                (1_000, "thousand"),
                (100, "hundred")
            };

            // Loop through each chunk to break the number down
            foreach (var (value, label) in chunks)
            {
                if (number >= value)
                {
                    var part = number / value;         // e.g. 123456 → 123
                    result.Add($"{SayItOutLoud(part)} {label}"); // Recursively convert the part
                    number %= value;                  // Reduce the number for next round
                }
            }

            // Now handle the leftovers (anything < 100)
            if (number > 0)
            {
                if (result.Count > 0)
                    result.Add("and"); 

                // Words for numbers < 20
                var smallNums = new[]
                {
                    "zero", "one", "two", "three", "four", "five",
                    "six", "seven", "eight", "nine", "ten", "eleven",
                    "twelve", "thirteen", "fourteen", "fifteen", "sixteen",
                    "seventeen", "eighteen", "nineteen"
                };

                // Words for multiples of ten
                var tens = new[]
                {
                    "", "", "twenty", "thirty", "forty",
                    "fifty", "sixty", "seventy", "eighty", "ninety"
                };

                // Handle small numbers directly
                if (number < 20)
                {
                    result.Add(smallNums[number]);
                }
                else
                {
                    var tenner = number / 10;
                    var remainder = number % 10;

                    var phrase = tens[tenner];

                    // Add hyphenated part if there's a leftover (like twenty-three)
                    if (remainder > 0)
                        phrase += "-" + smallNums[remainder];

                    result.Add(phrase);
                }
            }

            // Combine everything into one nice sentence
            return string.Join(" ", result).Trim();
        }
    }
}
