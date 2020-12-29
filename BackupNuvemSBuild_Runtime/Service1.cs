using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        #region diretorios
        string pathTxt = AppDomain.CurrentDomain.BaseDirectory + @"\Log\Log_Runtime.log";
        string pathObsoletos = AppDomain.CurrentDomain.BaseDirectory + @"\Log\Obsoletos\";
        #endregion
        #endregion

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
            StartTime();
        }

        private void StartTime()
        {
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
                    LogError("Erro na leitura da Versão.",
                                    MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().ToString(),
                                            ex.Message);
                }

                LogInfo("Starting Timer " + versao + ": " + DateTime.Now.ToString(),
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString());

                if (rotina == null)
                {
                    int timerIntervalSecs = 10;

                    TimeSpan tsInterval = new TimeSpan(0, 0, timerIntervalSecs);

                    rotina = new Timer(new TimerCallback(ExecutaRotina), null, tsInterval, tsInterval);
                }
            }
            catch (Exception ex)
            {
                LogError("Error - Start Timer",
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
                LogInfo("Stopping Timer " + versao + ": " + DateTime.Now.ToString(),
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString());


                if (rotina != null)
                {
                    rotina.Change(Timeout.Infinite, Timeout.Infinite);
                    rotina.Dispose();
                    rotina = null;
                }
            }
            catch (Exception ex)
            {
                LogError("Error - Stop Timer",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);
            }
        }




        private void ExecutaRotina(object sender)
        {

        }






        #region Log


        public void LogError(string mensagem, string classe, string metodo, string exception)
        {
            ValidaTxt();

            try
            {
                //Incrementa texto de log no txt com: data e hora, clase, método, mensagem e exceção
                File.AppendAllText(pathTxt, String.Format("{0}{1}{2}{3}{4}{5}",
                        DateTime.Now.ToString() + Environment.NewLine,
                        " | ERROR |" + Environment.NewLine,
                        " | Descrição: " + mensagem + Environment.NewLine,
                        " | Classe: " + classe + Environment.NewLine,
                        " | Método: " + metodo + Environment.NewLine,
                        " | Exception: " + exception) + Environment.NewLine
                        + Environment.NewLine);
            }
            catch (Exception)
            {

            }
        }


        public void LogInfo(string mensagem, string classe, string metodo)
        {
            ValidaTxt();

            try
            {
                //Incrementa texto de log no txt com: data e hora, clase, método, mensagem e exceção
                File.AppendAllText(pathTxt, String.Format("{0}{1}{2}{3}{4}",
                        DateTime.Now.ToString() + Environment.NewLine,
                        " | INFO |" + Environment.NewLine,
                        " | Descrição: " + mensagem + Environment.NewLine,
                        " | Classe: " + classe + Environment.NewLine,
                        " | Método: " + metodo + Environment.NewLine)
                        + Environment.NewLine);
            }
            catch (Exception)
            {

            }

        }


        private void ValidaTxt()
        {
            //Verifica se o arquivo txt de log Existe
            if (File.Exists(pathTxt))
            {
                string novoNome = "";

                try
                {
                    //Verifica tamanho do txt de log em Bytes
                    long tamanho = new FileInfo(pathTxt).Length;

                    //Verifica se tamanho do txt é maior que 1 Mb (1000000 Bytes)
                    if (tamanho > 1000000)
                    {
                        //cria novo nome de txt com data atual
                        novoNome = pathObsoletos + "Log_Runtime_" + DateTime.Now.Day.ToString()
                                                                        + DateTime.Now.Month.ToString()
                                                                        + DateTime.Now.Year.ToString() + "_"
                                                                        + DateTime.Now.Hour.ToString()
                                                                        + DateTime.Now.Minute.ToString()
                                                                        + DateTime.Now.Second.ToString() + ".log";

                        File.Move(pathTxt, novoNome);
                    }
                }
                catch (Exception)
                {
                }
            }
        }


        #endregion
    }
}
