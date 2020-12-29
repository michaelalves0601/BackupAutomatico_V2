using BackupNuvemSBuild_Configuration.Properties;
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

        bool vendoPassword = false;

        Configuration configuration = new Configuration();
        Email email = new Email();
        Log log = new Log();

        string pathConfiguration = @"Config/Configuration.ini";


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
                    break;
                case 2:
                    btn_email.BackColor = Color.Black;
                    btn_email.ForeColor = Color.White;
                    btn_email.BackgroundImage = Resources.emailmano;
                    pnlEmail.Visible = true;
                    btn_email.FlatAppearance.BorderSize = 0;
                    pnlEmail2.Visible = true;
                    break;
                case 3:
                    btn_pastas.BackColor = Color.Black;
                    btn_pastas.ForeColor = Color.White;
                    btn_pastas.BackgroundImage = Resources.folder_o;
                    pnlDiretorios.Visible = true;
                    btn_pastas.FlatAppearance.BorderSize = 0;
                    pnlDiretorios2.Visible = true;
                    break;
                case 4:
                    btn_service.BackColor = Color.Black;
                    btn_service.ForeColor = Color.White;
                    btn_service.BackgroundImage = Resources.serviço_on;
                    pnlServico.Visible = true;
                    btn_service.FlatAppearance.BorderSize = 0;
                    pnlServico2.Visible = true;
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
            this.Close();
            Environment.Exit(0);
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        #endregion


        #region Mover Tela
        private bool mouseIsDown = false;
        private Point firstPoint;

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


        private void FormHome_Load(object sender, EventArgs e)
        {
            AtualizaStatusBackup();

            TcpClient("Status");

            timerBackup.Tick += TimerBackup_Tick;
            timerBackup.Enabled = true;
            timerBackup.Interval = 1000;
            timerBackup.Start();

            configuration.RestauraConfiguracao(pathConfiguration);

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
            //ltvExclusao.Items = configuration. Listview ainda para ser colocada.

            //Restauração do menu Email
            txtOrigem.Text = configuration.EmailOrigem;
            txtSenha.Text = configuration.SenhaOrigem; //adicionar criptografia à senha


            email.RetornaEmails();
            foreach (var destino in email.Destinos)
                ltvEmail.Items.Add(destino);




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

            int horaBackupDif = Convert.ToInt32(configuration.HorarioDiferencial.Substring(0, 2));
            int minBackupDif = Convert.ToInt32(configuration.HorarioDiferencial.Substring(3, 2));
            dtpBkDif.Value = new DateTime(1970, 1, 1, horaBackupDif, minBackupDif, 0);

            int horaBackupFull = Convert.ToInt32(configuration.HorarioFull.Substring(0, 2));
            int minBackupFull = Convert.ToInt32(configuration.HorarioFull.Substring(3, 2));
            dtpBkFull.Value = new DateTime(1970, 1, 1, horaBackupFull, minBackupFull, 0);

            txbdiasFull.Text = configuration.BackupsFull.ToString();


        }


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
                    lblTempoEstimado.Visible = true;
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
        }
        private void btnOnBkpDif_Click(object sender, EventArgs e)
        {
            btnOffBkpDif.Visible = true;
            btnOnBkpDif.Visible = false;
            dtpBkDif.Enabled = false;

            configuration.BackupDiferencialHabilitado = false;
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
        }

        private void dtpBkDif_ValueChanged(object sender, EventArgs e)
        {
            configuration.HorarioDiferencial = dtpBkDif.Value.ToString("HH:mm");
        }

        private void txbdiasFull_TextChanged_1(object sender, EventArgs e)
        {
            configuration.BackupsFull = Convert.ToInt32(txbdiasFull.Text);
        }

        private void dtpBkFull_ValueChanged(object sender, EventArgs e)
        {
            configuration.HorarioFull = dtpBkFull.Value.ToString("HH:mm");
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
        #endregion


        #region Diretórios

        private void btn_offEspelho_Click(object sender, EventArgs e)
        {
            btn_offEspelho.Visible = false;
            btn_onEspelho.Visible = true;
            txbPastaEspelho.Enabled = true;
            btn_searchEspelho.Enabled = true;
        }
        private void btn_onEspelho_Click_1(object sender, EventArgs e)
        {
            btn_offEspelho.Visible = true;
            btn_onEspelho.Visible = false;
            txbPastaEspelho.Enabled = false;
            btn_searchEspelho.Enabled = false;
        }



        #region Selecao Pastas

        private void btn_searchOrigem_Click(object sender, EventArgs e)
        {
            configuration.PastaDrive = SearchPathFolder(configuration.PastaDrive);

            txbDrive.Text = configuration.PastaDrive;
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


        private void btnPauseBackup_Click_1(object sender, EventArgs e)
        {
            string messageResposta = TcpClient("Comando;Pause");

            string[] respostaArray = messageResposta.Split(';');

            atualizaIconLoading(respostaArray[0] == "OK" ? true : false);

        }

        private void btnAbortBackup_Click(object sender, EventArgs e)
        {
            string messageResposta = TcpClient("Comando;Abort");

            string[] respostaArray = messageResposta.Split(';');

            atualizaIconLoading(respostaArray[0] == "OK" ? true : false);

        }

        private void btnNewBackupDiferencial_Click(object sender, EventArgs e)
        {
            string messageResposta = TcpClient("Comando;BkpDiferencial");

            string[] respostaArray = messageResposta.Split(';');

            atualizaIconLoading(respostaArray[0] == "OK" ? true : false);


        }

        private void btnNewBackupFull_Click(object sender, EventArgs e)
        {
            string messageResposta = TcpClient("Comando;BkpFull");

            string[] respostaArray = messageResposta.Split(';');

            atualizaIconLoading(respostaArray[0] == "OK" ? true : false);

        }

        private void btnNewBackupEspelho_Click(object sender, EventArgs e)
        {
            string messageResposta = TcpClient("Comando;SyncEspelho");

            string[] respostaArray = messageResposta.Split(';');

            atualizaIconLoading(respostaArray[0] == "OK" ? true : false);
        }

        #endregion

        #region TCP
        public string TcpClient(string messageEnvio)
        {
            if (!isAlive)
            {
                String mensagemResposta = String.Empty;
                isAlive = true;
                try
                {
                    // Create a TcpClient.
                    // Note, for this client to work you need to have a TcpServer 
                    // connected to the same address as specified by the server, port
                    // combination.                
                    TcpClient client = new TcpClient(server, port);

                    // Translate the passed message into ASCII and store it as a Byte array.


                    Byte[] data = Encoding.ASCII.GetBytes(messageEnvio);

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
                    mensagemResposta = Encoding.ASCII.GetString(data, 0, bytes);



                    // Close everything.
                    stream.Close();
                    client.Close();
                }
                catch (Exception ex)
                {
                    mensagemResposta = "Erro 404";
                }
                isAlive = false;
                return mensagemResposta;
            }
            else
                return "Busy";

        }
        #endregion

        #region Timer
        private void TimerBackup_Tick(object sender, EventArgs e)
        {
            timerBackup.Stop();

            string messageResposta = TcpClient("Status");

            string[] respostaArray = messageResposta.Split(';');

            if (respostaArray[0] == "OK")
            {
                statusBackup = Convert.ToInt32(respostaArray[1]);
                statusPaused = Convert.ToBoolean(respostaArray[2]);
                statusPorcentagem = Convert.ToInt32(respostaArray[3]);
                statusDiretorio = Convert.ToString(respostaArray[4]);
                statusTempoRestante = Convert.ToInt32(respostaArray[5]);

                //terminar porcentagem [3], diretório [4] e tempo restante [5]
                AtualizaStatusBackup();
            }

            atualizaIconLoading(respostaArray[0] == "OK" ? true : false);

            timerBackup.Start();
        }


        #endregion

        private void txtOrigem_TextChanged(object sender, EventArgs e)
        {
            configuration.EmailOrigem = txtOrigem.Text;
        }

        private void txtSenha_TextChanged(object sender, EventArgs e)
        {
            configuration.SenhaOrigem = txtSenha.Text;
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
                
            
        }

        private void ltvEmail_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ltvEmail.SelectedItems.Count == 0)
                bnt_excluiremail.Enabled = false;
            else
            bnt_excluiremail.Enabled = true;
        }
    }


}
