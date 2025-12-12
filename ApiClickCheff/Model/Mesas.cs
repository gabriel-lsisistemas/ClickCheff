public class Mesas
{
    public int Id { get; set; }
    public int ID_STATUS_COMANDA { get; set; }
    public string DESCRICAO { get; set; }
    public string CorHex { get; set; } 
}

public class ComandaUpdateDto
{
    public int Id { get; set; }        // ID do registro a ser atualizado
    public int Idstatus { get; set; }        // ID do registro a ser atualizado
    public int IdComanda { get; set; } // Novo valor para ID_COMANDA
}
