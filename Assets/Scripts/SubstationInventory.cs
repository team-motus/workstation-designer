using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.Elements;

namespace WorkstationDesigner
{
    /// <summary>
    /// A substation's inventory of elements.
    /// </summary>
    public class SubstationInventory
    {
        /// <summary>
        /// Stock of a single element.
        /// </summary>
        private class Stock
        {
            public int Quantity;
            public Element Element;

            /// <summary>
            /// Create a new Stock.
            /// </summary>
            /// <param name="quantity">The quantity of the element stored</param>
            /// <param name="element">The element stored</param>
            public Stock(int quantity, Element element)
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
        private Stock FindStock(Element element)
        {
            return StockList.Find(stock => stock.Element == element);
        }

        /// <summary>
        /// Add a quantity of elements to a stock.
        /// </summary>
        /// <param name="element">The element to add</param>
        /// <param name="quantity">The quantity of elements to add</param>
        public void AddElements(Element element, int quantity)
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
        public void RemoveElements(Element element, int quantity)
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
        public int GetQuantity(Element element)
        {
            return FindStock(element).Quantity;
        }
    }
}
