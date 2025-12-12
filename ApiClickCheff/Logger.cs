using System;
using System.IO;

public class Logger
{
    private static string logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");

    // Função para criar pasta por mês e apagar logs antigos
    public static void LimparLogsAntigos()
    {
        try
        {
            // Verificar se a pasta principal existe
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Obter todos os subdiretórios que são pastas de logs por mês
            var diretorios = Directory.GetDirectories(logDirectory);

            foreach (var diretorio in diretorios)
            {
                // Verificar a data de modificação da pasta (última modificação de qualquer arquivo dentro dela)
                DateTime ultimaModificacao = Directory.GetLastWriteTime(diretorio);

                // Se a pasta não foi modificada nos últimos 30 dias, excluir a pasta inteira
                if (ultimaModificacao < DateTime.Now.AddMonths(-1))
                {
                    Directory.Delete(diretorio, true);
                    Console.WriteLine($"Pasta de log excluída: {diretorio}"); // Pode remover isso depois, caso não queira mostrar
                }
            }

            // Agora verificar os arquivos dentro da pasta do mês atual
            string mesAnoDiretorio = Path.Combine(logDirectory, DateTime.Now.ToString("yyyy-MM"));
            if (!Directory.Exists(mesAnoDiretorio))
            {
                Directory.CreateDirectory(mesAnoDiretorio);
            }

            // Obter todos os arquivos de log do mês atual
            var arquivosLog = Directory.GetFiles(mesAnoDiretorio, "*.txt");

            foreach (var arquivo in arquivosLog)
            {
                DateTime ultimaModificacao = File.GetLastWriteTime(arquivo);

                // Se o arquivo for mais velho do que 30 dias, excluir
                if (ultimaModificacao < DateTime.Now.AddMonths(-1))
                {
                    File.Delete(arquivo);
                    Console.WriteLine($"Log excluído: {arquivo}"); // Pode remover isso depois, caso não queira mostrar
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao limpar logs antigos: {ex.Message}");
        }
    }

    // Função de log de erro
    public static void LogErro(string mensagem, Exception ex = null)
    {
        try
        {
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            // Criar diretório do mês atual
            string mesAnoDiretorio = Path.Combine(logDirectory, DateTime.Now.ToString("yyyy-MM"));
            if (!Directory.Exists(mesAnoDiretorio))
                Directory.CreateDirectory(mesAnoDiretorio);

            // Limpar logs antigos antes de gravar um novo erro
            LimparLogsAntigos();

            // Criar o nome do arquivo de log baseado na data
            string nomeArquivo = $"Erro_{DateTime.Now:yyyy-MM-dd}.txt";
            string caminhoCompleto = Path.Combine(mesAnoDiretorio, nomeArquivo);

            using (StreamWriter sw = new StreamWriter(caminhoCompleto, true))
            {
                sw.WriteLine("----- ERRO -----");
                sw.WriteLine("Data: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                sw.WriteLine("Mensagem: " + mensagem);
                if (ex != null)
                {
                    sw.WriteLine("Exceção: " + ex.Message);
                    sw.WriteLine("StackTrace: " + ex.StackTrace);
                }
                sw.WriteLine();
            }
        }
        catch
        {
            // Aqui não faz nada para evitar exceções em cadeia
        }
    }
}
