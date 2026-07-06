namespace task03tests;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using task03;

public class IteratorTests
{
    [Fact]
    public void CustomCollection_GetEnumerator_ReturnsAllItems()
    {
        var collection = new CustomCollection<int>();
        collection.Add(1);
        collection.Add(2);

        var result = new List<int>();
        foreach (var item in collection)
        {
            result.Add(item);
        }

        Assert.Equal(new[] { 1, 2 }, result);
    }

    [Fact]
    public void GetReverseEnumerator_ReturnsItemsInReverseOrder()
    {
        var collection = new CustomCollection<int>();
        collection.Add(1);
        collection.Add(2);

        var result = collection.GetReverseEnumerator().ToList();
        Assert.Equal(new[] { 2, 1 }, result);
    }

    [Fact]
    public void GenerateSequence_ReturnsCorrectSequence()
    {
        var sequence = CustomCollection<int>.GenerateSequence(5, 3).ToList();
        Assert.Equal(new[] { 5, 6, 7 }, sequence);
    }

    [Fact]
    public void FilterAndSort_ReturnsFilteredAndSortedItems()
    {
        var collection = new CustomCollection<int>();
        collection.Add(3);
        collection.Add(1);
        collection.Add(2);

        var result = collection.FilterAndSort(x => x > 1, x => x).ToList();
        Assert.Equal(new[] { 2, 3 }, result);
    }

    [Fact]
    public void CustomCollection_RemoveItem_RemovesSuccessfully()
    {
        var collection = new CustomCollection<int>();
        collection.Add(1);
        collection.Add(2);
        
        var removed = collection.Remove(1);
        
        Assert.True(removed);
        Assert.Single(collection);
        Assert.Equal(2, collection.First());
    }

    [Fact]
    public void CustomCollection_Count_ReturnsCorrectCount()
    {
        var collection = new CustomCollection<int>();
        collection.Add(1);
        collection.Add(2);
        collection.Add(3);
        
        Assert.Equal(3, collection.Count);
    }

    [Fact]
    public void GetReverseEnumerator_EmptyCollection_ReturnsEmptySequence()
    {
        var collection = new CustomCollection<string>();
        
        var result = collection.GetReverseEnumerator().ToList();
        
        Assert.Empty(result);
    }

    [Fact]
    public void GetReverseEnumerator_SingleElement_ReturnsSameElement()
    {
        var collection = new CustomCollection<string>();
        collection.Add("тест");
        
        var result = collection.GetReverseEnumerator().ToList();
        
        Assert.Single(result);
        Assert.Equal("тест", result[0]);
    }

    [Fact]
    public void GenerateSequence_StartZero_ReturnsCorrectSequence()
    {
        var sequence = CustomCollection<int>.GenerateSequence(0, 4).ToList();
        
        Assert.Equal(new[] { 0, 1, 2, 3 }, sequence);
    }

    [Fact]
    public void GenerateSequence_CountZero_ReturnsEmptySequence()
    {
        var sequence = CustomCollection<int>.GenerateSequence(10, 0).ToList();
        
        Assert.Empty(sequence);
    }

    [Fact]
    public void GenerateSequence_NegativeCount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            CustomCollection<int>.GenerateSequence(5, -1).ToList());
    }

    [Fact]
    public void FilterAndSort_NullPredicate_ThrowsArgumentNullException()
    {
        var collection = new CustomCollection<int>();
        
        Assert.Throws<ArgumentNullException>(() => 
            collection.FilterAndSort(null, x => x).ToList());
    }

    [Fact]
    public void FilterAndSort_NullKeySelector_ThrowsArgumentNullException()
    {
        var collection = new CustomCollection<int>();
        
        Assert.Throws<ArgumentNullException>(() => 
            collection.FilterAndSort(x => true, null).ToList());
    }
    [Fact]
    public void FilterAndSort_NoMatchingItems_ReturnsEmptySequence()
    {
        var collection = new CustomCollection<int>();
        collection.Add(1);
        collection.Add(2);
        collection.Add(3);
        
        var result = collection.FilterAndSort(x => x > 5, x => x).ToList();
        
        Assert.Empty(result);
    }
    [Fact]
    public void FilterAndSort_WithStringCollection_WorksCorrectly()
    {
        var collection = new CustomCollection<string>();
        collection.Add("чебурек");
        collection.Add("самса");
        collection.Add("эчпочмак");
        
        var result = collection.FilterAndSort(x => x.Length > 4, x => x).ToList();

        Assert.Equal(3, result.Count);
        Assert.Contains("самса", result);
        Assert.Contains("чебурек", result);
        Assert.Contains("эчпочмак", result);

        Assert.True(string.Compare(result[0], result[1]) <= 0);
        Assert.True(string.Compare(result[1], result[2]) <= 0);
        }
}