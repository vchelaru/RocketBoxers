using System;
using System.Collections.Generic;
using System.Linq;

namespace RocketBoxers.GumRuntimes
{
    public partial class PlayerGameIconRuntime
    {
        partial void CustomInitialize()
        {

        }

        public void SetStockCount(int count)
        {
            while(this.StockContainer.Children.Count > count)
            {
                this.StockContainer.Children.Last().Parent = null;
            }
            while(this.StockContainer.Children.Count < count)
            {
                this.StockContainer.Children.Add(new StockCountRuntime());
            }
        }
    }
}
