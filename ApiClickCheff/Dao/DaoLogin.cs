using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace ApiClickCheff.Dao
{
    public class DaoLogin
    {
        private string conexao;
        private int idEmpresa_TERMINAL;

        public DaoLogin()
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
                    Logger.LogErro("Erro ao obter produtos.", ex);
                }
            }
            else
            {
                // Log para arquivo se o arquivo de configuração não for encontrado
                Console.WriteLine($"Arquivo de configuração 'Conexao.conf' não encontrado em {AppContext.BaseDirectory}");
            }
        }


        public Login GetLoginByCredentials(string login, string senhaEnviada)
        {
            if (string.IsNullOrEmpty(login))
                throw new ArgumentException("Login não pode ser nulo ou vazio.", nameof(login));

            if (string.IsNullOrEmpty(senhaEnviada))
                throw new ArgumentException("Senha não pode ser nula ou vazia.", nameof(senhaEnviada));

            Login userLogin = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(conexao))
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    F.ID, 
                    F.NOME, 
                    F.LOGIN, 
                    F.SENHA, 
                    F.EMAIL,
                    F.TELEFONE, 
                    COALESCE(F.TAXA_DESCONTO, 0) AS TAXA_DESCONTO, 
                    F.ID_EMPRESA,
                    COALESCE(cc.TAXA_SERVICO, 0) AS TAXA_SERVICO
                FROM 
                    ECF_FUNCIONARIO F
                INNER JOIN 
                    CARGO_FUNCIONARIO C ON F.ID = C.ID_FUNCIONARIO
                INNER JOIN
                    CONFIGURACOES_COMANDA cc on F.ID_EMPRESA = cc.ID
                WHERE 
                    C.ID_CARGO = 1 AND LOWER(F.LOGIN) = LOWER(@login)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", login);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userLogin = new Login
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    ID_EMPRESA = Convert.ToInt32(reader["ID_EMPRESA"]),
                                    NOME = reader["NOME"].ToString(),
                                    LOGIN = reader["LOGIN"].ToString(),
                                    SENHA = reader["SENHA"].ToString(),
                                    EMAIL = reader["EMAIL"].ToString(),
                                    TELEFONE = reader["TELEFONE"]?.ToString(),
                                    TAXA_SERVICO = Convert.ToDecimal(reader["TAXA_SERVICO"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao realizar o login: {ex.Message}", ex);
            }

            return userLogin;
        }

        public List<Login> GetLogins()
        {
            List<Login> logins = new List<Login>();

            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT ID, NOME, LOGIN, SENHA, EMAIL, TELEFONE, ID_EMPRESA
                    FROM ECF_FUNCIONARIO", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Login login = new Login
                            {
                                Id = Convert.ToInt32(reader["ID"]),
                                ID_EMPRESA = Convert.ToInt32(reader["ID_EMPRESA"]),
                                NOME = reader["NOME"].ToString(),
                                LOGIN = reader["LOGIN"].ToString(),
                                SENHA = reader["SENHA"].ToString(),
                                EMAIL = reader["EMAIL"].ToString(),
                                TELEFONE = reader["TELEFONE"].ToString()
                            };
                            logins.Add(login);
                        }
                    }
                }
            }

            return logins;
        }
    }
}
