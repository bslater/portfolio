using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class RatioStringJsonConverter 
    : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var text = reader.GetString();
            if (text == null)
            {
                // Handle null case
                return 0m;
            }

            var parts = text.Split(':');
            if (parts.Length != 2)
            {
                throw new JsonException("Invalid ratio format. Expected format: 'numerator:denominator'");
            }

            if (!int.TryParse(parts[0], out int numerator) || !int.TryParse(parts[1], out int denominator))
            {
                throw new JsonException("Invalid ratio values. Numerator and denominator must be int numbers.");
            }

            if (denominator == 0)
            {
                throw new JsonException("Denominator cannot be zero.");
            }

            return ((decimal)numerator / (decimal)denominator);
        }

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetDecimal(out decimal number))
        {
            return number;
        }

        throw new JsonException("Unexpected token type when reading ratio.");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {


        // Write simplified ratio
        writer.WriteStringValue(GetRatioString(value));
    }

    public static string GetRatioString(decimal value)
	{
		// Calculate greatest common divisor
		decimal gcd = CalculateGcd(value, 1m);

		// Return simplified ratio
		return $"{value / gcd}:{1 / gcd}";
	}

    private static decimal CalculateGcd(decimal a, decimal b)
        => b == 0 ? Math.Abs(a) : CalculateGcd(b, a % b);
}
