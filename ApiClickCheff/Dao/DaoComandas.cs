using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace ApiClickCheff.Dao
{
    public class DaoComandas
    {
        private string conexao;
        private int idEmpresa_TERMINAL;

        public DaoComandas()
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

        public List<Comandas> GetComandasMesa(int id)
        {
            List<Comandas> listaComandas = new List<Comandas>();
            try
            {
                using (SqlConnection conn = new SqlConnection(conexao))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                    DECLARE @ID_COMANDA INT;

                    SELECT @ID_COMANDA = ID_COMANDA
                    FROM vw_lst_identificadores_comanda
                    WHERE id = @id AND ID_STATUS_COMANDA IN (1, 2, 4);

                    SELECT ID, ID_ECF_FUNCIONARIO, DATA_COMANDA, ID_CLIENTE, QTD_PESSOAS,
                           VALOR_PRODUTOS, DESCONTO, VALOR_TOTAL, VALOR_SERVICO, ID_IDENTIFICADADOR_COMANDA 
                    FROM COMANDA 
                    WHERE ID = @ID_COMANDA;

                    SELECT CD.ID, CD.ID_COMANDA, CD.ID_PRODUTO, P.NOME, CD.QTD, CD.UNITARIO,
                    CD.DESCONTO, CD.VALOR_TOTAL, CD.OBSERVACAO 
                    FROM COMANDA_DETALHE CD
                    JOIN PRODUTO P ON CD.ID_PRODUTO = P.ID
                    WHERE ID_COMANDA = @ID_COMANDA AND CD.CANCELADO = 'N';", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            Comandas comanda = null;

                            if (reader.Read())
                            {
                                comanda = new Comandas
                                {
                                    id = Convert.ToInt32(reader["ID"]),
                                    idFuncionario = Convert.ToInt32(reader["ID_ECF_FUNCIONARIO"]),
                                    data_comanda = reader["DATA_COMANDA"].ToString(),
                                    qtdPessoas = Convert.ToDecimal(reader["QTD_PESSOAS"]),
                                    valorProdutos = Convert.ToDecimal(reader["VALOR_PRODUTOS"]),
                                    valorTotal = Convert.ToDecimal(reader["VALOR_TOTAL"]),
                                    valorServico = Convert.ToDecimal(reader["VALOR_SERVICO"]),
                                    idComanda = Convert.ToInt32(reader["ID_IDENTIFICADADOR_COMANDA"]),
                                    Itens = new List<ComandaItem>()
                                };
                            }

                            // Avança para o segundo result set (itens)
                            if (reader.NextResult() && comanda != null)
                            {
                                int contador = 0;
                                while (reader.Read())
                                {
                                    var item = new ComandaItem
                                    {
                                        contador = contador++,
                                        id = Convert.ToInt32(reader["ID"]),
                                        idMesa = Convert.ToInt32(reader["ID_COMANDA"]),
                                        idProduto = Convert.ToInt32(reader["ID_PRODUTO"]),
                                        nomeProduto = reader["NOME"].ToString(),
                                        qtde = Convert.ToDecimal(reader["QTD"]),
                                        valorUnit = Convert.ToDecimal(reader["UNITARIO"]),
                                        valorTotal = Convert.ToDecimal(reader["VALOR_TOTAL"]),
                                        obs = reader["OBSERVACAO"].ToString()
                                    };

                                    comanda.Itens.Add(item);
                                }

                                listaComandas.Add(comanda);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogErro("Erro ao obter produtos.", ex);
            }
            return listaComandas;
        }
        public void InserirComanda(ComandaInsert comanda)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conexao))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
            IF EXISTS (SELECT 1 FROM COMANDA WHERE ID = @ID)
            BEGIN
                UPDATE COMANDA SET
                    ID_OPERADOR = @ID_ECF_FUNCIONARIO,
                    ID_ECF_FUNCIONARIO = @ID_ECF_FUNCIONARIO,
                    DATA_COMANDA = @DATA_COMANDA,
                    ID_CLIENTE = 1,
                    QTD_PESSOAS = @QTD_PESSOAS,
                    VALOR_PRODUTOS = @VALOR_TOTAL,
                    DESCONTO = @DESCONTO,
                    VALOR_TOTAL = @VALOR_TOTAL,
                    VALOR_SERVICO = @VALOR_SERVICO,
                    CANCELADO = @CANCELADO,
                    ID_IDENTIFICADADOR_COMANDA = @ID_IDENTIFICADADOR_COMANDA,
                    NOME_CLIENTE = @NOME_CLIENTE
                WHERE ID = @ID
            END
            ELSE
            BEGIN
                INSERT INTO COMANDA(ID, ID_OPERADOR, ID_ECF_FUNCIONARIO, DATA_COMANDA, ID_CLIENTE,
                                    QTD_PESSOAS, VALOR_PRODUTOS, DESCONTO, VALOR_TOTAL,
                                    VALOR_SERVICO, CANCELADO, ID_IDENTIFICADADOR_COMANDA, NOME_CLIENTE)
                VALUES(@ID, @ID_ECF_FUNCIONARIO, @ID_ECF_FUNCIONARIO, @DATA_COMANDA, 1,
                       @QTD_PESSOAS, @VALOR_TOTAL, @DESCONTO, @VALOR_TOTAL,
                       @VALOR_SERVICO, @CANCELADO, @ID_IDENTIFICADADOR_COMANDA, @NOME_CLIENTE);
            UPDATE AUTO_INC SET ID_COMANDA = @ID
            END", conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", comanda.ID);
                        cmd.Parameters.AddWithValue("@ID_ECF_FUNCIONARIO", comanda.ID_ECF_FUNCIONARIO);
                        cmd.Parameters.AddWithValue("@DATA_COMANDA", comanda.DATA_COMANDA);
                        cmd.Parameters.AddWithValue("@ID_CLIENTE", comanda.ID_CLIENTE);
                        cmd.Parameters.AddWithValue("@QTD_PESSOAS", comanda.QTD_PESSOAS);
                        cmd.Parameters.AddWithValue("@VALOR_PRODUTOS", comanda.VALOR_PRODUTOS);
                        cmd.Parameters.AddWithValue("@DESCONTO", comanda.DESCONTO);
                        cmd.Parameters.AddWithValue("@VALOR_TOTAL", comanda.VALOR_TOTAL);
                        cmd.Parameters.AddWithValue("@VALOR_SERVICO", comanda.VALOR_SERVICO);
                        cmd.Parameters.AddWithValue("@CANCELADO", comanda.CANCELADO ?? "N");
                        cmd.Parameters.AddWithValue("@ID_IDENTIFICADADOR_COMANDA", comanda.ID_IDENTIFICADADOR_COMANDA);
                        cmd.Parameters.AddWithValue("@NOME_CLIENTE", comanda.NOME_CLIENTE ?? "");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogErro("Erro ao inserir ou atualizar comanda.", ex);
            }
        }
        public void InserirOuAtualizarComandaDetalhe(ComandaDetalheInsert detalhe)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conexao))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
DECLARE @NOVO_ID INT;
DECLARE @NOVO_ITEM INT;

-- Inicializa com o valor da AUTO_INC
SET @NOVO_ID = (SELECT ID_COMANDA_DET_SAB FROM AUTO_INC);

-- Garante que o ID seja único
WHILE EXISTS (SELECT 1 FROM COMANDA_DETALHE WHERE ID = @NOVO_ID)
BEGIN
    SET @NOVO_ID = @NOVO_ID + 1;
END

-- Determina o próximo número do item
SET @NOVO_ITEM = (SELECT COALESCE(MAX(ITEM), 0) + 1 FROM COMANDA_DETALHE WHERE ID_COMANDA = @ID_COMANDA);

-- Realiza o INSERT
INSERT INTO COMANDA_DETALHE (
    ID, ID_COMANDA, ID_PRODUTO, QTD, UNITARIO, DESCONTO,
    VALOR_TOTAL, OBSERVACAO, ITEM, IMPRESSO_SEPARACAO,
    CANCELADO, DATA_CADASTRO, ID_ECF_FUNCIONARIO,
    SABOR, VALOR_ADICIONAL, VALOR_SABOR, IMPRESSO_SEPARACAO_APP, NUMERO_CARTAO, ID_TIPO_ESTOQUE_PRODUTO,
    ID_TIPO_PRECO_PRODUTO, desconto_prom)
VALUES (
    @NOVO_ID, @ID_COMANDA, @ID_PRODUTO, @QTD, @UNITARIO + ISNULL(@VALOR_SABOR, 0), @DESCONTO,
    @VALOR_TOTAL + ISNULL(@VALOR_SABOR, 0), @OBSERVACAO, @NOVO_ITEM, 'X',
    @CANCELADO, @DATA_CADASTRO, @ID_ECF_FUNCIONARIO,
    @SABOR, @VALOR_ADICIONAL, @VALOR_SABOR, 'X', @NUMERO_CARTAO, 1, 1, 0);

-- Atualiza a AUTO_INC com o novo ID
UPDATE AUTO_INC SET ID_COMANDA_DET_SAB = @NOVO_ID + 1;
", conn))
                    {
                        cmd.Parameters.AddWithValue("@ID_COMANDA", detalhe.ID_COMANDA);
                        cmd.Parameters.AddWithValue("@ID_PRODUTO", detalhe.ID_PRODUTO);
                        cmd.Parameters.AddWithValue("@QTD", detalhe.QTD);
                        cmd.Parameters.AddWithValue("@UNITARIO", detalhe.UNITARIO);
                        cmd.Parameters.AddWithValue("@DESCONTO", detalhe.DESCONTO);
                        cmd.Parameters.AddWithValue("@VALOR_TOTAL", detalhe.VALOR_TOTAL);
                        cmd.Parameters.AddWithValue("@OBSERVACAO", detalhe.OBSERVACAO ?? "");
                        cmd.Parameters.AddWithValue("@IMPRESSO_SEPARACAO", detalhe.IMPRESSO_SEPARACAO ?? "N");
                        cmd.Parameters.AddWithValue("@CANCELADO", detalhe.CANCELADO ?? "N");
                        cmd.Parameters.AddWithValue("@DATA_CADASTRO", detalhe.DATA_CADASTRO);
                        cmd.Parameters.AddWithValue("@ID_ECF_FUNCIONARIO", detalhe.ID_ECF_FUNCIONARIO);
                        cmd.Parameters.AddWithValue("@SABOR", detalhe.SABOR ?? "");
                        cmd.Parameters.AddWithValue("@VALOR_ADICIONAL", detalhe.VALOR_ADICIONAL);
                        cmd.Parameters.AddWithValue("@VALOR_SABOR", (object?)detalhe.VALOR_SABOR ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IMPRESSO_SEPARACAO_APP", detalhe.IMPRESSO_SEPARACAO_APP ?? "N");
                        cmd.Parameters.AddWithValue("@NUMERO_CARTAO", detalhe.NUMERO_CARTAO ?? "");
                        int linhasAfetadas = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Sucesso! Linhas afetadas: {linhasAfetadas}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao inserir ou atualizar COMANDA_DETALHE:");
                Console.WriteLine("Mensagem: " + ex.Message);
                Console.WriteLine("StackTrace: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Erro interno: " + ex.InnerException.Message);
                }
            }
        }
        public List<UltimoIDComanda> GetUltimoID()
        {
            List<UltimoIDComanda> ultimoid = new List<UltimoIDComanda>();
            try
            {
                using (SqlConnection conn = new SqlConnection(conexao))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"select id_comanda from auto_inc", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var comando = new UltimoIDComanda
                                {
                                    id = Convert.ToInt32(reader["id_comanda"])
                                };

                                ultimoid.Add(comando);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogErro("Erro ao inserir ou atualizar comanda.", ex);
            }
            return ultimoid;
        }
        public void InserirOuAtualizarComandaDetalheSabores(int idProdutoSabor, int idProduto, int idComanda)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conexao)) // Use a string de conexão correta aqui
                {
                    conn.Open();

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        using (SqlCommand cmd = new SqlCommand(@"
                    DECLARE @NOVO_ID INT;
                    DECLARE @ID_COMANDA_DETALHE INT;

                    SELECT @NOVO_ID = ID_COMANDA_DET_SAB FROM AUTO_INC;

                    SELECT TOP 1 @ID_COMANDA_DETALHE = ID 
                    FROM COMANDA_DETALHE 
                    WHERE ID_COMANDA = @ID_COMANDA 
                        AND ID_PRODUTO = @ID_PRODUTO 
                        AND IMPRESSO_SEPARACAO_APP = 'X' 
                        AND IMPRESSO_SEPARACAO = 'X';

                    IF @ID_COMANDA_DETALHE IS NOT NULL
                    BEGIN
                        INSERT INTO COMANDA_DETALHE_SABORES (
                            ID, ID_COMANDA_DETALHE, ID_PRODUTO_SABOR
                        )
                        VALUES (
                            @NOVO_ID, 
                            @ID_COMANDA_DETALHE, 
                            @ID_PRODUTO_SABOR
                        );

                        UPDATE AUTO_INC SET ID_COMANDA_DET_SAB = @NOVO_ID + 1;
                    END", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ID_PRODUTO_SABOR", idProdutoSabor);
                            cmd.Parameters.AddWithValue("@ID_PRODUTO", idProduto);
                            cmd.Parameters.AddWithValue("@ID_COMANDA", idComanda);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao inserir ou atualizar COMANDA_DETALHE_SABORES: " + ex.Message);
            }
        }
        public void AtualizarImpressaoComandaDetalhe(int idComandaDetalhe)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conexao))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(@"
                UPDATE COMANDA_DETALHE 
                SET IMPRESSO_SEPARACAO_APP = 'N', 
                    IMPRESSO_SEPARACAO = 'N' 
                WHERE ID_COMANDA = @ID_COMANDA_DETALHE 
                AND IMPRESSO_SEPARACAO_APP = 'X' 
                AND IMPRESSO_SEPARACAO = 'X';
            ", conn))
                    {
                        cmd.Parameters.AddWithValue("@ID_COMANDA_DETALHE", idComandaDetalhe);
                        int linhasAfetadas = cmd.ExecuteNonQuery();
                        Console.WriteLine($"Atualização realizada. Linhas afetadas: {linhasAfetadas}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao atualizar campos de impressão da COMANDA_DETALHE:");
                Console.WriteLine("Mensagem: " + ex.Message);
            }
        }

    }
}
