﻿//--------------------------------------------------------------------------------------------------
// <copyright file="DebugTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ValidationTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Validation;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class DebugTests
    {
        [Fact]
        internal void True_WhenFalse_Throws()
        {
            // Arrange
            // Act
            // Assert
            var ex = Assert.Throws<ValidationException>(() => Debug.True(false, "some_evaluation"));

            Assert.Equal(typeof(ArgumentException), ex.InnerException.GetType());
        }

        [Fact]
        internal void True_WhenTrue_ReturnsNull()
        {
            // Arrange
            // Act
            // Assert
            Debug.True(true, "some_evaluation");
        }

        [Fact]
        internal void TrueIf_WhenConditionTrueAndPredicateFalse_Throws()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.TrueIf(true, false, "some_evaluation"));
        }

        [Fact]
        internal void TrueIf_WhenConditionTrueAndPredicateTrue_DoesNothing()
        {
            // Arrange
            // Act
            // Assert
            Debug.TrueIf(true, true, "some_evaluation");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        internal void NotNull_WithVariousObjectAndInvalidStrings_Throws(string value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotNull(value, nameof(value)));
        }

        [Fact]
        internal void NotNull_WithAString_DoesNothing()
        {
            // Arrange
            const string obj = "something";

            // Act
            // Assert
            Debug.NotNull(obj, nameof(obj));
        }

        [Fact]
        internal void NotNullTee_WithNullObject_Throws()
        {
            // Arrange
            object obj = null;

            // Act
            // Assert - Ignore expression is always null warning (the point of the test is to catch this condition).
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ValidationException>(() => Debug.NotNull(obj, nameof(obj)));
        }

        [Fact]
        internal void NotNullTee_WithAnObject_DoesNothing()
        {
            // Arrange
            object obj = new EventArgs();

            // Act
            // Assert
            Debug.NotNull(obj, nameof(obj));
        }

        [Fact]
        internal void NotDefault_WithNotDefaultStruct_DoesNothing()
        {
            // Arrange
            var obj = new TimeSpan(0, 0, 1);

            // Act
            // Assert
            Debug.NotDefault(obj, nameof(obj));
        }

        [Fact]
        internal void NotDefault_WithDefaultStruct_Throws()
        {
            // Arrange
            var point = TimeSpan.Zero;

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotDefault(point, nameof(point)));
        }

        [Fact]
        internal void Empty_WhenCollectionEmpty_DoesNothing()
        {
            // Arrange
            var collection = new List<string>();

            // Act
            // Assert
            Debug.Empty(collection, nameof(collection));
        }

        [Fact]
        internal void Empty_WhenCollectionNotEmpty_Throws()
        {
            // Arrange
            List<string> collection = new List<string> { "anElement" };

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.Empty(collection, nameof(collection)));
        }

        [Fact]
        internal void Empty_WhenCollectionNull_Throws()
        {
            // Arrange
            List<string> collection = null;

            // Act
            // Assert - Ignore expression is always null warning (the point of the test is to catch this condition).
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ValidationException>(() => Debug.Empty(collection, nameof(collection)));
        }

        [Fact]
        internal void NotNullOrEmpty_WhenCollectionNotEmpty_DoesNothing()
        {
            // Arrange
            var collection = new List<string> { "foo" };

            // Act
            // Assert
            Debug.NotNullOrEmpty(collection, nameof(collection));
        }

        [Fact]
        internal void NotNullOrEmpty_WhenCollectionNull_Throws()
        {
            // Arrange
            List<string> collection = null;

            // Act
            // Assert - Ignore expression is always null warning (the point of the test is to catch this condition).
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ValidationException>(() => Debug.NotNullOrEmpty(collection, nameof(collection)));
        }

        [Fact]
        internal void NotNullOrEmpty_WhenCollectionEmpty_Throws()
        {
            // Arrange
            var collection = new List<string>();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotNullOrEmpty(collection, nameof(collection)));
        }

        [Fact]
        internal void NotNullOrEmpty_WhenDictionaryNull_Throws()
        {
            // Arrange
            Dictionary<string, int> dictionary = null;

            // Act
            // Assert - Ignore expression is always null warning (the point of the test is to catch this condition).
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ValidationException>(() => Debug.NotNullOrEmpty(dictionary, nameof(dictionary)));
        }

        [Fact]
        internal void NotNullOrEmpty_WhenDictionaryEmpty_Throws()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotNullOrEmpty(dictionary, nameof(dictionary)));
        }

        [Fact]
        internal void Contains_WhenCollectionDoesNotContainElement_Throws()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string>();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.Contains(element, nameof(element), collection));
        }

        [Fact]
        internal void Contains_WhenCollectionContainsElement_DoesNothing()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string> { element };

            // Act
            // Assert
            Debug.Contains(element, nameof(element), collection);
        }

        [Fact]
        internal void DoesNotContain_WhenCollectionDoesNotContainElement_DoesNothing()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string>();

            // Act
            // Assert
            Debug.DoesNotContain(element, nameof(element), collection);
        }

        [Fact]
        internal void DoesNotContain_WhenCollectionContainsElement_Throws()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string> { element };

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.DoesNotContain(element, nameof(element), collection));
        }

        [Fact]
        internal void ContainsKey_WhenDictionaryDoesContainKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            var collection = new Dictionary<string, int> { { key, 1 } };

            // Act
            // Assert
            Debug.ContainsKey(key, nameof(key), collection);
        }

        [Fact]
        internal void ContainsKey_WhenDictionaryContainsKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            var collection = new Dictionary<string, int> { { key, 0 } };

            // Act
            // Assert
            Debug.ContainsKey(key, nameof(key), collection);
        }

        [Fact]
        internal void ContainsKey_WhenDictionaryDoesNotContainKey_Throws()
        {
            // Arrange
            var key = "the_key";
            var collection = new Dictionary<string, int>();

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.ContainsKey(key, nameof(key), collection));
        }

        [Fact]
        internal void DoesNotContainKey_WhenDictionaryDoesNotContainsKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            var collection = new Dictionary<string, int> { { "another_key", 2 } };

            // Act
            // Assert
            Debug.DoesNotContainKey(key, nameof(key), collection);
        }

        [Fact]
        internal void DoesNotContainKey_WhenDictionaryContainsKey_Throws()
        {
            // Arrange
            var key = "the_key";
            var collection = new Dictionary<string, int> { { key, 1 } };

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.DoesNotContainKey(key, nameof(key), collection));
        }

        [Fact]
        internal void EqualTo_ValuesAreEqual_DoesNothing()
        {
            // Arrange
            var object1 = 1;
            var object2 = 1;

            // Act
            // Assert
            Debug.EqualTo(object1, nameof(object1), object2);
        }

        [Fact]
        internal void EqualTo_ObjectsAreEqual_DoesNothing()
        {
            // Arrange
            var object1 = "object";
            var object2 = "object";

            // Act
            // Assert
            Debug.EqualTo(object1, nameof(object1), object2);
        }

        [Fact]
        internal void EqualTo_ValuesAreNotEqual_Throws()
        {
            // Arrange
            var object1 = 1;
            var object2 = 2;

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.EqualTo(object1, nameof(object1), object2));
        }

        [Fact]
        internal void EqualTo_ObjectsAreNotEqual_Throws()
        {
            // Arrange
            var object1 = "object1";
            var object2 = "object2";

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.EqualTo(object1, nameof(object1), object2));
        }

        [Fact]
        internal void NotEqualTo_ValuesAreNotEqual_DoesNothing()
        {
            // Arrange
            var object1 = 1;
            var object2 = 2;

            // Act
            // Assert
            Debug.NotEqualTo(object1, nameof(object1), object2);
        }

        [Fact]
        internal void NotEqualTo_ObjectsAreNotEqual_DoesNothing()
        {
            // Arrange
            var object1 = "object1";
            var object2 = "object2";

            // Act
            // Assert
            Debug.NotEqualTo(object1, nameof(object1), object2);
        }

        [Fact]
        internal void NotEqualTo_ValuesAreEqual_Throws()
        {
            // Arrange
            var object1 = 1;
            var object2 = 1;

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotEqualTo(object1, nameof(object1), object2));
        }

        [Fact]
        internal void NotEqualTo_ObjectsAreEqual_Throws()
        {
            // Arrange
            var object1 = "object";
            var object2 = "object";

            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotEqualTo(object1, nameof(object1), object2));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        internal void NotOutOfRangeInt32_VariousInInclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotOutOfRangeDecimal(value, nameof(value), 0, 3);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt32_VariousOutOfInclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeDecimal(value, nameof(value), 0, 1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        internal void NotOutOfRangeInt32_VariousInLowerExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotOutOfRangeDecimal(value, nameof(value), 0, 3, RangeEndPoints.LowerExclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(0)]
        [InlineData(2)]
        internal void NotOutOfRangeInt32_VariousOutOfLowerExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeDecimal(value, nameof(value), 0, 1, RangeEndPoints.LowerExclusive));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt32_VariousInUpperExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotOutOfRangeDecimal(value, nameof(value), 0, 3, RangeEndPoints.UpperExclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt32_VariousOutOfUpperExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeDecimal(value, nameof(value), 0, 2, RangeEndPoints.UpperExclusive));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt32_VariousInExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotOutOfRangeDecimal(value, nameof(value), 0, 3, RangeEndPoints.Exclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt32_VariousOutOfExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeDecimal(value, nameof(value), 0, 1, RangeEndPoints.Exclusive));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        internal void NotOutOfRangeInt64_VariousInInclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Validate.NotOutOfRangeInt64(value, nameof(value), 0, 3);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt64_VariousOutOfInclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeInt64(value, nameof(value), 0, 1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        internal void NotOutOfRangeInt64_VariousInLowerExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotOutOfRangeInt64(value, nameof(value), 0, 3, RangeEndPoints.LowerExclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(0)]
        [InlineData(2)]
        internal void NotOutOfRangeInt64_VariousOutOfLowerExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeInt64(value, nameof(value), 0, 1, RangeEndPoints.LowerExclusive));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt64_VariousInUpperExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotOutOfRangeInt64(value, nameof(value), 0, 3, RangeEndPoints.UpperExclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt64_VariousOutOfUpperExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeInt64(value, nameof(value), 0, 2, RangeEndPoints.UpperExclusive));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt64_VariousInExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotOutOfRangeInt64(value, nameof(value), 0, 3, RangeEndPoints.Exclusive);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt64_VariousOutOfExclusiveRangeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeInt64(value, nameof(value), 0, 1, RangeEndPoints.Exclusive));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1.1)]
        [InlineData(1.9)]
        [InlineData(2)]
        internal void NotOutOfRangeDouble_VariousInInclusiveRangeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotOutOfRangeDouble(value, nameof(value), 1, 2);
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.Epsilon)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(0.99999999999)]
        [InlineData(2.00000000001)]
        internal void NotOutOfRangeDouble_VariousOutOfInclusiveRangeValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeDouble(value, nameof(value), 1, 2));
        }

        [Theory]
        [InlineData(1.1)]
        [InlineData(1.9)]
        [InlineData(2)]
        internal void NotOutOfRangeDouble_VariousInLowerExclusiveRangeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotOutOfRangeDouble(value, nameof(value), 1, 2, RangeEndPoints.LowerExclusive);
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.Epsilon)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(1)]
        [InlineData(2.00000000001)]
        internal void NotOutOfRangeDouble_VariousOutOfLowerExclusiveRangeValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeDouble(value, nameof(value), 1, 2, RangeEndPoints.LowerExclusive));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1.9)]
        internal void NotOutOfRangeDouble_VariousInUpperExclusiveRangeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotOutOfRangeDouble(value, nameof(value), 1, 2, RangeEndPoints.UpperExclusive);
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.Epsilon)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(0)]
        [InlineData(2)]
        internal void NotOutOfRangeDouble_VariousOutOfUpperExclusiveRangeValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeDouble(value, nameof(value), 1, 2, RangeEndPoints.UpperExclusive));
        }

        [Theory]
        [InlineData(1.0000000001)]
        [InlineData(1.9999999999)]
        internal void NotOutOfRangeDouble_VariousInExclusiveRangeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotOutOfRangeDouble(value, nameof(value), 1, 2, RangeEndPoints.Exclusive);
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.Epsilon)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        [InlineData(0.9)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(2.1)]
        internal void NotOutOfRangeDouble_VariousOutOfExclusiveRangeValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeDouble(value, nameof(value), 1, 2, RangeEndPoints.Exclusive));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        internal void NotInvalidNumber_ValueInBounds_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotInvalidNumber(value, nameof(value));
        }

        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        internal void NotInvalidNumber_VariousOutOfBoundsValuesAndValueAtBounds_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotInvalidNumber(value, nameof(value)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1.000000000000000000000000000000000001)]
        [InlineData(1.999999999999999999999999999999999999)]
        [InlineData(2)]
        internal void NotOutOfRangeDecimal_VariousInInclusiveRangeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotOutOfRangeDecimal(value, nameof(value), 1, 2);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2.1)]
        internal void NotOutOfRangeDecimal_VariousOutOfInclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeDecimal(value, nameof(value), 1, 2));
        }

        [Theory]
        [InlineData(1.1)]
        [InlineData(1.999999999999999999999999999999999999)]
        [InlineData(2)]
        internal void NotOutOfRangeDecimal_VariousInLowerExclusiveRangeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotOutOfRangeDecimal(value, nameof(value), 1, 2, RangeEndPoints.LowerExclusive);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2.1)]
        internal void NotOutOfRangeDecimal_VariousOutOfLowerExclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeDecimal(value, nameof(value), 1, 2, RangeEndPoints.LowerExclusive));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1.9)]
        internal void NotOutOfRangeDecimal_VariousInUpperExclusiveRangeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotOutOfRangeDecimal(value, nameof(value), 1, 2, RangeEndPoints.UpperExclusive);
        }

        [Theory]
        [InlineData(0.9)]
        [InlineData(2)]
        internal void NotOutOfRangeDecimal_VariousOutOfUpperExclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeDecimal(value, nameof(value), 1, 2, RangeEndPoints.UpperExclusive));
        }

        [Theory]
        [InlineData(1.000000001)]
        [InlineData(1.999999999)]
        internal void NotOutOfRangeDecimal_VariousInExclusiveRangeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Debug.NotOutOfRangeDecimal(value, nameof(value), 1, 2, RangeEndPoints.Exclusive);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.9)]
        [InlineData(2)]
        [InlineData(2.1)]
        internal void NotOutOfRangeDecimal_VariousOutOfExclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ValidationException>(() => Debug.NotOutOfRangeDecimal(value, nameof(value), 1, 2, RangeEndPoints.Exclusive));
        }
    }
}
