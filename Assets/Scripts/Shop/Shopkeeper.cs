using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shopkeeper : MonoBehaviour, IInteractable
{
    private ShopkeeperUIManager uiManager;
    private ShopkeeperAIManager aiManager;
    private TextAnalysis textAnalysis;

    [SerializeField] private Image shopkeeperSprite;

    [SerializeField] private List<ShopItemData> allShopItems;
    [SerializeField] private int shopItemAmount;

    [SerializeField] private SimpleAnimation keycodeE;
    [SerializeField] private bool isDebugging;

    private PlayerController player;

    [SerializeField] private int rerollCost;
    [SerializeField] private int incrementRerollCost;
    [SerializeField] private float incrementModifier;

    [System.Serializable]
    public struct Expression
    {
        public enum ExpressionType
        {
            Happy,
            Angry,
            Neutral
        }
        public ExpressionType expressionType;
        public Sprite expression;
    }
    [SerializeField] private List<Expression> expressions = new();

    private void Awake()
    {
        uiManager = GetComponent<ShopkeeperUIManager>();
        aiManager = GetComponent<ShopkeeperAIManager>();
        textAnalysis = GetComponent<TextAnalysis>();

        textAnalysis.OnMoodChanged += OnMoodChanged;
    }

    private IEnumerator Start()
    {
        if (!isDebugging)
            aiManager.InitAIManager();

        player = PlayerController.Instance;
        uiManager.InitUIManager();

        yield return new WaitForSeconds(1f);

        SetupShop();
    }

    public void OnEnterRange()
    {
        keycodeE.Show();
    }

    public bool OnInteract()
    {
        uiManager.ShowUI();
        uiManager.isInteracting = true;
        uiManager.SetRerollCost(rerollCost);
        player.ChangeState(PlayerController.PlayerStates.Shop);
        return true;
    }

    public void OnLeaveRange()
    {
        keycodeE.Hide();
    }

    public void OnLeaveShopkeeper()
    {
        aiManager.OnLeaveShop();
        uiManager.OnLeaveShopkeeper();
        player.ChangeState(PlayerController.PlayerStates.Movement);
    }

    public void Reroll()
    {
        if (player.gold < rerollCost)
            return;

        player.RemoveGold(rerollCost);
        rerollCost += incrementRerollCost;
        incrementRerollCost = Mathf.CeilToInt(incrementRerollCost * incrementModifier);
        uiManager.OnReroll(rerollCost);
        SetupShop();
    }

    public void SetupShop()
    {
        int items = Mathf.FloorToInt(shopItemAmount / 2f);

        List<ShopItemData> itemItems = GetRandomShopItemOfType(ShopItemData.ShopItemType.Item, items);
        foreach (ShopItemData item in itemItems)
            uiManager.SpawnShopItem(item);

        List<ShopItemData> abilityItems = GetRandomShopItemOfType(ShopItemData.ShopItemType.Ability, shopItemAmount - items);
        foreach (ShopItemData ability in abilityItems)
            uiManager.SpawnShopItem(ability);
    }

    public List<ShopItemData> GetRandomShopItemOfType(ShopItemData.ShopItemType type, int amount)
    {
        List<ShopItemData> shopItems = new();

        foreach (ShopItemData item in allShopItems)
        {
            if (item.shopItemType.Equals(type) && !shopItems.Contains(item))
                shopItems.Add(item);
        }

        List<ShopItemData> itemsToSpawn = new();

        for (int i = 0; i < amount; i++)
        {
            int randNum = Random.Range(0, shopItems.Count);
            ShopItemData shopItem = shopItems[randNum];

            if (itemsToSpawn.Contains(shopItem))
                continue;

            itemsToSpawn.Add(shopItem);
        }

        return itemsToSpawn;
    }

    public void OnMoodChanged(float modifier, Vector3 results)
    {
        if (results.x >= 88f && results.z <= 12f)
            AssignExpression(Expression.ExpressionType.Happy);
        else if (results.z >= 10f && results.z > results.y)
            AssignExpression(Expression.ExpressionType. Neutral);
        else
            AssignExpression(Expression.ExpressionType.Angry);

        uiManager.ApplyDiscount(modifier);
    }

    private void AssignExpression(Expression.ExpressionType expressionType)
    {
        foreach (Expression expression in expressions)
        {
            if (expression.expressionType == expressionType)
                shopkeeperSprite.sprite = expression.expression;
        }
    }
}