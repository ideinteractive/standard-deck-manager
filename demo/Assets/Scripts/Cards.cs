using System.Collections.Generic;

/// <summary>
/// Cards
/// Description: Properties for each individual card.
/// </summary>

[System.Serializable]
public class Cards
{
    public string suit;
    public string color;
    public string rank;
}

/// <summary>
/// ListOfCards
/// Description: Contains a list of cards.
/// </summary>

[System.Serializable]
public class ListOfCards
{
    public List<Cards> deck;
}