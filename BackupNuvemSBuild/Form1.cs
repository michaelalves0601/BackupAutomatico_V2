using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xl = Microsoft.Office.Interop.Excel;

namespace BackupNuvemSBuild
{
    public partial class Form1 : Form
    {

        #region Globais
        Log log = new Log();


        string pathDIARIOS = "";
        string pathSEMANAIS = "";
        string pathMENSAIS = "";

        Configuration configuration = new Configuration();
        string pathConfiguration = Application.StartupPath + @"\Config\Configuration.ini";


        bool isAlive = false;

        BackgroundWorker bwBackup = null;

        string parentPathDrive = "";

        int progressTotal = 0;
        double progressTotalFloat = 0;

        int progressSub = 0;
        double progressSubFloat = 0;

        long restante = 0;
        long quantidade = 0;
        long totalQuantidade = 0;

        int etapa = 0;

        bool inicializado = false;

        int MAX_PATH = 260;
        int MAX_DIRECTORY = 248;

        System.Windows.Forms.Timer timerBackup = new System.Windows.Forms.Timer();

        List<Tuple<string, string>> bufferArquivos = new List<Tuple<string, string>>();
        List<string> bufferPastas = new List<string>();

        DateTime timeInicial = new DateTime();
        DateTime timeFinal = new DateTime();
        TimeSpan tempo = new TimeSpan();
        long tamanho = 0;

         DataTable dtRelatorio = new DataTable();
        string pathExcel = "";
        #endregion


        public Form1()
        {
            VerificaProcesso();

            InitializeComponent();
        }

        private void VerificaProcesso()
        {

            #region Reset atual
            DateTime antigoData = new DateTime(1970, 1, 1, 1, 1, 1);
            DateTime recenteData = DateTime.Now;

            Process[] processos = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);

            bool primeiraLeitura = true;
            bool killProcessCurrent = false;

            Process processoRecente = new Process();

            foreach (Process processo in processos)
            {
                recenteData = processo.StartTime;

                if ((recenteData >= antigoData) & !primeiraLeitura)
                {
                    if (processo.StartTime == Process.GetCurrentProcess().StartTime)
                        killProcessCurrent = true;
                    else
                        processo.Kill();
                }
                else
                {
                    if (primeiraLeitura)
                        primeiraLeitura = false;
                    else
                        if (processoRecente.StartTime == Process.GetCurrentProcess().StartTime)
                        killProcessCurrent = true;
                    else
                        processoRecente.Kill();

                    processoRecente = processo;
                    antigoData = processo.StartTime;
                }
            }

            if (killProcessCurrent)
                Process.GetCurrentProcess().Kill();
            #endregion

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            inicializado = false;

            try
            {
                txtVersao.Text = Assembly.GetEntryAssembly().GetName().Version.ToString();
                txtVersao.Text = "v" + txtVersao.Text.Substring(0, txtVersao.Text.Length - 2);
            }
            catch (Exception ex)
            {
                txtVersao.Text = "";
                log.LogError("Erro na leitura da Versão.",
                                MethodBase.GetCurrentMethod().DeclaringType.Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }


            RestauraConfiguracao();

            pathDIARIOS = configuration.PastaBackup + @"\D\";
            pathSEMANAIS = configuration.PastaBackup + @"\S\";
            pathMENSAIS = configuration.PastaBackup + @"\M\";


            timerBackup.Tick += TimerBackup_Tick;
            timerBackup.Interval = 60000;
            timerBackup.Enabled = true;
            //timerBackup.Start();


            this.WindowState = FormWindowState.Minimized;
            backupIcon.Visible = true;


            dtRelatorio.Columns.Add("DateTime", typeof(DateTime));
            dtRelatorio.Columns.Add("Tempo(ms)", typeof(decimal));
            dtRelatorio.Columns.Add("Size(B)", typeof(long));
            dtRelatorio.Columns.Add("Arquivo", typeof(string));

            inicializado = true;
        }


        private void RestauraConfiguracao()
        {
            bool resultado = configuration.RestauraConfiguracao(pathConfiguration);

            if (!resultado)
                MessageBox.Show("Não foi possível restaura as configuração do diretório: " + pathConfiguration);
            else
            {
                ckbBackupAutomatico.Checked = configuration.BackupAutomatico;
                txbPastaDrive.Text = configuration.PastaDrive;
                txbPastaBackup.Text = configuration.PastaBackup;
                ckbSync.Checked = configuration.HabilitaPastaEspelho;
                txbPastaSync.Text = configuration.PastaEspelho;
                txtLimiteDias.Text = configuration.BackupsDiarios.ToString();
                txtLimiteSemanais.Text = configuration.BackupsSemanais.ToString();
                txtLimiteMensais.Text = configuration.BackupsMensais.ToString();
            }
        }


        private void TimerBackup_Tick(object sender, EventArgs e)
        {
            timerBackup.Stop();

            DateTime dateTimeBackup = new DateTime(2000, 1, 1, 20, 0, 0);

            if ((DateTime.Now.Hour == dateTimeBackup.Hour
                    && DateTime.Now.Minute == dateTimeBackup.Minute) && ckbBackupAutomatico.Checked)
            {
                DateTime dataNewBackup = DateTime.Now;

                AssyncBackup(false, dataNewBackup);
            }

            timerBackup.Start();
        }


        #region Selecionar Diretorios

        private void btnSearchDrive_Click(object sender, EventArgs e)
        {
            configuration.PastaDrive = SearchPathFolder();

            txbPastaDrive.Text = configuration.PastaDrive;

            configuration.SalvaConfiguracao();
        }

        private void btnSearchPath_Click(object sender, EventArgs e)
        {
            configuration.PastaBackup = SearchPathFolder();

            pathDIARIOS = configuration.PastaBackup + @"\D\";
            pathSEMANAIS = configuration.PastaBackup + @"\S\";
            pathMENSAIS = configuration.PastaBackup + @"\M\";

            txbPastaBackup.Text = configuration.PastaBackup;

            configuration.SalvaConfiguracao();

        }

        private void btnSearchEspelho_Click(object sender, EventArgs e)
        {
            configuration.PastaEspelho = SearchPathFolder();

            txbPastaSync.Text = configuration.PastaEspelho;

            configuration.SalvaConfiguracao();
        }

        private string SearchPathFolder()
        {
            string diretorio = "";

            //Instancias para tela de escolha de psta
            DialogResult result = DialogResult.None;
            FolderBrowserDialog FD = new FolderBrowserDialog();

            try
            {
                //inicia com ultimo diretorio utilizado
                //FD.SelectedPath = atualDiretorio;

                //abre tela de confirmação do atual diretorio ou para escolha de um novo.
                //Retorna se pressionado OK ou Cancel
                result = FD.ShowDialog();

                //Caso não seja selecionado OK na escolha do diretorio, então anula diretorio. Senão atualiza atual diretorio
                if (result != DialogResult.OK)
                    diretorio = "";
                else
                {
                    diretorio = FD.SelectedPath;
                }


            }
            catch (Exception ex)
            {
                diretorio = "";
                log.LogError("Erro na escolha do diretório de Backup",
                                    MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().ToString(),
                                            ex.ToString());
            }

            return diretorio;
        }

        #endregion



        #region Icone
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            backupIcon.Visible = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                //backupIcon.BalloonTipText = "My application still working...";
                //backupIcon.BalloonTipTitle = "My Sample Application";
                backupIcon.BalloonTipIcon = ToolTipIcon.Info;
                //backupIcon.Icon = new Icon(SystemIcons.Shield, 40, 40);
                backupIcon.Visible = true;
                this.Hide();
            }

        }

        private void backupIcon_MouseClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

            this.Show();
            //backupIcon.Visible = false;
                   this.WindowState = FormWindowState.Normal;
        }

        #endregion


        #region Backup

        private void btnSync_Click(object sender, EventArgs e)
        {
            AssyncBackup(true, DateTime.Now);
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            DateTime dataNewBackup = dtpData.Value;

            // cria nome do backup Diario atual
            string newPathDiario = pathDIARIOS + dataNewBackup.ToString("yyMMdd");

            string newPathDiario_OLD = pathDIARIOS + dataNewBackup.ToString("yyyyMMdd");


            // Verifica se o backup já existe
            if (Directory.Exists(newPathDiario) || Directory.Exists(newPathDiario_OLD))
                MessageBox.Show("Backup '" + newPathDiario + "' já existe!");
            else
                AssyncBackup(false, dataNewBackup);
        }



        private void AssyncBackup(bool apenasSync, DateTime dataNewBackup)
        {
            if (!isAlive)
            {

                isAlive = true;

                progressSub = 0;
                progressSubFloat = 0;
                progressTotal = 0;
                progressTotalFloat = 0;

                pgbSubPasta.Value = 0;
                pgbTotal.Value = 0;
                txtPercentSubPasta.Text = "0.0%";
                txtPercentTotal.Text = "0.0%";

                txtPercentSubPasta.Visible = true;
                txtPercentTotal.Visible = true;
                txtPasta.Visible = true;
                txtArquivos.Visible = true;
                txtRestantes.Visible = true;

                txtPasta.Text = "";
                txtArquivos.Text = "";
                txtRestantes.Text = "";

                btnSearchDrive.Enabled = false;
                btnSearchPath.Enabled = false;
                btnSearchEspelho.Enabled = false;
                btnSync.Enabled = false;
                btnBackup.Enabled = false;

                txtLimiteDias.ReadOnly = true;
                txtLimiteSemanais.ReadOnly = true;
                txtLimiteMensais.ReadOnly = true;

                txtEtapa.Visible = true;
                txtEtapa.Text = "Calculando... (0/3)";
                etapa = 0;


                bool enableSync = ckbSync.Checked;


                //Inicia objeto assincrono UNICO para rodas apenas uma vez
                bwBackup = new BackgroundWorker();

                ////Declara auxiliares para o processo assincrono
                bwBackup.ProgressChanged +=
                    new ProgressChangedEventHandler(bwBackup_ProgressChanged);

                ////habilita propriedade de acompanhar progressão do processo
                bwBackup.WorkerReportsProgress = true;

                bwBackup.RunWorkerCompleted +=
                        new RunWorkerCompletedEventHandler(bwBackup_RunWorkerCompleted);

                //inicia processo assincrono
                bwBackup.DoWork += (Senderbw, args) =>
                {
                    try
                    {
                        // cria nome do backup Diario atual
                        string newPathDiario = pathDIARIOS + dataNewBackup.ToString("yyMMdd");

                        string newPathDiario_OLD = pathDIARIOS + dataNewBackup.ToString("yyyyMMdd");


                        if (!apenasSync)
                        {

                            // Verifica se o backup já existe
                            if (Directory.Exists(newPathDiario) || Directory.Exists(newPathDiario_OLD))
                                log.LogWarning("Backup '" + newPathDiario + "' já existe!",
                                                    MethodBase.GetCurrentMethod().Name,
                                                        MethodBase.GetCurrentMethod().ToString(), "");
                            else
                            {

                                NotificacaoEmail(dataNewBackup, true);

                                log.LogInfo("Iniciando Backup: " + dataNewBackup.ToString("yyyyMMdd"));
                                log.LogInfo("Iniciando Cópia do Drive.");


                                // Cria nova pasta para o Backup Diario
                                Directory.CreateDirectory(newPathDiario);

                                List<Tuple<string, long>> listFolders = SelecionaPastasDrive();


                                txtEtapa.Invoke((MethodInvoker)(() =>
                                    txtEtapa.Text = "Copiando Drive (1/3)"
                                ));

                                etapa = 1;

                                CopiaArquivos(listFolders, newPathDiario);

                                txtEtapa.Invoke((MethodInvoker)(() =>
                                    txtEtapa.Text = "Copiando Buffer (1/3)"
                                ));

                                CopiaBuffer();


                                Thread.Sleep(500);


                                log.LogInfo("Iniciando Organização das pastas de Backup.");

                                quantidade = totalQuantidade;
                                restante = quantidade;


                                txtEtapa.Invoke((MethodInvoker)(() =>
                                    txtEtapa.Text = "Organizando Pastas (2/3)"
                                ));

                                etapa = 2;

                                OrganizaBackupDiarios(newPathDiario, dataNewBackup);

                                Thread.Sleep(500);

                                if (enableSync)
                                {
                                    log.LogInfo("Iniciando Sincronização com para Espelho.");


                                    txtEtapa.Invoke((MethodInvoker)(() =>
                                                txtEtapa.Text = "Sincronizando Pasta Espelho (3/3)  - Calculando..."
                                            ));

                                    etapa = 3;

                                    SyncPastaEspelho();

                                    Thread.Sleep(500);
                                }


                                log.LogInfo("Finalizando Backup: " + dataNewBackup.ToString("yyyyMMdd"));

                                txtEtapa.Invoke((MethodInvoker)(() =>
                                                txtEtapa.Text = "Exportando Relatório..."
                                            ));

                                Exportar();


                                txtEtapa.Invoke((MethodInvoker)(() =>
                                                txtEtapa.Text = "Encaminhando Email..."
                                            ));

                                NotificacaoEmail(dataNewBackup, false);
                            }
                        }
                        else
                        {
                            log.LogInfo("Iniciando Sincronização com para Espelho.");


                            txtEtapa.Invoke((MethodInvoker)(() =>
                                        txtEtapa.Text = "Sincronizando Pasta Espelho (3/3)  - Calculando..."
                                    ));

                            etapa = 3;

                            SyncPastaEspelho();

                            Thread.Sleep(500);


                            log.LogInfo("Finalizando Backup: " + dataNewBackup.ToString("yyyyMMdd"));
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

        }

        private List<Tuple<string, long>> SelecionaPastasDrive()
        {
            string[] listSubPastas = Directory.GetDirectories(configuration.PastaDrive, "*", SearchOption.TopDirectoryOnly);

            List<Tuple<string, long>> listFolders = new List<Tuple<string, long>>();

            long quantidade = 0;
            string folderAtual = "";

            cklPastasDrive.Invoke((MethodInvoker)(() =>
                    cklPastasDrive.Items.Clear()
                ));


            string[] allfiles1 = Directory.GetFiles(configuration.PastaDrive, "*", SearchOption.AllDirectories);



            string[] allfolders1 = Directory.GetDirectories(configuration.PastaDrive, "*", SearchOption.AllDirectories);

            totalQuantidade = allfolders1.Count();


            foreach (string path in listSubPastas)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                parentPathDrive = directoryInfo.Parent.Name;

                string[] allfiles = Directory.GetFiles(path, "*", SearchOption.AllDirectories);



                string[] allfolders = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);

                quantidade = allfolders.Count() + allfiles.Count();

                listFolders.Add(new Tuple<string, long>(directoryInfo.Name, quantidade));

                folderAtual = directoryInfo.Name + " - (" + quantidade + ")";

                cklPastasDrive.Invoke((MethodInvoker)(() =>
                    cklPastasDrive.Items.Add(folderAtual)
                ));
            }

            return listFolders;
        }


        private void CopiaArquivos(List<Tuple<string, long>> listFolders, string newPathDiario)
        {
            string pathFolder = "";

            string destPathBackup = newPathDiario;

            Directory.CreateDirectory(destPathBackup);

            long quantidadeProgress = 0;


            for (int i = 0; i < listFolders.Count; i++)
            {
                pathFolder = configuration.PastaDrive + @"\" + listFolders[i].Item1;
                quantidade = listFolders[i].Item2;
                restante = quantidade;

                txtPasta.Invoke((MethodInvoker)(() =>
                    txtPasta.Text = listFolders[i].Item1
                ));

                txtArquivos.Invoke((MethodInvoker)(() =>
                    txtArquivos.Text = quantidade.ToString()
                ));

                txtRestantes.Invoke((MethodInvoker)(() =>
                    txtRestantes.Text = restante.ToString()
                ));


                Copy(pathFolder, destPathBackup + @"\" + listFolders[i].Item1);

                restante = 0;
                txtRestantes.Invoke((MethodInvoker)(() =>
                    txtRestantes.Text = restante.ToString()
                ));


                cklPastasDrive.Invoke((MethodInvoker)(() =>
                    cklPastasDrive.SetItemCheckState(i, CheckState.Checked)
                ));


                quantidadeProgress += quantidade;

                progressTotalFloat = Math.Round((quantidadeProgress * 100.0) / totalQuantidade, 1);

                progressTotal = Convert.ToInt32(progressTotalFloat);
                progressTotal = (progressTotal > 100 ? 100 : progressTotal);

                bwBackup.ReportProgress(0);
            }


            progressTotal = 100;
            progressTotalFloat = 100;
            progressSubFloat = 0;
            progressSub = 0;
            bwBackup.ReportProgress(0);

            txtPasta.Invoke((MethodInvoker)(() =>
                    txtPasta.Text = ""
                ));

            txtArquivos.Invoke((MethodInvoker)(() =>
                txtArquivos.Text = ""
            ));

            txtRestantes.Invoke((MethodInvoker)(() =>
                txtRestantes.Text = ""
            ));


            txtPercentSubPasta.Invoke((MethodInvoker)(() =>
                txtPercentSubPasta.Text = ""
            ));

        }


        private void CopiaBuffer()
        {
            bool copiado = false;

            for (int i = (bufferArquivos.Count - 1); i >= 0; i--)
            {
                Tuple<string, string> item = bufferArquivos[i];

                copiado = CopyArquive(item.Item1, item.Item2);

                if (copiado)
                {
                    bufferArquivos.Remove(item);
                    log.LogInfo("Copiado do Buffer o Arquivo: " + item.Item2);
                }
            }


        }


        #region Organiza Backups

        private void OrganizaBackupDiarios(string newPathDiario, DateTime dataNewBackup)
        {
            try
            {

                // Cria nova pasta para o Backup Diario
                Directory.CreateDirectory(newPathDiario);

                // Consulta Backups Diarios Existentes
                string[] listDiarios = Directory.GetDirectories(pathDIARIOS, "*", SearchOption.TopDirectoryOnly);


                bool habilitaSemanalExcessao = false;
                if (dataNewBackup.DayOfWeek == DayOfWeek.Sunday)
                    habilitaSemanalExcessao = true;

                //Verificar se o limite de backups diario foi atingido
                while (listDiarios.Count() > configuration.BackupsDiarios || habilitaSemanalExcessao)
                {
                    // seleciona o backup diario mais antigo para deletar e o mais atual
                    string delPathDiario = listDiarios[0];
                    string ultimoPathDiario = listDiarios[listDiarios.Count() - 1];


                    // Verificar se o Backup que será deletado, já tem um backup semanal
                    OrganizaBackupSemanais(delPathDiario.Replace(pathDIARIOS, ""),
                                                ultimoPathDiario.Replace(pathDIARIOS, ""),
                                                    dataNewBackup);

                    if (listDiarios.Count() > configuration.BackupsDiarios)
                    {
                        // deleta o backup diario mais antigo
                        new DirectoryInfo(@"\\?\" + delPathDiario).Delete(true);
                    }

                    habilitaSemanalExcessao = false;

                    listDiarios = Directory.GetDirectories(pathDIARIOS, "*", SearchOption.TopDirectoryOnly);
                }

            }
            catch (Exception ex)
            {
                log.LogError("Falha na Organização dos Backups Diarios, para o diretorio: " + newPathDiario,
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }
        }

        private void OrganizaBackupSemanais(string delPathDiario, string ultimoPathDiario, DateTime dataNewBackup)
        {

            try
            {
                if (!Directory.Exists(pathSEMANAIS))
                    Directory.CreateDirectory(pathSEMANAIS);

                // Consulta Backups Semanais Existentes
                string[] listSemanais = Directory.GetDirectories(pathSEMANAIS, "*", SearchOption.TopDirectoryOnly);


                bool newSemanal = false;

                // Verifica se existe backups semanais
                if (listSemanais.Count() == 0)
                    newSemanal = true;
                else
                {
                    // extrai a data do backup da semanaa mais recente
                    string ultimaSemana = listSemanais[listSemanais.Count() - 1];
                    ultimaSemana = ultimaSemana.Replace(pathSEMANAIS, "");
                    ultimaSemana = ultimaSemana.Substring(ultimaSemana.IndexOf("_") + 1);

                    //converte de string para data
                    DateTime dateUltimaSemana = new DateTime(Convert.ToInt16(ultimaSemana.Substring(0, 2)),
                                                                Convert.ToInt16(ultimaSemana.Substring(2, 2)),
                                                                    Convert.ToInt16(ultimaSemana.Substring(4, 2)));


                    // extrai e converte para data o backup diario que sera deletado
                    DateTime datediarioDeletar = new DateTime(Convert.ToInt16(delPathDiario.Substring(0, 2)),
                                                                Convert.ToInt16(delPathDiario.Substring(2, 2)),
                                                                    Convert.ToInt16(delPathDiario.Substring(4, 2)));


                    // Verifica se o backup que vai ser deletado, esta contigo em backup semanal
                    if (datediarioDeletar > dateUltimaSemana ||
                            dataNewBackup.DayOfWeek == DayOfWeek.Sunday)
                    {
                        newSemanal = true;

                        //Verifica se ira passar do limite de backups semanais
                        while (listSemanais.Count() >= configuration.BackupsSemanais)
                        {
                            // coleta o backup semanal mais antigo
                            string delPathSemanal = listSemanais[0];
                            string ultimoPathSemanal = listSemanais[listSemanais.Count() - 1];


                            // Verificar se o Backup que será deletado, já tem um backup mensal
                            OrganizaBackupMensais(delPathSemanal.Replace(pathSEMANAIS, ""), ultimoPathSemanal.Replace(pathSEMANAIS, ""));


                            // deleta o backup semanal mais antigo
                            new DirectoryInfo(@"\\?\" + delPathSemanal).Delete(true);


                            listSemanais = Directory.GetDirectories(pathSEMANAIS, "*", SearchOption.TopDirectoryOnly);
                        }
                    }

                }

                // Verifica se precisa criar uma nova pasta de backup semanal
                if (newSemanal)
                {
                    //cria diretorio para pasta de backup semanal
                    string newPathSemanal = pathSEMANAIS + delPathDiario.Replace(pathDIARIOS, "") + "_" + ultimoPathDiario.Replace(pathDIARIOS, "");

                    Directory.CreateDirectory(newPathSemanal);

                    //copia arquivos contidos na pasta do backup diario para o semanal
                    Copy(pathDIARIOS + ultimoPathDiario, newPathSemanal + @"\" + ultimoPathDiario);
                }
            }
            catch (Exception ex)
            {
                log.LogError("Falha na Organização dos Backups Semanais",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }
        }

        private void OrganizaBackupMensais(string delPathSemanal, string ultimoPathSemanal)
        {
            try
            {
                if (!Directory.Exists(pathMENSAIS))
                    Directory.CreateDirectory(pathMENSAIS);


                // Consulta Backups Mensais Existentes
                string[] listMensais = Directory.GetDirectories(pathMENSAIS, "*", SearchOption.TopDirectoryOnly);

                bool newMensal = false;


                // Verifica se existe backups mensais
                if (listMensais.Count() == 0)
                    newMensal = true;
                else
                {
                    // extrai a data do backup da mensal mais recente
                    string ultimoMensal = listMensais[listMensais.Count() - 1];
                    ultimoMensal = ultimoMensal.Replace(pathMENSAIS, "");
                    ultimoMensal = ultimoMensal.Substring(ultimoMensal.IndexOf("_") + 1);

                    //converte de string para data
                    DateTime dateUltimoMensal = new DateTime(Convert.ToInt16(ultimoMensal.Substring(0, 2)),
                                                                Convert.ToInt16(ultimoMensal.Substring(2, 2)),
                                                                    Convert.ToInt16(ultimoMensal.Substring(4, 2)));


                    // extrai e converte para data o backup diario que sera deletado
                    string semanalDeletarAux = delPathSemanal.Substring(delPathSemanal.IndexOf("_") + 1);
                    DateTime dateSemanalDeletar = new DateTime(Convert.ToInt16(semanalDeletarAux.Substring(0, 2)),
                                                                Convert.ToInt16(semanalDeletarAux.Substring(2, 2)),
                                                                    Convert.ToInt16(semanalDeletarAux.Substring(4, 2)));

                    // Verifica se o backup que vai ser deletado, esta contigo em backup semanal
                    if (dateSemanalDeletar > dateUltimoMensal)
                    {
                        newMensal = true;

                        //Verifica se ira passar do limite de backups mensais
                        while (listMensais.Count() >= configuration.BackupsMensais)
                        {
                            // coleta o backup mensal mais antigo
                            string delPathMensal = listMensais[0];

                            // deleta o backup semanal mais antigo
                            new DirectoryInfo(@"\\?\" + delPathMensal).Delete(true);


                            listMensais = Directory.GetDirectories(pathMENSAIS, "*", SearchOption.TopDirectoryOnly);
                        }
                    }
                }


                // Verifica se precisa criar uma nova pasta de backup mensla
                if (newMensal)
                {

                    //cria diretorio para pasta de backup semanal
                    string newPathMensal = pathMENSAIS + delPathSemanal.Substring(0, 6);
                    newPathMensal += "_" + ultimoPathSemanal.Substring(ultimoPathSemanal.IndexOf("_") + 1);

                    Directory.CreateDirectory(newPathMensal);

                    //copia arquivos contidos na pasta do backup semanal para o mensal
                    Copy(pathSEMANAIS + ultimoPathSemanal, newPathMensal + @"\" + ultimoPathSemanal);
                }
            }
            catch (Exception ex)
            {
                log.LogError("Falha na Organização dos Backups Mensais",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }
        }


        #endregion


        private void SyncPastaEspelho()
        {
            progressSubFloat = 0;
            progressSub = 0;
            progressTotal = 0;
            progressTotalFloat = 0;
            restante = 0;
            bwBackup.ReportProgress(0);



            quantidade = Directory.GetFiles(configuration.PastaBackup, "*", SearchOption.AllDirectories).Length
                            + Directory.GetDirectories(configuration.PastaBackup, "*", SearchOption.AllDirectories).Length;
            restante = quantidade;


            txtArquivos.Invoke((MethodInvoker)(() =>
                    txtArquivos.Text = quantidade.ToString()
                ));

            long quantidadeAux = quantidade;

            txtEtapa.Invoke((MethodInvoker)(() =>
                    txtEtapa.Text = "Sincronizando Pasta Espelho (3/3)  - Copiando..."
                ));

            Copy(configuration.PastaBackup, configuration.PastaEspelho);


            progressSubFloat = 0;
            progressSub = 0;
            progressTotal = 50;
            progressTotalFloat = 50;
            bwBackup.ReportProgress(0);


            quantidade = quantidadeAux;
            restante = quantidade;


            txtEtapa.Invoke((MethodInvoker)(() =>
                    txtEtapa.Text = "Sincronizando Pasta Espelho (3/3)  - Deletando..."
                ));


            DirectoryInfo origemPathInfo = new DirectoryInfo(configuration.PastaBackup);
            DirectoryInfo destinoPathInfo = new DirectoryInfo(configuration.PastaEspelho);

            DeleteOldPath(origemPathInfo, destinoPathInfo);

            progressSubFloat = 0;
            progressSub = 0;
            progressTotal = 100;
            progressTotalFloat = 100;


            txtArquivos.Invoke((MethodInvoker)(() =>
                    txtArquivos.Text = ""
                ));
        }


        private void NotificacaoEmail(DateTime dataNewBackup, bool inicializando)
        {
            if (true)
            {

                try
                {
                    Email email = new Email();

                    email.SmtpServerString = "smtp.gmail.com";
                    email.Origem = "sbuildautomacao@gmail.com";
                    email.Password = "sbuildTI01!";

                    email.Destinos = new string[1];
                   email.Destinos[0] = "beatriz.matsushita@sbuild.com.br";
                  //email.Destinos[1] = "rubens@sbuild.com.br";
                   // email.Destinos[2] = "wesley@sbuild.com.br";


                    email.Assunto = "Backup Drive Servidor: " + dataNewBackup.ToString("yyyyMMdd");

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

                            email.CorpoEmail += Environment.NewLine + Environment.NewLine;


                            if (bufferArquivos.Count > 0)
                            {
                                email.CorpoEmail += "Os seguintes Arquivos não foram copiados: " + Environment.NewLine;

                                foreach (Tuple<string, string> item in bufferArquivos)
                                    email.CorpoEmail += "   - " + item.Item2 + Environment.NewLine; ;
                            }

                            email.CorpoEmail += Environment.NewLine + Environment.NewLine;

                            email.Anexos = new Attachment[1];
                            email.Anexos[0] = new Attachment(log.pathTxt);
                            //email.Anexos[1] = new Attachment(pathExcel);
                        }
                        else
                        {
                            email.Anexos = new Attachment[0];
                            //email.Anexos[0] = new Attachment(pathExcel);

                            email.CorpoEmail = "Backup Realizado com Sucesso!" + Environment.NewLine;
                        }
                    }

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


        #region Copy & Delete


        private void Copy(string origemPath, string destinoPath)
        {
            progressSubFloat = 0;
            progressSub = 0;
            bwBackup.ReportProgress(0);

            DirectoryInfo origemPathInfo = new DirectoryInfo(origemPath);
            DirectoryInfo destinoPathInfo = new DirectoryInfo(destinoPath);

            CopyAll(origemPathInfo, destinoPathInfo);
        }

        private void CopyAll(DirectoryInfo origemPathInfoAux, DirectoryInfo destinoPathInfoAux)
        {
            DirectoryInfo origemPathInfo = null;
            DirectoryInfo destinoPathInfo = null;

            //// reflection
            //FieldInfo maxPathField = typeof(Path).GetField("MaxPath",
            //    BindingFlags.Static |
            //    BindingFlags.GetField |
            //    BindingFlags.NonPublic);

            //// invoke the field gettor, which returns 260
            //int MaxPathLength = (int)maxPathField.GetValue(null);

            try
            {
                if (origemPathInfoAux.FullName.Length >= MAX_DIRECTORY)
                    origemPathInfo = new DirectoryInfo(@"\\?\" + origemPathInfoAux.FullName);
                else
                    origemPathInfo = new DirectoryInfo(origemPathInfoAux.FullName);

                if (destinoPathInfoAux.FullName.Length >= MAX_DIRECTORY)
                    destinoPathInfo = new DirectoryInfo(@"\\?\" + destinoPathInfoAux.FullName);
                else
                    destinoPathInfo = new DirectoryInfo(destinoPathInfoAux.FullName);


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

                    FileInfo fileInfoDestino = new FileInfo(diretorioFile);

                    timeInicial = DateTime.Now;

                    if (File.Exists(diretorioFile))
                    {
                        if (fileInfo.LastWriteTime != fileInfoDestino.LastWriteTime)
                            fileInfo.CopyTo(diretorioFile, true);
                    }
                    else
                        fileInfo.CopyTo(diretorioFile, true);

                    timeFinal = DateTime.Now;

                    tempo = timeFinal - timeInicial;

                    tamanho = fileInfo.Length;

                    dtRelatorio.Rows.Add(timeFinal, tempo.TotalMilliseconds, tamanho, fileInfo.Name);

                    AtualizaProgressCopy();                   
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
                DirectoryInfo subPasta = null;

                try
                {
                    if (subPastaAux.FullName.Contains(@"\\?\"))
                        subPasta = new DirectoryInfo(subPastaAux.FullName.Replace(@"\\?\", ""));
                    else
                        subPasta = new DirectoryInfo(subPastaAux.FullName);


                    //DirectoryInfo poximaSubPasta = destinoPathInfo.CreateSubdirectory(subPasta.Name);
                    proximaSubPasta = new DirectoryInfo(Path.Combine(destinoPathInfoAux.FullName, subPasta.Name));
                    CopyAll(subPasta, proximaSubPasta);

                    AtualizaProgressCopy();
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

                    AtualizaProgressCopy();

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

                    AtualizaProgressCopy();

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


        private void AtualizaProgressCopy()
        {
            if (restante > 0)
                restante--;

            if (etapa == 1 || etapa == 3)
            {
                progressSubFloat = Math.Round((((quantidade - restante) * 100.0) / quantidade), 1);
                progressSubFloat = progressSubFloat > 100 ? 100 : progressSubFloat;
                progressSub = Convert.ToInt32(progressSubFloat);
            }
            else
            {
                progressTotalFloat = Math.Round((((quantidade - restante) * 100.0) / quantidade), 1);
                progressTotalFloat = progressTotalFloat > 100 ? 100 : progressTotalFloat;
                progressTotal = Convert.ToInt32(progressTotalFloat);
            }

            bwBackup.ReportProgress(0);
        }


        #endregion




        private void bwBackup_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgbTotal.Value = progressTotal;
            txtPercentTotal.Text = progressTotalFloat.ToString() + "%";

            pgbSubPasta.Value = progressSub;
            txtPercentSubPasta.Text = progressSubFloat.ToString() + "%";

            txtRestantes.Invoke((MethodInvoker)(() =>
                    txtRestantes.Text = restante.ToString()
                ));
        }


        private void bwBackup_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bwBackup.Dispose();

            pgbSubPasta.Value = 0;
            pgbTotal.Value = 0;

            txtPercentSubPasta.Visible = false;
            txtPercentTotal.Visible = false;
            txtPasta.Visible = false;
            txtArquivos.Visible = false;
            txtRestantes.Visible = false;
            txtEtapa.Visible = false;

            btnSearchDrive.Enabled = true;
            btnSearchPath.Enabled = true;
            btnSearchEspelho.Enabled = true;
            btnSync.Enabled = true;
            btnBackup.Enabled = true;

            txtLimiteDias.ReadOnly = false;
            txtLimiteSemanais.ReadOnly = false;
            txtLimiteMensais.ReadOnly = false;

            isAlive = false;
        }


        #endregion



        #region Parametros
        private void txtLimiteDias_TextChanged(object sender, EventArgs e)
        {
            if (inicializado)
            {
                txtLimiteDias.Text = ValidaNumerosDigitado(txtLimiteDias.Text);

                configuration.BackupsDiarios = Convert.ToInt32(txtLimiteDias.Text);

                configuration.SalvaConfiguracao();
            }
        }

        private void txtLimiteSemanais_TextChanged(object sender, EventArgs e)
        {
            if (inicializado)
            {
                txtLimiteSemanais.Text = ValidaNumerosDigitado(txtLimiteSemanais.Text);

                configuration.BackupsSemanais = Convert.ToInt32(txtLimiteSemanais.Text);

                configuration.SalvaConfiguracao();
            }
        }

        private void txtLimiteMensais_TextChanged(object sender, EventArgs e)
        {
            if (inicializado)
            {
                txtLimiteMensais.Text = ValidaNumerosDigitado(txtLimiteMensais.Text);

                configuration.BackupsMensais = Convert.ToInt32(txtLimiteMensais.Text);

                configuration.SalvaConfiguracao();
            }
        }


        private string ValidaNumerosDigitado(string valueTextBox)
        {
            string limiteDiarioAux = "";
            string caracter = "";

            for (int i = 0; i < valueTextBox.Length; i++)
            {
                caracter = valueTextBox.Substring(i, 1);

                if (caracter == "0" || caracter == "1" || caracter == "2" ||
                        caracter == "3" || caracter == "4" || caracter == "5" ||
                            caracter == "6" || caracter == "7" || caracter == "8" || caracter == "9")
                    limiteDiarioAux += caracter;
            }

            if (limiteDiarioAux == "")
                limiteDiarioAux = "0";

            return limiteDiarioAux;
        }



        private void ckbSync_CheckedChanged(object sender, EventArgs e)
        {
            if (inicializado)
            {
                configuration.HabilitaPastaEspelho = ckbSync.Checked;

                configuration.SalvaConfiguracao();
            }
        }

        private void ckbBackupAutomatico_CheckedChanged(object sender, EventArgs e)
        {
            if (inicializado)
            {
                configuration.BackupAutomatico = ckbBackupAutomatico.Checked;

                configuration.SalvaConfiguracao();
            }
        }


        #endregion




        #region Apenas para Testes
        private void txtVersao_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            dtpData.Visible = !dtpData.Visible;
        }

        #endregion




        #region Relatorio


        public void Exportar()
        {
            try
            {
                //Objetos do Excel em espera
                Xl.Application xlApp = null;
                object misValue = misValue = System.Reflection.Missing.Value;
                Xl.Workbook xlWorkBook = null;
                Xl.Worksheet xlWorkSheet = null;


                //variaveis para calculo de porcentagem
                int totalItems = dtRelatorio.Rows.Count;
                int itemsIndex = 0;
                int porcentagem = 0;

                //concatena pasta selecionada com nome do arquivo Excel com a data atual
                string pathTemplateExcel = Application.StartupPath + @"\Relatorio\Template\Relatorio - Modelo S. Build.xlsx";
                pathExcel = Application.StartupPath + @"\Relatorio\Relatorio_" + DateTime.Now.Year.ToString()
                                                                                        + DateTime.Now.Month.ToString()
                                                                                            + DateTime.Now.Day.ToString()
                                                                                                + "_" + DateTime.Now.Hour.ToString()
                                                                                                    + DateTime.Now.Minute.ToString()
                                                                                                        + DateTime.Now.Second.ToString() + ".xlsx";

                FileInfo templateExcel = new FileInfo(pathTemplateExcel);
                templateExcel.CopyTo(pathExcel, true);

                //Inicia com opção de pintura de linha em Branco
                int colunsCont = 0;

                try
                {
                    //Inicia aplicação EXCEL
                    xlApp = new Xl.Application();
                    //Inicia projeto do Excel
                    xlWorkBook = xlApp.Workbooks.Open(pathExcel, 0, false, 5, "", "", false, Xl.XlPlatform.xlWindows, "", true, false, 0, true, false, false);

                    //Inicia planilha vazia do Excel
                    xlWorkSheet = (Xl.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    //Inicia linhas, a partir da celular 12
                    int i = 12;


                    foreach (DataRow row in dtRelatorio.Rows)
                    {
                        int j = 1;
                        int k = 1;

                        foreach (DataColumn coluna in dtRelatorio.Columns)
                        {
                            //if (dtRelatorio.Columns[k - 1].Visible)
                            //{
                            //pega valor de cada célula do DataGridView e preenche célular do Excel.
                            //OBS: Excel inicia pela celular 1, já o datagridview inicia pela célular 0
                            xlWorkSheet.Cells[i, j] = row[k - 1];

                            ////Verifica se está linha será necessário deixar com fundo cinza
                            ////if (pintarLinha)
                            ////    xlWorkSheet.Cells[i, j].Interior.Color = Color.Gainsboro;

                            ////Caso seja coluna 9(Chip), copiar cor da célula do grid para a célular da planilhar excel. (Conforme Chip)
                            ////if (j == (tipoRelatorio == "CG" ? 8 : 9))
                            //xlWorkSheet.Cells[i, j].Font.Color = row.Cells[k - 1].Style.ForeColor;
                            //xlWorkSheet.Cells[i, j].Font.Bold = row.Cells[k - 1].Style.Font.Bold;
                            //xlWorkSheet.Cells[i, j].Interior.Color = row.Cells[k - 1].Style.BackColor;

                            j++;
                            //}

                            k++;
                        }

                        //Habilitar ou não a pintura do fundo da próxima linha
                        //pintarLinha = pintarLinha ? false : true;
                        i++;
                    }

                    //Formata em Negrito, as colunas na primeira linha do excel, e alinha texto no centro
                    //Microsoft.Office.Interop.Excel.Range excelRangeBold = xlWorkSheet.get_Range("A1:AZ1");
                    //excelRangeBold.Font.Bold = true;
                    //excelRangeBold.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    //Formata em Negrito, todas as linhas da coluna dos chip, e alinha texto no centro
                    //excelRangeBold = xlWorkSheet.get_Range((tipoRelatorio == "CG" ? "H2:H" : "I2:I") + (dtg.Rows.Count + 1).ToString());
                    //excelRangeBold.Font.Bold = true;
                    //excelRangeBold.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    //Formata todas as linhas da coluna 'Horas s/ Com' e Horas s/ Hist, alinhando texto no centro
                    //excelRangeBold = xlWorkSheet.get_Range((tipoRelatorio == "CG" ? "G2:G" : "H2:H") + (dtg.Rows.Count + 1).ToString());
                    //excelRangeBold.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    //excelRangeBold = xlWorkSheet.get_Range((tipoRelatorio == "CG" ? "E2:E" : "F2:F") + (dtg.Rows.Count + 1).ToString());
                    //excelRangeBold.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

                    //Formata tamanho das colunas automaticamente
                    xlWorkSheet.UsedRange.EntireColumn.AutoFit();
                }
                catch (Exception ex)
                {
                    log.LogError("Erro na exportação para Excel, no diretório: " + pathExcel,
                                    MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().ToString(),
                                            ex.ToString());
                }

                try
                {
                    //Salva planilha excel em diretorio selecionado
                    xlWorkBook.Save();
                }
                catch (Exception ex)
                {
                    log.LogError("Erro ao tentar Salvar o arquivo no diretório: " + pathExcel,
                                    MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().ToString(),
                                            ex.ToString());
                }

                //Fecha planilha e aplicativo do Excel
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                //Libera memória do excel, projeto excel e planilha
                ReleaseObject(xlWorkSheet);
                ReleaseObject(xlWorkBook);
                ReleaseObject(xlApp);


                log.LogInfo("Exportação para excel realisado com sucesso. Diretório de destino: " + pathExcel);
            }
            catch(Exception ex)
            {

            }
        }



        private void ReleaseObject(object obj)
        {
            try
            {
                //Libera memória do objeto selecionado
                Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                log.LogWarning("Após exportar o Excel, não foi possível realizar a liberação do arquivo: " + obj.ToString(),
                                    MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().ToString(),
                                            ex.ToString());
            }
            finally
            {
                //Libera memória guardada
                GC.Collect();
            }
        }



        #endregion

    }
}
