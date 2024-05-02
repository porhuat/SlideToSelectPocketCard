using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System.Linq;

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

        StartCoroutine(Frame());

        IEnumerator Frame()
        {
            yield return new WaitForSecondsRealtime(.1f);
            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i]._cardVisual != null)
                    _cards[i]._cardVisual.UpdateIndex(transform.childCount);
            }
        }
    }

    private void BeginDrag(Card card)
    {
        _selectedCard = card;
    }

    private void EndDrag(Card card)
    {
        if (_selectedCard == null)
            return;

        _selectedCard.transform.DOLocalMove(_selectedCard.selected ? new Vector3(0, _selectedCard.selectionOffset, 0) : Vector3.zero, _tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack);

        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        _selectedCard = null;
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
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (_hoveredCard != null)
            {
                Destroy(_hoveredCard.transform.parent.gameObject);
                _cards.Remove(_hoveredCard);

            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            foreach (Card card in _cards)
            {
                card.Deselect();
            }
        }

        if (_selectedCard == null)
            return;

        if (isCrossing)
            return;

        for (int i = 0; i < _cards.Count; i++)
        {

            if (_selectedCard.transform.position.x > _cards[i].transform.position.x)
            {
                if (_selectedCard.ParentIndex() < _cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }

            if (_selectedCard.transform.position.x < _cards[i].transform.position.x)
            {
                if (_selectedCard.ParentIndex() > _cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }
        }
    }

    void Swap(int index)
    {
        isCrossing = true;

        Transform focusedParent = _selectedCard.transform.parent;
        Transform crossedParent = _cards[index].transform.parent;

        _cards[index].transform.SetParent(focusedParent);
        _cards[index].transform.localPosition = _cards[index].selected ? new Vector3(0, _cards[index].selectionOffset, 0) : Vector3.zero;
        _selectedCard.transform.SetParent(crossedParent);

        isCrossing = false;

        if (_cards[index]._cardVisual == null)
            return;

        bool swapIsRight = _cards[index].ParentIndex() > _selectedCard.ParentIndex();
        _cards[index]._cardVisual.Swap(swapIsRight ? -1 : 1);

        //Updated Visual Indexes
        foreach (Card card in _cards)
        {
            card._cardVisual.UpdateIndex(transform.childCount);
        }
    }
}
