public class Comandas
{
    public int id { get; set; }
    public int idFuncionario { get; set; }
    public string data_comanda { get; set; }
    public decimal qtdPessoas { get; set; }
    public decimal valorProdutos { get; set; }
    public decimal valorTotal { get; set; }
    public decimal valorServico { get; set; }
    public int idComanda { get; set; }

    public List<ComandaItem> Itens { get; set; }
}
public class UltimoIDComanda
{
    public int id { get; set; }
}

public class ComandaItem
{
    public int id { get; set; }
    public int idMesa { get; set; }
    public int idComanda { get; set; }
    public int idProduto { get; set; }
    public string nomeProduto { get; set; }
    public decimal qtde { get; set; }
    public decimal valorUnit { get; set; }
    public decimal valorTotal { get; set; }
    public string obs { get; set; }
    public int contador { get; set; }
}

public class ComandaInsert
{
    public int ID { get; set; }
    public int ID_ECF_FUNCIONARIO { get; set; }
    public string DATA_COMANDA { get; set; }
    public int ID_CLIENTE { get; set; }
    public decimal QTD_PESSOAS { get; set; }
    public decimal VALOR_PRODUTOS { get; set; }
    public decimal DESCONTO { get; set; }
    public decimal VALOR_TOTAL { get; set; }
    public decimal VALOR_SERVICO { get; set; }
    public string CANCELADO { get; set; }
    public int ID_IDENTIFICADADOR_COMANDA { get; set; }
    public string NOME_CLIENTE { get; set; }
}
public class ComandaDetalheInsert
{
    public int ID_COMANDA { get; set; }
    public int ID_PRODUTO { get; set; }
    public decimal QTD { get; set; }
    public decimal UNITARIO { get; set; }
    public decimal DESCONTO { get; set; }
    public decimal VALOR_TOTAL { get; set; }
    public string OBSERVACAO { get; set; }
    public string IMPRESSO_SEPARACAO { get; set; }
    public string CANCELADO { get; set; }
    public string? DATA_CADASTRO { get; set; }
    public int ID_ECF_FUNCIONARIO { get; set; }
    public string SABOR { get; set; }
    public decimal VALOR_ADICIONAL { get; set; }
    public decimal VALOR_SABOR { get; set; }
    public string IMPRESSO_SEPARACAO_APP { get; set; }
    public string NUMERO_CARTAO { get; set; }
    public int? ID_PRODUTO_SABOR { get; set; }
}
