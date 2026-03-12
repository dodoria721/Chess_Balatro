using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    Item[] items;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);

        //CheckItems(); // 추가된 함수 호출
    }

    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.Instance.Stop();
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.Instance.Resume();
    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

    void Next()
    {
        // 1. 모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        // 2. 그 중에서 랜덤 3개 아이템 활성화
        int[] random = new int[3];
        while (true)
        {
            random[0] = Random.Range(0, items.Length);
            random[1] = Random.Range(0, items.Length);
            random[2] = Random.Range(0, items.Length);

            if (random[0] != random[1] && random[1] != random[2] && random[2] != random[0])
                break;
        }

        for (int index = 0; index < random.Length; index++)
        {
            Item randomItem = items[random[index]];

            // 3. 최고 레벨일 경우 등장시키지 않기 (미구현)
            /*
             * 모든 아이템을 다 획득할 정도로 레벨업을 할 수 있다면 비어있는 칸에 해당하는 아이템을 부여 해야되고
             * 아이템을 적당히 획득할 정도로 레벨업 한다면 등장만 안하면 됨
             */
            if (randomItem.level == randomItem.data.damages.Length)
            {

            }
            else
            {
                randomItem.gameObject.SetActive(true);
            }
        }

        
    }

    private void CheckItems()
    {
        Debug.Log("Checking all items in the LevelUp array...");
        for (int i = 0; i < items.Length; i++)
        {
            Item item = items[i];
            if (item != null)
            {
                // 각 아이템의 데이터를 콘솔에 출력
                Debug.Log($"Item Index: {i}, Name: {item.name}, Item ID: {item.data.itemId}, Current Level: {item.level}");
            }
            else
            {
                Debug.Log($"Item Index: {i} is null.");
            }
        }
    }

}
