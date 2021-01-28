using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Globalization;
using BackupNuvemSBuild_Models;
using Microsoft.VisualBasic;
using System.Reflection;
using BCrypt.Net;
using static BCrypt.Net.BCrypt;
using System.Net.Mail;
using System.ServiceProcess;
using BackupNuvemSBuild_Configuration.Properties;
using System.Threading;

namespace BackupNuvemSBuild_Configuration
{
    public partial class FormHome : Form
    {


        #region Global

        int statusBackup = 0; // 0 a 2 --> 0 stand by, 1 -- backup full -- 2, backup diferencial -- 3, espelho
        bool statusPaused = false; // se tá rodando ou não
        int port = 8910;
        string server = "127.0.0.1";
        System.Windows.Forms.Timer timerBackup = new System.Windows.Forms.Timer();
        bool isAlive = false;
        int statusPorcentagem = 0;
        string statusDiretorio = "";
        int statusTempoRestante = 0;
        string versao = "";

        bool linkOld = true;


        bool vendoPassword = false;

        Configuration configuration = new Configuration();
        Email email = new Email();
        Log log = new Log("Configuration");
        Log logTCPClient = new Log("TCPClient");
        Encrypt encrypt = new Encrypt("cenouras", "abacaxis");

        string pathConfiguration = @"Config\Configuration.ini";
        string pathPastasRestritas = @"Config\PastasRestritas.ini";
        string pathUltimoBackup = @"Config\UltimoBackup.ini";
        string pathEmails = @"Config\email.ini";

        string machineName = "localhost";
        string serviceName = "BackupNuvemSBuild_Runtime";

        string comandoSTART = "START";
        string comandoSTOP = "STOP";
        string comandoRESTART = "RESTART";
        string comando = "";

        double timeoutCommandService = 5000;

        BackgroundWorker bwService = null;

        BackgroundWorker bwTCPIP = null;

        string messageRespostaGlobal = "";
        string messageEnvioGlobal = "";

        #endregion

        public FormHome()
        {
            InitializeComponent();
        }

        #region Menu
        private void menu(int botaoMenu)
        {
            btn_home.BackColor = Color.FromArgb(35, 35, 35);
            btn_email.BackColor = Color.FromArgb(35, 35, 35);
            btn_pastas.BackColor = Color.FromArgb(35, 35, 35);
            btn_service.BackColor = Color.FromArgb(35, 35, 35);
            btn_agendamento.BackColor = Color.FromArgb(35, 35, 35);

            btn_home.ForeColor = Color.DimGray;
            btn_email.ForeColor = Color.DimGray;
            btn_agendamento.ForeColor = Color.DimGray;
            btn_pastas.ForeColor = Color.DimGray;
            btn_service.ForeColor = Color.DimGray;

            btn_home.BackgroundImage = Resources.HOME;
            btn_email.BackgroundImage = Resources.emial_dendieni1;
            btn_agendamento.BackgroundImage = Resources.agenda;
            btn_pastas.BackgroundImage = Resources.FOLDER;
            btn_service.BackgroundImage = Resources.serviço;

            pnlHome.Visible = false;
            pnlEmail.Visible = false;
            pnlDiretorios.Visible = false;
            pnlAgendamento.Visible = false;
            pnlServico.Visible = false;

            btn_home.FlatAppearance.BorderSize = 1;
            btn_email.FlatAppearance.BorderSize = 1;
            btn_agendamento.FlatAppearance.BorderSize = 1;
            btn_pastas.FlatAppearance.BorderSize = 1;
            btn_service.FlatAppearance.BorderSize = 1;

            pnlAgendamento2.Visible = false;
            pnlEmail2.Visible = false;
            pnlDiretorios2.Visible = false;
            pnlServico2.Visible = false;

            //check
            

            ptbCheckService.BackColor = Color.FromArgb(35, 35, 35);
            ptbCheckAgendamento.BackColor = Color.FromArgb(35, 35, 35);
            ptbCheckDiretorio.BackColor = Color.FromArgb(35, 35, 35);
            ptbCheckEmail.BackColor = Color.FromArgb(35, 35, 35);

            switch (botaoMenu)
            {
                case 0:
                    btn_home.BackColor = Color.Black;
                    btn_home.ForeColor = Color.White;
                    btn_home.BackgroundImage = Resources.home_on;
                    pnlHome.Visible = true;
                    btn_home.FlatAppearance.BorderSize = 0;
                    break;
                case 1:
                    btn_agendamento.BackColor = Color.Black;
                    btn_agendamento.ForeColor = Color.White;
                    btn_agendamento.BackgroundImage = Resources.agenda_on;
                    pnlAgendamento.Visible = true;
                    btn_agendamento.FlatAppearance.BorderSize = 0;
                    pnlAgendamento2.Visible = true;
                    //check
                    ptbCheckAgendamento.BackColor = Color.Black;

                    break;
                case 2:
                    btn_email.BackColor = Color.Black;
                    btn_email.ForeColor = Color.White;
                    btn_email.BackgroundImage = Resources.emailmano;
                    pnlEmail.Visible = true;
                    btn_email.FlatAppearance.BorderSize = 0;
                    pnlEmail2.Visible = true;
                    //check
                    ptbCheckEmail.BackColor = Color.Black;

                    break;
                case 3:
                    btn_pastas.BackColor = Color.Black;
                    btn_pastas.ForeColor = Color.White;
                    btn_pastas.BackgroundImage = Resources.folder_o;
                    pnlDiretorios.Visible = true;
                    btn_pastas.FlatAppearance.BorderSize = 0;
                    pnlDiretorios2.Visible = true;
                    //check
                    ptbCheckDiretorio.BackColor = Color.Black;

                    break;
                case 4:
                    btn_service.BackColor = Color.Black;
                    btn_service.ForeColor = Color.White;
                    btn_service.BackgroundImage = Resources.serviço_on;
                    pnlServico.Visible = true;
                    btn_service.FlatAppearance.BorderSize = 0;
                    pnlServico2.Visible = true;
                    //check
                    ptbCheckService.BackColor = Color.Black;

                    break;
                default:
                    break;
            }
        }

        private void btn_home_Click(object sender, EventArgs e)
        {
            menu(0);
        }

        private void btn_agendamento_Click(object sender, EventArgs e)
        {

            menu(1);

        }

        private void btn_email_Click(object sender, EventArgs e)
        {
            menu(2);
        }

        private void btn_pastas_Click(object sender, EventArgs e)
        {
            menu(3);
        }

        private void btn_service_Click(object sender, EventArgs e)
        {
            menu(4);

        }

        #endregion

        #region Menu Superior
        private void btnFechar_Enter(object sender, EventArgs e)
        {
            btnFechar.BackColor = Color.FromArgb(192, 0, 0);
        }

        private void btnFechar_Leave(object sender, EventArgs e)
        {
            btnFechar.BackColor = Color.Transparent;
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            if (btn_save.Enabled == true)
            {
                DialogResult botaoAcionado = MessageBox.Show("Deseja sair sem salvar ?", "Sair sem salvar", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (botaoAcionado == DialogResult.OK)
                {
                    this.Close();
                    Environment.Exit(0);
                }

            }
            else
            {
                this.Close();
                Environment.Exit(0);
            }

        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #endregion

        #region Mover Tela
        private bool mouseIsDown = false;
        private Point firstPoint;

        public string ServiceName { get => serviceName; set => serviceName = value; }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            firstPoint = e.Location;
            mouseIsDown = true;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseIsDown = false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseIsDown)
            {
                // Get the difference between the two points
                int xDiff = firstPoint.X - e.Location.X;
                int yDiff = firstPoint.Y - e.Location.Y;

                // Set the new point
                int x = this.Location.X - xDiff;
                int y = this.Location.Y - yDiff;
                this.Location = new Point(x, y);

                //Retira Maximize
                this.WindowState = FormWindowState.Normal;
            }
        }

        #endregion

        #region Inicialização e Restauração
        private void FormHome_Load(object sender, EventArgs e)
        {
            AtualizaStatusBackup();

            AssyncTCPClient("Status");
            //timer
            try
            {
                versao = Assembly.GetEntryAssembly().GetName().Version.ToString();
                versao = "v" + versao.Substring(0, versao.Length - 2);

                lblVersao.Text = versao;
            }
            catch (Exception ex)
            {
                versao = "";
                log.LogError("Erro na leitura da Versão.",
                                MethodBase.GetCurrentMethod().DeclaringType.Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }

            timerBackup.Tick += TimerBackup_Tick;
            timerBackup.Enabled = true;
            timerBackup.Interval = 1000;
            timerBackup.Start();

            bool restaurou = configuration.RestauraConfiguracao(pathConfiguration, pathPastasRestritas);

            lblLastBackup.Text = configuration.UltimoBackup;
            lblLastTipoBackup.Text = configuration.TipoUltimoBackup;
            lblLastBackupSize.Text = configuration.TamanhoUltimoBackup;

            //Restauração do menu Diretórios
            txbDrive.Text = configuration.PastaDrive;
            txbPastaDestino.Text = configuration.PastaBackup;
            if (!configuration.HabilitaPastaEspelho)
            {
                btn_offEspelho.Visible = true;
                btn_onEspelho.Visible = false;
                txbPastaEspelho.Enabled = false;
                btn_searchEspelho.Enabled = false;
                txbPastaEspelho.Text = configuration.PastaEspelho;

            }
            else
            {
                btn_offEspelho.Visible = false;
                btn_onEspelho.Visible = true;
                txbPastaEspelho.Enabled = true;
                btn_searchEspelho.Enabled = true;
                txbPastaEspelho.Text = configuration.PastaEspelho;
            }

            foreach (var item in configuration.PastasRestritas)
                ltvExclusao.Items.Add(item);

            SaveDesabilitado();

            //Restauração do menu Email
            txtOrigem.Text = configuration.EmailOrigem;
            txtSenha.Text = encrypt.Decrypto(configuration.SenhaOrigem);





            email.RetornaEmails();
            foreach (var destino in email.Destinos)
                ltvEmail.Items.Add(destino);


            SaveDesabilitado();

            // Restauração do menu Agendamentos
            if (configuration.BackupDiferencialHabilitado)
            {
                btnOffBkpDif.Visible = false;
                btnOnBkpDif.Visible = true;
                dtpBkDif.Enabled = true;

            }
            else
            {
                btnOffBkpDif.Visible = true;
                btnOnBkpDif.Visible = false;
                dtpBkDif.Enabled = false;

            }

            if (configuration.BackupFULLHabilitado)
            {
                btnDesabilitadoBkpfull.Visible = false;
                btnHabilitadoBkpfull.Visible = true;
                dtpBkFull.Enabled = true;

            }
            else
            {
                btnDesabilitadoBkpfull.Visible = true;
                btnHabilitadoBkpfull.Visible = false;
                dtpBkFull.Enabled = false;

            }

            int horaBackupDif = Convert.ToInt32(configuration.HorarioDiferencial.Substring(0, 2));
            int minBackupDif = Convert.ToInt32(configuration.HorarioDiferencial.Substring(3, 2));
            dtpBkDif.Value = new DateTime(1970, 1, 1, horaBackupDif, minBackupDif, 0);

            int horaBackupFull = Convert.ToInt32(configuration.HorarioFull.Substring(0, 2));
            int minBackupFull = Convert.ToInt32(configuration.HorarioFull.Substring(3, 2));
            dtpBkFull.Value = new DateTime(1970, 1, 1, horaBackupFull, minBackupFull, 0);

            txbdiasFull.Text = configuration.IntervaloBackupsFull.ToString();
            txbLimiteBkFull.Text = configuration.LimiteBackupsFull.ToString();

            SaveDesabilitado();
        }
        #endregion

        #region Animações Tela

        private void AtualizaStatusBackup()
        {

            TimeSpan tsTempoRestante = TimeSpan.FromSeconds(statusTempoRestante);



            switch (statusBackup)
            {
                //STAND BY
                case 0:

                    pcbTempoEstimado.Visible = false;
                    lblTempoEstimadoTitulo.Visible = false;
                    lblTempoEstimado.Visible = false;
                    pcbPastaAtualIcon.Visible = false;
                    lblPastaAtualTitulo.Visible = false;
                    lblPastaAtual.Visible = false;
                    lblPercentBackup.Visible = false;
                    lblPercentBackupEngUnit.Visible = false;
                    pnlPauseAbortBackup.Visible = false;
                    lblTipoBackup.Visible = false;

                    pnlNewBackup.Visible = true;

                    pgbBackup.Value = 0;
                    lblStatusBackup.Text = "STAND BY";
                    lblStatusBackup.ForeColor = Color.FromArgb(127, 127, 127);


                    lblLastBackupTitulo.Visible = true;
                    lblLastTipoBackup.Visible = true;
                    lblLastBackup.Visible = true;
                    lblLastBackupSizeTitulo.Visible = true;
                    lblLastBackupSize.Visible = true;

                    bool restaurou = configuration.RestauraUltimoBackup(pathUltimoBackup);

                    if (restaurou)
                    {
                        lblLastBackup.Text = configuration.UltimoBackup;
                        
                        lblLastTipoBackup.Text = configuration.TipoUltimoBackup;
                        if (configuration.TipoUltimoBackup.ToUpper() == "DIFERENCIAL")
                            lblLastTipoBackup.ForeColor = Color.FromArgb(13, 140, 202);
                        else
                            lblLastTipoBackup.ForeColor = Color.FromArgb(102, 255, 153);


                        lblLastBackupSize.Text = configuration.TamanhoUltimoBackup;
                    }
                    else
                    {
                        lblLastBackup.Text = "NotFound 404";
                        lblLastTipoBackup.Text = "NotFound 404";
                        lblLastBackupSize.Text = "NotFound 404";
                    }


                    break;


                //BACKUP FULL
                case 1:

                    pcbTempoEstimado.Visible = true;
                    lblTempoEstimadoTitulo.Visible = true;
                    lblTempoEstimado.Visible = true;
                    lblTempoEstimado.Text = Humanizer.TimeSpanHumanizeExtensions.Humanize(tsTempoRestante, 1, CultureInfo.GetCultureInfo("pt-br"));
                    pcbPastaAtualIcon.Visible = true;
                    lblPastaAtualTitulo.Visible = true;
                    lblPastaAtual.Visible = true;
                    lblPastaAtual.Text = statusDiretorio;
                    toolTip1.SetToolTip(lblPastaAtual, statusDiretorio);

                    pnlPauseAbortBackup.Visible = true;

                    lblTipoBackup.Visible = true;
                    lblTipoBackup.Text = "Backup\nFULL";

                    pnlNewBackup.Visible = false;

                    lblPercentBackup.Visible = true;
                    lblPercentBackup.Text = statusPorcentagem.ToString();
                    lblPercentBackup.ForeColor = Color.FromArgb(102, 255, 153);
                    lblPercentBackupEngUnit.Visible = true;
                    lblPercentBackupEngUnit.ForeColor = Color.FromArgb(102, 255, 153);

                    pgbBackup.Value = statusPorcentagem;
                    pgbBackup.ProgressColor = Color.FromArgb(102, 255, 153);

                    lblStatusBackup.Text = "RUNNING";
                    lblStatusBackup.ForeColor = Color.FromArgb(102, 255, 153);


                    lblLastBackupTitulo.Visible = false;
                    lblLastTipoBackup.Visible = false;
                    lblLastBackup.Visible = false;
                    lblLastBackupSizeTitulo.Visible = false;
                    lblLastBackupSize.Visible = false;

                    break;


                //BACKUP DIFERENCIAL
                case 2:

                    pcbTempoEstimado.Visible = true;
                    lblTempoEstimadoTitulo.Visible = true;
                    lblTempoEstimado.Visible = true;
                    lblTempoEstimado.Text = Humanizer.TimeSpanHumanizeExtensions.Humanize(tsTempoRestante, 1, CultureInfo.GetCultureInfo("pt-br"));
                    pcbPastaAtualIcon.Visible = true;
                    lblPastaAtualTitulo.Visible = true;
                    lblPastaAtual.Visible = true;
                    lblPastaAtual.Text = statusDiretorio;
                    pnlPauseAbortBackup.Visible = true;

                    lblTipoBackup.Visible = true;
                    lblTipoBackup.Text = "Backup\nDiferencial";

                    pnlNewBackup.Visible = false;

                    lblPercentBackup.Visible = true;
                    lblPercentBackup.Text = statusPorcentagem.ToString();
                    lblPercentBackup.ForeColor = Color.FromArgb(13, 140, 202);
                    lblPercentBackupEngUnit.Visible = true;
                    lblPercentBackupEngUnit.ForeColor = Color.FromArgb(13, 140, 202);

                    pgbBackup.Value = statusPorcentagem;
                    pgbBackup.ProgressColor = Color.FromArgb(13, 140, 202);

                    lblStatusBackup.Text = "RUNNING";
                    lblStatusBackup.ForeColor = Color.FromArgb(13, 140, 202);


                    lblLastBackupTitulo.Visible = false;
                    lblLastTipoBackup.Visible = false;
                    lblLastBackup.Visible = false;
                    lblLastBackupSizeTitulo.Visible = false;
                    lblLastBackupSize.Visible = false;

                    break;

                case 3:
                    pcbTempoEstimado.Visible = true;
                    lblTempoEstimadoTitulo.Visible = true;
                    lblTempoEstimado.Visible = false;
                    lblTempoEstimado.Text = Humanizer.TimeSpanHumanizeExtensions.Humanize(tsTempoRestante, 1, CultureInfo.GetCultureInfo("pt-br"));
                    pcbPastaAtualIcon.Visible = true;
                    lblPastaAtualTitulo.Visible = true;
                    lblPastaAtual.Visible = true;
                    lblPastaAtual.Text = statusDiretorio;
                    pnlPauseAbortBackup.Visible = true;

                    lblTipoBackup.Visible = true;
                    lblTipoBackup.Text = "Sincronização Espelho";

                    pnlNewBackup.Visible = false;

                    lblPercentBackup.Visible = true;
                    lblPercentBackup.Text = statusPorcentagem.ToString();
                    lblPercentBackup.ForeColor = Color.Gainsboro;
                    lblPercentBackupEngUnit.Visible = true;
                    lblPercentBackupEngUnit.ForeColor = Color.Gainsboro;

                    pgbBackup.Value = statusPorcentagem;
                    pgbBackup.ProgressColor = Color.Gainsboro;

                    lblStatusBackup.Text = "RUNNING";
                    lblStatusBackup.ForeColor = Color.Gainsboro;


                    lblLastBackupTitulo.Visible = false;
                    lblLastTipoBackup.Visible = false;
                    lblLastBackup.Visible = false;
                    lblLastBackupSizeTitulo.Visible = false;
                    lblLastBackupSize.Visible = false;

                    break;

                default:
                    break;
            }


            if (statusBackup > 0 && statusPaused)
            {
                lblPercentBackup.ForeColor = Color.FromArgb(255, 255, 163);
                lblPercentBackupEngUnit.ForeColor = Color.FromArgb(255, 255, 163);

                pgbBackup.ProgressColor = Color.FromArgb(255, 255, 163);

                lblStatusBackup.Text = "PAUSED";
                lblStatusBackup.ForeColor = Color.FromArgb(255, 255, 163);

                lblPauseBackupTitulo.Text = "  Play";

                if (statusBackup == 1)
                    btnPauseBackup.BackgroundImage = Resources.PlayBackupFull2;
                else
                    btnPauseBackup.BackgroundImage = Resources.PlayBackupDiferencial2;
            }
            else
            {
                lblPauseBackupTitulo.Text = "Pause";
                btnPauseBackup.BackgroundImage = Resources.PauseBackup;
            }
            pgbBackup.Refresh();
        }


        #endregion

        #region Agendamento
        private void btnOffBkpDif_Click(object sender, EventArgs e)
        {

            btnOffBkpDif.Visible = false;
            btnOnBkpDif.Visible = true;
            dtpBkDif.Enabled = true;

            configuration.BackupDiferencialHabilitado = true;

            DateTimePicker dtpBkFull = new DateTimePicker();
            Controls.AddRange(new Control[] { dtpBkFull });
            dtpBkFull.CalendarMonthBackground = Color.FromArgb(13, 13, 13);

            HabilitaSave();
        }
        private void btnOnBkpDif_Click(object sender, EventArgs e)
        {
            btnOffBkpDif.Visible = true;
            btnOnBkpDif.Visible = false;
            dtpBkDif.Enabled = false;

            configuration.BackupDiferencialHabilitado = false;

            DateTimePicker dtpBkFull = new DateTimePicker();
            Controls.AddRange(new Control[] { dtpBkFull });
            dtpBkFull.CalendarMonthBackground = Color.FromArgb(13, 13, 13);

            HabilitaSave();
        }
        private void txbdiasFull_TextChanged(object sender, EventArgs e)
        {
            string textOld = txbdiasFull.Text;
            string textNew = "";
            string caracter;
            string[] caracteresValidos = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            for (int i = 0; i < textOld.Length; i++)
            {
                caracter = textOld.Substring(i, 1);

                if (caracteresValidos.Contains(caracter))
                {
                    textNew += caracter;
                }
            }
            if (textNew == "" || textNew == "0")
            {
                textNew = "1";
            }
            txbdiasFull.Text = textNew;

            configuration.IntervaloBackupsFull = Convert.ToInt32(txbdiasFull.Text);

            HabilitaSave();
        }

        private void txbLimiteBkFull_TextChanged(object sender, EventArgs e)
        {
            string textOld = txbLimiteBkFull.Text;
            string textNew = "";
            string caracter;
            string[] caracteresValidos = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            for (int i = 0; i < textOld.Length; i++)
            {
                caracter = textOld.Substring(i, 1);

                if (caracteresValidos.Contains(caracter))
                {
                    textNew += caracter;
                }
            }
            if (textNew == "" || textNew == "0")
            {
                textNew = "1";
            }
            txbLimiteBkFull.Text = textNew;

            configuration.LimiteBackupsFull = Convert.ToInt32(txbLimiteBkFull.Text);

            HabilitaSave();
        }

        private void btnDesabilitadoBkpfull_Click(object sender, EventArgs e)
        {
            btnDesabilitadoBkpfull.Visible = false;
            btnHabilitadoBkpfull.Visible = true;
            dtpBkFull.Enabled = true;
            txbdiasFull.Enabled = true;
            txbLimiteBkFull.Enabled = true;
            btnOffBkpDif.Enabled = true;
            btnOnBkpDif.Enabled = true;
            dtpBkDif.Enabled = true;

            configuration.BackupFULLHabilitado = true;
            HabilitaSave();
            
        }

        private void btnHabilitadoBkpfull_Click(object sender, EventArgs e)
        {

            btnDesabilitadoBkpfull.Visible = true;
            btnHabilitadoBkpfull.Visible = false;
            dtpBkFull.Enabled = false;
            txbdiasFull.Enabled = false;
            txbLimiteBkFull.Enabled = false;
            btnOffBkpDif.Enabled = false;
            btnOffBkpDif.Visible = true;
            btnOnBkpDif.Visible = false;
            dtpBkDif.Enabled = false;

            configuration.BackupFULLHabilitado = false;
            HabilitaSave();
            
        }
        private void dtpBkDif_ValueChanged(object sender, EventArgs e)
        {
            configuration.HorarioDiferencial = dtpBkDif.Value.ToString("HH:mm");
            HabilitaSave();
        }


        private void dtpBkFull_ValueChanged(object sender, EventArgs e)
        {
            configuration.HorarioFull = dtpBkFull.Value.ToString("HH:mm");
            HabilitaSave();
        }



        #endregion

        #region Email

        private void btnSeePassword_Click(object sender, EventArgs e)
        {
            if (!vendoPassword)
            {
                btnSeePassword.Image = Resources.abreOlho;
                txtSenha.UseSystemPasswordChar = false;
                vendoPassword = true;

            }
            else
            {
                btnSeePassword.Image = Resources.fechaOlho;
                txtSenha.UseSystemPasswordChar = true;
                vendoPassword = false;
            }
        }
        private void txtOrigem_TextChanged(object sender, EventArgs e)
        {
            configuration.EmailOrigem = txtOrigem.Text;
            HabilitaSave();

        }

        private void txtSenha_TextChanged(object sender, EventArgs e)
        {
            configuration.SenhaOrigem = encrypt.Encrypto(txtSenha.Text);
            HabilitaSave();
        }

        private void btn_addemail_Click(object sender, EventArgs e)
        {

            string emailAux = Interaction.InputBox("Informe o email desejado:", "Email de Destino", "");

            if (emailAux.Trim().Length > 0)
            {
                ltvEmail.Items.Add(emailAux);

                string[] listaEmail = new string[ltvEmail.Items.Count];

                for (int i = 0; i < ltvEmail.Items.Count; i++)
                {
                    listaEmail[i] = ltvEmail.Items[i].Text;
                }
                email.Destinos = listaEmail;
            }
            HabilitaSave();
        }

        private void bnt_excluiremail_Click(object sender, EventArgs e)
        {
            if (ltvEmail.SelectedItems.Count > 0)
            {
                int emailSelecionadoIndex = ltvEmail.SelectedItems[0].Index;
                string emailSelecionado = ltvEmail.SelectedItems[0].Text;

                DialogResult botaoAcionado = MessageBox.Show("Deseja mesmo excluir o email: " + emailSelecionado + " ?", "Excluir Email", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (botaoAcionado == DialogResult.OK)
                {
                    ltvEmail.Items.RemoveAt(emailSelecionadoIndex);

                    string[] listaEmail = new string[ltvEmail.Items.Count];

                    for (int i = 0; i < ltvEmail.Items.Count; i++)
                    {
                        listaEmail[i] = ltvEmail.Items[i].Text;
                    }
                    email.Destinos = listaEmail;

                    bnt_excluiremail.Enabled = false;
                }
            }
            HabilitaSave();
        }
        private void ltvEmail_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ltvEmail.SelectedItems.Count == 0)
                bnt_excluiremail.Enabled = false;
            else
                bnt_excluiremail.Enabled = true;

            HabilitaSave();
        }

        private void btnTesteEmail_Click(object sender, EventArgs e)
        {
            if (NotificacaoEmail())
            {
                MessageBox.Show("Email conectado com sucesso", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Email não conectado", "Falha", MessageBoxButtons.OK, MessageBoxIcon.Error);



        }

        private bool NotificacaoEmail()
        {
            bool validaEmail = false;
            try
            {

                email.SmtpServerString = "smtp.gmail.com";
                email.Origem = configuration.EmailOrigem;
                email.Password = encrypt.Decrypto(configuration.SenhaOrigem);
                email.Assunto = "Backup Drive Servidor: teste ";

                email.Anexos = new Attachment[0];
                email.CorpoEmail = "Backup Foi Inicializado!" + Environment.NewLine;

                email.SendEmail();

                validaEmail = email.SendEmail();
            }
            catch (Exception ex)
            {
                log.LogError("Falha não envio do email.",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }
            return validaEmail;


        }

        #endregion

        #region Diretórios

        private void btn_offEspelho_Click(object sender, EventArgs e)
        {
            btn_offEspelho.Visible = false;
            btn_onEspelho.Visible = true;
            configuration.HabilitaPastaEspelho = true;
            txbPastaEspelho.Enabled = true;
            btn_searchEspelho.Enabled = true;

            HabilitaSave();
        }
        private void btn_onEspelho_Click_1(object sender, EventArgs e)
        {
            btn_offEspelho.Visible = true;
            btn_onEspelho.Visible = false;
            configuration.HabilitaPastaEspelho = false;
            txbPastaEspelho.Enabled = false;
            btn_searchEspelho.Enabled = false;

            HabilitaSave();
        }



        #region Selecao Pastas

        private void btn_searchOrigem_Click(object sender, EventArgs e)
        {
            configuration.PastaDrive = SearchPathFolder(configuration.PastaDrive);

            txbDrive.Text = configuration.PastaDrive;

            HabilitaSave();
        }
        private void btn_searchDestino_Click(object sender, EventArgs e)
        {
            configuration.PastaBackup = SearchPathFolder(configuration.PastaBackup);

            txbPastaDestino.Text = configuration.PastaBackup;

            HabilitaSave();
        }
        private void btn_searchEspelho_Click(object sender, EventArgs e)
        {
            configuration.PastaEspelho = SearchPathFolder(configuration.PastaEspelho);

            txbPastaEspelho.Text = configuration.PastaEspelho;

            HabilitaSave();
        }

        private string SearchPathFolder(string pastaAtual)
        {
            string diretorio = "";

            //Instancias para tela de escolha de psta
            DialogResult result = DialogResult.None;
            FolderBrowserDialog FD = new FolderBrowserDialog();

            try
            {
                //inicia com ultimo diretorio utilizado
                FD.SelectedPath = pastaAtual;

                //abre tela de confirmação do atual diretorio ou para escolha de um novo.
                //Retorna se pressionado OK ou Cancel
                result = FD.ShowDialog();

                //Caso não seja selecionado OK na escolha do diretorio, então anula diretorio. Senão atualiza atual diretorio
                if (result != DialogResult.OK)
                    diretorio = pastaAtual;
                else
                {
                    diretorio = FD.SelectedPath;
                }


            }
            catch (Exception ex)
            {
                diretorio = pastaAtual;
                log.LogError("Erro na escolha do diretório de Backup",
                                    MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().ToString(),
                                            ex.ToString());
            }

            return diretorio;
        }

        #endregion

        #region Exclusão de diretórios
        private void btn_adicionarPasta_Click(object sender, EventArgs e)
        {
            string pastaSelecionada = SearchPathFolder(configuration.PastaDrive);

            if (pastaSelecionada.Trim().Length > 0)
            {
                ltvExclusao.Items.Add(pastaSelecionada);

                configuration.PastasRestritas.Add(pastaSelecionada);
            }

            HabilitaSave();
        }


        private void btn_removerPasta_Click(object sender, EventArgs e)
        {
            if (ltvExclusao.SelectedItems.Count > 0)
            {
                int FolderSelecionadoIndex = ltvExclusao.SelectedItems[0].Index;
                string FolderSelecionado = ltvExclusao.SelectedItems[0].Text;

                DialogResult botaoAcionado = MessageBox.Show("Deseja mesmo retirar da lista de exclusão o diretório:  " + FolderSelecionado + " ?", "Retirar diretório", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (botaoAcionado == DialogResult.OK)
                {
                    ltvExclusao.Items.RemoveAt(FolderSelecionadoIndex);

                    configuration.PastasRestritas.RemoveAt(FolderSelecionadoIndex);
                    btn_removerPasta.Enabled = false;
                }
            }
            HabilitaSave();
        }

        private void ltvExclusao_SelectedIndexChanged(object sender, EventArgs e)
        {
            {
                if (ltvExclusao.SelectedItems.Count == 0)
                    btn_removerPasta.Enabled = false;
                else
                    btn_removerPasta.Enabled = true;
            }
            HabilitaSave();
        }
        #endregion
        #endregion

        #region Botao Circular
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    GraphicsPath forma = new GraphicsPath();
        //    forma.AddEllipse(0, 0, pictureBox8.Width, pictureBox8.Height);
        //    pictureBox8.Region = new Region(forma);
        //}
        #endregion

        #region Tela Inicial

        private void atualizaIconLoading(bool link)
        {
            if (linkOld != link)
            {
                linkOld = link;

                {

                }
                if (link)
                {
                    pcbLoading.Image = Resources.loading;
                    pcbLoading.Size = new Size(47, 32);
                    pcbLoading.Location = new Point(428, 28);
                }
                else
                {
                    pcbLoading.Image = Resources.linkError;
                    pcbLoading.Size = new Size(60, 50);
                    pcbLoading.Location = new Point(442, 18);
                }
            }

        }


        private void btnPauseBackup_Click_1(object sender, EventArgs e)
        {
            AssyncTCPClient("Comando;Pause");
        }

        private void btnAbortBackup_Click(object sender, EventArgs e)
        {
            //AssyncTCPClient("Comando; Aborts");
            ServiceController service = new ServiceController(serviceName, machineName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutCommandService);
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);


            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
        }

        private void btnNewBackupDiferencial_Click(object sender, EventArgs e)
        {
            AssyncTCPClient("Comando;BkpDiferencial");
        }

        private void btnNewBackupFull_Click(object sender, EventArgs e)
        {
            AssyncTCPClient("Comando;BkpFull");
        }

        private void btnNewBackupEspelho_Click(object sender, EventArgs e)
        {
            AssyncTCPClient("Comando;SyncEspelho");
        }

        #endregion

        #region TCP
        public string TcpClient(string messageEnvio)
        {

            String mensagemResposta = String.Empty;

            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.                
                TcpClient client = new TcpClient(server, port);


                // Translate the passed message into ASCII and store it as a Byte array.


                Byte[] data = Encoding.UTF8.GetBytes(messageEnvio);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                mensagemResposta = Encoding.UTF8.GetString(data, 0, bytes);



                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                mensagemResposta = "Erro 404";
                log.LogError("Erro na comunicação TCP",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }

            return mensagemResposta;

        }
        private void AssyncTCPClient(string messageEnvio)
        {
            logTCPClient.LogInfo("isAlive = " + isAlive.ToString());


            if (!isAlive)
            {
                isAlive = true;

                messageEnvioGlobal = messageEnvio;

                //Inicia objeto assincrono UNICO para roda apenas uma vez
                bwTCPIP = new BackgroundWorker();

                ////Declara auxiliares para o processo assincrono
                //bwConfigSQL.ProgressChanged +=
                //    new ProgressChangedEventHandler(bwConfigSQL_ProgressChanged);

                ////habilita propriedade de acompanhar progressão do processo
                //bwConfigSQL.WorkerReportsProgress = true;

                bwTCPIP.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(bwTCPIP_RunWorkerCompleted);

                //inicia processo assincrono
                bwTCPIP.DoWork += (Senderbw, args) =>
                {
                    try
                    {
                        var task = Task.Run(() => messageRespostaGlobal = TcpClient(messageEnvioGlobal));

                        if (!task.Wait(TimeSpan.FromSeconds(10)))
                            log.LogError("Timeout na consulta TCP IP.",
                                MethodBase.GetCurrentMethod().DeclaringType.Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        new Exception("Timed out").Message);

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
                bwTCPIP.RunWorkerAsync();

            }
        }

        private void bwTCPIP_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isAlive = false;

            bwTCPIP.Dispose();            

            try
            {
                logTCPClient.LogInfo("messageRespostaGlobal = " + messageRespostaGlobal);
                logTCPClient.LogInfo("messageEnvioGlobal = " + messageEnvioGlobal);

                string[] respostaArray = null;

                if (messageEnvioGlobal == "Status")
                {
                    respostaArray = messageRespostaGlobal.Split(';');

                    if (respostaArray[0] == "OK")
                    {
                        statusBackup = Convert.ToInt32(respostaArray[1]);

                        if (statusBackup > 0)
                        {
                            statusPaused = Convert.ToBoolean(respostaArray[2]);
                            statusPorcentagem = Convert.ToInt32(respostaArray[3]);
                            statusDiretorio = Convert.ToString(respostaArray[4]);
                            statusTempoRestante = Convert.ToInt32(respostaArray[5]);

                        }
                        AtualizaStatusBackup();
                    }

                    atualizaIconLoading(respostaArray[0] == "OK" ? true : false);
                }
                else
                {
                    respostaArray = messageRespostaGlobal.Split(';');

                    atualizaIconLoading(respostaArray[0] == "OK" ? true : false);
                }

            }
            catch (Exception ex)
            {
                logTCPClient.LogError("Erro na comunicação TCP Client",
                               MethodBase.GetCurrentMethod().Name,
                                   MethodBase.GetCurrentMethod().ToString(),
                                       ex.Message);
            }

            messageRespostaGlobal = String.Empty;
        }
        #endregion

        #region Timer
        private void TimerBackup_Tick(object sender, EventArgs e)
        {
            timerBackup.Stop();


            AssyncTCPClient("Status");


            ConfiguracaoServiçoWindows();


            CheckConfiguration();


            timerBackup.Start();
        }





        #endregion

        #region Botão Save
        private void btn_save_Click(object sender, EventArgs e)
        {
            configuration.SalvaConfiguracao();
            configuration.SalvaConfiguracoesEmail(email.Destinos, pathEmails);
            SaveDesabilitado();
        }

        private void HabilitaSave()
        {
            btn_save.BackgroundImage = Resources._2305609_32;
            btn_save.Enabled = true;
        }

        private void SaveDesabilitado()
        {
            btn_save.BackgroundImage = Resources.savecinza;
            btn_save.Enabled = false;
        }

        #endregion

        #region Check Configuração
        private void CheckConfiguration()
        {
            ptbCheckAgendamento.Visible = false;
            ptbCheckDiretorio.Visible = false;
            ptbCheckEmail.Visible = false;

            //agendamento
            if (configuration.BackupFULLHabilitado && configuration.LimiteBackupsFull != 0 && configuration.IntervaloBackupsFull != 0)
            {
                ptbCheckAgendamento.Visible = true;

            }

            //email
            if (txtOrigem.Text != "" && txtSenha.Text != "" && ltvEmail.Items.Count > 0)
            {
                ptbCheckEmail.Visible = true;

            }

            //diretorios
            if (txbDrive.Text != "" && txbPastaDestino.Text != "")
            {
                ptbCheckDiretorio.Visible = true;
            }


        }


        #endregion

        #region Serviço Windows 

        private void ConfiguracaoServiçoWindows()
        {

            try
            {
                ServiceController[] services = ServiceController.GetServices(machineName);
                var service = services.FirstOrDefault(s => s.ServiceName == serviceName);
                
                if (service != null)
                {

                    switch (service.Status)
                    {


                        case ServiceControllerStatus.Running:
                            ptbFuncionando.Visible = true;
                            ptbLoading.Visible = false;
                            ptbDesligado.Visible = false;
                            ptbAviso.Visible = false;

                            ptbCheckService.Image = Resources._3209280_24__1_;

                            btn_replayServico.Enabled = true;
                            btn_playServico.Enabled = false;
                            btn_stopServico.Enabled = true;

                            btn_playServico.BackgroundImage = Resources.play_cinza;
                            btn_replayServico.BackgroundImage = Resources.restart;
                            btn_stopServico.BackgroundImage = Resources.stop;



                            break;

                        case ServiceControllerStatus.Stopped:
                            ptbFuncionando.Visible = false;
                            ptbDesligado.Visible = true;
                            ptbAviso.Visible = false;
                            ptbLoading.Visible = false;

                            ptbCheckService.Image = Resources._3209280_24;

                            btn_replayServico.Enabled = false;
                            btn_playServico.Enabled = true;
                            btn_stopServico.Enabled = false;

                            btn_replayServico.BackgroundImage = Resources.restart_cinza;
                            btn_stopServico.BackgroundImage = Resources.stop_cinza;
                            btn_playServico.BackgroundImage = Resources.play;

                            break;


                        case ServiceControllerStatus.Paused:
                            ptbFuncionando.Visible = false;
                            ptbDesligado.Visible = true;
                            ptbAviso.Visible = false;
                            ptbLoading.Visible = false;

                            btn_replayServico.Enabled = false;
                            btn_playServico.Enabled = true;
                            btn_stopServico.Enabled = false;

                            ptbCheckService.Image = Resources._3209280_24;


                            btn_replayServico.BackgroundImage = Resources.restart_cinza;
                            btn_stopServico.BackgroundImage = Resources.stop_cinza;
                            btn_playServico.BackgroundImage = Resources.play;


                            break;

                        case ServiceControllerStatus.StopPending:

                            ptbLoading.Visible = true;
                            ptbAviso.Visible = false;
                            ptbDesligado.Visible = false;
                            ptbFuncionando.Visible = false;

                            btn_replayServico.Enabled = true;
                            btn_playServico.Enabled = false;
                            btn_stopServico.Enabled = false;

                            ptbCheckService.Image = Resources.loading_service;

                            btn_playServico.BackgroundImage = Resources.play_cinza;
                            btn_stopServico.BackgroundImage = Resources.stop_cinza;
                            btn_replayServico.BackgroundImage = Resources.restart;

                            break;

                        case ServiceControllerStatus.StartPending:
                            ptbLoading.Visible = true;
                            ptbAviso.Visible = false;
                            ptbDesligado.Visible = false;
                            ptbFuncionando.Visible = false;

                            btn_replayServico.Enabled = true;
                            btn_stopServico.Enabled = true;
                            btn_playServico.Enabled = false;

                            ptbCheckService.Image = Resources.loading_service;

                            btn_playServico.BackgroundImage = Resources.play_cinza;
                            btn_replayServico.BackgroundImage = Resources.restart;
                            btn_stopServico.BackgroundImage = Resources.stop;

                            break;

                        default:
                            ptbDesligado.Visible = false;
                            ptbFuncionando.Visible = false;
                            ptbAviso.Visible = false;

                            btn_playServico.Enabled = false;
                            btn_replayServico.Enabled = false;
                            btn_stopServico.Enabled = false;

                            ptbCheckService.Image = Resources.loading_service;

                            btn_playServico.BackgroundImage = Resources.play_cinza;
                            btn_stopServico.BackgroundImage = Resources.stop_cinza;
                            btn_replayServico.BackgroundImage = Resources.restart_cinza;

                            break;
                    }

                    ptbAviso.Visible = false;
                    lblServicoNI.Visible = false;
                }
                else
                {

                    ptbDesligado.Visible = false;
                    ptbFuncionando.Visible = false;

                    ptbAviso.Visible = true;
                    lblServicoNI.Visible = true;

                    ptbCheckService.Image = Resources._4075935_24;

                    btn_playServico.Enabled = false;
                    btn_replayServico.Enabled = false;
                    btn_stopServico.Enabled = false;

                    btn_playServico.BackgroundImage = Resources.play_cinza;
                    btn_stopServico.BackgroundImage = Resources.stop_cinza;
                    btn_replayServico.BackgroundImage = Resources.restart_cinza;

                }
            }
            catch (Exception ex)
            {
                ptbDesligado.Visible = false;
                ptbFuncionando.Visible = false;

                ptbAviso.Visible = true;
                lblServicoNI.Visible = true;
            }


        }

        private void ComandosService()
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);

                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutCommandService);


                if (comando == comandoSTART)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
                else if (comando == comandoSTOP)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
                else
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);


                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }


            }
            catch (Exception ex)
            {
                log.LogError("Comando de " + comando + " Negado para o Serviço: " + serviceName + " - na Máquina: " + machineName,
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);

                MessageBox.Show("Comando de " + comando + " Negado para o Serviço: " + serviceName + " - na Máquina: " + machineName, "Falha", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AssyncCommandService()
        {
            if (!isAlive)
            {
                try
                {
                    isAlive = true;


                    //Inicia objeto assincrono UNICO para rodas apenas uma vez
                    bwService = new BackgroundWorker();

                    ////Declara auxiliares para o processo assincrono
                    //bwConfigSQL.ProgressChanged +=
                    //    new ProgressChangedEventHandler(bwConfigSQL_ProgressChanged);

                    ////habilita propriedade de acompanhar progressão do processo
                    //bwConfigSQL.WorkerReportsProgress = true;

                    bwService.RunWorkerCompleted +=
                        new RunWorkerCompletedEventHandler(bwService_RunWorkerCompleted);

                    //inicia processo assincrono
                    bwService.DoWork += (Senderbw, args) =>
                    {
                        try
                        {
                            ComandosService();
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
                    bwService.RunWorkerAsync();

                }
                catch (Exception e)
                {
                    log.LogError("Erro nos comandos de serviço",
                        MethodBase.GetCurrentMethod().Name,
                                        MethodBase.GetCurrentMethod().ToString(),
                                            e.Message);
                }
            }
        }

        private void bwService_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bwService.Dispose();

            isAlive = false;

        }


        private void btn_stopServico_Click(object sender, EventArgs e)
        {
            comando = comandoSTOP;
            AssyncCommandService();

        }

        private void btn_replayServico_Click(object sender, EventArgs e)
        {
            comando = comandoRESTART;
            AssyncCommandService();
        }

        private void btn_playServico_Click(object sender, EventArgs e)
        {
            comando = comandoSTART;
            AssyncCommandService();
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txbPastaEspelho_TextChanged(object sender, EventArgs e)
        {

        }

    }
    #endregion
}
