public class FormaPagamento
{
    public int ID { get; set; }
    public int CODIGO { get; set; }
    public string DESCRICAO { get; set; }
    public int ID_GRUPO_PAGAMENTO { get; set; }
}
public class ComandaPagamentoRequest
{
    public int ID_ECF_TIPO_PAGAMENTO { get; set; }
    public int ID_COMANDA { get; set; }
    public decimal VALOR { get; set; }
}