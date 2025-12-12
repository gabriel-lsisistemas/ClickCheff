using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace ApiClickCheff.Dao
{
    public class DaoProduto
    {
        private string conexao;
        private int idEmpresa_TERMINAL;

        public DaoProduto()
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

        public List<GrupoProduto> GetGrupoProdutos()
        {
            List<GrupoProduto> grupos = new List<GrupoProduto>();
            try { 
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT 
                    GRU.ID,
                    GRU.NOME AS GRUPO,
                    COUNT(PRO.ID) AS QTD_PRODUTOS,
                    SUM(ISNULL(SABORES.QTD_SABORES, 0)) AS TOTAL_SABORES,
                    SUM(ISNULL(ADICS.QTD_ADICS, 0)) AS TOTAL_ADICIONAIS
                FROM PRODUTO PRO
                INNER JOIN GRUPO_PRODUTO GRU ON GRU.ID = PRO.ID_GRUPO_PRODUTO
                OUTER APPLY (
                    SELECT COUNT(*) AS QTD_SABORES 
                    FROM PRODUTO_SABOR R 
                    WHERE R.ID_PRODUTO = PRO.ID
                ) SABORES
                OUTER APPLY (
                    SELECT COUNT(*) AS QTD_ADICS 
                    FROM ACOMPANHA_PRODUTO C 
                    WHERE C.ID_PRODUTO = PRO.ID
                ) ADICS
                GROUP BY GRU.ID, GRU.NOME
                ORDER BY GRU.NOME", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            GrupoProduto grupo = new GrupoProduto
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                GRUPO = reader["GRUPO"].ToString(),
                                QTD_PRODUTOS = Convert.ToDecimal(reader["QTD_PRODUTOS"])
                            };
                            grupos.Add(grupo);
                        }
                    }
                }
            }
            }
            catch (Exception ex)
            {
                Logger.LogErro("Erro ao obter produtos.", ex);
            }
            return grupos;
        }

        public List<Produto> GetProdutosComGrupo(int id)
        {
            List<Produto> produtosComGrupos = new List<Produto>();
            try { 
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT PRO.ID, PRO.NOME AS DESCRICAO,
                       COALESCE(PRO.VALOR_VENDA, 0) AS PRECOVENDA,
                       COALESCE((SELECT COUNT(*) FROM PRODUTO_SABOR R WHERE R.ID_PRODUTO = PRO.ID), 0) AS SABORES,
                       COALESCE((SELECT COUNT(*) FROM ACOMPANHA_PRODUTO C WHERE C.ID_PRODUTO = PRO.ID), 0) AS ADIC
                FROM PRODUTO PRO
                INNER JOIN GRUPO_PRODUTO GRU ON GRU.ID = PRO.ID_GRUPO_PRODUTO
                WHERE PRO.ID_STATUS_PRODUTO = 1
                  AND PRO.ID_GRUPO_PRODUTO = @idGrupo
                ORDER BY PRO.NOME;", conn))
                {
                    cmd.Parameters.AddWithValue("@idGrupo", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Produto produtoComGrupo = new Produto
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                DESCRICAO = reader["DESCRICAO"].ToString(),
                                PRECOVENDA = Convert.ToDecimal(reader["PRECOVENDA"])
                            };
                            produtosComGrupos.Add(produtoComGrupo);
                        }
                    }
                }
            }
            }
            catch (Exception ex)
            {
                Logger.LogErro("Erro ao obter produtos.", ex);
            }
            return produtosComGrupos;
        }
        public List<Produto> GetProdutos()
        {
            List<Produto> produtos = new List<Produto>();
            try
            {
                using (SqlConnection conn = new SqlConnection(conexao))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                SELECT PRO.ID, PRO.NOME AS DESCRICAO,
                       COALESCE(PRO.VALOR_VENDA, 0) AS PRECOVENDA,
                       COALESCE((SELECT COUNT(*) FROM PRODUTO_SABOR R WHERE R.ID_PRODUTO = PRO.ID), 0) AS SABORES,
                       COALESCE((SELECT COUNT(*) FROM ACOMPANHA_PRODUTO C WHERE C.ID_PRODUTO = PRO.ID), 0) AS ADIC
                FROM PRODUTO PRO
                INNER JOIN GRUPO_PRODUTO GRU ON GRU.ID = PRO.ID_GRUPO_PRODUTO
                WHERE PRO.ID_STATUS_PRODUTO = 1
                ORDER BY PRO.NOME;", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Produto produto = new Produto
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    DESCRICAO = reader["DESCRICAO"].ToString(),
                                    PRECOVENDA = Convert.ToDecimal(reader["PRECOVENDA"])
                                };
                                produtos.Add(produto);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogErro("Erro ao obter produtos.", ex);
            }
            return produtos;
        }
        public List<Produto> GetMaisVendidos()
        {
            List<Produto> maisvendidos = new List<Produto>();
            try { 
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT DISTINCT P.ID, P.NOME AS DESCRICAO,
                       COALESCE(P.VALOR_VENDA, 0) AS PRECOVENDA,
                       COALESCE((SELECT COUNT(*) FROM PRODUTO_SABOR R WHERE R.ID_PRODUTO = P.ID), 0) AS SABORES,
                       COALESCE((SELECT COUNT(*) FROM ACOMPANHA_PRODUTO C WHERE C.ID_PRODUTO = P.ID), 0) AS ADIC,
                       CAST(COALESCE((SELECT SUM(D1.QTD)
                                      FROM COMANDA_DETALHE D1
                                      WHERE D1.ID_PRODUTO = D.ID_PRODUTO AND D1.CANCELADO = 'N'), 0) AS DECIMAL(18,6)) AS QTDE
                FROM COMANDA_DETALHE D
                INNER JOIN PRODUTO P ON P.ID = D.ID_PRODUTO
                INNER JOIN COMANDA C ON C.ID = D.ID_COMANDA
                WHERE D.CANCELADO = 'N'
                  AND C.CANCELADO = 'N'", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Produto maisvendido = new Produto
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                DESCRICAO = reader["DESCRICAO"].ToString(),
                                PRECOVENDA = Convert.ToDecimal(reader["PRECOVENDA"]),
                                QTDE = Convert.ToDecimal(reader["QTDE"])
                            };
                            maisvendidos.Add(maisvendido);
                        }
                    }
                }
            }
                        }
            catch (Exception ex)
            {
                Logger.LogErro("Erro ao obter produtos.", ex);
            }
            return maisvendidos;
        }
        public List<Sabores> GetSabores(int idProduto)
        {
            List<Sabores> sabores = new List<Sabores>();
            try { 
            using (SqlConnection conn = new SqlConnection(conexao))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                select ps.ID, ps.ID_PRODUTO, ps.ID_SABOR, s.DESCRICAO, COALESCE(ps.ACRESCENTA_VALOR, 'N') AS ACRESCENTA_VALOR, ps.VALOR from PRODUTO_SABOR ps
                INNER JOIN SABORES s on s.ID = ps.ID_SABOR
                WHERE ID_PRODUTO = @idProduto", conn))
                {
                    cmd.Parameters.AddWithValue("@idProduto", idProduto);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sabores.Add(new Sabores
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                ID_PRODUTO = Convert.ToInt32(reader["ID_PRODUTO"]),
                                ID_SABOR = Convert.ToInt32(reader["ID_SABOR"]),
                                DESCRICAO = reader["DESCRICAO"].ToString(),
                                ACRESCENTA_VALOR = reader["ACRESCENTA_VALOR"] != DBNull.Value? reader["ACRESCENTA_VALOR"].ToString(): string.Empty,
                                VALOR = reader["VALOR"] != DBNull.Value? Convert.ToDecimal(reader["VALOR"]): 0
                            });
                        }
                    }
                }
            }
                        }
            catch (Exception ex)
            {
                Logger.LogErro("Erro ao obter sabores.", ex);
            }
            return sabores;
        }
    }
}
