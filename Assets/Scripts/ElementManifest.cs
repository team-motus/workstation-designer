using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.ConstructionElements;

namespace WorkstationDesigner
{
    /// <summary>
    /// A manifest that tracks construction elements and their quantities.
    /// </summary>
    public class ElementManifest
    {
        /// <summary>
        /// Stock of a single element.
        /// </summary>
        private class Stock
        {
            public int Quantity;
            public ConstructionElement Element;

            /// <summary>
            /// Create a new Stock.
            /// </summary>
            /// <param name="quantity">The quantity of the element stored</param>
            /// <param name="element">The element stored</param>
            public Stock(int quantity, ConstructionElement element)
            {
                this.Quantity = quantity;
                this.Element = element;
            }
        }

        private List<Stock> StockList = new List<Stock>();

        /// <summary>
        /// Find a stock matching a given element.
        /// </summary>
        /// <param name="element">The element to search for</param>
        /// <returns></returns>
        private Stock FindStock(ConstructionElement element)
        {
            return StockList.Find(stock => stock.Element == element);
        }

        /// <summary>
        /// Add a quantity of elements to a stock.
        /// </summary>
        /// <param name="element">The element to add</param>
        /// <param name="quantity">The quantity of elements to add</param>
        public void AddElements(ConstructionElement element, int quantity)
        {
            Stock stock = FindStock(element);
            if (stock == null)
            {
                stock = new Stock(0, element);
                StockList.Add(stock);
            }
            
            stock.Quantity += quantity;
        }

        /// <summary>
        /// Remove a quantity of elements from a stock.
        /// </summary>
        /// <param name="element">The element to remove</param>
        /// <param name="quantity">The quantity of elements to remove</param>
        public void RemoveElements(ConstructionElement element, int quantity)
        {
            Stock stock = FindStock(element);
            if (stock == null)
            {
                throw new Exception(); //TODO: Not worrying about error handling for now
            }
            
            stock.Quantity -= quantity;
            if (stock.Quantity < 0)
            {
                throw new Exception(); //TODO: Not worrying about error handling for now
            }
        }

        /// <summary>
        /// Get the quantity stored of an element.
        /// </summary>
        /// <param name="element">The element to get the quantity of</param>
        /// <returns>The quantity stored of the element</returns>
        public int GetQuantity(ConstructionElement element)
        {
            Stock matchingStock = FindStock(element);
            if (matchingStock == null)
            {
                return 0;
            }
            return matchingStock.Quantity;
        }

        /// <summary>
        /// Check if this manifest is a subset of another.
        /// </summary>
        /// <param name="manifest">The manifest to check if this is a subset of.</param>
        /// <returns>Whether the manifest is a subset of the provided manifest.</returns>
        public bool Subset(ElementManifest manifest)
        {
            foreach (Stock stock in this.StockList)
            {
                if (stock.Quantity > manifest.GetQuantity(stock.Element))
                {
                    return false;
                }
            }
            return true;
        }

        public void ForEach(Action<ConstructionElement, int> callback)
        {
            foreach (Stock stock in StockList)
            {
                callback(stock.Element, stock.Quantity);
            }
        }
    }
}
