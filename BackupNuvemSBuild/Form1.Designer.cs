namespace BackupNuvemSBuild
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.dtpData = new System.Windows.Forms.DateTimePicker();
            this.txbPastaBackup = new System.Windows.Forms.TextBox();
            this.btnSearchPath = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBackup = new System.Windows.Forms.Button();
            this.backupIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.txbPastaDrive = new System.Windows.Forms.TextBox();
            this.cklPastasDrive = new System.Windows.Forms.CheckedListBox();
            this.pgbSubPasta = new System.Windows.Forms.ProgressBar();
            this.pgbTotal = new System.Windows.Forms.ProgressBar();
            this.txtPercentSubPasta = new System.Windows.Forms.Label();
            this.txtPercentTotal = new System.Windows.Forms.Label();
            this.txtRestantes = new System.Windows.Forms.Label();
            this.txtArquivos = new System.Windows.Forms.Label();
            this.txtPasta = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtEtapa = new System.Windows.Forms.Label();
            this.txtLimiteDias = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtLimiteSemanais = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtLimiteMensais = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.ckbBackupAutomatico = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnSearchEspelho = new System.Windows.Forms.Button();
            this.txbPastaSync = new System.Windows.Forms.TextBox();
            this.btnSync = new System.Windows.Forms.Button();
            this.ckbSync = new System.Windows.Forms.CheckBox();
            this.txtVersao = new System.Windows.Forms.Label();
            this.btnSearchDrive = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dtpData
            // 
            this.dtpData.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpData.Location = new System.Drawing.Point(216, 63);
            this.dtpData.Name = "dtpData";
            this.dtpData.Size = new System.Drawing.Size(97, 20);
            this.dtpData.TabIndex = 0;
            this.dtpData.Visible = false;
            // 
            // txbPastaBackup
            // 
            this.txbPastaBackup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txbPastaBackup.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txbPastaBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbPastaBackup.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txbPastaBackup.Location = new System.Drawing.Point(368, 68);
            this.txbPastaBackup.Name = "txbPastaBackup";
            this.txbPastaBackup.ReadOnly = true;
            this.txbPastaBackup.Size = new System.Drawing.Size(394, 21);
            this.txbPastaBackup.TabIndex = 1;
            // 
            // btnSearchPath
            // 
            this.btnSearchPath.BackColor = System.Drawing.Color.Transparent;
            this.btnSearchPath.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSearchPath.BackgroundImage")));
            this.btnSearchPath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSearchPath.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearchPath.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSearchPath.Location = new System.Drawing.Point(760, 66);
            this.btnSearchPath.Name = "btnSearchPath";
            this.btnSearchPath.Size = new System.Drawing.Size(29, 25);
            this.btnSearchPath.TabIndex = 2;
            this.btnSearchPath.Text = "...";
            this.btnSearchPath.UseVisualStyleBackColor = false;
            this.btnSearchPath.Click += new System.EventHandler(this.btnSearchPath_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.Silver;
            this.label1.Location = new System.Drawing.Point(348, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Backup:";
            // 
            // btnBackup
            // 
            this.btnBackup.BackColor = System.Drawing.Color.Transparent;
            this.btnBackup.BackgroundImage = global::BackupNuvemSBuild_Configuration.Properties.Resources.iconfinder_document_save_118916;
            this.btnBackup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnBackup.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBackup.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnBackup.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnBackup.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBackup.Location = new System.Drawing.Point(276, 21);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(37, 36);
            this.btnBackup.TabIndex = 4;
            this.btnBackup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip1.SetToolTip(this.btnBackup, "Force Backup");
            this.btnBackup.UseVisualStyleBackColor = false;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // backupIcon
            // 
            this.backupIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("backupIcon.Icon")));
            this.backupIcon.Text = "Backup SBuild";
            this.backupIcon.Visible = true;
            this.backupIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.backupIcon_MouseClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.Silver;
            this.label2.Location = new System.Drawing.Point(348, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Drive:";
            // 
            // txbPastaDrive
            // 
            this.txbPastaDrive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txbPastaDrive.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txbPastaDrive.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbPastaDrive.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txbPastaDrive.Location = new System.Drawing.Point(368, 23);
            this.txbPastaDrive.Name = "txbPastaDrive";
            this.txbPastaDrive.ReadOnly = true;
            this.txbPastaDrive.Size = new System.Drawing.Size(394, 21);
            this.txbPastaDrive.TabIndex = 5;
            // 
            // cklPastasDrive
            // 
            this.cklPastasDrive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.cklPastasDrive.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cklPastasDrive.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cklPastasDrive.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.cklPastasDrive.FormattingEnabled = true;
            this.cklPastasDrive.Location = new System.Drawing.Point(352, 163);
            this.cklPastasDrive.Name = "cklPastasDrive";
            this.cklPastasDrive.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.cklPastasDrive.Size = new System.Drawing.Size(436, 144);
            this.cklPastasDrive.TabIndex = 8;
            // 
            // pgbSubPasta
            // 
            this.pgbSubPasta.Location = new System.Drawing.Point(13, 13);
            this.pgbSubPasta.Name = "pgbSubPasta";
            this.pgbSubPasta.Size = new System.Drawing.Size(227, 18);
            this.pgbSubPasta.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pgbSubPasta.TabIndex = 9;
            // 
            // pgbTotal
            // 
            this.pgbTotal.Location = new System.Drawing.Point(13, 126);
            this.pgbTotal.Name = "pgbTotal";
            this.pgbTotal.Size = new System.Drawing.Size(346, 23);
            this.pgbTotal.TabIndex = 10;
            // 
            // txtPercentSubPasta
            // 
            this.txtPercentSubPasta.AutoSize = true;
            this.txtPercentSubPasta.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPercentSubPasta.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtPercentSubPasta.Location = new System.Drawing.Point(242, 11);
            this.txtPercentSubPasta.Name = "txtPercentSubPasta";
            this.txtPercentSubPasta.Size = new System.Drawing.Size(54, 20);
            this.txtPercentSubPasta.TabIndex = 11;
            this.txtPercentSubPasta.Text = "##.#%";
            this.txtPercentSubPasta.Visible = false;
            // 
            // txtPercentTotal
            // 
            this.txtPercentTotal.AutoSize = true;
            this.txtPercentTotal.BackColor = System.Drawing.Color.Transparent;
            this.txtPercentTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPercentTotal.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtPercentTotal.Location = new System.Drawing.Point(365, 126);
            this.txtPercentTotal.Name = "txtPercentTotal";
            this.txtPercentTotal.Size = new System.Drawing.Size(54, 20);
            this.txtPercentTotal.TabIndex = 12;
            this.txtPercentTotal.Text = "##.#%";
            this.txtPercentTotal.Visible = false;
            // 
            // txtRestantes
            // 
            this.txtRestantes.AutoSize = true;
            this.txtRestantes.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.txtRestantes.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtRestantes.Location = new System.Drawing.Point(85, 73);
            this.txtRestantes.Name = "txtRestantes";
            this.txtRestantes.Size = new System.Drawing.Size(62, 17);
            this.txtRestantes.TabIndex = 17;
            this.txtRestantes.Text = "######";
            this.txtRestantes.Visible = false;
            // 
            // txtArquivos
            // 
            this.txtArquivos.AutoSize = true;
            this.txtArquivos.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.txtArquivos.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtArquivos.Location = new System.Drawing.Point(85, 56);
            this.txtArquivos.Name = "txtArquivos";
            this.txtArquivos.Size = new System.Drawing.Size(62, 17);
            this.txtArquivos.TabIndex = 16;
            this.txtArquivos.Text = "######";
            this.txtArquivos.Visible = false;
            // 
            // txtPasta
            // 
            this.txtPasta.AutoSize = true;
            this.txtPasta.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.txtPasta.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtPasta.Location = new System.Drawing.Point(85, 39);
            this.txtPasta.Name = "txtPasta";
            this.txtPasta.Size = new System.Drawing.Size(62, 17);
            this.txtPasta.TabIndex = 15;
            this.txtPasta.Text = "######";
            this.txtPasta.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label5.ForeColor = System.Drawing.Color.Silver;
            this.label5.Location = new System.Drawing.Point(10, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 17);
            this.label5.TabIndex = 14;
            this.label5.Text = "Restantes:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label4.ForeColor = System.Drawing.Color.Silver;
            this.label4.Location = new System.Drawing.Point(9, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 17);
            this.label4.TabIndex = 13;
            this.label4.Text = "Arquivos:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label3.ForeColor = System.Drawing.Color.Silver;
            this.label3.Location = new System.Drawing.Point(9, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Pasta:";
            // 
            // txtEtapa
            // 
            this.txtEtapa.AutoSize = true;
            this.txtEtapa.BackColor = System.Drawing.Color.Transparent;
            this.txtEtapa.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.txtEtapa.ForeColor = System.Drawing.Color.Silver;
            this.txtEtapa.Location = new System.Drawing.Point(10, 106);
            this.txtEtapa.Name = "txtEtapa";
            this.txtEtapa.Size = new System.Drawing.Size(62, 17);
            this.txtEtapa.TabIndex = 18;
            this.txtEtapa.Text = "######";
            this.txtEtapa.Visible = false;
            // 
            // txtLimiteDias
            // 
            this.txtLimiteDias.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txtLimiteDias.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLimiteDias.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtLimiteDias.Location = new System.Drawing.Point(73, 29);
            this.txtLimiteDias.Name = "txtLimiteDias";
            this.txtLimiteDias.Size = new System.Drawing.Size(40, 20);
            this.txtLimiteDias.TabIndex = 19;
            this.txtLimiteDias.Text = "7";
            this.txtLimiteDias.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtLimiteDias.TextChanged += new System.EventHandler(this.txtLimiteDias_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Silver;
            this.label6.Location = new System.Drawing.Point(27, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Diários:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Silver;
            this.label7.Location = new System.Drawing.Point(13, 51);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Semanais:";
            // 
            // txtLimiteSemanais
            // 
            this.txtLimiteSemanais.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txtLimiteSemanais.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLimiteSemanais.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtLimiteSemanais.Location = new System.Drawing.Point(73, 48);
            this.txtLimiteSemanais.Name = "txtLimiteSemanais";
            this.txtLimiteSemanais.Size = new System.Drawing.Size(40, 20);
            this.txtLimiteSemanais.TabIndex = 21;
            this.txtLimiteSemanais.Text = "5";
            this.txtLimiteSemanais.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtLimiteSemanais.TextChanged += new System.EventHandler(this.txtLimiteSemanais_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.Silver;
            this.label8.Location = new System.Drawing.Point(18, 70);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Mensais:";
            // 
            // txtLimiteMensais
            // 
            this.txtLimiteMensais.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txtLimiteMensais.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLimiteMensais.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtLimiteMensais.Location = new System.Drawing.Point(73, 67);
            this.txtLimiteMensais.Name = "txtLimiteMensais";
            this.txtLimiteMensais.Size = new System.Drawing.Size(40, 20);
            this.txtLimiteMensais.TabIndex = 23;
            this.txtLimiteMensais.Text = "2";
            this.txtLimiteMensais.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtLimiteMensais.TextChanged += new System.EventHandler(this.txtLimiteMensais_TextChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.ckbBackupAutomatico);
            this.panel2.Controls.Add(this.txtLimiteMensais);
            this.panel2.Controls.Add(this.txtLimiteDias);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.txtLimiteSemanais);
            this.panel2.Location = new System.Drawing.Point(164, 89);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(167, 122);
            this.panel2.TabIndex = 18;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.label10.Location = new System.Drawing.Point(36, 3);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(91, 17);
            this.label10.TabIndex = 16;
            this.label10.Text = "Parâmetros";
            // 
            // ckbBackupAutomatico
            // 
            this.ckbBackupAutomatico.AutoSize = true;
            this.ckbBackupAutomatico.BackColor = System.Drawing.Color.Transparent;
            this.ckbBackupAutomatico.ForeColor = System.Drawing.Color.Silver;
            this.ckbBackupAutomatico.Location = new System.Drawing.Point(16, 100);
            this.ckbBackupAutomatico.Name = "ckbBackupAutomatico";
            this.ckbBackupAutomatico.Size = new System.Drawing.Size(92, 17);
            this.ckbBackupAutomatico.TabIndex = 24;
            this.ckbBackupAutomatico.Text = "Agendamento";
            this.ckbBackupAutomatico.UseVisualStyleBackColor = false;
            this.ckbBackupAutomatico.CheckedChanged += new System.EventHandler(this.ckbBackupAutomatico_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.ForeColor = System.Drawing.Color.Silver;
            this.label9.Location = new System.Drawing.Point(348, 97);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Espelho:";
            // 
            // btnSearchEspelho
            // 
            this.btnSearchEspelho.BackColor = System.Drawing.Color.Transparent;
            this.btnSearchEspelho.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSearchEspelho.BackgroundImage")));
            this.btnSearchEspelho.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSearchEspelho.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearchEspelho.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSearchEspelho.Location = new System.Drawing.Point(760, 111);
            this.btnSearchEspelho.Name = "btnSearchEspelho";
            this.btnSearchEspelho.Size = new System.Drawing.Size(29, 25);
            this.btnSearchEspelho.TabIndex = 20;
            this.btnSearchEspelho.Text = "...";
            this.btnSearchEspelho.UseVisualStyleBackColor = false;
            this.btnSearchEspelho.Click += new System.EventHandler(this.btnSearchEspelho_Click);
            // 
            // txbPastaSync
            // 
            this.txbPastaSync.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txbPastaSync.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txbPastaSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbPastaSync.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txbPastaSync.Location = new System.Drawing.Point(368, 113);
            this.txbPastaSync.Name = "txbPastaSync";
            this.txbPastaSync.ReadOnly = true;
            this.txbPastaSync.Size = new System.Drawing.Size(394, 21);
            this.txbPastaSync.TabIndex = 19;
            // 
            // btnSync
            // 
            this.btnSync.BackColor = System.Drawing.Color.Transparent;
            this.btnSync.BackgroundImage = global::BackupNuvemSBuild_Configuration.Properties.Resources.iconfinder_50_61514;
            this.btnSync.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSync.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSync.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnSync.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnSync.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSync.Location = new System.Drawing.Point(216, 21);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(37, 36);
            this.btnSync.TabIndex = 22;
            this.btnSync.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip1.SetToolTip(this.btnSync, "Sync Espelho");
            this.btnSync.UseVisualStyleBackColor = false;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // ckbSync
            // 
            this.ckbSync.AutoSize = true;
            this.ckbSync.Location = new System.Drawing.Point(352, 116);
            this.ckbSync.Name = "ckbSync";
            this.ckbSync.Size = new System.Drawing.Size(15, 14);
            this.ckbSync.TabIndex = 23;
            this.ckbSync.UseVisualStyleBackColor = true;
            this.ckbSync.CheckedChanged += new System.EventHandler(this.ckbSync_CheckedChanged);
            // 
            // txtVersao
            // 
            this.txtVersao.AutoSize = true;
            this.txtVersao.BackColor = System.Drawing.Color.Transparent;
            this.txtVersao.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVersao.ForeColor = System.Drawing.Color.Silver;
            this.txtVersao.Location = new System.Drawing.Point(7, 39);
            this.txtVersao.Name = "txtVersao";
            this.txtVersao.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtVersao.Size = new System.Drawing.Size(41, 15);
            this.txtVersao.TabIndex = 25;
            this.txtVersao.Text = "V#.#.#";
            this.txtVersao.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.txtVersao.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.txtVersao_MouseDoubleClick);
            // 
            // btnSearchDrive
            // 
            this.btnSearchDrive.BackColor = System.Drawing.Color.Transparent;
            this.btnSearchDrive.BackgroundImage = global::BackupNuvemSBuild_Configuration.Properties.Resources.iconfinder_icon_101_folder_search_314678;
            this.btnSearchDrive.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSearchDrive.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearchDrive.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSearchDrive.Location = new System.Drawing.Point(760, 21);
            this.btnSearchDrive.Name = "btnSearchDrive";
            this.btnSearchDrive.Size = new System.Drawing.Size(29, 25);
            this.btnSearchDrive.TabIndex = 6;
            this.btnSearchDrive.Text = "...";
            this.btnSearchDrive.UseVisualStyleBackColor = false;
            this.btnSearchDrive.Click += new System.EventHandler(this.btnSearchDrive_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.label11.Location = new System.Drawing.Point(3, 13);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(181, 22);
            this.label11.TabIndex = 26;
            this.label11.Text = "Backup Automático";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pgbSubPasta);
            this.panel1.Controls.Add(this.txtRestantes);
            this.panel1.Controls.Add(this.txtPercentSubPasta);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtArquivos);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txtPasta);
            this.panel1.Controls.Add(this.pgbTotal);
            this.panel1.Controls.Add(this.txtEtapa);
            this.panel1.Controls.Add(this.txtPercentTotal);
            this.panel1.Location = new System.Drawing.Point(352, 317);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(437, 173);
            this.panel1.TabIndex = 27;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.ReshowDelay = 100;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(11)))), ((int)(((byte)(11)))));
            this.BackgroundImage = global::BackupNuvemSBuild_Configuration.Properties.Resources.PlanoDeFundoVM_21;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(815, 504);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtVersao);
            this.Controls.Add(this.ckbSync);
            this.Controls.Add(this.btnSync);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnSearchEspelho);
            this.Controls.Add(this.txbPastaSync);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSearchDrive);
            this.Controls.Add(this.txbPastaDrive);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSearchPath);
            this.Controls.Add(this.txbPastaBackup);
            this.Controls.Add(this.dtpData);
            this.Controls.Add(this.cklPastasDrive);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Backup Nuvem S.Build";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpData;
        private System.Windows.Forms.TextBox txbPastaBackup;
        private System.Windows.Forms.Button btnSearchPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.NotifyIcon backupIcon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSearchDrive;
        private System.Windows.Forms.TextBox txbPastaDrive;
        private System.Windows.Forms.CheckedListBox cklPastasDrive;
        private System.Windows.Forms.ProgressBar pgbSubPasta;
        private System.Windows.Forms.ProgressBar pgbTotal;
        private System.Windows.Forms.Label txtPercentSubPasta;
        private System.Windows.Forms.Label txtPercentTotal;
        private System.Windows.Forms.Label txtRestantes;
        private System.Windows.Forms.Label txtArquivos;
        private System.Windows.Forms.Label txtPasta;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label txtEtapa;
        private System.Windows.Forms.TextBox txtLimiteDias;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtLimiteSemanais;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtLimiteMensais;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnSearchEspelho;
        private System.Windows.Forms.TextBox txbPastaSync;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.CheckBox ckbSync;
        private System.Windows.Forms.CheckBox ckbBackupAutomatico;
        private System.Windows.Forms.Label txtVersao;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

