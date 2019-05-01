﻿//--------------------------------------------------------------------------------------------------
// <copyright file="PreconditionTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ValidationTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Correctness;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class PreconditionTests
    {
        [Fact]
        internal void True_WhenFalse_Throws()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.True(false, "some_evaluation"));
        }

        [Fact]
        internal void True_WhenTrue_ReturnsNull()
        {
            // Arrange
            // Act
            // Assert
            Precondition.True(true, "some_evaluation");
        }

        [Fact]
        internal void TrueIf_WhenConditionTrueAndPredicateFalse_Throws()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.TrueIf(true, false, "some_evaluation"));
        }

        [Fact]
        internal void TrueIf_WhenConditionTrueAndPredicateTrue_DoesNothing()
        {
            // Arrange
            // Act
            // Assert
            Precondition.TrueIf(true, true, "some_evaluation");
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
            Assert.Throws<ArgumentException>(() => Precondition.NotEmptyOrWhiteSpace(value, nameof(value)));
        }

        [Fact]
        internal void NotNull_WithAString_DoesNothing()
        {
            // Arrange
            var obj = "something";

            // Act
            // Assert
            Precondition.NotEmptyOrWhiteSpace(obj, nameof(obj));
        }

        [Fact]
        internal void NotNullTee_WithAnObject_DoesNothing()
        {
            // Arrange
            object obj = new EventArgs();

            // Act
            // Assert
            Precondition.NotNull(obj, nameof(obj));
        }

        [Fact]
        internal void NotDefault_WithNotDefaultStruct_DoesNothing()
        {
            // Arrange
            var obj = new TimeSpan(0, 0, 1);

            // Act
            // Assert
            Precondition.NotDefault(obj, nameof(obj));
        }

        [Fact]
        internal void NotDefault_WithDefaultStruct_Throws()
        {
            // Arrange
            var point = default(TimeSpan);

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.NotDefault(point, nameof(point)));
        }

        [Fact]
        internal void Empty_WhenCollectionEmpty_DoesNothing()
        {
            // Arrange
            var collection = new List<string>();

            // Act
            // Assert
            Precondition.Empty(collection, nameof(collection));
        }

        [Fact]
        internal void Empty_WhenCollectionNotEmpty_Throws()
        {
            // Arrange
            var collection = new List<string> { "anElement" };

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.Empty(collection, nameof(collection)));
        }

        [Fact]
        internal void NotEmpty_WhenCollectionNotEmpty_DoesNothing()
        {
            // Arrange
            var collection = new List<string> { "foo" };

            // Act
            // Assert
            Precondition.NotEmpty(collection, nameof(collection));
        }

        [Fact]
        internal void NotEmpty_WhenCollectionEmpty_Throws()
        {
            // Arrange
            var collection = new List<string>();

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.NotEmpty(collection, nameof(collection)));
        }

        [Fact]
        internal void NotEmpty_WhenDictionaryEmpty_Throws()
        {
            // Arrange
            var dictionary = new Dictionary<string, int>();

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.NotEmpty(dictionary, nameof(dictionary)));
        }

        [Fact]
        internal void IsIn_WhenCollectionDoesNotContainElement_Throws()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string>();

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.IsIn(element,  collection, nameof(element), nameof(collection)));
        }

        [Fact]
        internal void IsIn_WhenCollectionContainsElement_DoesNothing()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string> { element };

            // Act
            // Assert
            Precondition.IsIn(element, collection, nameof(element), nameof(collection));
        }

        [Fact]
        internal void NotIn_WhenCollectionDoesNotContainElement_DoesNothing()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string>();

            // Act
            // Assert
            Precondition.NotIn(element, collection, nameof(element), nameof(collection));
        }

        [Fact]
        internal void NotIn_WhenCollectionContainsElement_Throws()
        {
            // Arrange
            var element = "the_fifth_element";
            var collection = new List<string> { element };

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.NotIn(element, collection, nameof(element), nameof(collection)));
        }

        [Fact]
        internal void KeyIn_WhenDictionaryDoesContainKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            var dictionary = new Dictionary<string, int> { { key, 1 } };

            // Act
            // Assert
            Precondition.KeyIn(key, dictionary, nameof(key), nameof(dictionary));
        }

        [Fact]
        internal void KeyIn_WhenDictionaryContainsKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            var dictionary = new Dictionary<string, int> { { key, 0 } };

            // Act
            // Assert
            Precondition.KeyIn(key, dictionary, nameof(key), nameof(dictionary));
        }

        [Fact]
        internal void KeyIn_WhenDictionaryDoesNotContainKey_Throws()
        {
            // Arrange
            var key = "the_key";
            var dictionary = new Dictionary<string, int>();

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.KeyIn(key, dictionary, nameof(key), nameof(dictionary)));
        }

        [Fact]
        internal void KeyNotIn_WhenDictionaryDoesNotContainsKey_DoesNothing()
        {
            // Arrange
            var key = "the_key";
            var dictionary = new Dictionary<string, int> { { "another_key", 2 } };

            // Act
            // Assert
            Precondition.KeyNotIn(key, dictionary, nameof(key), nameof(dictionary));
        }

        [Fact]
        internal void KeyNotIn_WhenDictionaryContainsKey_Throws()
        {
            // Arrange
            var key = "the_key";
            var dictionary = new Dictionary<string, int> { { key, 1 } };

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.KeyNotIn(key, dictionary, nameof(key), nameof(dictionary)));
        }

        [Fact]
        internal void EqualTo_ValuesAreEqual_DoesNothing()
        {
            // Arrange
            var object1 = 1;
            var object2 = 1;

            // Act
            // Assert
            Precondition.EqualTo(object1, object2, nameof(object1));
        }

        [Fact]
        internal void EqualTo_ObjectsAreEqual_DoesNothing()
        {
            // Arrange
            var object1 = "object";
            var object2 = "object";

            // Act
            // Assert
            Precondition.EqualTo(object1, nameof(object1), object2);
        }

        [Fact]
        internal void EqualTo_ValuesAreNotEqual_Throws()
        {
            // Arrange
            var object1 = 1;
            var object2 = 2;

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.EqualTo(object1, object2, nameof(object1)));
        }

        [Fact]
        internal void EqualTo_ObjectsAreNotEqual_Throws()
        {
            // Arrange
            var object1 = "object1";
            var object2 = "object2";

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.EqualTo(object1, object2, nameof(object1)));
        }

        [Fact]
        internal void NotEqualTo_ValuesAreNotEqual_DoesNothing()
        {
            // Arrange
            var object1 = 1;
            var object2 = 2;

            // Act
            // Assert
            Precondition.NotEqualTo(object1, object2, nameof(object1));
        }

        [Fact]
        internal void NotEqualTo_ObjectsAreNotEqual_DoesNothing()
        {
            // Arrange
            var object1 = "object1";
            var object2 = "object2";

            // Act
            // Assert
            Precondition.NotEqualTo(object1, nameof(object1), object2);
        }

        [Fact]
        internal void NotEqualTo_ValuesAreEqual_Throws()
        {
            // Arrange
            var object1 = 1;
            var object2 = 1;

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.NotEqualTo(object1, object2, nameof(object1)));
        }

        [Fact]
        internal void NotEqualTo_ObjectsAreEqual_Throws()
        {
            // Arrange
            var object1 = "object";
            var object2 = "object";

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.NotEqualTo(object1, nameof(object1), object2));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        internal void PositiveInt32_VariousPositiveValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Precondition.PositiveInt32(value, nameof(value));
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        internal void PositiveInt32_VariousNotPositiveValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.PositiveInt32(value, nameof(value)));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        internal void NotNegativeInt32_VariousNotNegativeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Precondition.NotNegativeInt32(value, nameof(value));
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        internal void NotNegativeInt32_VariousNegativeValues_Throws(int value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.NotNegativeInt32(value, nameof(value)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        internal void PositiveInt64_VariousPositiveValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Precondition.PositiveInt64(value, nameof(value));
        }

        [Theory]
        [InlineData(long.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        internal void PositiveInt64_VariousNotPositiveValues_Throws(long value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.PositiveInt64(value, nameof(value)));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(long.MaxValue)]
        internal void NotNegativeInt64_VariousNotNegativeValues_DoesNothing(long value)
        {
            // Arrange
            // Act
            // Assert
            Precondition.NotNegativeInt64(value, nameof(value));
        }

        [Theory]
        [InlineData(long.MinValue)]
        [InlineData(-1)]
        internal void NotNegativeInt64_VariousNegativeValues_Throws(long value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.NotNegativeInt64(value, nameof(value)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(double.MaxValue)]
        internal void PositiveDouble_VariousPositiveValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Precondition.PositiveDouble(value, nameof(value));
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        internal void PositiveDouble_VariousNotPositiveValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.PositiveDouble(value, nameof(value)));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(double.MaxValue)]
        internal void NotNegativeDouble_VariousNotNegativeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Precondition.NotNegativeDouble(value, nameof(value));
        }

        [Theory]
        [InlineData(double.MinValue)]
        [InlineData(-1)]
        internal void NotNegativeDouble_VariousNegativeValues_Throws(double value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.NotNegativeDouble(value, nameof(value)));
        }

        [Theory]
        [InlineData(0.1)]
        [InlineData(1.0)]
        internal void PositiveDecimal_VariousPositiveValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Precondition.PositiveDecimal(value, nameof(value));
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(-1.0)]
        internal void PositiveDecimal_VariousNotPositiveValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.PositiveDecimal(value, nameof(value)));
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(1)]
        internal void NotNegativeDecimal_VariousNotNegativeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Precondition.NotNegativeDecimal(value, nameof(value));
        }

        [Theory]
        [InlineData(-0.1)]
        [InlineData(-1.0)]
        internal void NotNegativeDecimal_VariousNegativeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.NotNegativeDecimal(value, nameof(value)));
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
            Precondition.NotOutOfRangeInt32(value,  0, 3, nameof(value));
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
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeInt32(value, 0, 1, nameof(value)));
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
            Precondition.NotOutOfRangeInt32(value, 0, 3, nameof(value));
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
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeInt32(value, 0, 1, nameof(value)));
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
            Precondition.NotOutOfRangeInt32(value, 0, 3, nameof(value));
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
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeInt32(value, 0, 2, nameof(value)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt32_VariousInExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Precondition.NotOutOfRangeInt32(value, 0, 3, nameof(value));
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
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeInt32(value, 0, 1, nameof(value)));
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
            Precondition.NotOutOfRangeInt64(value, 0, 3, nameof(value));
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
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeInt64(value, 0, 1, nameof(value)));
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
            Precondition.NotOutOfRangeInt64(value, 0, 3, nameof(value));
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
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeInt64(value, 0, 1, nameof(value)));
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
            Precondition.NotOutOfRangeInt64(value, 0, 3, nameof(value));
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
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeInt64(value, 0, 2, nameof(value)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        internal void NotOutOfRangeInt64_VariousInExclusiveRangeValues_DoesNothing(int value)
        {
            // Arrange
            // Act
            // Assert
            Precondition.NotOutOfRangeInt64(value, 0, 3, nameof(value));
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
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeInt64(value, 0, 1, nameof(value)));
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
            Precondition.NotOutOfRangeDouble(value, 1, 2, nameof(value));
        }

        [Theory]
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
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeDouble(value, 1, 2, nameof(value)));
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
            Precondition.NotOutOfRangeDouble(value, 1, 2, nameof(value));
        }

        [Theory]
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
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeDouble(value, 1, 2, nameof(value)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1.9)]
        internal void NotOutOfRangeDouble_VariousInUpperExclusiveRangeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Precondition.NotOutOfRangeDouble(value, 1, 2, nameof(value));
        }

        [Theory]
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
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeDouble(value, 1, 2, nameof(value)));
        }

        [Theory]
        [InlineData(1.0000000001)]
        [InlineData(1.9999999999)]
        internal void NotOutOfRangeDouble_VariousInExclusiveRangeValues_DoesNothing(double value)
        {
            // Arrange
            // Act
            // Assert
            Precondition.NotOutOfRangeDouble(value, 1, 2, nameof(value));
        }

        [Theory]
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
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeDouble(value, 1, 2, nameof(value)));
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
            Precondition.NotOutOfRangeDecimal(value, 1, 2, nameof(value));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2.1)]
        internal void NotOutOfRangeDecimal_VariousOutOfInclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeDecimal(value, 1, 2, nameof(value)));
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
            Precondition.NotOutOfRangeDecimal(value, 1, 2, nameof(value));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2.1)]
        internal void NotOutOfRangeDecimal_VariousOutOfLowerExclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeDecimal(value, 1, 2, nameof(value)));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1.9)]
        internal void NotOutOfRangeDecimal_VariousInUpperExclusiveRangeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Precondition.NotOutOfRangeDecimal(value, 1, 2, nameof(value));
        }

        [Theory]
        [InlineData(0.9)]
        [InlineData(2)]
        internal void NotOutOfRangeDecimal_VariousOutOfUpperExclusiveRangeValues_Throws(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeDecimal(value, 1, 2, nameof(value)));
        }

        [Theory]
        [InlineData(1.000000001)]
        [InlineData(1.999999999)]
        internal void NotOutOfRangeDecimal_VariousInExclusiveRangeValues_DoesNothing(decimal value)
        {
            // Arrange
            // Act
            // Assert
            Precondition.NotOutOfRangeDecimal(value, 1, 2, nameof(value));
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
            Assert.Throws<ArgumentException>(() => Precondition.NotOutOfRangeDecimal(value, 1, 2, nameof(value)));
        }
    }
}