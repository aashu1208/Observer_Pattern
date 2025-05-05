using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyAndSellStock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(MaxProfit(new int[]{7,1,5,3,6,4}));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int MaxProfit(int[] prices){

        int minPrice = int.MaxValue;
        int maxProfit = 0;

        for(int i = 0; i<prices.Length; i++){
            if(prices[i]<minPrice){
                minPrice = prices[i];
            }
            else
            {
                int profit = prices[i] - minPrice;
                maxProfit = profit;
            }
        }
        return maxProfit;
    }
}
