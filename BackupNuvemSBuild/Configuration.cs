using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupNuvemSBuild
{
    class Configuration
    {
        public bool BackupAutomatico { get; set; }
        public string PastaDrive { get; set; }
        public string PastaBackup { get; set; }
        public bool HabilitaPastaEspelho { get; set; }
        public string PastaEspelho { get; set; }
        public int BackupsDiarios { get; set; }
        public int BackupsSemanais { get; set; }
        public int BackupsMensais { get; set; }

        bool restaurando = false;

        string pathConfiguration = "";

        public bool RestauraConfiguracao(string pathConfiguration)
        {
            restaurando = true;

            this.pathConfiguration = pathConfiguration;

            bool resultado = false;

            try
            {
                if (File.Exists(pathConfiguration))
                {
                    int totalItens = File.ReadLines(pathConfiguration).Count();

                    string item = "";
                    string value = "";

                    if (totalItens == 8)
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
                                        BackupAutomatico = Convert.ToBoolean(value);
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
                                        BackupsDiarios = Convert.ToInt32(value);
                                        break;

                                    case 6:
                                        BackupsSemanais = Convert.ToInt32(value);
                                        break;

                                    case 7:
                                        BackupsMensais = Convert.ToInt32(value);
                                        break;


                                    default:
                                        break;
                                }
                            }
                            catch (Exception)
                            {

                            }      
                        }

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


        public void SalvaConfiguracao()
        {
            File.WriteAllText(pathConfiguration,
                                "BackupAutomatico:" + BackupAutomatico.ToString() + Environment.NewLine +
                                "PastaDrive:" + PastaDrive + Environment.NewLine +
                                "PastaBackup:" + PastaBackup + Environment.NewLine +
                                "HabilitarPastaEspelho:" + HabilitaPastaEspelho.ToString() + Environment.NewLine +
                                "PastaEspelho:" + PastaEspelho + Environment.NewLine +
                                "BackupsDiarios:" + BackupsDiarios.ToString() + Environment.NewLine +
                                "BackupsSemanais:" + BackupsSemanais.ToString() + Environment.NewLine +
                                "BackupsMensais:" + BackupsMensais.ToString());
        }

    }
}
