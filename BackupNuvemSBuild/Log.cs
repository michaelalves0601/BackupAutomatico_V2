using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackupNuvemSBuild
{
    class Log
    {
        #region diretorios
        public string pathTxt = Application.StartupPath + @"\Log\Log_Configuration.log";
        string pathObsoletos = Application.StartupPath + @"\Log\Obsoletos\";
        #endregion

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

        public void LogWarning(string mensagem, string classe, string metodo, string exception)
        {
            ValidaTxt();

            try
            {
                //Incrementa texto de log no txt com: data e hora, clase, método, mensagem e exceção
                File.AppendAllText(pathTxt, String.Format("{0}{1}{2}{3}{4}{5}",
                        DateTime.Now.ToString() + Environment.NewLine,
                        " | ATENÇÃO |" + Environment.NewLine,
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

        internal void LogError(string v)
        {
           // throw new NotImplementedException();
        }

        public void LogInfo(string mensagem)
        {
            ValidaTxt();

            try
            {
                //Incrementa texto de log no txt com: data e hora, clase, método, mensagem e exceção
                File.AppendAllText(pathTxt, String.Format("{0}{1}{2}",
                        DateTime.Now.ToString() + Environment.NewLine,
                        " | INFO |" + Environment.NewLine,
                        " | Descrição: " + mensagem + Environment.NewLine
                        + Environment.NewLine));
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
                        novoNome = pathObsoletos + "Log_Configuration_" + DateTime.Now.Day.ToString()
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
    }
}
