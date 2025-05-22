using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TwoSum2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //TwoSum_Leetcode(new int[]{2,7,11,15});
        Debug.Log(HasPairSum(new int[]{2,7,11,15}));
    }

    private void TwoSum_Leetcode(int[] nums)
    {

        nums = new int[]{2,11,7,15};
        var dict = new Dictionary<int, int>();
        int target = 9;
        int complement;
        for(int i = 0; i<nums.Length; i++)
        {

            complement = target-nums[i];
            if(dict.ContainsKey(complement))
            {

                Debug.Log(dict[complement]+","+i);
            }

            dict[nums[i]] = i;
        }
    }


    private bool HasPairSum(int[] nums)
    {
        var dict = new Dictionary<int, int>();
        int target = 9;
        

        for(int i = 0; i<nums.Length; i++)
        {

            int complement = target - nums[i];
            if(dict.ContainsKey(complement))
            {

                return true;
            }

            dict[nums[i]] = i;
        }

        
        return false;

    }
}
