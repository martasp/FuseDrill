using FuseDrill.Core;

namespace ListPermutationExtension.Tests
{
    public class PermutationGeneratorTests
    {
        [Fact]
        public void GetPermutationsOfTwo_WithListOfFourElements_Returns12Permutations()
        {
            var list = new List<int> { 1, 2, 3, 4 };
            var result = PermutationGenerator.GetPermutationsOfTwo(list);

            Assert.Equal(12, result.Count);
            Assert.Contains(new List<int> { 1, 2 }, result);
            Assert.Contains(new List<int> { 2, 1 }, result);
            Assert.Contains(new List<int> { 3, 4 }, result);
            // Additional assertions can be added as needed
        }

        [Fact]
        public void GetPermutationsOfTwo_WithEmptyList_ReturnsEmptyResult()
        {
            var list = new List<int>();
            var result = PermutationGenerator.GetPermutationsOfTwo(list);

            Assert.Empty(result);
        }

        [Fact]
        public void GetPermutationsOfTwo_WithSingleElementList_ReturnsEmptyResult()
        {
            var list = new List<int> { 1 };
            var result = PermutationGenerator.GetPermutationsOfTwo(list);

            Assert.Empty(result);
        }


        [Fact]
        public void GetPermutations_Of_Null_Should_Return_Empty()
        {
            List<int>? nullList = null;
            var result = PermutationGenerator.GetPermutations(nullList);
            Assert.Empty(result); // Expect an empty result for null input
        }

        [Fact]
        public void GetPermutations_Of_Empty_List_Should_Return_Empty()
        {
            var result = PermutationGenerator.GetPermutations(new List<int>());
            Assert.Empty(result); // Expect an empty result for an empty list
        }

        [Fact]
        public void GetPermutations_Of_Single_Element_Should_Return_Same_Element()
        {
            var list = new List<int> { 1 };
            var result = PermutationGenerator.GetPermutations(list);
            Assert.Single(result); // Expect one permutation
            Assert.Equal([1], result[0]); // Expect the same single element
        }

        [Fact]
        public void GetPermutations_Of_Two_Elements_Should_Return_Two_Permutations()
        {
            var list = new List<int> { 1, 2 };
            var result = PermutationGenerator.GetPermutations(list);
            Assert.Equal(2, result.Count); // Expect two permutations
            Assert.Contains([1, 2], result);
            Assert.Contains([2, 1], result);
        }

        [Fact]
        public void GetPermutations_Of_Three_Elements_Should_Return_Six_Permutations()
        {
            var list = new List<int> { 1, 2, 3 };
            var result = PermutationGenerator.GetPermutations(list);
            Assert.Equal(6, result.Count); // Expect six permutations

            var expectedPermutations = new List<List<int>>
            {
                new List<int> { 1, 2, 3 },
                new List<int> { 1, 3, 2 },
                new List<int> { 2, 1, 3 },
                new List<int> { 2, 3, 1 },
                new List<int> { 3, 1, 2 },
                new List<int> { 3, 2, 1 }
            };

            foreach (var expected in expectedPermutations)
            {
                Assert.Contains(expected, result); // Ensure all expected permutations are present
            }
        }
    }
}
