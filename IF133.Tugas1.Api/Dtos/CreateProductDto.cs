using System.ComponentModel.DataAnnotations;

namespace ProductApi.Dtos;

public class CreateProductDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Nama produk maksimal 100 karakter.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Deskripsi maksimal 500 karakter.")]
    public string? Description { get; set; }

    [Range(0.01, 100000000, ErrorMessage = "Harga harus lebih dari 0.")]
    public decimal Price { get; set; }

    [Range(0, 100000000, ErrorMessage = "Stok harus lebih dari 0.")]
    public decimal Stock { get; set; }
}