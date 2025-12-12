using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace ApiClickCheff.Dao
{
    public class DaoPagamentos
    {
        private string conexao;
        private int idEmpresa_TERMINAL;

        public DaoPagamentos()
        {
            LoadConnectionConexaoString();
        }

        private void LoadConnectionConexaoString()
        {
            string configFilePath = Path.Combine(AppContext.BaseDirectory, "Conexao.conf");

            if (File.Exists(configFilePath))
            {
                try
                {
                    string[] linhas = File.ReadAllLines(configFilePath);
                    string servidor = null, banco = null, usuario = null, senha = null, tipoBanco = null;
                    bool dentroGestor = false;

                    foreach (string linha in linhas)
                    {
                        if (linha.Trim().StartsWith("[GESTOR]"))
                        {
                            dentroGestor = true;
                        }
                        else if (linha.StartsWith("[") && linha.EndsWith("]"))
                        {
                            dentroGestor = false;
                        }
                        else if (dentroGestor)
                        {
                            if (linha.StartsWith("SERVIDOR="))
                            {
                                servidor = linha.Substring("SERVIDOR=".Length);
                            }
                            else if (linha.StartsWith("BANCO="))
                            {
                                banco = linha.Substring("BANCO=".Length);
                            }
                            else if (linha.StartsWith("USUARIO="))
                            {
                                usuario = linha.Substring("USUARIO=".Length);
                            }
                            else if (linha.StartsWith("SENHA="))
                            {
                                senha = linha.Substring("SENHA=".Length);
                            }
                            else if (linha.StartsWith("TIPO_BANCO="))
                            {
                                tipoBanco = linha.Substring("TIPO_BANCO=".Length);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(servidor) && !string.IsNullOrEmpty(banco) && !string.IsNullOrEmpty(usuario) && !string.IsNullOrEmpty(senha) && !string.IsNullOrEmpty(tipoBanco))
                    {
                        conexao = $"Data Source={servidor};Initial Catalog={banco};User ID={usuario};Password={senha};";
                        Console.WriteLine("Conexão inicializada: " + conexao);  // Verifique no log se a conexão foi configurada corretamente
                    }
                    else
                    {
                        Console.WriteLine("Erro: Parâmetros da conexão não foram preenchidos corretamente.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogErro("Erro ao carregar configuração de conexão.", ex);
                }
            }
            else
            {
                // Log para arquivo se o arquivo de configuração não for encontrado
                Console.WriteLine($"Arquivo de configuração 'Conexao.conf' não encontrado em {AppContext.BaseDirectory}");
            }
        }
        public List<FormaPagamento> GetTipoPagamento()
        {
            List<FormaPagamento> tipos = new List<FormaPagamento>();
            try
            {
                using (SqlConnection conn = new SqlConnection(conexao))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                    select ID, CODIGO, DESCRICAO, ID_GRUPO_PAGAMENTO from ECF_TIPO_PAGAMENTO", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                FormaPagamento tipo = new FormaPagamento
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    CODIGO = Convert.ToInt32(reader["CODIGO"]),
                                    DESCRICAO = reader["DESCRICAO"].ToString(),
                                    ID_GRUPO_PAGAMENTO = Convert.ToInt32(reader["ID_GRUPO_PAGAMENTO"])
                                };
                                tipos.Add(tipo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogErro("Erro ao obter tipos.", ex);
            }
            return tipos;
        }
        public bool InserirComandaPagamento(int idTipoPagamento, int idComanda, decimal valor)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conexao))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO COMANDA_PAGAMENTO (ID, ID_ECF_TIPO_PAGAMENTO, ID_COMANDA, VALOR)
                VALUES ((SELECT COALESCE(MAX(ID), 0) + 1 FROM COMANDA_PAGAMENTO), @ID_TIPO_PAGAMENTO, @ID_COMANDA, @VALOR)", conn))
                    {
                        cmd.Parameters.AddWithValue("@ID_TIPO_PAGAMENTO", idTipoPagamento);
                        cmd.Parameters.AddWithValue("@ID_COMANDA", idComanda);
                        cmd.Parameters.AddWithValue("@VALOR", valor);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogErro("Erro ao inserir COMANDA_PAGAMENTO.", ex);
                return false;
            }
        }

    }
}
