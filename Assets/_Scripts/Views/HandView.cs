using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MineCombat;
using UnityEngine;
using UnityEngine.Splines;

// this scripts is dealing with the hand view and some animations
public class HandView : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;

    private readonly List<CardView> cards = new();

    public IEnumerator AddCard(CardView cardView)
    {
        if(cardView == null)
        {
            Debug.LogWarning("HandView AddCard called with null CardView.");
            yield return UpdateCardPositions(0.15f);
        }
        cards.Add(cardView);
        cardView.transform.SetParent(transform, false);
        yield return UpdateCardPositions(0.15f);
    }

    private IEnumerator UpdateCardPositions(float duration)
    {


        if (cards.Count == 0) yield break;
        float cardSpacing = 1f / 10f;
        float firstCardPosition = 0.5f - (cards.Count - 1) * cardSpacing / 2f;

        Spline spline = splineContainer.Spline;

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.position = new Vector3(
                cards[i].transform.position.x,
                cards[i].transform.position.y,
                0
            );

            float p = firstCardPosition + i * cardSpacing;
            Vector3 targetPosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(-up, Vector3.Cross(-up, forward).normalized);

            cards[i].transform.DOMove(targetPosition + transform.position + 0.01f * i * Vector3.back, duration);
            cards[i].transform.DORotate(rotation.eulerAngles, duration);
        }
        yield return new WaitForSeconds(duration);
    }

    /// <summary>
    /// 公开方法，刷新当前手牌布局
    /// </summary>
    public void RefreshLayout(float duration = 0.15f)
    {
        StopAllCoroutines(); // 停掉之前的动画，避免叠加
        StartCoroutine(UpdateCardPositions(duration));
    }
}