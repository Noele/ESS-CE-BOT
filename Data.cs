using System.Collections.Generic;

namespace CommonErrorsBot.CEB
{
    public class TextOverlay {
        public List<object> Lines { get; set; } 
        public bool HasOverlay { get; set; } 
        public string Message { get; set; } 
    }

    public class ParsedResult {
        public TextOverlay TextOverlay { get; set; } 
        public string TextOrientation { get; set; } 
        public int FileParseExitCode { get; set; } 
        public string ErrorMessage { get; set; } 
        public string ErrorDetails { get; set; } 
        public string ParsedText { get; set; } 
    }

    public class Data {
        public List<ParsedResult> ParsedResults { get; set; } 
        public int OCRExitCode { get; set; } 
        public bool IsErroredOnProcessing { get; set; } 
        public string ProcessingTimeInMilliseconds { get; set; } 
        public string SearchablePDFURL { get; set; } 
    }


}