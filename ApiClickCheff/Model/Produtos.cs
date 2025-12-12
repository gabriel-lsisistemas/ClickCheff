using System.Text.RegularExpressions;

public class Produto
{
    public int ID { get; set; }
    public int ID_STATUS_COMANDA { get; set; }
    public string DESCRICAO { get; set; }
    public decimal PRECOVENDA { get; set; }
    public decimal QTDE { get; set; }
}

public class GrupoProduto
{
    public int ID { get; set; }
    public string GRUPO { get; set; }
    public decimal QTD_PRODUTOS { get; set; } 
}
