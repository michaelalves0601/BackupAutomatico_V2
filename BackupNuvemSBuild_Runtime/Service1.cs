using BackupNuvemSBuild_Models;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackupNuvemSBuild_Runtime
{
    public partial class Service1 : ServiceBase
    {

        #region Globais
        Timer rotina = null;
        string versao = "";

        string pathFULL = "";
        string pathDiferencial = "";
        int MAX_PATH = 260;
        int MAX_DIRECTORY = 248;
        string pathConfiguration = AppDomain.CurrentDomain.BaseDirectory + @"\Config\Configuration.ini";
        string pathPastasRestritas = AppDomain.CurrentDomain.BaseDirectory + @"\Config\PastasRestritas.ini";
        string pathUltimoBackup = AppDomain.CurrentDomain.BaseDirectory + @"\Config\UltimoBackup.ini";


        string folderAtualStatus = ""; //pasta atual do backup
        int typeBackupStatus; //tipo de backup sendo executado 0 - nao executado, 1 - diferencial, 2 - full
        string timeEstimatedBackup = ""; //estado de tempo estimado de conclusao do backup
        bool pausedBackup = false; // estado do backup
        long quantidade = 0; //quantidade de arquivos para fazer backup

        bool pause = false; //estado do pause
        bool abort = false; //estado do abort

        double tamanho = 0; //tamanho passado no intervalo de 1s
        double restante = 0; // aux de quantidadeProgresso
        double tamanhoTransferido = 0; //tamanho ja copiado do backup
        long quantidadeTotal = 0; // quantidade total de arquivos
        double quantidadeProgresso = 0; //porcentagem da conclusão do backup
        long totalQuantidade = 0; //quantiade de pastas total
        long tamanhoTotal = 0; //total de tamanho do backup
        double tempoestimado; // aux de timeEstimatedBackup

        string parentPathDrive = "";

        bool enableSync = false;

        bool isAlive = false;

        List<Tuple<string, string>> bufferArquivos = new List<Tuple<string, string>>();
        List<string> bufferPastas = new List<string>();

        Log log = new Log("Runtime");
        Configuration configuration = new Configuration();

        SimpleTcpServer server;
        string[] msgRespota_Status = new string[6];

        Encrypt encrypt = new Encrypt("cenouras", "abacaxis");


        BackgroundWorker bwBackup = null;
        #endregion

        #region Service
        public Service1()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            //ExecutaRotina();
            StartTime();
        }

        private void StartTime()
        {
            pausedBackup = false;
            try
            {
                try
                {
                    versao = Assembly.GetEntryAssembly().GetName().Version.ToString();
                    versao = "v" + versao.Substring(0, versao.Length - 2);
                }
                catch (Exception ex)
                {
                    versao = "";
                    log.LogError("Erro na leitura da Versão.",
                                    MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().ToString(),
                                            ex.Message);
                }

                log.LogInfo("Starting Timer " + versao + ": " + DateTime.Now.ToString());

                if (rotina == null)
                {
                    int timerIntervalSecs = 1;

                    TimeSpan tsInterval = new TimeSpan(0, 0, timerIntervalSecs);

                    rotina = new Timer(new TimerCallback(ExecutaRotina), null, tsInterval, tsInterval);
                }
            }
            catch (Exception ex)
            {
                log.LogError("Error - Start Timer",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }


            try
            {
                StartTCP("127.0.0.1", "8910");
            }
            catch (Exception ex)
            {
                log.LogError("Error - Start TCP",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }

        }

        protected override void OnStop()
        {
            StopTimer();
        }

        private void StopTimer()
        {
            try
            {
                log.LogInfo("Stopping Timer " + versao + ": " + DateTime.Now.ToString());


                if (rotina != null)
                {
                    rotina.Change(Timeout.Infinite, Timeout.Infinite);
                    rotina.Dispose();
                    rotina = null;
                }
            }
            catch (Exception ex)
            {
                log.LogError("Error - Stop Timer",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }

            try
            {
                StopTCP();
            }
            catch (Exception ex)
            {
                log.LogError("Error - Stop TCP",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }
        }

        #endregion

        #region TCP IP

        private void ConfigTCP()
        {
            server = new SimpleTcpServer();
            server.Delimiter = 0x13;
            server.StringEncoder = Encoding.UTF8;
            server.DataReceived += Server_DataReceived;
        }

        private void Server_DataReceived(object sender, Message e)
        {
            string msgRecebida = "";
            string msgResposta = "";

            msgRecebida = e.MessageString;

            try
            {
                string[] arrayMsgRecebida = msgRecebida.Split(';');


                if (arrayMsgRecebida[0] == "Comando")
                {
                    switch (arrayMsgRecebida[1])
                    {
                        case "BkpDiferencial":
                            string[] listPastas = Directory.GetDirectories(configuration.PastaBackup, "*", SearchOption.TopDirectoryOnly);
                            string nomeBackupFull = listPastas[listPastas.Count() - 1];
                            this.pathDiferencial = nomeBackupFull + @"\";
                            string newPathDiario = pathDiferencial + "DIF_" + DateTime.Now.ToString("yyMMdd");
                            if (Directory.Exists(newPathDiario))
                                log.LogInfo("Backup '" + newPathDiario + "' já existe!");
                            else
                                AssyncBackup(true, false, DateTime.Now, newPathDiario);
                            break;

                        case "BkpFull":
                            this.pathFULL = configuration.PastaBackup + @"\";
                            newPathDiario = pathFULL + DateTime.Now.ToString("yyMMdd") + @"\FULL_" + DateTime.Now.ToString("yyMMdd") + @"\";
                            if (Directory.Exists(newPathDiario))
                                log.LogInfo("Backup '" + newPathDiario + "' já existe!");
                            else
                                AssyncBackup(false, false, DateTime.Now, newPathDiario);
                            break;

                        case "SyncEspelho":
                            AssyncBackup(false, true, DateTime.Now, "batata");
                            break;

                        case "Pause":
                            if (!pause)
                                this.pause = true;
                            else
                                this.pause = false;

                            break;

                        case "Abort":
                            abort = true;
                            break;
                    }

                    pausedBackup = pause;

                    msgResposta = "OK;";


                }
                else if (arrayMsgRecebida[0] == "Status")
                {
                    msgRespota_Status[0] = "OK";
                    msgRespota_Status[1] = typeBackupStatus.ToString();

                    if (typeBackupStatus != 0)
                    {
                        int quantidadeProgressoInt = Convert.ToInt32(quantidadeProgresso);
                        quantidadeProgressoInt = quantidadeProgressoInt > 100 ? 100 : quantidadeProgressoInt;

                        msgRespota_Status[2] = pausedBackup.ToString();
                        msgRespota_Status[3] = quantidadeProgressoInt.ToString();
                        msgRespota_Status[4] = folderAtualStatus == "" ? "404" : folderAtualStatus;
                        msgRespota_Status[5] = timeEstimatedBackup == "" || timeEstimatedBackup.Substring(0, 1) == "-" ? "0" : timeEstimatedBackup;
                    }
                    else
                    {
                        msgRespota_Status[2] = "404";
                        msgRespota_Status[3] = "404";
                        msgRespota_Status[4] = "404";
                        msgRespota_Status[5] = "404";
                    }

                    msgResposta = msgRespota_Status[0] + ";" + msgRespota_Status[1] + ";" + msgRespota_Status[2] + ";" + msgRespota_Status[3] + ";" + msgRespota_Status[4] + ";" + msgRespota_Status[5] + ";";
                    tamanho = 0;

                }
                else
                {
                    msgResposta = "404;";
                }
            }
            catch (Exception ex)
            {
                msgResposta = "404;";
                log.LogError("Erro na comunicação TCP",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }

            e.ReplyLine(string.Format(msgResposta));
        }

        private void StartTCP(string host, string port)
        {
            string IP = "";
            string Port = "";

            ConfigTCP();

            string ServerConfig = host + "." + port;

            string[] ServerInfo = ServerConfig.Split('.');

            IP = ServerInfo[3] + "." + ServerInfo[2] + "." + ServerInfo[1] + "." + ServerInfo[0];

            string IPSERVER = ((long)(uint)IPAddress.NetworkToHostOrder((int)IPAddress.Parse(IP).Address)).ToString();
            IPAddress ip = new IPAddress(Convert.ToInt64(IPSERVER));

            try
            {
                server.Stop();

                if (!server.IsStarted)
                {
                    server.Start(ip, Convert.ToInt16(ServerInfo[4]));
                }
            }
            catch (Exception ex)
            {
                log.LogError("Erro na iniciação do TCP",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }
        }

        private void StopTCP()
        {
            if (server.IsStarted)
                server.Stop();
        }

        #endregion

        #region Rotina
        private void ExecutaRotina(object sender)
        {
            TESTE_LogTCP();


            AtualizaTempoEstimado();


            RestauraConfiguracao();
            // cd C:\Windows\Microsoft.NET\Framework64\v4.0.30319     
            //InstallUtil.exe C:\ProjetosC#\GitHub\BackupAutomatico_V2\BackupNuvemSBuild_Runtime\bin\Release\BackupNuvemSBuild_Runtime.exe
            //sc delete BackupNuvemSBuild_Runtime

            try
            {
                if (!Directory.Exists(configuration.PastaBackup))
                    Directory.CreateDirectory(configuration.PastaBackup);

                if (configuration.BackupFULLHabilitado)
                {

                    bool bkpDiferencial = VerificaTipoBackup();

                    int hora = 0;
                    int minuto = 0;


                    if (bkpDiferencial && configuration.BackupDiferencialHabilitado)
                    {
                        hora = Convert.ToInt32(configuration.HorarioDiferencial.Substring(0, 2));
                        minuto = Convert.ToInt32(configuration.HorarioDiferencial.Substring(3, 2));

                        DateTime dateTimeBackupDif = new DateTime(1970, 01, 01, hora, minuto, 0);

                        if (DateTime.Now.Hour == dateTimeBackupDif.Hour
                                && DateTime.Now.Minute == dateTimeBackupDif.Minute || true) //TIRA ISSO DAQUI............
                        {
                            string[] listPastas = Directory.GetDirectories(configuration.PastaBackup, "*", SearchOption.TopDirectoryOnly);

                            string nomeBackupFull = listPastas[listPastas.Count() - 1];

                            this.pathDiferencial = nomeBackupFull + @"\";

                            // cria nome do backup Diferencial atual
                            string newPathDiario = pathDiferencial + "DIF_" + DateTime.Now.ToString("yyMMdd");

                            if (Directory.Exists(newPathDiario))
                                log.LogInfo("Backup '" + newPathDiario + "' já existe!");
                            else
                                AssyncBackup(true, false, DateTime.Now, newPathDiario);

                        }
                    }
                    else if (!bkpDiferencial)
                    {
                        hora = Convert.ToInt32(configuration.HorarioFull.Substring(0, 2));
                        minuto = Convert.ToInt32(configuration.HorarioFull.Substring(3, 2));

                        DateTime dateTimeBackupFULL = new DateTime(1970, 01, 01, hora, minuto, 0);

                        if (DateTime.Now.Hour == dateTimeBackupFULL.Hour
                                && DateTime.Now.Minute == dateTimeBackupFULL.Minute) //TIRA ISSO DAQUI............
                        {
                            this.pathFULL = configuration.PastaBackup + @"\";
                            // cria nome do backup Diferencial atual
                            string newPathDiario = pathFULL + DateTime.Now.ToString("yyMMdd") + @"\FULL_" + DateTime.Now.ToString("yyMMdd") + @"\";

                            if (Directory.Exists(newPathDiario))
                                log.LogInfo("Backup '" + newPathDiario + "' já existe!");
                            else
                                AssyncBackup(false, false, DateTime.Now, newPathDiario);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError("Erro na busca de backup automatico",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }
        }

        private void AtualizaTempoEstimado()
        {
            try
            {
                if (typeBackupStatus != 0)
                {
                    if (tamanhoTotal != 0 && tamanho != 0)
                    {
                        tamanhoTransferido += tamanho;

                        Log logTempoEstimado = new Log("TempoEstimado");

                        logTempoEstimado.LogInfo("tamanhoTotal = " + tamanhoTotal.ToString() + Environment.NewLine
                                                    + "tamanhoTransferido = " + tamanhoTransferido.ToString() + Environment.NewLine);

                        double tamanhoRestante = tamanhoTotal - tamanhoTransferido;

                        logTempoEstimado.LogInfo("tamanhoRestante = " + tamanhoTotal.ToString() + Environment.NewLine
                                                    + "tamanho = " + tamanho.ToString() + Environment.NewLine);

                        tempoestimado = tamanhoRestante / tamanho;
                        
                        logTempoEstimado.LogInfo("tempoestimado = " + tempoestimado.ToString() + Environment.NewLine);

                        tempoestimado = tempoestimado < 0 ? 0 : Math.Round(tempoestimado, 0);

                        timeEstimatedBackup = tempoestimado.ToString().Replace(",", ".");

                        logTempoEstimado.LogInfo("timeEstimatedBackup = " + timeEstimatedBackup);

                        tamanho = 0;
                    }
                }
            }
            catch(Exception ex)
            {
                log.LogError("Falha ao calcular o tempo estimado do Backup",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }
         }


        private void TESTE_LogTCP()
        {
            string[] msgRespota_Status = new string[6];

            Log logTESTE = new Log("TESTE");

            try
            {
                msgRespota_Status[0] = "OK";
                msgRespota_Status[1] = typeBackupStatus.ToString();

                if (typeBackupStatus != 0)
                {
                    int quantidadeProgressoInt = Convert.ToInt32(quantidadeProgresso);
                    quantidadeProgressoInt = quantidadeProgressoInt > 100 ? 100 : quantidadeProgressoInt;

                    msgRespota_Status[2] = pausedBackup.ToString();
                    msgRespota_Status[3] = quantidadeProgressoInt.ToString();
                    msgRespota_Status[4] = folderAtualStatus == "" ? "404" : folderAtualStatus;
                    msgRespota_Status[5] = timeEstimatedBackup;
                }
                else
                {
                    msgRespota_Status[2] = "404";
                    msgRespota_Status[3] = "404";
                    msgRespota_Status[4] = "404";
                    msgRespota_Status[5] = "404";
                }



                logTESTE.LogInfo("msgRespota_Status[0] = " + msgRespota_Status[0] + Environment.NewLine +
                                    "msgRespota_Status[1] = " + msgRespota_Status[1] + Environment.NewLine +
                                        "msgRespota_Status[2] = " + msgRespota_Status[2] + Environment.NewLine +
                                            "msgRespota_Status[3] = " + msgRespota_Status[3] + Environment.NewLine +
                                                "msgRespota_Status[4] = " + msgRespota_Status[4] + Environment.NewLine +
                                                    "msgRespota_Status[5] = " + msgRespota_Status[5] + Environment.NewLine);

            }
            catch (Exception ex)
            {
                logTESTE.LogError("Falha no TESTE do TCP",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }
        }

        private void RestauraConfiguracao()
        {
            bool resultado = configuration.RestauraConfiguracao(pathConfiguration, pathPastasRestritas);
        }

        private bool VerificaTipoBackup()
        {
            //true = Diferencial  -----  false = FULL
            bool tipoBackup = false;

            //Verifica se existe algum backup FULL
            string[] backupsExistentes = Directory.GetDirectories(configuration.PastaBackup, "*", SearchOption.TopDirectoryOnly);

            if (backupsExistentes.Length == 0)
                tipoBackup = false;
            else
            {
                DateTime dateIndex = DateTime.Now;
                string pathIndex = "";

                //Consulta de existe algum backup FULL nos dias de intervalo conforme configuration
                for (int i = 0; i < configuration.IntervaloBackupsFull; i++)
                {
                    pathIndex = configuration.PastaBackup + @"\" + dateIndex.ToString("yyMMdd");

                    if (Directory.Exists(pathIndex))
                    {
                        tipoBackup = true;
                        break;
                    }

                    dateIndex = dateIndex.AddDays(-1);
                }
            }


            return tipoBackup;
        }

        #endregion

        #region Backup
        private void AssyncBackup(bool bkpDiferencial, bool apenasSync, DateTime dataNewBackup, string newPathDiario)
        {
            if (!isAlive)
            {
                isAlive = true;

                bool bkpDiferencialDoWork = bkpDiferencial;
                bool apenasSyncDoWork = apenasSync;
                DateTime dataNewBackupDoWork = dataNewBackup;
                string newPathDiarioDoWork = newPathDiario;


                try
                {
                    //Inicia objeto assincrono UNICO para roda apenas uma vez
                    bwBackup = new BackgroundWorker();

                    ////Declara auxiliares para o processo assincrono
                    //bwConfigSQL.ProgressChanged +=
                    //    new ProgressChangedEventHandler(bwConfigSQL_ProgressChanged);

                    ////habilita propriedade de acompanhar progressão do processo
                    //bwConfigSQL.WorkerReportsProgress = true;

                    bwBackup.RunWorkerCompleted +=
                        new RunWorkerCompletedEventHandler(bwBackup_RunWorkerCompleted);

                    //inicia processo assincrono
                    bwBackup.DoWork += (Senderbw, args) =>
                    {
                        try
                        {

                            tamanhoTotal = 0;
                            tamanhoTransferido = 0;

                            pausedBackup = false;


                            if (!apenasSyncDoWork)
                            {
                                typeBackupStatus = bkpDiferencialDoWork ? 2 : 1;


                                folderAtualStatus = "Encaminhando Email...";

                                NotificacaoEmail(dataNewBackupDoWork, true);


                                log.LogInfo("Iniciando Backup: " + dataNewBackupDoWork.ToString("yyyyMMdd"));
                                log.LogInfo("Iniciando Cópia do Drive.");


                                Directory.CreateDirectory(newPathDiarioDoWork);


                                if (!bkpDiferencialDoWork)
                                {
                                    log.LogInfo("Iniciando Organização das pastas de Backup.");
                                    folderAtualStatus = "Organizando Pastas...";
                                    OrganizarPastasFull(newPathDiarioDoWork);
                                }

                                folderAtualStatus = "Calculando Arquivos e Pastas...";

                                List<Tuple<string, long>> listFolders = SelecionaPastasDrive();

                                while (pause)
                                    Thread.Sleep(500);

                                if (abort)
                                    return;

                                CopiaArquivos(listFolders, newPathDiarioDoWork, bkpDiferencialDoWork);

                                if (abort)
                                    return;

                                while (pause)
                                    Thread.Sleep(500);

                                folderAtualStatus = "Copiando Buffer...";

                                CopiaBuffer();

                                if (abort)
                                    return;                           


                                Thread.Sleep(500);

                                if (configuration.HabilitaPastaEspelho)
                                {
                                    log.LogInfo("Iniciando Sincronização com para Espelho.");
                                    folderAtualStatus = "Espelhando Backups...";

                                    SyncPastaEspelho();

                                    Thread.Sleep(500);
                                }


                                configuration.UltimoBackup = dataNewBackupDoWork.ToString("dd/MM/yyyy");

                                if (bkpDiferencialDoWork)
                                    configuration.TipoUltimoBackup = "DIFERENCIAL";
                                else
                                    configuration.TipoUltimoBackup = "FULL";

                                double tamanhoBackupAux = 0;

                                try
                                {
                                    tamanhoBackupAux = Math.Round((Convert.ToDouble(tamanhoTotal) / 1000000000), 2);
                                    tamanhoBackupAux = tamanhoBackupAux < 0 ? 0 : tamanhoBackupAux;
                                }
                                catch(Exception ex)
                                {
                                    tamanhoBackupAux = 0;
                                    log.LogError("Falha no cálculo do tamanho do último Backup.",
                                                    MethodBase.GetCurrentMethod().DeclaringType.Name,
                                                        MethodBase.GetCurrentMethod().ToString(),
                                                            ex.Message);
                                }


                                configuration.TamanhoUltimoBackup = tamanhoBackupAux.ToString() + " GB";

                                configuration.SalvaUltimoBackup(pathUltimoBackup);


                                log.LogInfo("Encaminhando Email...");
                                NotificacaoEmail(dataNewBackupDoWork, false);


                                log.LogInfo("Finalizando Backup: " + dataNewBackupDoWork.ToString("yyyyMMdd"));
                            }
                            else
                            {
                                NotificacaoEmail(dataNewBackupDoWork, true);


                                typeBackupStatus = 3;

                                log.LogInfo("Iniciando Sincronização com para Espelho.");

                                SyncPastaEspelho();

                                Thread.Sleep(500);


                                log.LogInfo("Finalizando Sincronização: " + dataNewBackupDoWork.ToString("yyyyMMdd"));
                            }


                        }
                        catch (Exception ex)
                        {
                            log.LogError("Erro durante o Background Worker.",
                                    MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().ToString(),
                                            ex.Message);
                        }

                    };
                    //indica função acima como assincrona
                    bwBackup.RunWorkerAsync();

                }
                catch (Exception ex)
                {
                    log.LogError("Erro durante o Background Worker.",
                            MethodBase.GetCurrentMethod().DeclaringType.Name,
                                MethodBase.GetCurrentMethod().ToString(),
                                    ex.Message);
                }

            }

        }

        private void bwBackup_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bwBackup.Dispose();

            isAlive = false;
            pausedBackup = false;
            typeBackupStatus = 0;
            quantidadeProgresso = 0;
            tempoestimado = 0;

            log.LogInfo("Finalizando Backup");            
        }

        private List<Tuple<string, long>> SelecionaPastasDrive()
        {
            string[] listSubPastas = Directory.GetDirectories(configuration.PastaDrive, "*", SearchOption.TopDirectoryOnly);

            List<Tuple<string, long>> listFolders = new List<Tuple<string, long>>();

            string folderAtual = "";
            string[] allfiles1 = Directory.GetFiles(configuration.PastaDrive, "*", SearchOption.AllDirectories);



            string[] allfolders1 = Directory.GetDirectories(configuration.PastaDrive, "*", SearchOption.AllDirectories);

            totalQuantidade = allfolders1.Count();


            while (pause)
                Thread.Sleep(500);

            quantidadeTotal = 0;
            restante = 0;


            foreach (string path in listSubPastas)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                parentPathDrive = directoryInfo.Parent.Name;

                string[] allfiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

                while (pause)
                    Thread.Sleep(500);


                string[] allfolders = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);

                quantidade = allfolders.Count() + allfiles.Count();
                quantidadeTotal += quantidade;

                listFolders.Add(new Tuple<string, long>(directoryInfo.Name, quantidade));

                folderAtual = directoryInfo.Name + " - (" + quantidade + ")";

            }

            DirectoryInfo pastaDrive = new DirectoryInfo(configuration.PastaDrive);

            verificaTamanho(pastaDrive);

            return listFolders;
        }


        private void verificaTamanho(DirectoryInfo pastaDrive)
        {
            foreach (FileInfo fileInfo in pastaDrive.GetFiles())
            {
                this.tamanhoTotal += fileInfo.Length;
            }

            foreach (DirectoryInfo subPastaAux in pastaDrive.GetDirectories())
            {
                DirectoryInfo subPasta = null;

                try
                {
                    if (subPastaAux.FullName.Contains(@"\\?\"))
                        subPasta = new DirectoryInfo(subPastaAux.FullName.Replace(@"\\?\", ""));
                    else
                        subPasta = new DirectoryInfo(subPastaAux.FullName);

                    verificaTamanho(subPasta);

                }
                catch (Exception e)
                {
                    log.LogError("Falha ao calcular tamanho total: " + subPasta.FullName,
                                MethodBase.GetCurrentMethod().DeclaringType.Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        e.Message);
                }
            }

        }



        private void CopiaArquivos(List<Tuple<string, long>> listFolders, string newPathDiario, bool bkpDiferencial)
        {
            try
            {
                string pathFolder = "";

                string destPathBackup = newPathDiario;

                string nomeBackupFullinFullIndex = "";
                string nomeBackupFullInFull = "";

                string[] listPastas = Directory.GetDirectories(configuration.PastaBackup, "*", SearchOption.TopDirectoryOnly);
                string nomeBackupFull = listPastas[listPastas.Count() - 1];
                string[] listPastaFullFull = Directory.GetDirectories(nomeBackupFull, "*", SearchOption.TopDirectoryOnly);
                nomeBackupFullInFull = listPastaFullFull[listPastaFullFull.Count() - 1];


                string destinoPath = "";

                for (int i = 0; i < listFolders.Count; i++)
                {

                    destinoPath = destPathBackup + @"\" + listFolders[i].Item1;
                    pathFolder = configuration.PastaDrive + @"\" + listFolders[i].Item1;
                    nomeBackupFullinFullIndex = nomeBackupFullInFull + @"\" + listFolders[i].Item1;

                    DirectoryInfo origemPathInfo = new DirectoryInfo(pathFolder);
                    DirectoryInfo destinoPathInfo = new DirectoryInfo(destinoPath);
                    DirectoryInfo backupFullPathInfo = new DirectoryInfo(nomeBackupFullinFullIndex);

                    if ((!configuration.PastasRestritas.Contains(origemPathInfo.FullName) && bkpDiferencial) || !bkpDiferencial)
                    {
                        CopyAll(origemPathInfo, destinoPathInfo, backupFullPathInfo, bkpDiferencial);
                    }

                    if (abort)
                        return;

                    while (pause)
                        Thread.Sleep(500);
                }

                quantidadeProgresso = 100;
            }
            catch (Exception ex)
            {
                log.LogError("Erro na busca de pastas para backup",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }

        }

        private void CopyAll(DirectoryInfo origemPathInfoAux, DirectoryInfo destinoPathInfoAux, DirectoryInfo backupFullPathInfoAux, bool bkpDiferencial)
        {
            DirectoryInfo origemPathInfo = null;
            DirectoryInfo destinoPathInfo = null;
            DirectoryInfo backupFullPathInfo = null;


            try
            {
                if (origemPathInfoAux.FullName.Length >= MAX_DIRECTORY)
                    origemPathInfo = new DirectoryInfo(@"\\?\" + origemPathInfoAux.FullName);
                else
                    origemPathInfo = new DirectoryInfo(origemPathInfoAux.FullName);
                folderAtualStatus = origemPathInfoAux.FullName;
                if (destinoPathInfoAux.FullName.Length >= MAX_DIRECTORY)
                    destinoPathInfo = new DirectoryInfo(@"\\?\" + destinoPathInfoAux.FullName);
                else
                    destinoPathInfo = new DirectoryInfo(destinoPathInfoAux.FullName);

                if (backupFullPathInfoAux.FullName.Length >= MAX_DIRECTORY)
                    backupFullPathInfo = new DirectoryInfo(@"\\?\" + backupFullPathInfoAux.FullName);
                else
                    backupFullPathInfo = new DirectoryInfo(backupFullPathInfoAux.FullName);

                if (abort)
                    return;

                while (pause)
                    Thread.Sleep(500);


                if (!Directory.Exists(destinoPathInfo.FullName))
                    Directory.CreateDirectory(destinoPathInfo.FullName);

            }
            catch (Exception ex)
            {
                bufferPastas.Add(destinoPathInfoAux.FullName);

                log.LogError("Falha ao Criar o diretorio: " + destinoPathInfo.FullName,
                                MethodBase.GetCurrentMethod().DeclaringType.Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }


            string diretorioFile = "";
            FileInfo[] backupFullFiles = new FileInfo[0];

            try
            {
                if (Directory.Exists(backupFullPathInfo.FullName))                
                    backupFullFiles = backupFullPathInfo.GetFiles();
            }
            catch(Exception ex)
            {
                log.LogError("Falha ao ler os arquivos do Backup FULL da pasta: " + backupFullPathInfo,
                                    MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().ToString(),
                                            ex.Message);
            }


            // copia arquivos para o pasta de destino
            foreach (FileInfo fileInfoAux in origemPathInfo.GetFiles())
            {
                try
                {
                    FileInfo fileInfo = null;

                    if (fileInfoAux.FullName.Length >= MAX_PATH)
                        fileInfo = new FileInfo(@"\\?\" + fileInfoAux.FullName.Replace(@"\\?\", ""));
                    else
                        fileInfo = new FileInfo(fileInfoAux.FullName.Replace(@"\\?\", ""));


                    diretorioFile = Path.Combine(destinoPathInfoAux.FullName.Replace(@"\\?\", ""), fileInfo.Name);

                    if (diretorioFile.Length >= MAX_PATH)
                        diretorioFile = @"\\?\" + diretorioFile;


                    if (abort)
                        return;

                    while (pause)
                        Thread.Sleep(500);


                    tamanho += fileInfo.Length;                    

                    FileInfo fileInfoDestino = new FileInfo(diretorioFile);



                    if (File.Exists(diretorioFile))
                    {
                        if (fileInfo.LastWriteTime != fileInfoDestino.LastWriteTime)
                            fileInfo.CopyTo(diretorioFile, true);
                    }
                    else
                    {
                        if (!bkpDiferencial)
                            fileInfo.CopyTo(diretorioFile, true);
                        else
                        {
                            if (backupFullFiles.Where(f => f.Name == fileInfo.Name).Count() > 0)
                            {
                                if (backupFullFiles.Where(b => b.Name == fileInfo.Name).FirstOrDefault().LastWriteTime != fileInfo.LastWriteTime)
                                    fileInfo.CopyTo(diretorioFile, true);
                            }
                            else
                                fileInfo.CopyTo(diretorioFile, true);
                        }
                    }

                    restante++;

                    quantidadeProgresso = (restante * 100) / quantidadeTotal;

                    if (abort)
                        return;

                    while (pause)
                        Thread.Sleep(500);

                }
                catch (Exception ex)
                {
                    bufferArquivos.Add(new Tuple<string, string>(destinoPathInfoAux.FullName.Replace(@"\\?\", ""),
                                                                   fileInfoAux.FullName.Replace(@"\\?\", "")));

                    log.LogError("Falha ao Copiar o arquivo: " + diretorioFile,
                                    MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().ToString(),
                                            ex.Message);
                }

            }



            // copia as SubPastas para o Destino, usando Recursividade
            foreach (DirectoryInfo subPastaAux in origemPathInfo.GetDirectories())
            {
                DirectoryInfo proximaSubPasta = null;
                DirectoryInfo proximaSubPastaFull = null;
                DirectoryInfo subPasta = null;

                try
                {

                    if (abort)
                        return;

                    while (pause)
                        Thread.Sleep(500);

                    if (subPastaAux.FullName.Contains(@"\\?\"))
                        subPasta = new DirectoryInfo(subPastaAux.FullName.Replace(@"\\?\", ""));
                    else
                        subPasta = new DirectoryInfo(subPastaAux.FullName);


                    proximaSubPasta = new DirectoryInfo(Path.Combine(destinoPathInfoAux.FullName, subPasta.Name));

                    proximaSubPastaFull = new DirectoryInfo(Path.Combine(backupFullPathInfo.FullName, subPasta.Name));

                    if ((!configuration.PastasRestritas.Contains(subPastaAux.FullName) && bkpDiferencial) || !bkpDiferencial)
                    {
                        CopyAll(subPasta, proximaSubPasta, proximaSubPastaFull, bkpDiferencial);
                    }


                }
                catch (Exception ex)
                {
                    bufferPastas.Add(subPastaAux.FullName.Replace(@"\\?\", ""));

                    log.LogError("Falha ao Copiar a pasta: " + proximaSubPasta.FullName,
                                    MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().ToString(),
                                            ex.Message);
                }
            }



        }

        private void DeleteOldPath(DirectoryInfo origemPathInfoAux, DirectoryInfo destinoPathInfoAux)
        {
            DirectoryInfo origemPathInfo = null;
            DirectoryInfo destinoPathInfo = null;


            try
            {

                if (abort)
                    return;

                while (pause)
                    Thread.Sleep(500);

                if (origemPathInfoAux.FullName.Length >= MAX_DIRECTORY)
                    origemPathInfo = new DirectoryInfo(@"\\?\" + origemPathInfoAux.FullName);
                else
                    origemPathInfo = new DirectoryInfo(origemPathInfoAux.FullName);

                if (destinoPathInfoAux.FullName.Length >= MAX_DIRECTORY)
                    destinoPathInfo = new DirectoryInfo(@"\\?\" + destinoPathInfoAux.FullName);
                else
                    destinoPathInfo = new DirectoryInfo(destinoPathInfoAux.FullName);
            }
            catch (Exception ex)
            {
                log.LogError("Falha ao Converter pastas para modelo longo.",
                                MethodBase.GetCurrentMethod().DeclaringType.Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }



            string diretorioFile = "";

            // copia arquivos para o pasta de destino
            foreach (FileInfo fileInfoAux in destinoPathInfo.GetFiles())
            {
                try
                {
                    FileInfo fileInfo = null;

                    if (fileInfoAux.FullName.Length >= MAX_PATH)
                        fileInfo = new FileInfo(@"\\?\" + fileInfoAux.FullName.Replace(@"\\?\", ""));
                    else
                        fileInfo = new FileInfo(fileInfoAux.FullName);


                    diretorioFile = Path.Combine(destinoPathInfoAux.FullName.Replace(@"\\?\", ""), fileInfo.Name);

                    if (diretorioFile.Length >= MAX_PATH)
                        diretorioFile = @"\\?\" + diretorioFile;


                    if (!File.Exists(diretorioFile))
                        fileInfo.Delete();

                }
                catch (Exception ex)
                {
                    log.LogError("Falha ao Deletar o aquivo antigo do Espelho: " + diretorioFile,
                                    MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().ToString(),
                                            ex.Message);
                }
            }


            // copia as SubPastas para o Destino, usando Recursividade
            foreach (DirectoryInfo subPastaAux1 in destinoPathInfo.GetDirectories())
            {
                DirectoryInfo subPasta = null;
                DirectoryInfo subPastaAux = null;
                DirectoryInfo subPastaDeletar = null;
                string diretorioFileAux = "";

                try
                {
                    if (subPastaAux1.FullName.Contains(@"\\?\"))
                        subPastaAux = new DirectoryInfo(subPastaAux1.FullName.Replace(@"\\?\", ""));
                    else
                        subPastaAux = new DirectoryInfo(subPastaAux1.FullName);


                    if (subPastaAux.FullName.Length >= MAX_DIRECTORY)
                        subPasta = new DirectoryInfo(@"\\?\" + subPastaAux.FullName);
                    else
                        subPasta = new DirectoryInfo(subPastaAux.FullName.Replace(@"\\?\", ""));


                    diretorioFileAux = Path.Combine(origemPathInfoAux.FullName.Replace(@"\\?\", ""), subPasta.Name);
                    diretorioFile = (diretorioFileAux.Length >= MAX_DIRECTORY ? @"\\?\" + diretorioFileAux : diretorioFileAux);

                    if (!Directory.Exists(diretorioFile))
                    {
                        subPastaDeletar = new DirectoryInfo(@"\\?\" + subPasta.FullName);

                        subPastaDeletar.Delete(true);
                    }
                    else
                    {
                        DirectoryInfo proximaSubPastaOrigem = new DirectoryInfo(diretorioFileAux);
                        DeleteOldPath(proximaSubPastaOrigem, subPastaAux);
                    }

                }
                catch (Exception ex)
                {
                    log.LogError("Falha ao Deletar as pastas antigas do Espelho.",
                                    MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().ToString(),
                                            ex.Message);
                }
            }

        }

        private bool CopyArquive(string destinoPathInfoAux, string fileInfoAux)
        {
            string diretorioFile = "";

            try
            {
                FileInfo fileInfo = null;

                if (fileInfoAux.Length >= MAX_PATH)
                    fileInfo = new FileInfo(@"\\?\" + fileInfoAux.Replace(@"\\?\", ""));
                else
                    fileInfo = new FileInfo(fileInfoAux.Replace(@"\\?\", ""));


                diretorioFile = Path.Combine(destinoPathInfoAux.Replace(@"\\?\", ""), fileInfo.Name);

                if (diretorioFile.Length >= MAX_PATH)
                    diretorioFile = @"\\?\" + diretorioFile;

                FileInfo fileInfoDestino = new FileInfo(diretorioFile);


                if (File.Exists(diretorioFile))
                {
                    if (fileInfo.LastWriteTime != fileInfoDestino.LastWriteTime)
                        fileInfo.CopyTo(diretorioFile, true);
                }
                else
                    fileInfo.CopyTo(diretorioFile, true);


                return true;
            }
            catch (Exception ex)
            {
                log.LogError("Falha ao Copiar o arquivo do Buffer: " + diretorioFile,
                                MethodBase.GetCurrentMethod().DeclaringType.Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);

                return false;
            }

        }

        private void SyncPastaEspelho()
        {

            quantidade = Directory.GetFiles(configuration.PastaBackup, "*", SearchOption.AllDirectories).Length
                            + Directory.GetDirectories(configuration.PastaBackup, "*", SearchOption.AllDirectories).Length;
            restante = quantidade;

            if (abort)
                return;

            while (pause)
                Thread.Sleep(500);

            DirectoryInfo origemPathInfo = new DirectoryInfo(configuration.PastaBackup);
            DirectoryInfo destinoPathInfo = new DirectoryInfo(configuration.PastaEspelho);

            CopyAll(origemPathInfo, destinoPathInfo, destinoPathInfo, false);

            if (abort)
                return;

            while (pause)
                Thread.Sleep(500);

            DeleteOldPath(origemPathInfo, destinoPathInfo);

        }

        private void CopiaBuffer()
        {
            bool copiado = false;

            for (int i = (bufferArquivos.Count - 1); i >= 0; i--)
            {
                Tuple<string, string> item = bufferArquivos[i];

                copiado = CopyArquive(item.Item1, item.Item2);

                if (abort)
                    return;

                if (copiado)
                    bufferArquivos.Remove(item);
                log.LogInfo("Copiado do Buffer o Arquivo: " + item.Item2);
            }


        }

        #endregion

        #region Pastas

        private void OrganizarPastasFull(string newPathDiario)
        {
            try
            {
                string[] listPastasFull = Directory.GetDirectories(configuration.PastaBackup, "*", SearchOption.TopDirectoryOnly);

                while (listPastasFull.Count() > configuration.LimiteBackupsFull)
                {
                    string delPathDiario = listPastasFull[0];

                    //deleta a pasta
                    new DirectoryInfo(@"\\?\" + delPathDiario).Delete(true);

                    listPastasFull = Directory.GetDirectories(pathFULL, "*", SearchOption.TopDirectoryOnly);
                }
            }
            catch (Exception ex)
            {
                log.LogError("Falha na Organização dos Backups Full, para o diretorio: " + pathFULL,
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }

        }

        #endregion

        #region Email
        private void NotificacaoEmail(DateTime dataNewBackup, bool inicializando)
        {
            if (true)
            {

                try
                {
                    Email email = new Email();

                    email.SmtpServerString = "smtp.gmail.com";
                    email.Origem = configuration.EmailOrigem;
                    email.Password = encrypt.Decrypto(configuration.SenhaOrigem);
                    email.Assunto = "Backup Drive Servidor: " + dataNewBackup.ToString("yyyyMMdd");

                    while (pause)
                        Thread.Sleep(500);

                    if (inicializando)
                    {
                        email.Anexos = new Attachment[0];
                        email.CorpoEmail = "Backup Foi Inicializado!" + Environment.NewLine;
                    }
                    else
                    {
                        email.CorpoEmail = "Backup Foi Finalizado com Falhas:" + Environment.NewLine;

                        if (bufferArquivos.Count > 0 || bufferPastas.Count > 0)
                        {
                            if (bufferPastas.Count > 0)
                            {
                                email.CorpoEmail += "As seguintes Pastas não foram criadas ou copiadas: " + Environment.NewLine;

                                foreach (string item in bufferPastas)
                                    email.CorpoEmail += "   - " + item + Environment.NewLine;
                            }

                            if (abort)
                                return;

                            email.CorpoEmail += Environment.NewLine + Environment.NewLine;


                            while (pause)
                                Thread.Sleep(500);

                            if (bufferArquivos.Count > 0)
                            {
                                email.CorpoEmail += "Os seguintes Arquivos não foram copiados: " + Environment.NewLine;

                                foreach (Tuple<string, string> item in bufferArquivos)
                                    email.CorpoEmail += "   - " + item.Item2 + Environment.NewLine;
                            }

                            email.CorpoEmail += Environment.NewLine + Environment.NewLine;

                            email.Anexos = new Attachment[1];
                            email.Anexos[0] = new Attachment(log.pathTxt);
                        }
                        else
                        {
                            email.Anexos = new Attachment[0];

                            email.CorpoEmail = "Backup Realizado com Sucesso!" + Environment.NewLine;
                        }
                    }

                    while (pause)
                        Thread.Sleep(500);

                    if (abort)
                        return;
                    email.SendEmail();

                }
                catch (Exception ex)
                {
                    log.LogError("Falha na monstagem do email.",
                                    MethodBase.GetCurrentMethod().Name,
                                        MethodBase.GetCurrentMethod().ToString(),
                                            ex.Message);
                }

                bufferPastas.Clear();
                bufferArquivos.Clear();
            }
        }

        #endregion
    }
}
