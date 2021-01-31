using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections;

public class IAPManager : MonoBehaviour, IStoreListener
{
    [Header("List of IAPs")]
    public IAPItem[] iaps;

    IStoreController storeController;
    IExtensionProvider storeExtensionProvider;

    void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        if (storeController == null)
            InitializePurchasing();

        // Wait a while and set the prices correctly
        StartCoroutine(setPrices());
    }

    // Sets the price after a while
    IEnumerator setPrices()
    {
        // Wait a second
        yield return new WaitForSeconds(1f);

        // Check if we have a store controller
        //if (!IsInitialized()) InputManager.ShowError("Failed to load IAP Controller. In-app purchases most likely won't work");

        // Set all the prices
        for (int i = 0; i < iaps.Length; i++) { iaps[i].priceText.text = storeController.products.WithID(iaps[i].id).metadata.localizedPriceString; }
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
            return;

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        // Add all the items
        for (int i = 0; i < iaps.Length; i++) { builder.AddProduct(iaps[i].id, ProductType.Consumable); }
        // Init the purchasing
        UnityPurchasing.Initialize(this, builder);
    }

    // Only say we are initialized if both the Purchasing references are set.
    private bool IsInitialized() { return storeController != null && storeExtensionProvider != null; }

    // Buys using a IAP id
    public void BuyIAP(int i) { BuyProductID(iaps[i].id); }
    // Buys using a product id
    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // Get the product
            Product product = storeController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                // Log the success
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // Initiate purchase
                storeController.InitiatePurchase(product);
            }
            // Otherwise ...
            else { InputManager.ShowError("Purchase failed, transaction failed or item unavailable"); }
        }
        // Otherwise ...
        else { InputManager.ShowError("Purchase failed, code failed to initialize transaction manager. Try again later"); }
    }

    // Set values on initialization
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Set some values
        storeController = controller;
        storeExtensionProvider = extensions;
    }

    // Purchasing set-up has not succeeded
    public void OnInitializeFailed(InitializationFailureReason error) { InputManager.ShowError("Initialization of transaction failed, error: " + error); }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        for (int i = 0; i < iaps.Length; i++)
        {
            if (String.Equals(args.purchasedProduct.definition.id, iaps[i].id, StringComparison.Ordinal))
            {
                // Log it!
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

                // Give the player an item and give thank you note
                InputManager.instance.ToggleUISectionsInt(14);
                Inventory.instance.Add(iaps[i].item);

                // Return
                return PurchaseProcessingResult.Complete;
            }
        }

        // Show an error
        InputManager.ShowError("Purchase id not recognized, please contact milandhsoftware@gmail.com with the following product id: " + args.purchasedProduct.definition.id);
        // Return
        return PurchaseProcessingResult.Complete;
    }

    // Gets called by Unity when a purchase fails
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // Show an error
        InputManager.ShowError(string.Format("Purchase failed, google play returned an error. Contact milandhsoftware@gmail.com with the product id: '{0}' & the failure error: '{1}'", product.definition.storeSpecificId, failureReason));
    }

    [System.Serializable]
    public class IAPItem
    {
        [Header("The name & id of the IAP")]
        public string id = "small_contribution";
        [Header("The item we give after paying")]
        public ItemSettings item;
        [Header("The text to show the price")]
        public Text priceText;
    }
}