namespace Fortuna.Application.Products;

public static class ProductNumberGenerator
{
    public static string GenerateAccountNumber(long sequence)
        => $"1369696969{sequence:D16}";

    public static string GenerateCardNumber(long sequence)
        => $"54006969{sequence:D8}";
}
