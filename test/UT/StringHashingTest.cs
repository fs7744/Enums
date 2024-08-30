using SV;

namespace UT
{
    public class StringHashingTest
    {
        [Fact]
        public void NormalizedHash()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var hash = typeof(string).GetMethod("GetNonRandomizedHashCodeOrdinalIgnoreCase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).CreateDelegate<Func<string, int>>();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.Equal("   ".HashOrdinalIgnoreCase(), "   ".HashOrdinalIgnoreCase());
            Assert.Equal(" asds dsff".HashOrdinalIgnoreCase(), " ASDS dsff".HashOrdinalIgnoreCase());
            Assert.Equal(1666770079, " asds dsff".HashOrdinalIgnoreCase());
            Assert.Equal(1666770079, hash(" asds dsff"));
            Assert.Equal(" asds dsff".HashOrdinalIgnoreCase(), hash(" ASDS dsff"));
        }
    }
}