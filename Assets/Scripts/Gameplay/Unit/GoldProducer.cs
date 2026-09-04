using System.Collections;
using UnityEngine;

public class GoldProducer : MonoBehaviour
{
    private int goldPerInterval;
    private float interval;
    private Coroutine productionCoroutine;

    public void Initialize(int newGoldPerInterval, float newInterval)
    {
        goldPerInterval = newGoldPerInterval;
        interval = newInterval;
        if (productionCoroutine != null)
            StopCoroutine(productionCoroutine);
        if (goldPerInterval > 0 && interval > 0f)
            productionCoroutine = StartCoroutine(ProduceGold());
    }

    private void OnDisable()
    {
        StopCoroutine(productionCoroutine);
        productionCoroutine = null;
    }

    private IEnumerator ProduceGold()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            if (ResourceManager.Instance != null)
                ResourceManager.Instance.Add(ResourceType.Gold, goldPerInterval);
        }
    }
}
