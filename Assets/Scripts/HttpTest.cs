using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class Position
{
    public int x;
    public int y;
}

[Serializable]
class GetUserBagItemsResult
{
    public string ID;
    public string Name;
    public string Quantity;
    public int CurrentDurabilityPoints;
    public Position Position;
}

class DeleteItemInBagBody : Gaenodap.HttpBody
{
    public int id;
    public int decrementCount;

    public DeleteItemInBagBody(int id, int decrementCount)
    {
        this.id = id;
        this.decrementCount = decrementCount;
    }
}

[Serializable]
class DeleteItemInBagResult
{
    public int id;
    public int decrementCount;
}

class AddItemToBagBody : Gaenodap.HttpBody
{
    public int id;
    public string name;
    public string type;
    public int x;
    public int y;

    public AddItemToBagBody(int id, string name, string type, int x, int y)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.x = x;
        this.y = y;
    }
}

[Serializable]
class AddItemToBagResult
{
    public int id;
}

[Serializable]
class ItemStatistics
{
    public int ArmorRating;
    public int DamageValue;
    public int RestorationAmount;
    public int MaxDurabilityPoints;
    public int SellValue;
}

[Serializable]
class ItemTextData
{
    public string Name;
    public string Description;
    public string InteractionText;
    public string UsageText;
}

[Serializable]
class ItemNumericData
{
    public double Weight;
    public int MaxStackSize;
    public bool bIsStackable;
}

[Serializable]
class GetItemListResult
{
    public string Name;
    public string ID;
    public string ItemType;
    public string ItemQuality;
    public ItemStatistics ItemStatistics;
    public ItemTextData ItemTextData;
    public ItemNumericData ItemNumericData;
}

public class HttpTest : MonoBehaviour
{
    private void Start()
    {
        string userId = "test";

        // getUserBagItems api
        Gaenodap.Http.Client.Request<GetUserBagItemsResult[]>(
            this,
            $"/keepout/api/test/{userId}/inventory",
            Gaenodap.Http.Method.GET,
            header: new Dictionary<string, string>()
            {
                { "Authorization", "test" }
            },
            onSuccess: result =>
            {
                Debug.Log("status code = " + result.statusCode);
                Debug.Log("message = " + result.msg);
                int i = 1;
                foreach (GetUserBagItemsResult item in result.data)
                {
                    Debug.Log(i + "th");
                    Debug.Log("\t ID = " + item.ID);
                    Debug.Log("\t Name = " + item.Name);
                    Debug.Log("\t Quantity = " + item.Quantity);
                    Debug.Log("\t CurrentDurabilityPoints = " + item.CurrentDurabilityPoints);
                    Debug.Log("\t x = " + item.Position.x);
                    Debug.Log("\t y = " + item.Position.y);
                    i++;
                }
            },
            onFailure: Fail);

        // deleteItemInBag api
        Gaenodap.Http.Client.Request<DeleteItemInBagResult>(
            this,
            "/keepout/api/test/inventory/item",
            Gaenodap.Http.Method.DELETE,
            header: new Dictionary<string, string>()
            {
                { "Authorization", "test" }
            },
            body: new DeleteItemInBagBody(
                id: 1,
                decrementCount: 1
            ),
            onSuccess: result =>
            {
                Debug.Log("status code = " + result.statusCode);
                Debug.Log("message = " + result.msg);

                Debug.Log("item id = " + result.data.id);
                Debug.Log("decrementCount = " + result.data.decrementCount);
            },
            onFailure: Fail);

        // addItemToBag api
        Gaenodap.Http.Client.Request<AddItemToBagResult>(
            this,
            "/keepout/api/test/inventory/item",
            Gaenodap.Http.Method.POST,
            header: new Dictionary<string, string>()
            {
                { "Authorization", "test" }
            },
            body: new AddItemToBagBody(
                id: 1,
                name: "Á¤Âû Á¶³¢",
                type: "armor",
                x: 0,
                y: 0
            ),
            onSuccess: result =>
            {
                Debug.Log("status code = " + result.statusCode);
                Debug.Log("message = " + result.msg);

                Debug.Log("item id = " + result.data.id);
            },
            onFailure: Fail);

        // getItemList api
        Gaenodap.Http.Client.Request<GetItemListResult[]>(
            this,
            "/keepout/api/itemList",
            Gaenodap.Http.Method.GET,
            onSuccess: result =>
            {
                Debug.Log("status code = " + result.statusCode);
                Debug.Log("message = " + result.msg);
                int i = 1;
                foreach (GetItemListResult item in result.data)
                {
                    Debug.Log(i + "th");
                    Debug.Log("\t Name = " + item.Name);
                    Debug.Log("\t ID = " + item.ID);
                    Debug.Log("\t Item Type = " + item.ItemType);
                    Debug.Log("\t Item Quality = " + item.ItemQuality);

                    Debug.Log("\t item staticstics armor rating = " + item.ItemStatistics.ArmorRating);
                    Debug.Log("\t item staticstics damage value = " + item.ItemStatistics.DamageValue);
                    Debug.Log("\t item staticstics restoration amount = " + item.ItemStatistics.RestorationAmount);
                    Debug.Log("\t item staticstics max durability points = " + item.ItemStatistics.MaxDurabilityPoints);
                    Debug.Log("\t item staticstics sell value = " + item.ItemStatistics.SellValue);

                    Debug.Log("\t item text data name = " + item.ItemTextData.Name);
                    Debug.Log("\t item text data description = " + item.ItemTextData.Description);
                    Debug.Log("\t item text data interaction text = " + item.ItemTextData.InteractionText);
                    Debug.Log("\t item text data usage text = " + item.ItemTextData.UsageText);

                    Debug.Log("\t item numeric data weight = " + item.ItemNumericData.Weight);
                    Debug.Log("\t item numeric data max stack size = " + item.ItemNumericData.MaxStackSize);
                    Debug.Log("\t item numeric data is stackable = " + item.ItemNumericData.bIsStackable);
                    i++;
                }
            },
            onFailure: Fail);
    }

    private void Success(string result)
    {
        Debug.Log("Success: " + result);
    }

    private void Fail(string result)
    {
        Debug.Log("Fail: " + result);
    }

    private void Update()
    {
        
    }
}