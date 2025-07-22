using Xunit;
using TCGPlayerPricingApp.Utilities;

namespace TCGPlayerPricingApp.Tests
{
    public class CsvParserTests
    {
        [Fact]
        public void ParseLine_ShouldHandleSimpleCsv()
        {
            // Arrange
            var line = "a,b,c,d";
            
            // Act
            var result = CsvParser.ParseLine(line);
            
            // Assert
            Assert.Equal(4, result.Length);
            Assert.Equal("a", result[0]);
            Assert.Equal("b", result[1]);
            Assert.Equal("c", result[2]);
            Assert.Equal("d", result[3]);
        }
        
        [Fact]
        public void ParseLine_ShouldHandleQuotedFields()
        {
            // Arrange
            var line = "a,\"b,c\",d";
            
            // Act
            var result = CsvParser.ParseLine(line);
            
            // Assert
            Assert.Equal(3, result.Length);
            Assert.Equal("a", result[0]);
            Assert.Equal("b,c", result[1]);
            Assert.Equal("d", result[2]);
        }
        
        [Fact]
        public void ParseLine_ShouldHandleEscapedQuotes()
        {
            // Arrange
            var line = "a,\"b\"\"c\",d";
            
            // Act
            var result = CsvParser.ParseLine(line);
            
            // Assert
            Assert.Equal(3, result.Length);
            Assert.Equal("a", result[0]);
            Assert.Equal("b\"c", result[1]);
            Assert.Equal("d", result[2]);
        }
        
        [Theory]
        [InlineData("123.45", 123.45)]
        [InlineData("0.5", 0.5)]
        [InlineData("", 0.0)]
        [InlineData("invalid", 0.0)]
        public void ParseDouble_ShouldHandleVariousInputs(string input, double expected)
        {
            // Act
            var result = CsvParser.ParseDouble(input);
            
            // Assert
            Assert.Equal(expected, result);
        }
        
        [Theory]
        [InlineData("123", 123)]
        [InlineData("0", 0)]
        [InlineData("", 0)]
        [InlineData("invalid", 0)]
        public void ParseInt_ShouldHandleVariousInputs(string input, int expected)
        {
            // Act
            var result = CsvParser.ParseInt(input);
            
            // Assert
            Assert.Equal(expected, result);
        }
    }
}
