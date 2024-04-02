using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;
using System.Configuration;


namespace xServerLog
{
    class Program
    {
        static void Main(string[] args)
        {

            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            // detect the control+c 
            Console.CancelKeyPress += delegate {
                // call methods to clean up
                xfileActions.CloseAppwithControlC();
            };


            // Set the culture of the application and force the date format
            xfileActions.SetCultureLanguage();

            // Inizialize the log file
            // xfileActions.FileSysLogInit();

            // validate the license
            //xfileActions.LicValidation();

            // verify the cleanup log routine
            //xfileActions.CleanUpLogFiles();

            // Create the header of the service 
            xfileActions.AppHeader("Console");
            xfileActions.MachineInformation("Console");

            // get the informations about address and port from the configuration file
            string serveraddress = ConfigurationManager.AppSettings["LogServerAddress"];
            int servertcpport = Convert.ToInt32(ConfigurationManager.AppSettings["LogServerPortTCP"]);

            // Start the server defining IP, Port and Maximum connections
            xfileActions.StartSupportServer(serveraddress, servertcpport);

            // application terminated
            xfileActions.FileSysLogAdd("Server terminated!", "Operation");

        }
    }
}
