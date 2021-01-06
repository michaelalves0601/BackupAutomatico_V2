using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupNuvemSBuild_Models
{
    public class Configuration
    {
        public bool BackupDiferencialHabilitado { get; set; }
        public bool BackupFULLHabilitado { get; set; }
        public string PastaDrive { get; set; }
        public string PastaBackup { get; set; }
        public bool HabilitaPastaEspelho { get; set; }
        public string PastaEspelho { get; set; }
        public int BackupsFull { get; set; }
        public string HorarioDiferencial { get; set; }
        public string HorarioFull { get; set; }
        public string EmailOrigem { get; set; }
        public string SenhaOrigem { get; set; }
        public List<string> PastasRestritas { get; set; }
        public int LimiteBackupsFull { get; set; }

        bool restaurando = false;

        string pathConfiguration = "";

        string pathPastasRestritas = "";
     

        public bool RestauraConfiguracao(string pathConfiguration, string pathPastasRestritas)
        {
            restaurando = true;

            this.pathConfiguration = pathConfiguration;

            this.pathPastasRestritas = pathPastasRestritas;

            bool resultado = false;

            try
            {
                if (File.Exists(pathConfiguration))
                {
                    int totalItens = File.ReadLines(pathConfiguration).Count();

                    string item = "";
                    string value = "";

                    if (totalItens == 12)
                    {
                        for (int i = 0; i < totalItens; i++)
                        { 
                            item = File.ReadLines(pathConfiguration).Skip(i).Take(1).First().ToString();

                            try
                            {
                                value = item.Substring(item.IndexOf(":") + 1);
                            }
                            catch (Exception)
                            {
                                value = "";
                            }


                            try
                            {
                                switch (i)
                                {
                                    case 0:
                                        BackupDiferencialHabilitado = Convert.ToBoolean(value);
                                        break;

                                    case 1:
                                        PastaDrive = value;
                                        break;

                                    case 2:
                                        PastaBackup = value;
                                        break;

                                    case 3:
                                        HabilitaPastaEspelho = Convert.ToBoolean(value);
                                        break;

                                    case 4:
                                        PastaEspelho = value;
                                        break;

                                    case 5:
                                        BackupsFull = Convert.ToInt32(value);
                                        break;

                                    case 6:
                                        HorarioDiferencial = value;
                                        break;

                                    case 7:
                                        HorarioFull = value;
                                        break;

                                    case 8:
                                        EmailOrigem = value;
                                        break;

                                    case 9:
                                        SenhaOrigem = value;
                                        break;

                                    case 10:
                                        BackupFULLHabilitado = Convert.ToBoolean(value);
                                        break;

                                    case 11:
                                        LimiteBackupsFull = Convert.ToInt32(value);
                                        break;

                                    default:
                                        break;
                                }
                            }
                            catch (Exception)
                            {

                            }      
                        }
                        RestauraPastasRestritas(pathPastasRestritas);
                        resultado = true;
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            restaurando = false;

            return resultado;
        }

        private void RestauraPastasRestritas(string pathPastasRestritas)
        {
            if (File.Exists(pathPastasRestritas))
            {
                int totalItens = File.ReadLines(pathPastasRestritas).Count();
        
                string item = "";

                PastasRestritas = new List<string>();


                for (int i = 0; i < totalItens; i++)
                {
                    item = File.ReadLines(pathPastasRestritas).Skip(i).Take(1).First().ToString();

                    PastasRestritas.Add(item);
                }
            }
        }


        public void SalvaConfiguracao()
        {
            File.WriteAllText(pathConfiguration,
                                "BackupDiferencialHabilitado:" + BackupDiferencialHabilitado.ToString() + Environment.NewLine +
                                "PastaDrive:" + PastaDrive + Environment.NewLine +
                                "PastaBackup:" + PastaBackup + Environment.NewLine +
                                "HabilitarPastaEspelho:" + HabilitaPastaEspelho.ToString() + Environment.NewLine +
                                "PastaEspelho:" + PastaEspelho + Environment.NewLine +
                                "BackupsFull:" + BackupsFull.ToString() + Environment.NewLine +
                                "HorarioDiferencial:" + HorarioDiferencial.ToString() + Environment.NewLine +
                                "HorarioFull:" + HorarioFull.ToString() + Environment.NewLine +
                                "EmailOrigem:" + EmailOrigem.ToString() + Environment.NewLine +
                                "SenhaOrigem:" + SenhaOrigem.ToString() + Environment.NewLine +
                                "BackupFULLHabilitado:" + BackupFULLHabilitado.ToString());
            SalvaConfiguracoesPastasRestritas(pathPastasRestritas);
        }

        public void SalvaConfiguracoesEmail(string[] emails, string pathEmails)
        {

            File.WriteAllText(pathEmails, "");


            for (int i = 0; i < emails.Length; i++)
                File.AppendAllText(pathEmails, emails[i] + Environment.NewLine);
        }
        private void SalvaConfiguracoesPastasRestritas(string pathPastasRestritas)
        {

            File.WriteAllText(pathPastasRestritas, "");


            foreach (var item in PastasRestritas)
                 File.AppendAllText(pathPastasRestritas, item + Environment.NewLine);
        
               
        }

    }
}
