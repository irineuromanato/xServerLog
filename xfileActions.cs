/*
Copyright (c) 2024  xfileActions

Module Name:
    xfileActions.cs 

Abstract:
    This module contains code which is used in all the xServerControl Suite
    Library with all the class, functions and methods used in all the system and applications

Author:
    Irineu Romanato (irineuromanato) 03-Jan-2024

Environment:
    xServerControl Suite

Revision History:
    version 1.0.0.0 - pre-publish review (cleanup the code)

*/

/*
 Definicao do arquivo de configuracao 

    <add key="AppLanguage" value="en-US" /> 
                Define o Idioma da aplicacao, se alterar para pt-br o software ira traduzir todos os textos que estao no arquivo de resource chamado localization.resx

    <add key="AppPath" value="C:\xServerLog\" />
                Define o caminho do sistema, diretorios aonde sao gerados logs e salvo a licenca

    <add key="AppRunningInMinutes" value="1" />
                Define o numero de minutos para executar os jobs

    <add key="AppWorkingPath" value="C:\xServerLog\Working\" />
                Define o caminho para a pasta de trabalho, utilizada para manipular informacoes internas

    <add key="AppSysLogPath" value="C:\xServerLog\Log\" />
                Define o caminho onde sao salvos os arquivo de log do sistema

    <add key="AppActLogPath" value="C:\xServerLog\ActLog\" />
                Define o caminho onde sao salvos os arquivo de log de execucao 

    <add key="AppBackupPath" value="C:\xServerLog\Backup\" />
                Define o caminho onde sao salvos os arquivo de backup, utilizado para salvar informacoes internas

    <add key="AppBackupName" value="Backup" />
                ****
                *
    <add key="AppCriticalErrorFile" value="CriticalError.log" />
                Define o nome do arquivo que ira conter mensagens de erros criticas que nao conseguem ser salvas nos arquivos, pois sao rotinas executadas antes da 
                iniciacao de logs e demais. Esses sao error muito criticos.

    <add key="AppLogFileName" value="xSysLog_" />
                Define o nome do arquivo de log que contem os eventos de sistema

    <add key="AppActLogName" value="xJobLog_" />
                Define o nome do arquivo de log que contem os jobs que sao executados

	<add key="AppSecLogName" value="xSecLog_" />
                Define o nome do arquivo de log que contem itens de segurança

    <add key="AppLogMode" value="Normal" />
                Define o modo como o log estara sendo executado, por enquanto so aceita normal

    <add key="AppMessageMode" value="Syslog" />
                Define o modo como o log e o applicativo esta sendo executado, pode ser Normal aonde tem o header e Syslog aonde nao tem o header

    <add key="AppRunningMode" value="Console" />
                Define o modo como o log e o applicativo esta sendo executado, pode ser console, service ou application 
                Console ecoa tudo em modo console (tela preta do dos )
                Service ecoa tudo no event viewer e ou no log
                Application deve ecoar somente dentro do log

    <add key="AppDebugMode" value="No" />
                Define se esta em modo debug ou seja as mensagem sao mais verbosas e mais completas, pode ser Yes ou No. Default No

    <add key="AppSendMailWhenStart" value="No" />
                Pode ser Yes or No e ira enviar um email toda vez que a aplicacao for iniciada.

    <add key="AppEmailAddressTo" value="irineu.romanato@gmail.com" />
                Endereco de email que ira receber os emails para cada execucao 

    <add key="AppEmailAddressFrom" value="iromanato@gmail.com" />
                Endereco de email de saida ou seja o email e enviado por ESSA caixa

    <add key="AppEmailOnError" value="act@gmail.com" />
                 Endereco de email caso ocorra erro critico

    <add key="AppSMTPUserAuth" value="iromanato@gmail.com" />
                Login utilizado para autenticar no servidor de SMTP

    <add key="AppSMTPUserAuthPass" value="jbblvhlkzbsyqchw" />
                Senha ou token utilizado para autenticar no servidor SMTP (Ainda em cleartext, precisa ser alterada)

    <add key="AppSMTPAddress" value="smtp.gmail.com" />
                Endereco do servidor SMTP

    <add key="AppSMTPPort" value="587" />
                Porta SMTP para envio de email

    <add key="AppServiceName" value="xfileActions" />
                Nome do Aplicativo e do Name espace do applicativo

    <add key="AppServiceStart" value="Automatic" />
    <add key="AppServiceInitiated" value="Yes" />
    <add key="AppLicenseFile" value="xfileActions.lic" />
                Define o nome do arquivo de licenca que contem o proprietario e serial e quando vence o software

    <add key="AppRegFile" value="xfileActions.reg" />
                Define o nome do arquivo de registro

    <add key="AppDefFile" value="xfileActions.def" />
                Define o nome do arquivo de definicao que contem a versao do software e dados de conexao

    <add key="AppExecInstrutionFile" value="xfileActions.ins" />
                Define o nome do arquivo de instrucoes do job que diz como cada job deve ser executado.

    <add key="AppDBConectionFile" value="xfileActions.con" />
                Define o nome do arquivo de conexao com o banco de dados 

    <add key="LogDateFormat" value="pt-BR" />
                Define o modo como esta o formato da data, isso é muito importante para controlar execucao e verificar os arquivos que serao copiados.

    <add key="LogCreationMode" value="Daily" />
                Define o modo como os arquivos de log sao criados, Daily é para diario.

    <add key="RegistryRServer" value="Yes" />
                Define se deve ser registrado ou nao

    <add key="NotifyOnTaskBar" value="Yes" />
                Define se deve ter notificacao quando minimizado

    <add key="DataType" value="MyDB" />
                Define se deve conectar com banco de dados

    <add key="AppDBDataType" value="MSSQL" />
                Define se o tipo de banco de dados

    <add key="CleanLogFiles" value="5" />
                Define o numero de dias para tras para limpar os logs antigos

	<add key="CompanyName" value="Used to Create the License" />  
                Usada para criar o nome da empresa que esta usando a licença do produto

*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace xServerLog
{
    public class xfileActions
    {

        // define namespace
        Type myApp = typeof(xServerLog.xfileActions);
        
        //return the operation system plataform
        public static OSPlatform GetRuntimePlatform()
        {
            //System.Environment.OSVersion
            // verify if MACOS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // return the value
                return OSPlatform.OSX;
            }

            // verify if Linux
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // return the value
                return OSPlatform.Linux;
            }

            // verify if Windows
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // return the value
                return OSPlatform.Windows;
            }

            throw new Exception(Localization.MsgErrCannotDetcOS);
            // return the value
        }

        // return a string to be used as complement in log or temp file
        public String ReturnDateTimeforFile()
        {
            // take date and time and convert to string
            DateTime DataHoraAgora = DateTime.Now;
            string varDataHoraAgora = DataHoraAgora.ToString("dd/MM/yyyy hh:mm");

            // replace caracters in the name, leaving only number
            String FileDataHoraAgora = varDataHoraAgora.Replace("/", "");
            FileDataHoraAgora = FileDataHoraAgora.Replace(" ", "");
            FileDataHoraAgora = FileDataHoraAgora.Replace(":", "");

            // return the value 
            return FileDataHoraAgora;
        }

        // Set the culture of the application and force the date format
        public void SetCultureLanguage()
        {

            // define the curture according the configuration file and the date time format according the brazil to use dd/MM/yyyy
            Thread.CurrentThread.CurrentCulture = new CultureInfo(ConfigurationManager.AppSettings["AppLanguage"], false);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(ConfigurationManager.AppSettings["AppLanguage"]);

            if (ConfigurationManager.AppSettings["LogDateFormat"] == "pt-BR")
            {
                Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                Thread.CurrentThread.CurrentCulture.DateTimeFormat.DateSeparator = "/";
            }
            else
            {
                // define the curture according the configuration file and the date time format according the brazil to use dd/MM/yyyy
                Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                Thread.CurrentThread.CurrentCulture.DateTimeFormat.DateSeparator = "/";
            }

        }

        // return a string to be used as complement in log or temp file
        public String ReturnDateforFile()
        {

            // define the string with the value to return
            var FileDataHoraAgora = DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "");

            // return the value 
            return FileDataHoraAgora;
        }

        // Create a Random files according the parameter passed 
        // Normally used in the test routine to create a amount of files
        public void CreateRandomFiles(int NumberOfFiles, string TargetFolderName, string FileExtensionName)
        {

            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            // if the number of files parameters is null set the number iqual 1
            if (NumberOfFiles == 0)
            {
                // if the number of files parameters is null set the number iqual 1
                NumberOfFiles = 1;
            }

            // if the TargetFolderName is Null set a value
            if (TargetFolderName == null)
            {
                // if the number of files parameters is null set the number iqual 1
                if (xfileActions.GetRuntimePlatform() == OSPlatform.Windows)
                {
                    TargetFolderName = "C:\\Temp\\";
                }
                else
                {
                    TargetFolderName = "/tmp/";
                }
            }

            // if the FileExtensionName is Null set a value
            if (FileExtensionName == null)
            {
                // if the number of files parameters is null set the number iqual 1
                FileExtensionName = ".txt";
            }

            // create the files x times according the parameter
            int xNumberOfFiles = 1;

            while (xNumberOfFiles <= NumberOfFiles)
            {
                var myUniqueFileName = $@"{DateTime.Now.Ticks}" + FileExtensionName; ;

                try
                {
                    FileStream fs = new FileStream(TargetFolderName + myUniqueFileName, FileMode.Create);
                    fs.Seek(1024 * 6, SeekOrigin.Begin);
                    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                    fs.Write(encoding.GetBytes("Creating a random file for test porpuse with random content"), 0, 4);
                    fs.Close();
                    xNumberOfFiles++;
                }
                catch (Exception ex)
                {

                    // verify if the system is runnign in service mode
                    Console.WriteLine(Localization.MsgErrorGenFiles + ex);
                }
            }
        }

        // Send Email Message to according the parameters in config file and parameters
        // passed in the method
        public String SendEmailto(String SubjectTxt, String BodyText)
        {

            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            // value to return
            var ReturnEmailSentTo = "Sent";

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["AppSMTPAddress"], Int32.Parse(ConfigurationManager.AppSettings["AppSMTPPort"]));

                mail.From = new MailAddress(ConfigurationManager.AppSettings["AppEmailAddressFrom"]);
                mail.To.Add(ConfigurationManager.AppSettings["AppEmailAddressTo"]);
                mail.Subject = SubjectTxt;
                mail.Body = BodyText;
                mail.IsBodyHtml = false;
                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.Priority = MailPriority.High;

                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.Port = Int32.Parse(ConfigurationManager.AppSettings["AppSMTPPort"]);
                SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["AppSMTPUserAuth"], ConfigurationManager.AppSettings["AppSMTPUserAuthPass"]);
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);

                // return the value
                return ReturnEmailSentTo;

            }
            catch (Exception ex)
            {

                // verify if the system is runnign in service mode
                if (ConfigurationManager.AppSettings["AppRunningMode"] == "Console")
                {
                    // write in log file
                    xfileActions.FileSysLogAdd(Localization.MsgFatalErrorSendMail, "Operation");
                    xfileActions.FileSysLogAdd(Localization.StrError + ex.ToString(), "Operation");
                }
            }

            // return the value
            return ReturnEmailSentTo;
        }


        // Metodo to encrypt data
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "x94bz4tt8876";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        // Metodo to dencrypt data
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "x94bz4tt8876";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        
        // Inicializa a criacao do arquivo de log 

        public void FileSysLogInit()
        {
            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            // declare the AppSystemLog variable
            var AppSystemLog = " ";
            var AppSystemLogDate = " ";

            // define namespace
            //Type myApp = typeof(xfileActions);

            // verify the parameter about the daily log file
            if (ConfigurationManager.AppSettings["LogCreationMode"] == "Daily")
            {
                // define Log file daily name for Application and Action Log
                AppSystemLogDate = xfileActions.ReturnDateforFile();
                AppSystemLog = ConfigurationManager.AppSettings["AppSysLogPath"] + ConfigurationManager.AppSettings["AppLogFileName"] + AppSystemLogDate + ".log";

            }
            else
            {
                // define Log file daily name for Application and Action Log
                AppSystemLogDate = "Fixed";
                AppSystemLog = ConfigurationManager.AppSettings["AppSysLogPath"] + ConfigurationManager.AppSettings["AppLogFileName"] + AppSystemLogDate + ".log";

            }

            // if file exist create else append the header and verify again
            if (File.Exists(AppSystemLog))
            {
                xfileActions.FileSysLogCreation();
                xfileActions.FileSysLogAdd(Localization.StrLogCreatedby + " " + myApp.Namespace, "Operation");
                xfileActions.FileSysLogAdd(Localization.StrSysteminit, "Operation");

            }
            else
            {
                xfileActions.FileSysLogCreation();
                xfileActions.FileSysLogAdd(Localization.StrLogCreatedby + " " + myApp.Namespace + " " + Localization.StrApplication, "Operation");
                xfileActions.FileSysLogAdd(Localization.StrSysteminit, "Operation");
                xfileActions.FileSysLogAdd(Localization.MsgAppLogFile + " (" + ConfigurationManager.AppSettings["AppLogFileName"] + AppSystemLogDate + ".log" + ")" + " " + Localization.StrNoFound, "Operation");
                xfileActions.FileSysLogAdd(Localization.MsgAppLogFileCrtdSuccess, "Operation");
                

                // gera o hash do arquivo do dia anterior
                xfileActions.CreateOneDayBackHash();
            }

            // Log entry to inform the application running mode
            xfileActions.FileSysLogAdd(Localization.StrSystemRunning + " " + Localization.StrMode + " " + ConfigurationManager.AppSettings["AppRunningMode"], "Operation");

            // Send and email informing the system is up and running.
            if (ConfigurationManager.AppSettings["AppSendMailWhenStart"] == "Yes")
            {
                DateTime DataAgora = DateTime.Now;
                xfileActions.SendEmailto(Localization.StrSysteminit, Localization.StrSystemStartedandUpRunn + " " + DataAgora.ToString("dd/MM/yyyy hh:mm"));
            }

        }

        public String FileSysLogCreation()
        {
            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            // value to return
            var ReturnFileSysLogCreation = "initiated";

            // declare the AppSystemLog variable
            var AppSystemLog = " ";
            var AppFileLogDate = " ";

            // verify the parameter about the daily log file
            if (ConfigurationManager.AppSettings["LogCreationMode"] == "Daily")
            {
                // define Log file daily name for Application and Action Log
                AppFileLogDate = xfileActions.ReturnDateforFile();
            }
            else
            {
                // define Log file daily name for Application and Action Log
                AppFileLogDate = "Fixed";
            }

            // Define corretamente o nome do arquivo de log e seguranca
            AppSystemLog = ConfigurationManager.AppSettings["AppLogFileName"] + AppFileLogDate + ".log";

            // combine or compose the path and the name (.log)
            var FullPathLogSysFile = System.IO.Path.Combine(ConfigurationManager.AppSettings["AppSysLogPath"], AppSystemLog);

            // Define the content that will be inside of the file
            {
                List<string> ConfigLogAppText = new List<string>();

                // verify if the log goes to a log file normal or to a syslog file (noheader)
                if (ConfigurationManager.AppSettings["AppMessageMode"] == "Normal")
                {
                    ConfigLogAppText.Add(" ");
                    ConfigLogAppText.Add(xfileActions.VerifyDefinitionFile("apptitle") + " - " + Localization.StrVersion + " [" + xfileActions.VerifyDefinitionFile("appversion") + "] - Release: " + xfileActions.VerifyDefinitionFile("apprelease"));
                    ConfigLogAppText.Add(xfileActions.VerifyDefinitionFile("appplataform"));
                    ConfigLogAppText.Add(xfileActions.VerifyDefinitionFile("applicense") + " - " + xfileActions.ReadLicenseFile("LicMode") + " " + Localization.StrLicense);
                    ConfigLogAppText.Add(Localization.StrLicIniDate + ": " + xfileActions.ReadLicenseFile("DataInit") + " - " + Localization.StrExpireIn + ": " + xfileActions.ReadLicenseFile("DataFin"));
                    ConfigLogAppText.Add(Localization.StrLicTo + " " + xfileActions.ReadLicenseFile("LicNamed") + " - " + Localization.StrSerialNumber + " " + xfileActions.ReadLicenseFile("LicSerialNum"));
                    ConfigLogAppText.Add("   ");
                    ConfigLogAppText.Add("[Data, Time, Application Name, Hostname, Logged User, Operation type, Message]");
                }
                else
                {
                    // Non informations to prevent blank line
                    //
                    if (!File.Exists(FullPathLogSysFile))
                    {
                        ConfigLogAppText.Add("[Data, Time, Application Name, Hostname, Logged User, Operation type, Message]");
                    }
                }
                //
                // save the lines from the array ConfigLogAppText inside of LOG file (.log)
                try
                {
                    System.IO.File.AppendAllLines(FullPathLogSysFile, ConfigLogAppText);
                }
                catch (Exception ex)
                {

                    // FATAL ERROR Directory for log not found or no permission to write in a disk
                    // Write error in a log file in the application default directory...
                    System.IO.File.AppendAllText(ConfigurationManager.AppSettings["AppPath"] + ConfigurationManager.AppSettings["AppCriticalErrorFile"],
                                                 DateTime.Now.ToString() + " " + Localization.MsgFatalErrorlogFolder + " " +
                                                 Localization.MsgAppTerminated + Environment.NewLine);

                    System.IO.File.AppendAllText(ConfigurationManager.AppSettings["AppPath"] + ConfigurationManager.AppSettings["AppCriticalErrorFile"], ex.ToString() + Environment.NewLine);
                    return ReturnFileSysLogCreation = "NoLogFolder";

                }
            }

            // returnin a empty information
            return ReturnFileSysLogCreation;
        }

        // Create a Log file about the system informations
        public String FileSysLogAdd(String MessageLogText, String OpTypeText)
        {

            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            // declare the AppSystemLog variable
            var AppSystemLog = " ";
            var AppSystemLogDate = " ";

            // take the default path, where the binary was initiated
            // var AppRunningPath = Directory.GetCurrentDirectory();

            // value to return 
            var ReturnFileSysLogAdd = "add";

            // verify the parameter about the daily log file
            if (ConfigurationManager.AppSettings["LogCreationMode"] == "Daily")
            {
                // define Log file name for Application and Action Log
                AppSystemLogDate = xfileActions.ReturnDateforFile();
            }
            else
            {
                // define Log file daily name for Application and Action Log
                AppSystemLogDate = "fixed";
            }

            // Verifica se o log é de operacoes ou seguranca e direciona para o local certo.
            if (OpTypeText == "Operation")
            { 
                AppSystemLog = ConfigurationManager.AppSettings["AppLogFileName"] + AppSystemLogDate + ".log";
            }   

            // combine or compose the path and the name (.log)
            var FullPathLogSysFile = System.IO.Path.Combine(ConfigurationManager.AppSettings["AppSysLogPath"], AppSystemLog);

            if (ConfigurationManager.AppSettings["LogCreationMode"] == "Daily")
            {

                // routine used to verify if the day was changed, and create a new file.
                if (!File.Exists(FullPathLogSysFile))
                {
                    if (ConfigurationManager.AppSettings["AppMessageMode"] == "Normal")
                    {
                        xfileActions.FileSysLogCreation();
                        xfileActions.FileSysLogAdd(Localization.MsgContLogFile, "Operation");
                        xfileActions.FileSysLogAdd(Localization.MsgAppLogFile + " (" + ConfigurationManager.AppSettings["AppLogFileName"] + AppSystemLogDate + ".log" + ") " + Localization.StrCreated, "Operation");
                        xfileActions.FileSysLogAdd(Localization.MsgAppLogFileCrtdSuccess, "Operation");

                    }

                    // gera o hash do arquivo do dia anterior
                    xfileActions.CreateOneDayBackHash();

                    // the day was changed and execute the cleanup log routine
                    xfileActions.CleanUpLogFiles();
                }

            }

            // Take the Hostname of the machine
            String HostName = Dns.GetHostName();
            String ApplName = myApp.Namespace;
                //ConfigurationManager.AppSettings["AppServiceName"];
            String LoggedUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            String LogOperationType = OpTypeText;

            // Start to append in the Log if file exist
            // Define the content that will be inside of the file
            String DataLog = DateTime.Now.ToString("dd/MM/yyyy");
            String HoraLog = DateTime.Now.ToString("HH:mm:ss");
            String LogLineText = DataLog + "," + HoraLog + "," + ApplName + "," + HostName + "," + LoggedUserName + "," + LogOperationType + "," + MessageLogText;

            // save the lines from the string to the log file (.log)
            try
            {
                System.IO.File.AppendAllText(FullPathLogSysFile, LogLineText + Environment.NewLine);
            }
            catch (Exception ex)
            {

                // FATAL ERROR Directory for log not found or no permission to write in a disk
                // Write error in a log file in the application default directory...
                System.IO.File.AppendAllText(ConfigurationManager.AppSettings["AppPath"] + ConfigurationManager.AppSettings["AppCriticalErrorFile"],
                                            DateTime.Now.ToString() + " " + Localization.MsgFatalErrorlogFolder + " " +
                                            Localization.MsgAppTerminated + Environment.NewLine);
                System.IO.File.AppendAllText(ConfigurationManager.AppSettings["AppPath"] + ConfigurationManager.AppSettings["AppCriticalErrorFile"], ex.ToString() + Environment.NewLine);

                // return the value
                return ReturnFileSysLogAdd = "NoLogFolder";
            }

            // return a empty information
            return ReturnFileSysLogAdd;
        }

        // Create a Log file about the system informations
        public void EchoLogToDisk(String MessageLogText)
        {
            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            // declare the AppSystemLog variable
            string AppEchLog = " ";
            string AppSystemLogDate = "";

            // verify the parameter about the daily log file
            if (ConfigurationManager.AppSettings["LogCreationMode"] == "Daily")
            {
                // define Log file name for Application and Action Log
                AppSystemLogDate = xfileActions.ReturnDateforFile();
            }
            else
            {
                // define Log file daily name for Application and Action Log
                AppSystemLogDate = "fixed";
            }

            // Verifica se o log é de operacoes ou seguranca e direciona para o local certo.
            AppEchLog = ConfigurationManager.AppSettings["AppEchFileName"] + AppSystemLogDate + ".log";

            // combine or compose the path and the name (.log)
            var FullPathLogEchFile = System.IO.Path.Combine(ConfigurationManager.AppSettings["AppSysLogPath"], AppEchLog);

            // verify if the log must be create daily
            if (ConfigurationManager.AppSettings["LogCreationMode"] == "Daily")
            {

                // routine used to verify if the day was changed, and create a new file.
                if (!File.Exists(FullPathLogEchFile))
                {
                    if (ConfigurationManager.AppSettings["AppMessageMode"] == "Normal")
                    {

                    // verify if the log goes to a log file normal or to a syslog file (noheader)
                    List<string> ConfigLogEchText = new List<string>();
                        ConfigLogEchText.Add(xfileActions.VerifyDefinitionFile("apptitle") + " - " + Localization.StrVersion + " [" + xfileActions.VerifyDefinitionFile("appversion") + "] - Release: " + xfileActions.VerifyDefinitionFile("apprelease"));
                        ConfigLogEchText.Add(xfileActions.VerifyDefinitionFile("appplataform"));
                        ConfigLogEchText.Add(xfileActions.VerifyDefinitionFile("applicense") + " - " + xfileActions.ReadLicenseFile("LicMode") + " " + Localization.StrLicense);
                        ConfigLogEchText.Add(Localization.StrLicIniDate + ": " + xfileActions.ReadLicenseFile("DataInit") + " - " + Localization.StrExpireIn + ": " + xfileActions.ReadLicenseFile("DataFin"));
                        ConfigLogEchText.Add(Localization.StrLicTo + " " + xfileActions.ReadLicenseFile("LicNamed") + " - " + Localization.StrSerialNumber + " " + xfileActions.ReadLicenseFile("LicSerialNum"));
                        ConfigLogEchText.Add("   ");
                        ConfigLogEchText.Add("[Data, Time, Application Name, Hostname, Logged User, Operation type, Message]");

                    // escreve as informacoes no arquivo novo
                    System.IO.File.AppendAllLines(FullPathLogEchFile, ConfigLogEchText);

                    }
                }
            }

            // save the lines from the string to the log file (.log)
            try
            {
                System.IO.File.AppendAllText(FullPathLogEchFile, MessageLogText + Environment.NewLine);
            }
            catch (Exception ex)
            {

                // FATAL ERROR Directory for log not found or no permission to write in a disk
                // Write error in a log file in the application default directory...
                System.IO.File.AppendAllText(ConfigurationManager.AppSettings["AppPath"] + ConfigurationManager.AppSettings["AppCriticalErrorFile"], ex.ToString() + Environment.NewLine);
            }

            // return a empty information
            return;
        }


        // Create a one day back hash file.
        public string CreateOneDayBackHash()
        {
            String OneDayHashCode = "";

            // calcula os dados para encontrar o nome do arquivo do dia anterior
            DateTime OneDayBack = (DateTime.Now.AddDays(-1));
            String OneDayBackSysFileName = ConfigurationManager.AppSettings["AppLogFileName"] + OneDayBack.ToString("dd/MM/yyyy").Replace("/", "");
            String OneDayBackSysFullLogName = System.IO.Path.Combine(ConfigurationManager.AppSettings["AppSysLogPath"], OneDayBackSysFileName + ".log");
            String OneDayBackSysFullFileHash = System.IO.Path.Combine(ConfigurationManager.AppSettings["AppSysLogPath"], OneDayBackSysFileName + ".hash");

            // verifica se o arquivo de sistema existe e calcula o hash
            if (File.Exists(OneDayBackSysFullLogName))
            {

                // cria um hash md5 para o arquivo de sistema
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(OneDayBackSysFullLogName))
                    {
                        var hash = md5.ComputeHash(stream);
                        String OneDayBackHashSys = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                        File.WriteAllText(OneDayBackSysFullFileHash, OneDayBackHashSys);
                    }
                }
            }

            // return the information
            return OneDayHashCode;
        }


        // Licensing file Validation        
        public string LicValidation()
        {

            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            // value to return
            var ReturnLicValidation = "valid";

            // if license file does not exist generate a trial period
            if (!File.Exists(ConfigurationManager.AppSettings["AppPath"] + ConfigurationManager.AppSettings["AppLicenseFile"]))
            {
                // Generate a trial period
                xfileActions.GenerateLicenseInTrial();
            }


            if (!String.IsNullOrEmpty(xfileActions.ReadLicenseFile("LicSerialNum")))
            {

                // create log entry with License file load sucess but test if the system run in console or service mode
                xfileActions.FileSysLogAdd(Localization.StrLicLoadSuc, "Operation");
            }
            else
            {
                // create log entry with License file load UNSuccess but test if the system run in console or service mode
                xfileActions.FileSysLogAdd(Localization.MsgAppLicNotValid, "Operation");
                xfileActions.FileSysLogAdd(Localization.MsgAppTerminated, "Operation");

                // exist forced
                Console.WriteLine(Localization.MsgAppLicNotValid);
                System.Environment.Exit(-1);

            }

            //
            // Validate the active period
            // create log entry with License Validation 
            CultureInfo cultures = new CultureInfo(ConfigurationManager.AppSettings["LogDateFormat"]);
            DateTime LicInitData = Convert.ToDateTime(xfileActions.ReadLicenseFile("DataInit"), cultures);
            DateTime LicFinalData = Convert.ToDateTime(xfileActions.ReadLicenseFile("DataFin"), cultures);

            // counting how many day left for the license
            int daysdiff = ((TimeSpan)(LicFinalData - DateTime.Now)).Days;
            if (daysdiff > 0)
            {
                // return a value and create a log entry to inform that lic period is ok and day left.
                ReturnLicValidation = "DaysLeft";
                xfileActions.FileSysLogAdd(Localization.StrSystemRunningActiveLicPer + " " + xfileActions.ReadLicenseFile("DataFin") + ".", "Operation");
                xfileActions.FileSysLogAdd(Localization.StrLicDayRemaing + " " + daysdiff.ToString() + ". ", "Operation");
            }
            else
            {
                // in this case return a value informind NO DAYS LEFT, create a log entry and exit the application
                ReturnLicValidation = "NODaysLeft";
                xfileActions.FileSysLogAdd(Localization.MsgAppLicOutOfPeriod + " " + xfileActions.ReadLicenseFile("DataFin") + ".", "Operation");
                xfileActions.FileSysLogAdd(Localization.StrLicDayRemaing + " " + daysdiff.ToString() + ". ", "Operation");
                xfileActions.FileSysLogAdd(Localization.MsgAppTerminated, "Operation");

                // exist forced
                Console.WriteLine(Localization.MsgAppLicOutOfPeriod);
                System.Environment.Exit(-1);
            }

            // return value
            return ReturnLicValidation;
        }

        // Create a Serial Number according the parameter passed
        public String CreateSerialKey(int keyLength)
        {
            string newSerialNumber = "";
            string SerialNumber = Guid.NewGuid().ToString("N").Substring(0, (int)keyLength).ToUpper();
            for (int iCount = 0; iCount < (int)keyLength; iCount += 4)
                newSerialNumber = newSerialNumber + SerialNumber.Substring(iCount, 4) + "-";
            newSerialNumber = newSerialNumber.Substring(0, newSerialNumber.Length - 1);
            return newSerialNumber;
        }

        // create a license file with the parameters and encrypt the content to save the file
        public String CreateLicenseFile(string xLicenseInicialDate, string xLicenseFinalDate, string xLicenseMode, string xLicenseNamed)
        {

            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            //value to return
            var LicCreated = "Created";

            // define a variable with the serial number
            var xLicenseSerialNumer = xfileActions.CreateSerialKey(24);

            // verify if the license file exist
            if (!File.Exists(ConfigurationManager.AppSettings["AppPath"] + ConfigurationManager.AppSettings["AppLicenseFile"]))
            {

                xfileActions.FileSysLogAdd(Localization.StrLicFileNotFound, "Operation");
                // comcat the total information passed as a parameter and compond the value
                var xLicenseFileContent = "[" + xLicenseInicialDate + ";" + xLicenseFinalDate + ";" + xLicenseMode + ";" + xLicenseSerialNumer + ";" + xLicenseNamed + "]";
                var encxLicenseFileContent = xfileActions.Encrypt(xLicenseFileContent);

                //define the license file and start to write the content to the file
                using (var fileLic = new StreamWriter(ConfigurationManager.AppSettings["AppPath"] + ConfigurationManager.AppSettings["AppLicenseFile"], true, System.Text.Encoding.Default))
                {
                    try
                    {
                        // write the content of the variable and create the license file
                        fileLic.WriteLine(encxLicenseFileContent);
                    }
                    catch (Exception ex)
                    {

                        // create a log entry informing the license file was not created.
                        xfileActions.FileSysLogAdd(Localization.MsgErrCrtLicenseFile, "Operation");
                        xfileActions.FileSysLogAdd(Localization.MsgAppTerminated, "Operation");
                        xfileActions.FileSysLogAdd(Localization.StrError + " " + ex, "Operation");

                        // exist forced
                        Console.WriteLine(Localization.MsgErrCrtLicenseFile);
                        System.Environment.Exit(-1);

                    }

                }
            }

            xfileActions.FileSysLogAdd(Localization.StrLicFile + " " + ConfigurationManager.AppSettings["AppLicenseFile"] + " " + Localization.MsgCrtdWithSuccess, "Operation");
            //return the value
            return LicCreated;
        }


        // read the license file with the parameters and dencrypt the content 
        public String ReadLicenseFile(string xLicenseParmContent)
        {

            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            //value to return
            var LicRead = "Read";

            // verify if the license file exist
            if (File.Exists(ConfigurationManager.AppSettings["AppPath"] + ConfigurationManager.AppSettings["AppLicenseFile"]))
            {

                // read the file and create the variable with the encrypted file content
                var encxLicenseFileContent = System.IO.File.ReadAllText(ConfigurationManager.AppSettings["AppPath"] + ConfigurationManager.AppSettings["AppLicenseFile"]);

                // decrypt the content of the file and associate with a variable
                var xLicenseFileContent = xfileActions.Decrypt(encxLicenseFileContent);

                int Tamanho = xLicenseFileContent.Length;

                var xLicenseAllFileContent = (DataInit: xLicenseFileContent.Substring(1, 10), DataFin: xLicenseFileContent.Substring(12, 10), LicMode: xLicenseFileContent.Substring(23, 8), LicSerialNum: xLicenseFileContent.Substring(32, 29), LicNamed: xLicenseFileContent.Substring(62, Tamanho - 63));

                //Test of the parameter passed and return the exactly value
                if (xLicenseParmContent == "All")
                {
                    // return all the file content decrypted
                    LicRead = xLicenseFileContent;
                }

                //Test of the parameter passed and return the exactly value
                if (xLicenseParmContent == "DataInit")
                {
                    // return all the file content decrypted
                    LicRead = xLicenseAllFileContent.DataInit;
                }

                //Test of the parameter passed and return the exactly value
                if (xLicenseParmContent == "DataFin")
                {
                    // return all the file content decrypted
                    LicRead = xLicenseAllFileContent.DataFin;
                }

                //Test of the parameter passed and return the exactly value
                if (xLicenseParmContent == "LicMode")
                {
                    // return all the file content decrypted
                    LicRead = xLicenseAllFileContent.LicMode;
                }

                //Test of the parameter passed and return the exactly value
                if (xLicenseParmContent == "LicSerialNum")
                {
                    // return all the file content decrypted
                    LicRead = xLicenseAllFileContent.LicSerialNum;
                }

                //Test of the parameter passed and return the exactly value
                if (xLicenseParmContent == "LicNamed")
                {
                    // return all the file content decrypted
                    LicRead = xLicenseAllFileContent.LicNamed;
                }

            }
            else
            {

                // determine the trial use for 45 days
                xfileActions.GenerateLicenseInTrial();


            }
            //return the value
            return LicRead;
        }

        // create a trial period license
        public void GenerateLicenseInTrial()
        {

            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            //rotina para criar arquivo de licença em modo trial
            var LicToday = (DateTime.Now.AddDays(-1)).ToString("dd/MM/yyyy");
            var LicTrialPeriod = (DateTime.Now.AddDays(59)).ToString("dd/MM/yyyy");
            xfileActions.CreateLicenseFile(LicToday, LicTrialPeriod, "TrialUse", ConfigurationManager.AppSettings["CompanyName"]);

        }
      
        // create a definition file with the parameters and encrypt the content to save the file
        public String CreateDefinitionFile(string xapptitle, string xappversion, string xapprelease, string xappplataform,
                                           string xapplicense, string xappServerRegistration, string xappDbServerInstance,
                                           string xappDbUser, string xappDbUserPass, string xappDbName)
        {

            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            //value to return
            var DefCreated = "Created";

            // verify if the Definition file exist
            if (!File.Exists(ConfigurationManager.AppSettings["AppPath"] + ConfigurationManager.AppSettings["AppDefFile"]))
            {

                // inform that the definition file does not exist and create 
                xfileActions.FileSysLogAdd(Localization.MsgDefFileNotFound, "Operation");

                try
                {
                    // create the definiton file writing the lines encrypted in the file.
                    var xDefContent = xapptitle + "\n" + xappversion + "\n" + xapprelease + "\n" + xappplataform + "\n" + xapplicense + "\n" + xappServerRegistration + "\n" + xappDbServerInstance + "\n" + xappDbUser + "\n" + xappDbUserPass + "\n" + xappDbName;

                    // Encrypt the string
                    string xDefFileContent = xfileActions.Encrypt(xDefContent);
                    Console.WriteLine(xDefFileContent);

                    //Write the encrypted content to the file
                    System.IO.File.WriteAllText(ConfigurationManager.AppSettings["AppPath"] + ConfigurationManager.AppSettings["AppDefFile"], xDefFileContent);

                }
                catch (Exception ex)
                {
                    // output the error in the console 
                    Console.WriteLine(Localization.MsgErrCrtDefFile);
                    Console.Write(ex.ToString());
                }

                // Write the log information about the file created with success
                xfileActions.FileSysLogAdd(Localization.MsgDefFile + " " + ConfigurationManager.AppSettings["AppDefFile"] + Localization.MsgCrtdWithSuccess, "Operation");
            }

            //return the value
            return DefCreated;
        }


        // read the Definition file with the parameters and decrypt the content
        public String VerifyDefinitionFile(string DefinitionParmContent)
        {

            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            //value to return
            var DefRead = "Read";

            // verify if the Definition file exist
            if (File.Exists(ConfigurationManager.AppSettings["AppPath"] + ConfigurationManager.AppSettings["AppDefFile"]))
            {

                // read the file and create the variable with the encrypted file content
                var encxDefFileContent = System.IO.File.ReadAllText(ConfigurationManager.AppSettings["AppPath"] + ConfigurationManager.AppSettings["AppDefFile"]);

                // decrypt the content of the file and associate with a variable
                var encxDefContent = xfileActions.Decrypt(encxDefFileContent);

                // Split the content of the variable into an array where each line is a value.
                //string[] lines = encxDefContent.Split("\n");
                string[] lines = encxDefContent.Split(
                    new[] { "\n" }, StringSplitOptions.None);

                //Test of the parameter passed and return the exactly value
                if (DefinitionParmContent == "All")
                {
                    // return all the file content decrypted
                    DefRead = encxDefContent;
                }

                //Test of the parameter passed and return the exactly value
                if (DefinitionParmContent == "apptitle")
                {
                    // return all the file content decrypted
                    DefRead = lines[0].ToString();
                }

                //Test of the parameter passed and return the exactly value
                if (DefinitionParmContent == "appversion")
                {
                    // return all the file content decrypted
                    DefRead = lines[1].ToString();
                }

                //Test of the parameter passed and return the exactly value
                if (DefinitionParmContent == "apprelease")
                {
                    // return all the file content decrypted
                    DefRead = lines[2].ToString();
                }

                //Test of the parameter passed and return the exactly value
                if (DefinitionParmContent == "appplataform")
                {
                    // return all the file content decrypted
                    DefRead = lines[3].ToString();
                }

                //Test of the parameter passed and return the exactly value
                if (DefinitionParmContent == "applicense")
                {
                    // return all the file content decrypted
                    DefRead = lines[4].ToString();
                }

                //Test of the parameter passed and return the exactly value
                if (DefinitionParmContent == "appServerRegistration")
                {
                    // return all the file content decrypted
                    DefRead = lines[5].ToString();
                }

                //Test of the parameter passed and return the exactly value
                if (DefinitionParmContent == "appDbServerInstance")
                {
                    // return all the file content decrypted
                    DefRead = lines[6].ToString();
                }

                //Test of the parameter passed and return the exactly value
                if (DefinitionParmContent == "appDbUser")
                {
                    // return all the file content decrypted
                    DefRead = lines[7].ToString();
                }

                //Test of the parameter passed and return the exactly value
                if (DefinitionParmContent == "appDbUserPass")
                {
                    // return all the file content decrypted
                    DefRead = lines[8].ToString();
                }

                //Test of the parameter passed and return the exactly value
                if (DefinitionParmContent == "appDbName")
                {
                    // return all the file content decrypted
                    DefRead = lines[9].ToString();
                }
            }
            else
            {
                // Inform that definition file was not found. 
                if (ConfigurationManager.AppSettings["AppRunningMode"] == "Console")
                {
                    Console.WriteLine(Localization.MsgDefFileNotFoundExit);
                }
                
                // exist forced
                System.Environment.Exit(-1);                
            }

            //return the value
            return DefRead;
        }

        // Method created to allow to display the Application header in console mode or service mode
        // service mode echo in a event log file
        // console mode echo in a screen and event log file
        public string AppHeader(string Modo)
        {

            // value to return
            var ReturnAppH = "valid";

            // Initialize a variable to be used as a control
            // with this variable the display mode, Service or Console, define where the
            // information will be displayed or Echoed.
            string AppHeaderDisplayMode = Modo;

            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            // verify if the application are running in Console mode
            if (AppHeaderDisplayMode == "Console")
            {
                // Software versioning and presentation header
                // Present software basic information
                Console.WriteLine($" {xfileActions.VerifyDefinitionFile("apptitle")}. Version: [{xfileActions.VerifyDefinitionFile("appversion")}] Release:  {xfileActions.VerifyDefinitionFile("apprelease")}");
                Console.WriteLine($" {xfileActions.VerifyDefinitionFile("appplataform")}");
                Console.WriteLine($" {xfileActions.VerifyDefinitionFile("applicense")} - {xfileActions.ReadLicenseFile("LicMode")} License");
                Console.WriteLine(" " + Localization.StrLicTo + " " + $"{xfileActions.ReadLicenseFile("LicNamed")}");
                Console.WriteLine(" " + Localization.StrSerialNumber + " " + $"{xfileActions.ReadLicenseFile("LicSerialNum")} \n");
            }

            return ReturnAppH;
        }

        // Method created to display the Machine Information in console mode or service mode
        // service mode echo in a event log file
        // console mode echo in a screen and event log file
        public string MachineInformation(String Modo)
        {

            // value to return
            var ReturnAppMI = "valid";

            // Initialize a variable to be used as a control
            // with this variable the display mode, Service or Console, define where the
            // information will be displayed or Echoed.
            var MachineAppHeaderDisplayMode = Modo;

            //
            if (MachineAppHeaderDisplayMode == "Console")
            {

                // Computer basic informations about the environment
                // Present computer basic information  
                Console.WriteLine(" " + Localization.MsgRungComputer);
                Console.WriteLine($" CPU Cores: {Environment.ProcessorCount} - Model {Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")} ");

                // Verify Processor Architecture
                if (Environment.Is64BitProcess == true)
                {
                    Console.WriteLine(" " + Localization.StrProcPlatForm);
                }
                Console.WriteLine(" " + Localization.StrOperationSystem + " " + $"{Environment.OSVersion.VersionString} {Environment.OSVersion.Platform}");
                Console.WriteLine(" " + Localization.StrComputerName + " " + $"{Environment.MachineName}" + "\n");
            }

            // return the value
            return ReturnAppMI;
        }

        // Execute the CleanUP log function
        public void CleanUpLogFiles()
        {

            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            if (ConfigurationManager.AppSettings["CleanLogFiles"] != "0")
            {
                // Create a log entry informing the log cleanup routine was executed 
                if (ConfigurationManager.AppSettings["AppLogMode"] == "Normal")
                {
                    xfileActions.FileSysLogAdd(Localization.StrLogCleanexec, "Operation");
                }

                // define the folders for each log type
                string AppSysFileLog = ConfigurationManager.AppSettings["AppSysLogPath"];
                string AppLogFileName = ConfigurationManager.AppSettings["AppLogFileName"];


                // Use Try to control and handle file or folder exception.
                var SysFilesFound = Directory.GetFiles(AppSysFileLog, AppLogFileName + "*.log", SearchOption.TopDirectoryOnly);
                DateTime dataLimiteVida = DateTime.Now.AddDays((-1) * Int32.Parse(ConfigurationManager.AppSettings["CleanLogFiles"]));

                // start the procedure to verify and remove the Application system log files
                foreach (var SysitemFile in SysFilesFound)
                {
                    FileInfo itemFileInfo = new FileInfo(SysitemFile);
                    {
                        DateTime dataAcesso = itemFileInfo.LastAccessTime;
                        DateTime dataCriacao = itemFileInfo.LastWriteTime;
                        DateTime dataModificacao = itemFileInfo.CreationTime;

                        bool deleteFile = ((dataAcesso < dataLimiteVida) &&
                                            (dataCriacao < dataLimiteVida) &&
                                            (dataModificacao < dataLimiteVida));

                        if (deleteFile)
                        {
                            try
                            {
                                // delete the action log file
                                itemFileInfo.Attributes = FileAttributes.Archive;
                                itemFileInfo.Delete();

                                // Create a log entry informing the Job is active 
                                if (ConfigurationManager.AppSettings["AppLogMode"] == "Normal")
                                {
                                    xfileActions.FileSysLogAdd(Localization.MsgAppSysLogFile + " (" + itemFileInfo.Name + " ) " + Localization.StrWasDeleted, "Operation");
                                }

                            }
                            catch (Exception ex)
                            {
                                // Create a log entry informing the error 
                                xfileActions.FileSysLogAdd("             -" + Localization.MsgErrTryRemFile + " " + itemFileInfo, "Operation");
                                xfileActions.FileSysLogAdd("             -" + Localization.MsgErrProcFile + " " + ex.Message, "Operation");
                            }
                        }
                    }
                }
            }
        }


        // Server Socket Listener acts as a server and listens to the incoming   
        // messages on the specified port and protocol.  
        public static void StartSupportServer(String serveripaddress, int servertcpport)
        {
            // iniziate the xfileActions
            var xfileActions = new xfileActions();
           
            // informe Ip and port for connections
            Console.WriteLine(" Server IP Address: " + serveripaddress);
            Console.WriteLine(" TCP Port Address: " + servertcpport + "\n");

            // informing that the system is running
            Console.WriteLine(" Server is up and running...");
            Console.WriteLine(" Press <Ctrl> + C to exit... " + "\n");
            
            // Create a server listener with null 
            TcpListener server = null;
                        
            // try to prevent program exit
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = servertcpport;
                IPAddress localAddr = IPAddress.Parse(serveripaddress);

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);
              
                // create log entry about listener
                xfileActions.FileSysLogAdd("Listener initiated!", "Operation");

                // Start listening for client requests.
                server.Start();

                // Create a Log entry with all data about the server status
                xfileActions.FileSysLogAdd(Localization.MsgServerStarted, "Operation");
                xfileActions.FileSysLogAdd("Server are running with IP Address: " + serveripaddress + " and TCP Port: " + servertcpport.ToString() + ".", "Operation");

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // seta uma variavel de controle
                int connected = 0;

                // Create a Log entry with waiting for a connection 
                Console.Write(" Waiting for a connection... \n");
                xfileActions.FileSysLogAdd("Waiting for a connection...", "Operation");

                // Enter the listening loop.
                while (true)
                {
                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();

                    // set a control to show when is connected or waiting connection.
                    if (connected == 0)
                    {
                        // Create a Log entry with connected
                        Console.WriteLine(" Connected!");
                        xfileActions.FileSysLogAdd("Connected!", "Operation");
                        
                        // seta a variavel para controle
                        connected = 1;
                    }

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    //
                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Console.WriteLine("Received: {0}", data);

                        // save information to the disk in a log file
                        if (ConfigurationManager.AppSettings["EchoToLogFileOnDisk"] == "Yes")
                        {
                            xfileActions.EchoLogToDisk(data);
                        }

                        // Process the data sent by the client.
                        // data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);

                        //Console.WriteLine("Sent: {0}", data); 

                    }
                    // Shutdown and end connection
                    client.Close();

                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();

                if (ConfigurationManager.AppSettings["AppMessageMode"] == "Normal")
                {
                    // Create a Log entry with close the connection
                    xfileActions.FileSysLogAdd("Server Stopped!", "Operation");
                }
            }
        }

        // To control the application by clickng control c  
        public static void CloseAppwithControlC()
        {
            // iniziate the xfileActions
            var xfileActions = new xfileActions();

            // informe Ip and port for connections
            Console.WriteLine("\n");
            Console.WriteLine(" Detected Control+C to Close the Application ");
            Console.WriteLine(" Waiting.. Closing the connections!" + "\n");

            // create log entry about the close process with control c detected
            xfileActions.FileSysLogAdd("Application Close with a Control+C ", "Operation");

            // create log entry about the close process
            xfileActions.FileSysLogAdd("Closing the connections...", "Operation");

            // create log entry about the close process
            xfileActions.FileSysLogAdd("Server Stopped", "Operation");

            // 
            Thread.Sleep(500);

            // close the application
            Environment.Exit(0);

        }

    }
}
