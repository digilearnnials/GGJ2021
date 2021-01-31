using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MatchTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro = default;
    
    public UnityEvent onTimerEnd = new UnityEvent();

    private int matchDuration = 0;

    public void StartTimer(int seconds)
    {
        matchDuration = seconds;

        StartCoroutine(TimerCoroutine());
    }

    public void StopTimer()
    {
        StopAllCoroutines();
    }

    IEnumerator TimerCoroutine()
    {
        textMeshPro.text = $"Match Time: {matchDuration}";
        
        while (matchDuration > 0)
        {
            yield return new WaitForSeconds(1f);

            matchDuration--;
            
            textMeshPro.text = $"Match Time: {matchDuration}";
        }
        
        onTimerEnd?.Invoke();
    }
}
