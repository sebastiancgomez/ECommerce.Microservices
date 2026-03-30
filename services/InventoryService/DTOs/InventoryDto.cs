namespace InventoryService.DTOs;

public class InventoryDto
{
    public int ProductId { get; set; }
    public int AvailableStock { get; set; }
    public int ReservedStock { get; set; }
    public int TotalStock { get; set; }
}