using System;
using System.ComponentModel.DataAnnotations;

namespace RPPP_WebApp.ViewModels
{
    public class LogViewModel
    {
        public DateTime? EnteredDate { get; set; }
        public string LogDate { get; set; }
        public string LogLevel { get; set; }

        public string LogLogger { get; set; }

        public string LogMessage { get; set; }

        public string LogException { get; set; }
    }
}
