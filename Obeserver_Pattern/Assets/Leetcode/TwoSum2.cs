using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TwoSum2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TwoSum_Leetcode(new int[]{2,7,11,15});
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TwoSum_Leetcode(int[] nums)
    {


        // target = 9
        // array = [2,7,11,15]

        // Output - [0,1]
        // target - nums[0] = nums[1]
        nums = new int[]{2,11,7,15};

        for(int i = 0; i < nums.Length; i++)
        {

            for(int j = i+1; j< nums.Length; j++)
            {

                if(nums[i] + nums[j] == 9)
                {
                    Debug.Log("found the 2 numbers: "+i+" and "+j);



                }
            }
        }
    }
}
