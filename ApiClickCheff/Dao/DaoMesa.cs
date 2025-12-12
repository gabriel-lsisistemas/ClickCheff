using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace ApiClickCheff.Dao
{
    public class DaoMesa
    {
        private string conexao;
        private int idEmpresa_TERMINAL;

        public DaoMesa()
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


        public List<Mesas> GetMesas()
        {
            List<Mesas> mesas = new List<Mesas>();
            try { 
                using (SqlConnection conn = new SqlConnection(conexao))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                          SELECT 
                            ID,
                            ID_STATUS_COMANDA,
                            DESCRICAO,
                            CASE ID_STATUS_COMANDA
                                WHEN 1 THEN '#ee8b60'
                                WHEN 2 THEN '#0098d8'
                                WHEN 3 THEN '#47da07'
                                WHEN 4 THEN '#eebf3d'
                                ELSE '#000000'
                            END AS COR_HEX
                        FROM IDENTIFICADADOR_COMANDA", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Mesas mesa = new Mesas
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    ID_STATUS_COMANDA = Convert.ToInt32(reader["ID_STATUS_COMANDA"]),
                                    DESCRICAO = reader["DESCRICAO"].ToString(),
                                    CorHex = reader["COR_HEX"].ToString()    // <<< aqui
                                };
                                mesas.Add(mesa);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogErro("Erro ao obter produtos.", ex);
            }

            return mesas;
        }

        public List<Mesas> VerificaMesa(int id)
        {
            List<Mesas> mesas = new List<Mesas>();
            try { 
                using (SqlConnection conn = new SqlConnection(conexao))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                  SELECT * FROM IDENTIFICADADOR_COMANDA WHERE ID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Mesas mesa = new Mesas
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    ID_STATUS_COMANDA = Convert.ToInt32(reader["ID_STATUS_COMANDA"]),
                                    DESCRICAO = reader["DESCRICAO"].ToString()
                                };
                                mesas.Add(mesa);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogErro("Erro ao obter produtos.", ex);
            }
            return mesas;
        }

        public bool AtualizarComanda(ComandaUpdateDto dto)
        {

            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
            UPDATE IDENTIFICADADOR_COMANDA
            SET ID_STATUS_COMANDA = @IDstatus,
                ID_COMANDA = @ID_COMANDA
            WHERE ID = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("@ID_COMANDA", dto.IdComanda);
                    cmd.Parameters.AddWithValue("@ID", dto.Id);
                    cmd.Parameters.AddWithValue("@IDstatus", dto.Idstatus);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
