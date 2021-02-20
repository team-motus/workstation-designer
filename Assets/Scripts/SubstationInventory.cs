using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorkstationDesigner.Elements;

namespace WorkstationDesigner
{
    public class SubstationInventory
    {
        private class Stock
        {
            public int Quantity;
            public Element Element;

            public Stock(int quantity, Element element)
            {
                this.Quantity = quantity;
                this.Element = element;
            }
        }

        private List<Stock> StockList = new List<Stock>();

        private Stock FindStock(Element element)
        {
            return StockList.Find(stock => stock.Element == element);
        }

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

        public int GetQuantity(Element element)
        {
            return FindStock(element).Quantity;
        }
    }
}
