using System.Buffers;
using System.Security.Cryptography;
using System.Text;

namespace FIA.SME.Aquisicao.Core.Helpers
{
    public static class HashExtensions
    {
        public static string ToSHA256(this string str)
        {
            ReadOnlySpan<char> input = str;

            return ToSHA256(input).ToString();
        }

        public static ReadOnlySpan<char> ToSHA256(this ReadOnlySpan<char> input)
        {
            using var hasher = SHA256.Create();

            return Hash(input, hasher);
        }

        public static ReadOnlySpan<char> Hash(ReadOnlySpan<char> input, HashAlgorithm hasher)
            => Hash(input, hasher, Encoding.UTF8);

        public static ReadOnlySpan<char> Hash(ReadOnlySpan<char> input, HashAlgorithm hasher, Encoding encoding)
        {
            var inputCount = encoding.GetByteCount(input);

            // Usamos ArrayPool já que essa função será executada várias vezes, assim evitando alocação de arrays
            // adicionais.
            var inputBytesArr = ArrayPool<byte>.Shared.Rent(inputCount);
            var hashBytesArr = ArrayPool<byte>.Shared.Rent(hasher.HashSize / 8);

            // Spans apareceram a partir do C# 7.2 ela é uma "ref struct" o que quer dizer que é sempre alocada
            // na stack e nunca cria cópias independentes de quantos lugares aparecerem, essa estrutura permite
            // contextos "zero alloc".
            Span<byte> inputBytes = inputBytesArr;
            Span<byte> hashBytes = hashBytesArr;

            try
            {
                _ = encoding.GetBytes(input, inputBytes);

                // O ArrayPool retorna um array de tamanho mínimo desejado porém nesse caso é necessário que seja
                // do tamanho exato, então daremos um Slice pegando todas as posições para trás de inputCount.
                var finalInputBytes = inputBytes[..inputCount];

                _ = hasher.TryComputeHash(finalInputBytes, hashBytes, out var bytesWritten);

                Span<char> stringBuffer = new char[bytesWritten * 2];
                var finalBuffer = stringBuffer;

                for (var i = 0; i < bytesWritten; i++)
                {
                    var computedByte = hashBytes[i];

                    _ = computedByte.TryFormat(finalBuffer, out var charsWritten, "x2");

                    // Retorna um buffer "removendo" as posições escritas.
                    finalBuffer = finalBuffer[charsWritten..];
                }

                return stringBuffer;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(inputBytesArr, true);
                ArrayPool<byte>.Shared.Return(hashBytesArr, true);
            }
        }
    }
}
