using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;

public class HorizontalCardHolder : MonoBehaviour
{
    [SerializeField] private Card _selectedCard;
    [SerializeReference] private Card _hoveredCard;

    [SerializeField] private GameObject _cardPrefab;
    private RectTransform rect;

    [Header("Spawn settings")]
    [SerializeField] private int _cardsToSpawn = 7;
    public List<Card> _cards;

    private bool isCrossing = false;
    [SerializeField] private bool _tweenCardReturn = true;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _cardsToSpawn; i++)
        {
            Instantiate(_cardPrefab, transform);
        }

        rect = GetComponent<RectTransform>();
        _cards = GetComponentsInChildren<Card>().ToList();

        int cardCount = 0;

        foreach (Card card in _cards)
        {
            card.PointerEnterEvent.AddListener(CardPointerEnter);
            card.PointerExitEvent.AddListener(CardPointerExit);
            card.BeginDragEvent.AddListener(BeginDrag);
            card.EndDragEvent.AddListener(EndDrag);
            card.name = cardCount.ToString();
            cardCount++;
        }
    }

    private void BeginDrag(Card card)
    {
        _selectedCard = card;
    }

    private void EndDrag(Card card)
    {
        
    }
    void CardPointerEnter(Card card)
    {
        _hoveredCard = card;
    }

    void CardPointerExit(Card card)
    {
        _hoveredCard = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
